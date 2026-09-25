using UnityEngine;

/// <summary>
/// 武器の攻撃判定を管理するクラス
/// </summary>
public class Weapon_HitBox : MonoBehaviour
{
    private Player_WeaponController_New weaponController; // 親の WeaponController への参照

    /// <summary>
    /// 武器の攻撃判定を管理する WeaponController を初期化するメソッド
    /// </summary>
    public void Initialize(Player_WeaponController_New controller)
    {
        weaponController = controller;
    }

    /// <summary>
    /// 武器の攻撃判定が他のコライダーに接触したときに呼ばれるメソッド
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (weaponController != null)
        {
            // 接触情報を親の WeaponController に通知
            weaponController.OnWeaponTriggerEnter(other);
        }
    }
}
