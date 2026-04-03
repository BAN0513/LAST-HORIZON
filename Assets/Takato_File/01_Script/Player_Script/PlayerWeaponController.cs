using Takato;
using UnityEngine;

/// <summary>
/// プレイヤーの武器を制御するクラス
/// </summary>
namespace Takato
{
    public class PlayerWeaponController : MonoBehaviour
    {
        [Header("プレイヤーの武器制御")]
        [Space(10)]

        [Header("装備中の武器")]
        [SerializeField] private Weapon equippedWeapon;

        [Header("武器生成用プレハブ")]
        [SerializeField] private Weapon weaponPrefab;

        [Header("武器を格納する親オブジェクト")]
        [SerializeField] private Transform weaponFolder;

        [HideInInspector]
        public Weapon EquippedWeapon => equippedWeapon;

        private void Start()
        {
            EquipWeapon(); // 武器を生成して装備
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
    }
}
