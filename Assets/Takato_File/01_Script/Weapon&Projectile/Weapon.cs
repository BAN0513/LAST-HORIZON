using UnityEngine;

/// <summary>
/// 武器のクラス
/// </summary>
namespace Takato
{
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

        private Collider weaponCollider;  // 武器のコライダー
        private bool isAttacking = false; // 攻撃中かどうか

        private void Awake()
        {
            weaponCollider = GetComponent<Collider>();  // 武器のコライダーを取得
            DisableCollider();                          // 初期状態ではコライダーを無効化
        }

        /// <summary>
        /// コライダーを有効化
        /// </summary>
        public void EnableCollider()
        {
            weaponCollider.enabled = true;
            isAttacking = true; // 攻撃開始
        }

        /// <summary>
        /// コライダーを無効化
        /// </summary>
        public void DisableCollider()
        {
            weaponCollider.enabled = false;
            isAttacking = false; // 攻撃終了
        }

        /// <summary>
        /// 敵に攻撃が当たった時の処理
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            SoundManager soundmanager = FindAnyObjectByType<SoundManager>();

            if(soundmanager != null)
            {
                soundmanager.PlaySE(1); // 攻撃ヒットのSEを再生
            }

            if (!isAttacking) return; // 攻撃中のみ判定

            // ここで敵かどうか判定し、ダメージ処理
            var enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage((int)AttackDamage);
            }
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
    }
}
