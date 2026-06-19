using Takato;
using UnityEngine;

namespace Takato
{
    /// <summary>
    /// プレイヤーの武器を制御するクラス
    /// </summary>
    public class PlayerWeaponController : MonoBehaviour
    {
        [Header("プレイヤーの武器制御")]
        [Space(10)]

        [Header("現在装備している武器")]
        [SerializeField] private Weapon equippedWeapon;
        [Header("武器のプレハブ")]
        [SerializeField] private Weapon weaponPrefab;
        [Header("武器を配置する空のオブジェクト")]
        [SerializeField] private Transform weaponFolder;

        [Space(10)]
        [Header("魔法武器の場合のFirePoint")]
        [SerializeField] private Transform magicFirePoint; // 魔法武器の発射位置

        [HideInInspector]
        public Weapon EquippedWeapon => equippedWeapon; // 現在装備している武器を外部から参照できるようにするプロパティ

        private void Start()
        {
            EquipWeapon(); // 武器を生成して装備
        }

        private void Update()
        {
            // magicFirePoint が未設定なら、装備中の武器の子から探す（1フレーム毎に探すが見つかれば以降はスキップ）
            if (magicFirePoint == null && equippedWeapon != null)
            {
                magicFirePoint = FindChildRecursive(equippedWeapon.transform, "FirePoint");
            }

            //PlayerInputControllerのAttackInputがtrueのときにFireMagicを呼び出す
            var inputController = GetComponent<PlayerInputController>();
            if (inputController != null && inputController.IsAttackInput)
            {
                FireMagic();// 攻撃入力があれば魔法を発射
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
                Destroy(equippedWeapon.gameObject); // 既存の武器を削除
            }

            if (weaponPrefab != null && weaponFolder != null)
            {
                // プレハブから武器を生成し、WeaponFolderの子にする
                Weapon newWeapon = Instantiate(weaponPrefab, weaponFolder);
                newWeapon.transform.localPosition = Vector3.zero;
                newWeapon.transform.localRotation = Quaternion.identity;
                equippedWeapon = newWeapon;

                // 装備直後に weapon の子から FirePoint を探して設定（見つかれば以降の探索を不要にする）
                if (magicFirePoint == null)
                {
                    magicFirePoint = FindChildRecursive(equippedWeapon.transform, "FirePoint");
                }
            }
            else
            {
                Debug.LogWarning("weaponPrefab または weaponFolder が設定されていません。");
            }
        }

        /// <summary>
        /// 武器コライダーを有効化
        /// </summary>
        public void EnableWeaponCollider()
        {
            if (equippedWeapon != null)
            {
                equippedWeapon.EnableCollider(); // 武器のコライダーを有効化
            }
        }

        /// <summary>
        /// 武器コライダーを無効化
        /// </summary>
        public void DisableWeaponCollider()
        {
            if (equippedWeapon != null)
            {
                equippedWeapon.DisableCollider(); // 武器のコライダーを無効化
            }
        }

        /// <summary>
        /// 魔法を発射する
        /// 実際の生成・設定は Weapon 側の FireMagic に移譲する
        /// </summary>
        public void FireMagic()
        {
            if (magicFirePoint == null)
            {
                Debug.LogWarning("magicFirePoint が設定されていません。");
                return;
            }

            if (equippedWeapon == null)
            {
                Debug.LogWarning("装備中の武器がありません。");
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
