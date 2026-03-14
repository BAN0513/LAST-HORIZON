using UnityEngine;

/// <summary>
/// プレイヤーの武器を制御するクラス
/// </summary>
public class PlayerWeaponController : MonoBehaviour
{
    private CapsuleCollider weaponCollider;

    private void Start()
    {
        weaponCollider = GetComponent<CapsuleCollider>(); // 武器のコライダーを取得
        weaponCollider.enabled = false;                   // 初期状態では武器のコライダーを無効化
    }

    /// <summary>
    /// 武器コライダーを有効化
    /// </summary>
    public void EnableWeaponCollider()
    {
        weaponCollider.enabled = true;
        //Debug.Log("コライダーを有効化");
    }

    /// <summary>
    /// 武器コライダーを無効化
    /// </summary>
    public void DisableWeaponCollider()
    {
        weaponCollider.enabled = false;
        //Debug.Log("コライダーを無効化");
    }

    /// <summary>
    /// 敵のタグに当たった時の処理
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("攻撃が当たった");
        }
    }
}
