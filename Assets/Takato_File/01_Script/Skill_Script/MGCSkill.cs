using Takato;
using UnityEngine;

/// <summary>
/// 魔法攻撃を管理するクラス
/// </summary>
namespace Takato
{
    [CreateAssetMenu(menuName = "Takato/Skill/MGCSkill")]
    public class MGCSkill : SkillBase
    {
        [Header("(魔法攻撃スキルのステータス)")]
        [Space(10)]

        [Header("ホーミング弾プレハブ")]
        [SerializeField] private HomingProjectile homingProjectilePrefab;
        [Header("魔法攻撃時のParticle")]
        [SerializeField] private ParticleSystem magicEffectPrefab;
        [Header("弾の速度（初期値）")]
        [SerializeField] private float baseProjectileSpeed;
        [Header("ホーミング回転速度（初期値）")]
        [SerializeField] private float baseHomingRotationSpeed;
        [Header("弾のダメージ（初期値）")]
        [SerializeField] private float baseProjectileDamage;
        [Header("スキルレベル")]
        [SerializeField, Min(1)] private int skillLevel;
        [Header("最大スキルレベル")]
        [SerializeField, Min(1)] private int maxSkillLevel;
        [Header("レベルごとのダメージ増加量")]
        [SerializeField] private float damagePerLevel;
        [Header("レベルごとの速度増加量")]
        [SerializeField] private float speedPerLevel;

        public override void Activate(PlayerController player)
        {
            // 魔法攻撃のエフェクトを生成
            if (magicEffectPrefab != null)
            {
                ParticleSystem effect = Instantiate(magicEffectPrefab, player.transform.position, Quaternion.identity);
                effect.transform.SetParent(player.transform); // プレイヤーに追従させる
                effect.Play();
                // エフェクトの再生時間
                Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
            }

            // 最も近い敵を探す
            Enemy targetEnemy = FindNearestEnemy(player.transform.position);
            if (targetEnemy == null)
            {
                Debug.Log("ターゲットとなる敵がいません。");
                return;
            }

            // FirePointを自動取得（なければplayerの前方）
            Transform autoFirePoint = player.transform.Find("FirePoint");
            Vector3 spawnPos = autoFirePoint != null
                ? autoFirePoint.position
                : player.transform.position + player.transform.forward;

            // レベルに応じたパラメータ計算
            float projectileSpeed = baseProjectileSpeed + speedPerLevel * (skillLevel - 1);
            float homingRotationSpeed = baseHomingRotationSpeed; // 必要ならレベルで増加
            float projectileDamage = baseProjectileDamage + damagePerLevel * (skillLevel - 1);

            // ホーミング弾を生成
            HomingProjectile projectile = Instantiate(homingProjectilePrefab, spawnPos, Quaternion.identity);
            projectile.speed = projectileSpeed;
            projectile.rotationSpeed = homingRotationSpeed;
            projectile.damage = projectileDamage;
            projectile.SetTarget(targetEnemy.transform);

            Debug.Log($"{skillName}（Lv.{skillLevel}）スキルが {player.name} によって発動されました。");
        }

        private Enemy FindNearestEnemy(Vector3 fromPosition)
        {
            Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            Enemy nearest = null;
            float minDist = float.MaxValue;
            foreach (var enemy in enemies)
            {
                float dist = Vector3.Distance(fromPosition, enemy.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = enemy;
                }
            }
            return nearest;
        }

        /// <summary>
        /// スキルレベルを外部から変更する場合用
        /// </summary>
        public void SetSkillLevel(int level)
        {
            skillLevel = Mathf.Clamp(level, 1, maxSkillLevel);
        }
    }
}
