using UnityEngine;

/// <summary>
/// プレイヤーのScriptableObjectを管理するクラス(New)
/// </summary>
[CreateAssetMenu(fileName = "PlayerSO_New", menuName = "ScriptableObjects/PlayerSO_New", order = 1)]
public class PlayerSO_New : ScriptableObject
{
    [Header("プレイヤーのステータス詳細設定")]

    [Header("プレイヤーの最大体力")]
    [SerializeField] private float maxHealth;

    [Header("スタミナ設定")]
    [Space(10)]
    [Header("プレイヤーの最大スタミナ")]
    [SerializeField] private float maxStamina;
    [Header("スタミナ消費量")]
    [SerializeField] private float staminaDrainRate;
    [Header("スタミナ回復量")]
    [SerializeField] private float staminaRegenRate;
    [Header("スタミナ回復遅延時間")]
    [SerializeField] private float staminaRegenDelay;


    [Header("プレイヤーの移動速度")]
    [SerializeField] private float moveSpeed;
    [Header("移動速度の倍率(スプリント時)")]
    [SerializeField] private float speedMultiplier;
    [Header("移動速度の加速倍率")]
    [SerializeField] private float accelerationMultiplier;
    [Header("移動停止時の減速倍率")]
    [SerializeField] private float decelerationMultiplier;
    [Header("プレイヤーのジャンプの高さ")]
    [SerializeField] private float jumpHeight;
    [Header("プレイヤーのロール時の移動速度倍率")]
    [SerializeField] private float rollSpeedMultiplier;
    [Header("重力の設定")]
    [SerializeField] private float gravityScale;


    // プロパティ経由で値を参照できるように設定
    public float MaxHealth => maxHealth;                           // 最大体力のプロパティ

    public float MaxStamina => maxStamina;                         // 最大スタミナのプロパティ
    public float StaminaDrainRate => staminaDrainRate;             // スタミナ消費量のプロパティ
    public float StaminaRegenRate => staminaRegenRate;             // スタミナ回復量のプロパティ
    public float StaminaRegenDelay => staminaRegenDelay;           // 回復遅延時間のプロパティ

    public float MoveSpeed => moveSpeed;                           // 移動速度のプロパティ
    public float SpeedMultiplier => speedMultiplier;               // 移動速度の倍率のプロパティ
    public float AccelerationMultiplier => accelerationMultiplier; // 加速倍率のプロパティ
    public float DecelerationMultiplier => decelerationMultiplier; // 減速倍率のプロパティ

    public float JumpHeight => jumpHeight;                         // ジャンプの高さのプロパティ
    public float RollSpeedMultiplier => rollSpeedMultiplier;       // ロール時の移動速度倍率のプロパティ
    public float GravityScale => gravityScale;                     // 重力の設定のプロパティ
}
