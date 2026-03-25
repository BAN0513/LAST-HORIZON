using UnityEngine;

/// <summary>
/// 武器の基本クラス
/// </summary>
public class Weapon : MonoBehaviour
{
    [Header("武器ステータス")]
    [Space(10)]
    [Header("基本攻撃力")]
    [SerializeField] private float baseAttackDamage;

    [Header("熟練度")]
    [Header("最低熟練度")]
    [SerializeField] private int weaponLevel;
    [Header("最大熟練度")]
    [SerializeField] private int maxLevel;

    private Collider weaponCollider; // 武器のコライダー

    private void Awake()
    {
        weaponCollider = GetComponent<Collider>();
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }

    /// <summary>
    /// コライダーを有効化
    /// </summary>
    public void EnableCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
            Debug.Log($"{gameObject.name} のコライダーを有効化");
        }
    }

    /// <summary>
    /// コライダーを無効化
    /// </summary>
    public void DisableCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
            Debug.Log($"{gameObject.name} のコライダーを無効化");
        }
    }

    /// <summary>
    /// 敵のタグに当たった時の処理
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage((int)AttackDamage);
                Debug.Log("敵にダメージを与えました！");
            }
        }
    }

    /// <summary>
    /// 現在の攻撃力（熟練度による補正も含んでいる）
    /// </summary>
    public float AttackDamage
    {
        get
        {
            // 熟練度が上がるごとに攻撃力が10%増加する。
            if(weaponLevel > 0)
            {
                return baseAttackDamage * (1 + 0.1f * weaponLevel); // 熟練度が1なら10%増、2なら20%増とかになる
            }
            else
            {
                return baseAttackDamage; // 熟練度が0なら基本攻撃力のみ
            }
        }
    }

    public int WeaponLevel => weaponLevel;  // 現在の熟練度
    public int MaxLevel => maxLevel;        // 最大熟練度

    /// <summary>
    /// 熟練度を上げる
    /// </summary>
    public void LevelUp()
    {
        if (weaponLevel < maxLevel)
        {
            weaponLevel++;
        }
    }
}
