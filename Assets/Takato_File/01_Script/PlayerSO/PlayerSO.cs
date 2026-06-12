using UnityEngine;

/// <summary>
/// プレイヤーのScriptableObject（プレイヤーのステータス値）
/// </summary>
[CreateAssetMenu(menuName = "Takato/PlayerSO", fileName = "PlayerSO")]
public class PlayerSO : ScriptableObject
{
    [Header("プレイヤーの基礎情報一覧")]
    [Space(10)]

    [Header("プレイヤーのPrefab")]
    [SerializeField] private GameObject playerPrefab;

    [Header("プレイヤーのステータス値")]
    [Header("プレイヤーの最大HP")]
    [SerializeField] private int maxHP;

    [Header("開始スキルコスト")]
    [SerializeField] private int startingCost;

    [Header("プレイヤーの移動速度")]
    [SerializeField] private float moveSpeed;

    [Header("プレイヤーのジャンプ力")]
    [SerializeField] private float jumpForce;

    [Header("プレイヤーの重力")]
    [SerializeField] private float gravity;

    [Header("プレイヤーのダメージカット率")]
    [Range(0f, 1f)]
    [SerializeField] private float damageCutRate;

    // 読み取り専用のプロパティ

    // プレイヤーのPrefabへのプロパティ
    public GameObject PlayerPrefab => playerPrefab;

    // プレイヤーの最大HPへのプロパティ
    public int MaxHP => maxHP;

    // 開始スキルコストへのプロパティ
    public int StartingCost => startingCost;

    // プレイヤーの移動速度へのプロパティ
    public float MoveSpeed => moveSpeed;

    // プレイヤーのジャンプ力へのプロパティ
    public float JumpForce => jumpForce;

    // プレイヤーの重力へのプロパティ
    public float Gravity => gravity;

    // プレイヤーのダメージカット率へのプロパティ
    public float DamageCutRate => damageCutRate;  
}
