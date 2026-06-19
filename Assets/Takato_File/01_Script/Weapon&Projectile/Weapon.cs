using UnityEngine;

namespace Takato
{
    /// <summary>
    /// 武器のクラス
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Weapon : MonoBehaviour
    {
        [Header("武器ステータス")]
        [Space(10)]
        [Header("基本攻撃力")]
        [SerializeField] private float baseAttackDamage;
        [Header("現在レベル")]
        [SerializeField] private int weaponLevel;
        [Header("最大レベル")]
        [SerializeField] private int maxLevel;

        [Space(10)]
        [Header("魔法武器設定")]
        [SerializeField] private bool isMagicWeapon; // 魔法武器フラグ
        [Header("魔法弾のプレハブ（魔法武器の場合）")]
        [SerializeField] private HomingProjectile magicProjectilePrefab; // HomingProjectile のプレハブ

        [Space(10)]
        [Header("魔法攻撃のクールタイム（秒）")]
        [SerializeField] private float magicCooldown; // インスペクタで調整可能なクールタイム

        private float nextMagicFireTime = 0f; // 次に発射できる時刻
        private Collider weaponCollider;  // 武器のコライダー
        private bool isAttacking = false; // 攻撃中かどうか

        // キャッシュ
        private SoundManager soundManager;

        private void Awake()
        {
            // コライダーを取得
            weaponCollider = GetComponent<Collider>();
            if (weaponCollider == null)
            {
                Debug.LogWarning($"{nameof(Weapon)}: Collider が見つかりません。");
            }
            else
            {
                DisableCollider(); // 初期状態ではコライダーを無効化
            }

            // SoundManager をキャッシュ（存在しない場合は null のまま）
            soundManager =FindAnyObjectByType<SoundManager>();
            if (soundManager == null)
            {
                Debug.LogWarning($"{nameof(Weapon)}: SoundManager がシーン内に見つかりません。SE を再生できません。");
            }

            // 魔法弾プレハブが必要な場合の事前警告
            if (isMagicWeapon && magicProjectilePrefab == null)
            {
                Debug.LogWarning($"{nameof(Weapon)}: isMagicWeapon が true ですが magicProjectilePrefab が設定されていません。");
            }
        }

        /// <summary>
        /// 魔法弾を発射する（Player から呼び出す）
        /// Weapon 側で生成・ダメージ設定・ターゲット設定を行う
        /// </summary>
        public void FireMagic(Transform firePoint)
        {
            if (!isMagicWeapon) return;

            // クールタイム中は発射しない
            if (Time.time < nextMagicFireTime)
            {
                Debug.Log("魔法がクールタイム中です。");
                return;
            }

            if (firePoint == null)
            {
                Debug.LogWarning($"{nameof(FireMagic)}: firePoint が設定されていません。");
                return;
            }

            if (magicProjectilePrefab == null)
            {
                Debug.LogWarning($"{nameof(FireMagic)}: magicProjectilePrefab が設定されていません。");
                return;
            }

            // プレハブから生成
            HomingProjectile proj = Instantiate(
                magicProjectilePrefab,
                firePoint.position,
                firePoint.rotation
            );

            // ダメージを装備武器の攻撃力に合わせる
            proj.damage = AttackDamage;

            // 近い敵をターゲットに設定（なければ null のまま飛ばす）
            Transform nearest = FindNearestEnemyTransform(firePoint.position);
            if (nearest != null)
            {
                proj.SetTarget(nearest);
            }

            // 発射後のクールタイム開始
            nextMagicFireTime = Time.time + magicCooldown;
        }

        /// <summary>
        /// 指定位置から最も近い Enemy の Transform を返す（見つからなければ null）
        /// </summary>
        private Transform FindNearestEnemyTransform(Vector3 fromPosition)
        {
            Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            Transform best = null;
            float bestDist = float.MaxValue;

            foreach (var e in enemies)
            {
                if (e == null) continue;
                float d = Vector3.Distance(fromPosition, e.transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = e.transform;
                }
            }

            return best;
        }

        /// <summary>
        /// コライダーを有効化
        /// </summary>
        public void EnableCollider()
        {
            if (weaponCollider != null)
            {
                weaponCollider.enabled = true;
            }
            isAttacking = true; // 攻撃開始
        }

        /// <summary>
        /// コライダーを無効化
        /// </summary>
        public void DisableCollider()
        {
            if (weaponCollider != null)
            {
                weaponCollider.enabled = false;
            }
            isAttacking = false; // 攻撃終了
        }

        /// <summary>
        /// 基本攻撃力を設定するメソッド
        /// </summary>
        public void SetAttackDamage(float value)
        {
            baseAttackDamage = value;
        }

        /// <summary>
        /// 攻撃力を計算して返すプロパティ
        /// </summary>
        public float AttackDamage
        {
            get
            {
                // レベルや強化に応じて計算
                return baseAttackDamage + weaponLevel * 2f;
            }
        }

        public int WeaponLevel => weaponLevel; // 現在の武器レベルを取得
        public int MaxLevel => maxLevel;       // 最大レベルを取得

        /// <summary>
        /// 武器のレベルを上げるメソッド
        /// </summary>
        public void LevelUp()
        {
            if (weaponLevel < maxLevel)
            {
                weaponLevel++;
            }
        }

        // --- クールタイム情報の取得ヘルパー ---
        public bool CanFireMagic() => Time.time >= nextMagicFireTime;
        public float GetRemainingMagicCooldown() => Mathf.Max(0f, nextMagicFireTime - Time.time);
    }
}
