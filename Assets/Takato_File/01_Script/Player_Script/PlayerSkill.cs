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
        [Header("スキルのクールタイム")]
        [SerializeField] private float skillCooldown;
        [Header("攻撃スキルの上昇倍率(基準値)")]
        [SerializeField] private float attackBuffMultiplier;
        [Header("防御スキルのダメージカット率(基準値)")]
        [SerializeField] private float defenseBuffCutRate;
        [Header("スキルの持続時間(基準値)")]
        [SerializeField] private float skillDuration;

        [Header("レベルごとの上昇倍率加算値")]
        [Space(10)]
        [Header("レベルごとの攻撃力アップ倍率加算値")]
        [SerializeField] private float attackBuffMultiplierPerLevel;
        [Header("レベルごとのダメージカット加算値")]
        [SerializeField] private float defenseBuffCutRatePerLevel;
        [Header("レベルごとの効果時間加算値")]
        [SerializeField] private float skillDurationPerLevel;

        [Header("スキルレベル")]
        [SerializeField] private int skillLevel;
        [Header("スキル最大レベル")]
        [SerializeField] private int maxSkillLevel;

        [Header("攻撃スキルのパーティクル")]
        [SerializeField] private ParticleSystem attackBuffEffect;
        [Header("防御スキルのパーティクル")]
        [SerializeField] private ParticleSystem defenseBuffEffect;

        private float skillTimer; // スキルのクールタイム管理
        private bool isSkillActive; // スキルが現在発動中かどうか

        private PlayerWeaponController weaponController; // 武器管理クラスへの参照
        private Takato.PlayerController playerController;// プレイヤー管理クラスへの参照

        private float originalAttackDamage; // スキル発動前の攻撃力を保存する変数
        private float originalDamageCutRate; // スキル発動前のダメージカット率を保存する変数

        private float currentSkillDuration; // 現在のスキルの持続時間を管理する変数

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
                    EndSkill();
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

            /// 攻撃力アップのエフェクトをスキル発動中に武器の位置に生成
            if(attackBuffEffect != null)
            {
                Instantiate(attackBuffEffect, weapon.transform.position, Quaternion.identity, weapon.transform);
            }

            int level = skillLevel;

            float levelAttackBuffMultiplier = attackBuffMultiplier + attackBuffMultiplierPerLevel * (level - 1);
            currentSkillDuration = skillDuration + skillDurationPerLevel * (level - 1);

            originalAttackDamage = weapon.AttackDamage;
            weapon.SetAttackDamage(originalAttackDamage * levelAttackBuffMultiplier);

            isSkillActive = true;
            currentSkillDuration = Mathf.Max(currentSkillDuration, 0.1f);
            skillTimer = skillCooldown;
        }

        /// <summary>
        /// 防御力アップスキル発動
        /// </summary>
        public void ActivateDefenseBuff()
        {
            if (skillTimer > 0f || isSkillActive || playerController == null) return;

            /// 防御力アップのエフェクトをスキル発動中にプレイヤーの位置に生成
            if(defenseBuffEffect != null)
            {
                Instantiate(defenseBuffEffect, transform.position, Quaternion.identity, transform);
            }

            int level = skillLevel;

            float levelDefenseBuffCutRate = defenseBuffCutRate + defenseBuffCutRatePerLevel * (level - 1);
            currentSkillDuration = skillDuration + skillDurationPerLevel * (level - 1);

            originalDamageCutRate = playerController.GetDamageCutRate();
            playerController.SetDamageCutRate(originalDamageCutRate + levelDefenseBuffCutRate);

            isSkillActive = true;
            currentSkillDuration = Mathf.Max(currentSkillDuration, 0.1f);
            skillTimer = skillCooldown;
        }

        /// <summary>
        /// スキル効果終了
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
