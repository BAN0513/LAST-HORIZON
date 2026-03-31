using UnityEngine;

/// <summary>
/// プレイヤーのスキルを管理するクラス
/// </summary>
namespace Takato
{
    public class PlayerSkill : MonoBehaviour
    {
        [Header("スキルのステータス")]
        [Space(10)]

        [Header("攻撃スキル")]
        [Space(5)]
        [Header("攻撃スキルのクールタイム")]
        [SerializeField] private float attackSkillCooldown;
        [Header("攻撃スキルの上昇倍率(基準値)")]
        [SerializeField] private float attackBuffMultiplier;
        [Header("攻撃スキルの持続時間(基準値)")]
        [SerializeField] private float attackSkillDuration;
        [Header("攻撃スキルのパーティクル")]
        [SerializeField] private ParticleSystem attackBuffEffect;
        [Header("攻撃スキルのレベルごとの倍率加算値")]
        [SerializeField] private float attackBuffMultiplierPerLevel;
        [Header("攻撃スキルのレベルごとの効果時間加算値")]
        [SerializeField] private float attackSkillDurationPerLevel;

        [Header("防御スキル")]
        [Space(5)]
        [Header("防御スキルのクールタイム")]
        [SerializeField] private float defenseSkillCooldown;
        [Header("防御スキルのダメージカット率(基準値)")]
        [SerializeField] private float defenseBuffCutRate;
        [Header("防御スキルの持続時間(基準値)")]
        [SerializeField] private float defenseSkillDuration;
        [Header("防御スキルのパーティクル")]
        [SerializeField] private ParticleSystem defenseBuffEffect;
        [Header("防御スキルのレベルごとのカット加算値")]
        [SerializeField] private float defenseBuffCutRatePerLevel;
        [Header("防御スキルのレベルごとの効果時間加算値")]
        [SerializeField] private float defenseSkillDurationPerLevel;

        [Header("魔法スキル")]
        [Space(5)]
        [Header("魔法スキルのクールタイム")]
        [SerializeField] private float magicSkillCooldown;
        [Header("魔法スキル用ホーミング弾Prefab")]
        [SerializeField] private HomingProjectile homingProjectilePrefab;
        [Header("ホーミング弾の発射位置")]
        [SerializeField] private Transform projectileSpawnPoint;

        [Header("スキル共通")]
        [Space(5)]
        [Header("スキルレベル")]
        [SerializeField] private int skillLevel;
        [Header("スキル最大レベル")]
        [SerializeField] private int maxSkillLevel;


        private ParticleSystem activeAttackBuffEffect;      // 現在再生中の攻撃スキルエフェクト
        private ParticleSystem activeDefenseBuffEffect;     // 現在再生中の防御スキルエフェクト

        private float skillTimer;                           // スキルのクールタイム管理
        private bool isSkillActive;                         // スキルが現在発動中かどうか

        private PlayerWeaponController weaponController;    // 武器管理クラスへの参照
        private Takato.PlayerController playerController;   // プレイヤー管理クラスへの参照

        private float originalAttackDamage;                 // スキル発動前の攻撃力を保存する変数
        private float originalDamageCutRate;                // スキル発動前のダメージカット率を保存する変数

        private float currentSkillDuration;                 // 現在のスキルの持続時間を管理する変数

        private void Start()
        {
            skillTimer = 0f;
            isSkillActive = false;
            weaponController = GetComponentInChildren<PlayerWeaponController>();
            playerController = GetComponent<Takato.PlayerController>();
        }

        private void Update()
        {
            if (skillTimer > 0f)
            {
                skillTimer -= Time.deltaTime;
            }

            if (isSkillActive)
            {
                currentSkillDuration -= Time.deltaTime;
                if (currentSkillDuration <= 0f)
                {
                    EndSkill(); // スキルの効果終了
                }
            }
        }

        /// <summary>
        /// 攻撃力アップスキル発動
        /// </summary>
        public void ActivateAttackBuff()
        {
            if (skillTimer > 0f || isSkillActive || weaponController == null) return;

            var weapon = GetEquippedWeapon();
            if (weapon == null) return;

            // エフェクトをインスタンス化して再生
            if (attackBuffEffect != null)
            {
                activeAttackBuffEffect = Instantiate(attackBuffEffect, transform);
                activeAttackBuffEffect.transform.localPosition = Vector3.zero;
                activeAttackBuffEffect.Play();

            }

            int level = skillLevel; //武器のレベルに応じた倍率を計算

            float levelAttackBuffMultiplier = attackBuffMultiplier + attackBuffMultiplierPerLevel * (level - 1);
            currentSkillDuration = attackSkillCooldown + attackSkillDuration * (level - 1);

            originalAttackDamage = weapon.AttackDamage;
            weapon.SetAttackDamage(originalAttackDamage * levelAttackBuffMultiplier);

            isSkillActive = true;
            currentSkillDuration = Mathf.Max(currentSkillDuration, 0.1f);
            skillTimer = attackSkillCooldown;
        }

        /// <summary>
        /// 防御力アップスキル発動
        /// </summary>
        public void ActivateDefenseBuff()
        {
            if (skillTimer > 0f || isSkillActive || playerController == null) return;

            // エフェクトをインスタンス化して再生
            if (defenseBuffEffect != null)
            {
                activeDefenseBuffEffect = Instantiate(defenseBuffEffect, transform);
                activeDefenseBuffEffect.transform.localPosition = Vector3.zero;
                activeDefenseBuffEffect.Play();
            }

            int level = skillLevel; //武器のレベルに応じたダメージカット率を計算

            float levelDefenseBuffCutRate = defenseBuffCutRate + defenseBuffCutRatePerLevel * (level - 1);
            currentSkillDuration = attackSkillCooldown + attackSkillDuration * (level - 1);

            originalDamageCutRate = playerController.GetDamageCutRate();
            playerController.SetDamageCutRate(originalDamageCutRate + levelDefenseBuffCutRate);

            isSkillActive = true;
            currentSkillDuration = Mathf.Max(currentSkillDuration, 0.1f);
            skillTimer = defenseSkillCooldown;
        }

        /// <summary>
        /// 魔法スキル発動
        /// </summary>
        public void ActivateMagicHomingSkill()
        {
            if (skillTimer > 0f || isSkillActive) return;

            // 近くの敵を探す（最も近いEnemyタグのオブジェクト）
            GameObject targetEnemy = FindClosestEnemy();
            if (targetEnemy == null) return;

            // ホーミング弾を生成
            HomingProjectile projectile = Instantiate(
                homingProjectilePrefab,
                projectileSpawnPoint != null ? projectileSpawnPoint.position : transform.position + transform.forward,
                Quaternion.identity
            );
            projectile.SetTarget(targetEnemy.transform);

            // クールタイム設定
            skillTimer = magicSkillCooldown;
        }

        // 近くの敵を探すユーティリティ
        private GameObject FindClosestEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject closest = null;
            float minDist = float.MaxValue;
            Vector3 currentPos = transform.position;
            foreach (GameObject enemy in enemies)
            {
                float dist = Vector3.Distance(enemy.transform.position, currentPos);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = enemy;
                }
            }
            return closest;
        }

        /// <summary>
        /// スキルの効果終了
        /// </summary>
        private void EndSkill()
        {
            var weapon = GetEquippedWeapon();
            if (weapon != null && originalAttackDamage > 0)
            {
                weapon.SetAttackDamage(originalAttackDamage);
            }
            if (playerController != null && originalDamageCutRate >= 0)
            {
                playerController.SetDamageCutRate(originalDamageCutRate);
            }
            // エフェクトを停止＆破棄
            if (activeAttackBuffEffect != null)
            {
                activeAttackBuffEffect.Stop();
                Destroy(activeAttackBuffEffect.gameObject);
                activeAttackBuffEffect = null;
            }
            if (activeDefenseBuffEffect != null)
            {
                activeDefenseBuffEffect.Stop();
                Destroy(activeDefenseBuffEffect.gameObject);
                activeDefenseBuffEffect = null;
            }
            isSkillActive = false;
            originalAttackDamage = 0;
            originalDamageCutRate = -1;
        }

        /// <summary>
        /// スキルレベルを上げる
        /// </summary>
        public void LevelUpSkill()
        {
            if (skillLevel < maxSkillLevel)
            {
                skillLevel++;
            }
        }

        /// <summary>
        /// 現在のスキルレベルを取得
        /// </summary>
        public int SkillLevel => skillLevel;

        /// <summary>
        /// 最大スキルレベルを取得
        /// </summary>
        public int MaxSkillLevel => maxSkillLevel;

        private Weapon GetEquippedWeapon()
        {
            return weaponController != null ? typeof(PlayerWeaponController)
                .GetField("equippedWeapon", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(weaponController) as Weapon : null;
        }
    }
}
