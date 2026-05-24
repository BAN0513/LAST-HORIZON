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
        public bool IsSkill4Input { get; private set; }  //プレイヤーのアルティメットスキル入力を格納するプロパティ
        public Vector2 LookInput { get; private set; }   // プレイヤーの視点入力を格納するプロパティ
        public bool IsInventoryInput { get; private set; } // プレイヤーのインベントリ入力を格納するプロパティ
        public bool IsCharChange { get; private set; } // プレイヤーのキャラチェンジ入力を格納するプロパティ

        // 内部バックフィールド
        private bool isLookEnabled = false;

        // 外部から視点入力を受け付けるかを制御するフラグ
        public bool IsLookEnabled
        {
            get => isLookEnabled; // 現在の状態を返す
            set
            {
                isLookEnabled = value; // 状態を更新

                if (playerInputActions != null)
                {
                    if (isLookEnabled)
                    {
                        // Look アクションのみ有効化
                        playerInputActions.Player.Look.Enable();
                    }
                    else
                    {
                        // Look アクションのみ無効化
                        playerInputActions.Player.Look.Disable();
                    }
                }
            }
        }

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

            //Skill4入力イベント登録
            playerInputActions.Player.Ult.started += context => IsSkill4Input = context.ReadValueAsButton();   // 入力が開始されたときのイベント
            playerInputActions.Player.Ult.performed += context => IsSkill4Input = context.ReadValueAsButton(); // 入力が実行されたときのイベント
            playerInputActions.Player.Ult.canceled += context => IsSkill4Input = context.ReadValueAsButton();  // 入力がキャンセルされたときのイベント

            //Look入力イベント登録
            playerInputActions.Player.Look.started += context => LookInput = IsLookEnabled ? context.ReadValue<Vector2>() * Time.deltaTime : Vector2.zero;
            playerInputActions.Player.Look.performed += context => LookInput = IsLookEnabled ? context.ReadValue<Vector2>() * Time.deltaTime : Vector2.zero;
            playerInputActions.Player.Look.canceled += context => LookInput = IsLookEnabled ? context.ReadValue<Vector2>() * Time.deltaTime : Vector2.zero;

            //Inventory入力イベント登録
            playerInputActions.Player.Inventory.started += context => IsInventoryInput = context.ReadValueAsButton();   // 入力が開始されたときのイベント
            playerInputActions.Player.Inventory.performed += context => IsInventoryInput = context.ReadValueAsButton(); // 入力が実行されたときのイベント
            playerInputActions.Player.Inventory.canceled += context => IsInventoryInput = context.ReadValueAsButton();  // 入力がキャンセルされたときのイベント

            //CharChange入力イベント登録
            playerInputActions.Player.CharChange.started += context => IsCharChange = context.ReadValueAsButton();   // 入力が開始されたときのイベント
            playerInputActions.Player.CharChange.performed += context => IsCharChange = context.ReadValueAsButton(); // 入力が実行されたときのイベント
            playerInputActions.Player.CharChange.canceled += context => IsCharChange = context.ReadValueAsButton();  // 入力がキャンセルされたときのイベント
        }

        /// <summary>
        /// プレイヤーの入力を有効/無効にするためのメソッド
        /// </summary>
        public void SetGamePlayEnable(bool isEnable)
        {
            if (isEnable)
            {
                playerInputActions.Player.Enable(); // ゲームプレイ入力を有効にする
            }
            else
            {
                playerInputActions.Player.Disable(); // ゲームプレイ入力を無効にする
            }
        }


        /// <summary>
        /// プレイヤーの入力を有効にするためのメソッド
        /// </summary>
        private void OnEnable()
        {
            // デフォルトはゲームプレイ入力を有効、UIは無効にしておく
            playerInputActions.Player.Enable();
            playerInputActions.UI.Disable();
        }

        /// <summary>
        /// プレイヤーの入力を無効にするためのメソッド
        /// </summary>
        private void OnDisable()
        {
            playerInputActions.Player.Disable();
            playerInputActions.UI.Disable();
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
        /// ゲームプレイ (Player) アクションマップを有効/無効にします。
        /// </summary>
        public void SetGameplayEnabled(bool enabled)
        {
            if (playerInputActions == null) return;
            if (enabled) playerInputActions.Player.Enable();
            else playerInputActions.Player.Disable();
        }

        /// <summary>
        /// UI アクションマップを有効/無効にします。
        /// </summary>
        public void SetUIEnabled(bool enabled, bool keepInventory = true)
        {
            if (playerInputActions == null) return;

            if (enabled)
            {
                playerInputActions.UI.Enable();

                // スキルUIなどを開いたときでもインベントリ入力を維持したい場合はここで明示的に有効化する
                if (keepInventory)
                {
                    playerInputActions.Player.Inventory.Enable();
                }
                else
                {
                    // 明示的に無効にしたい場合
                    playerInputActions.Player.Inventory.Disable();
                }
            }
            else
            {
                playerInputActions.UI.Disable();
                // UIを閉じる際、Inventoryの状態は変更しない（必要なら外部から SetInventoryEnabled を呼ぶ）
            }
        }

        /// <summary>
        /// Inventory アクションだけを有効/無効にします。
        /// </summary>
        public void SetInventoryEnabled(bool enabled)
        {
            if (playerInputActions == null) return;

            if (enabled) playerInputActions.Player.Inventory.Enable();
            else playerInputActions.Player.Inventory.Disable();
        }
    }
}
