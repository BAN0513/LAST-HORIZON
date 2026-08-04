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
    [Header("プレイヤーのジャンプの高さ")]
    [SerializeField] private float jumpHeight;
    [Header("重力の設定")]
    [SerializeField] private float gravityScale;


    // プロパティ経由で値を参照できるように設定
    public float MoveSpeed => moveSpeed;
    public float JumpHeight => jumpHeight;
    public float GravityScale => gravityScale;
}
