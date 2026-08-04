using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの入力を管理するクラス(New)
/// </summary>
public class Player_Input_New : MonoBehaviour
{
   private InputSystem_Actions playerInputActions; //プレイヤーの入力アクションを格納する変数


    public Vector2 MoveInput { get; private set; }   // プレイヤーの移動入力を格納するプロパティ
    public bool JumpInput { get; private set; }      // プレイヤーのジャンプ入力を格納するプロパティ

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
        MoveInput = context.ReadValue<Vector2>();
    }
}
