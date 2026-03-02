using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの入力を管理するクラス(InputSystemを使用)
/// </summary>
namespace Takato
{
    public class PlayerInputController : MonoBehaviour
    {
        private InputSystem_Actions playerInputActions; // 入力アクションのインスタンス

        public Vector2 MoveInput { get; private set; } // プレイヤーの移動入力を格納するプロパティ

        private void Awake()
        {
            playerInputActions = new InputSystem_Actions(); // 入力アクションのインスタンスを作成

            // Move入力イベント登録
            playerInputActions.Player.Move.started += OnMoveInput;   // 入力が開始されたときのイベント
            playerInputActions.Player.Move.performed += OnMoveInput; // 入力が実行されたときのイベント
            playerInputActions.Player.Move.canceled += OnMoveInput;  // 入力がキャンセルされたときのイベント
        }

        /// <summary>
        /// プレイヤーの入力を有効にするためのメソッド
        /// </summary>
        private void OnEnable()
        {
            playerInputActions.Player.Enable();
        }

        /// <summary>
        /// プレイヤーの入力を無効にするためのメソッド
        /// </summary>
        private void OnDisable()
        {
            playerInputActions.Player.Disable();
        }

        /// <summary>
        /// プレイヤーの移動入力を処理するメソッド
        /// </summary>
        private void OnMoveInput(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }
    }
}
