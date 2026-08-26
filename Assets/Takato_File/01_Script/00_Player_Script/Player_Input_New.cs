using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの入力を管理するクラス(New)
/// </summary>
public class Player_Input_New : MonoBehaviour
{
    private InputSystem_Actions playerInputActions; // プレイヤーの入力アクションを格納する変数

    public Vector2 MoveInput { get; private set; }       // プレイヤーの移動入力を格納するプロパティ
    public bool JumpInput { get; private set; }          // プレイヤーのジャンプ入力を格納するプロパティ
    public bool IsSprinting { get; private set; } = false; // プレイヤーのダッシュ入力を格納するプロパティ

    //前転用のプロパティと内部変数
    public bool RollInput { get; private set; }
    private float lastForwardTapTime = 0f;
    [SerializeField] private float doubleTapThreshold = 0.3f; // ダブルタップと判定する制限時間(秒)

    private void Awake()
    {
        playerInputActions = new InputSystem_Actions(); // InputSystem_Actionsのインスタンスを作成

        // Move入力イベント登録
        playerInputActions.Player.Move.started += OnMoveInput;   // 入力が開始されたときのイベント
        playerInputActions.Player.Move.performed += OnMoveInput; // 入力が実行されたときのイベント
        playerInputActions.Player.Move.canceled += OnMoveInput;  // 入力がキャンセルされたときのイベント

        // Jump入力イベント登録
        playerInputActions.Player.Jump.started += context => JumpInput = true;   // ジャンプ入力が開始されたときのイベント
        playerInputActions.Player.Jump.canceled += context => JumpInput = false; // ジャンプ入力がキャンセルされたときのイベント

        // Sprint入力イベント登録
        playerInputActions.Player.Sprint.started += context => IsSprinting = true;   // ダッシュ入力が開始されたときのイベント
        playerInputActions.Player.Sprint.canceled += context => IsSprinting = false; // ダッシュ入力がキャンセルされたときのイベント
    }

    private void OnEnable()
    {
        playerInputActions.Player.Enable(); // プレイヤーの入力アクションを有効化
    }

    private void OnDisable()
    {
        playerInputActions.Player.Disable(); // プレイヤーの入力アクションを無効化
    }

    /// <summary>
    /// プレイヤーの移動入力を処理するメソッド
    /// </summary>
    private void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 newInput = context.ReadValue<Vector2>();

        //Wキーまたは上矢印キーが押された場合のダブルタップ判定
        if (context.started && newInput.y > 0.5f)
        {
            if (Time.time - lastForwardTapTime <= doubleTapThreshold)
            {
                RollInput = true; // ダブルタップ成立
            }
            lastForwardTapTime = Time.time;
        }

        MoveInput = newInput; // プレイヤーの移動入力を更新
    }

    /// <summary>
    /// 入力消費後にフラグをリセットするメソッド
    /// </summary>
    public void ResetRollInput()
    {
        RollInput = false;
    }
}