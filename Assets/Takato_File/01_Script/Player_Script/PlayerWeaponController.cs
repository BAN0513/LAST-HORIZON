using Takato;
using UnityEngine;

namespace Takato
{
    /// <summary>
    /// プレイヤーの武器を制御するクラス
    /// </summary>
    [RequireComponent(typeof(PlayerInputController))] // 必須コンポーネントを保証
    public class PlayerWeaponController : MonoBehaviour
    {
        [Header("--- 武器の設定 ---")]
        [SerializeField] private Weapon weaponPrefab;
        [SerializeField] private Transform weaponFolder;

        [Header("--- 魔法武器の設定 ---")]
        [SerializeField] private Transform magicFirePoint; // 魔法武器の発射位置

        [Header("--- デバッグ・確認用 ---")]
        [SerializeField] private Weapon equippedWeapon; // 現在装備している武器

        // 現在装備している武器を外部から参照できるようにするプロパティ
        public Weapon EquippedWeapon => equippedWeapon;

        // キャッシュ用変数
        private PlayerInputController _inputController;

        private void Awake()
        {
            // 事前にコンポーネントを取得しておく（Updateでの負荷軽減）
            _inputController = GetComponent<PlayerInputController>();
        }

        private void Start()
        {
            EquipWeapon(); // 武器を生成して装備
        }

        private void Update()
        {
            // 攻撃入力があれば魔法を発射
            if (_inputController != null && _inputController.IsAttackInput)
            {
                FireMagic();
            }
        }

        /// <summary>
        /// 武器を生成して装備する
        /// </summary>
        public void EquipWeapon()
        {
            // 既存の武器がシーン上に存在する場合のみ削除
            if (equippedWeapon != null && equippedWeapon.gameObject.scene.IsValid())
            {
                Destroy(equippedWeapon.gameObject);
                magicFirePoint = null; // 古い発射位置をクリア
            }

            if (weaponPrefab == null || weaponFolder == null)
            {
                Debug.LogWarning("weaponPrefab または weaponFolder が設定されていません。");
                return;
            }

            // プレハブから武器を生成し、WeaponFolderの子にする
            equippedWeapon = Instantiate(weaponPrefab, weaponFolder);
            equippedWeapon.transform.localPosition = Vector3.zero;
            equippedWeapon.transform.localRotation = Quaternion.identity;

            // 装備直後に一度だけ FirePoint を探して設定
            if (magicFirePoint == null)
            {
                magicFirePoint = FindChildRecursive(equippedWeapon.transform, "FirePoint");
            }
        }

        /// <summary>
        /// 武器コライダーを有効化
        /// </summary>
        public void EnableWeaponCollider()
        {
            if (equippedWeapon != null) equippedWeapon.EnableCollider();
        }

        /// <summary>
        /// 武器コライダーを無効化
        /// </summary>
        public void DisableWeaponCollider()
        {
            if (equippedWeapon != null) equippedWeapon.DisableCollider();
        }

        /// <summary>
        /// 魔法を発射する
        /// </summary>
        public void FireMagic()
        {
            if (equippedWeapon == null)
            {
                Debug.LogWarning("装備中の武器がありません。");
                return;
            }

            if (magicFirePoint == null)
            {
                Debug.LogWarning("magicFirePoint が設定されていません。");
                return;
            }

            // 実際の魔法弾生成と設定は Weapon に任せる
            equippedWeapon.FireMagic(magicFirePoint);
        }

        /// <summary>
        /// 武器の熟練度を上げる
        /// </summary>
        public void LevelUpWeapon()
        {
            if (equippedWeapon != null)
            {
                equippedWeapon.LevelUp();
                Debug.Log($"武器のレベルが{equippedWeapon.WeaponLevel}になりました！");
            }
        }

        /// <summary>
        /// 再帰的に子トランスフォームを探すユーティリティ
        /// </summary>
        private Transform FindChildRecursive(Transform parent, string childName)
        {
            if (parent == null) return null;
            foreach (Transform child in parent)
            {
                if (child.name == childName) return child;
                Transform found = FindChildRecursive(child, childName);
                if (found != null) return found;
            }
            return null;
        }
    }
}