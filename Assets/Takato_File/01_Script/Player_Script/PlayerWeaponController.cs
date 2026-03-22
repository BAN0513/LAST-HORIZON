using UnityEngine;

/// <summary>
/// プレイヤーの武器を制御するクラス
/// </summary>
public class PlayerWeaponController : MonoBehaviour
{
    [Header("武器のステータス")]
    [Space(10)]
    [Header("武器の攻撃力")]
    [SerializeField] private float attackDamage;


    private CapsuleCollider weaponCollider; // 武器のコライダー

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
        Debug.Log("コライダーを有効化");
    }

    /// <summary>
    /// 武器コライダーを無効化
    /// </summary>
    public void DisableWeaponCollider()
    {
        weaponCollider.enabled = false;
        Debug.Log("コライダーを無効化");
    }

    /// <summary>
    /// 敵のタグに当たった時の処理
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            //Enemyのコンポーネントを取得してダメージを与える
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage((int)attackDamage); //与えるダメージ
                Debug.Log("敵にダメージを与えました！");
            }
        }
    }
}
