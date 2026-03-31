using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの入力を管理するクラス(InputSystemを使用)
/// </summary>
namespace Takato
{
    public class PlayerInputController : MonoBehaviour
    {
        private InputSystem_Actions playerInputActions;  // 入力アクションのインスタンス

        public Vector2 MoveInput { get; private set; }   // プレイヤーの移動入力を格納するプロパティ
        public bool JumpInput { get; private set; }      // プレイヤーのジャンプ入力を格納するプロパティ
        public bool BlockInput { get; private set; }     //プレイヤーのガード入力を格納するプロパティ
        public bool IsAttackInput { get; private set; }  //プレイヤーの攻撃入力を格納するプロパティ
        public bool IsSkillInput { get; private set; }   //プレイヤーのスキル入力を格納するプロパティ
        public bool IsSkill2Input { get; private set; }  //プレイヤーのスキル2入力を格納するプロパティ
        public bool IsSkill3Input { get; private set; }  //プレイヤーのスキル3入力を格納するプロパティ
        public Vector2 LookInput { get; private set; }   // プレイヤーの視点入力を格納するプロパティ

        private void Awake()
        {
            playerInputActions = new InputSystem_Actions(); // 入力アクションのインスタンスを作成

            // Move入力イベント登録
            playerInputActions.Player.Move.started += OnMoveInput;   // 入力が開始されたときのイベント
            playerInputActions.Player.Move.performed += OnMoveInput; // 入力が実行されたときのイベント
            playerInputActions.Player.Move.canceled += OnMoveInput;  // 入力がキャンセルされたときのイベント

            //Jump入力イベント登録
            playerInputActions.Player.Jump.started += OnJumpInput;   // 入力が開始されたときのイベント
            playerInputActions.Player.Jump.performed += OnJumpInput; // 入力が実行されたときのイベント
            playerInputActions.Player.Jump.canceled += OnJumpInput;  // 入力がキャンセルされたときのイベント

            //Block入力イベント登録
            playerInputActions.Player.Block.started += context => BlockInput = context.ReadValueAsButton();   // 入力が開始されたときのイベント
            playerInputActions.Player.Block.performed += context => BlockInput = context.ReadValueAsButton(); // 入力が実行されたときのイベント
            playerInputActions.Player.Block.canceled += context => BlockInput = context.ReadValueAsButton();  // 入力がキャンセルされたときのイベント

            //Attack入力イベント登録
            playerInputActions.Player.Attack.started += context => IsAttackInput = context.ReadValueAsButton();   // 入力が開始されたときのイベント
            playerInputActions.Player.Attack.performed += context => IsAttackInput = context.ReadValueAsButton(); // 入力が実行されたときのイベント
            playerInputActions.Player.Attack.canceled += context => IsAttackInput = context.ReadValueAsButton();  // 入力がキャンセルされたときのイベント

            //Skill入力イベント登録
            playerInputActions.Player.Skill.started += context => IsSkillInput = context.ReadValueAsButton();   // 入力が開始されたときのイベント
            playerInputActions.Player.Skill.performed += context => IsSkillInput = context.ReadValueAsButton(); // 入力が実行されたときのイベント
            playerInputActions.Player.Skill.canceled += context => IsSkillInput = context.ReadValueAsButton();  // 入力がキャンセルされたときのイベント

            //Skill2入力イベント登録
            playerInputActions.Player.Skill2.started += context => IsSkill2Input = context.ReadValueAsButton();   // 入力が開始されたときのイベント
            playerInputActions.Player.Skill2.performed += context => IsSkill2Input = context.ReadValueAsButton(); // 入力が実行されたときのイベント
            playerInputActions.Player.Skill2.canceled += context => IsSkill2Input = context.ReadValueAsButton();  // 入力がキャンセルされたときのイベント

            //Skill3入力イベント登録
            playerInputActions.Player.Skill3.started += context => IsSkill3Input = context.ReadValueAsButton();   // 入力が開始されたときのイベント
            playerInputActions.Player.Skill3.performed += context => IsSkill3Input = context.ReadValueAsButton(); // 入力が実行されたときのイベント
            playerInputActions.Player.Skill3.canceled += context => IsSkill3Input = context.ReadValueAsButton();  // 入力がキャンセルされたときのイベント

            //Look入力イベント登録
            playerInputActions.Player.Look.started += context => LookInput = context.ReadValue<Vector2>();   // 入力が開始されたときのイベント
            playerInputActions.Player.Look.performed += context => LookInput = context.ReadValue<Vector2>(); // 入力が実行されたときのイベント
            playerInputActions.Player.Look.canceled += context => LookInput = context.ReadValue<Vector2>();  // 入力がキャンセルされたときのイベント
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

        /// <summary>
        /// プレイヤーのジャンプ入力を処理するメソッド
        /// </summary>
        private void OnJumpInput(InputAction.CallbackContext context)
        {
            JumpInput = context.ReadValueAsButton();
        }

        /// <summary>
        /// プレイヤーの視点入力を処理するメソッド
        /// </summary>
        private void OnLookInput(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }
    }
}
