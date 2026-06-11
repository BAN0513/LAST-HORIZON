using UnityEngine;

/// <summary>
/// 魔法使いが使用する魔法武器のコントローラークラス
/// </summary>
public class PlayerMagicWeaponController : MonoBehaviour
{
    [Header("魔法武器のステータス")]
    [Space(10)]
    [Header("魔法攻撃の弾を生成させる場所")]
    [SerializeField] private Transform magicAttackSpawnPoint;
    [Header("魔法攻撃力")]
    [SerializeField] private float magicAttackDamage;
    [Header("現在レベル")]
    [SerializeField] private int weaponLevel;
    [Header("最大レベル")]
    [SerializeField] private int maxLevel;

    private bool isAttacking = false; // 攻撃中かどうか
    private bool isEnemy = false;     // 敵に攻撃が当たったかどうか
}
