using UnityEngine;

/// <summary>
/// プレイヤーのScriptableObjectを管理するクラス(New)
/// </summary>
[CreateAssetMenu(fileName = "PlayerSO_New", menuName = "ScriptableObjects/PlayerSO_New", order = 1)]
public class PlayerSO_New : ScriptableObject
{
    [Header("プレイヤーのステータス詳細設定")]

    [Header("プレイヤーの移動速度")]
    [SerializeField] private float moveSpeed;
    [Header("移動速度の加速倍率")]
    [SerializeField] private float accelerationMultiplier;
    [Header("移動停止時の減速倍率")]
    [SerializeField] private float decelerationMultiplier;
    [Header("プレイヤーのジャンプの高さ")]
    [SerializeField] private float jumpHeight;
    [Header("重力の設定")]
    [SerializeField] private float gravityScale;


    // プロパティ経由で値を参照できるように設定
    public float MoveSpeed => moveSpeed;                           // 移動速度のプロパティ
    public float AccelerationMultiplier => accelerationMultiplier; // 加速倍率のプロパティ
    public float DecelerationMultiplier => decelerationMultiplier; // 減速倍率のプロパティ
    public float JumpHeight => jumpHeight;                         // ジャンプの高さのプロパティ
    public float GravityScale => gravityScale;                     // 重力の設定のプロパティ
}
