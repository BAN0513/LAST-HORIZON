using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ロールの種類を表す列挙型
/// </summary>
public enum RollType
{
    None,
    Forward,
    Backward
}

/// <summary>
/// プレイヤーの入力を管理するクラス(New)
/// </summary>
public class Player_Input_New : MonoBehaviour
{
    private InputSystem_Actions playerInputActions;

    [Header("長押し判定設定")]
    [SerializeField] private float heavyAttackHoldThreshold;
    [Header("ダブルタップ判定設定")]
    [SerializeField] private float doubleTapThreshold;

    // 入力状態を保持するプロパティ
    public Vector2 MoveInput { get; private set; } // 移動入力
    public bool JumpInput { get; private set; }    // ジャンプ入力
    public bool IsSprinting { get; private set; } = false; // ダッシュ入力
    public Vector2 LookInput { get; private set; } // 視点入力
    public bool IsSkillInput { get; private set; } // スキル入力
    public bool IsSkillInput2 { get; private set; } // スキル2入力
    public bool IsSkillInput3 { get; private set; } // スキル3入力

    // 攻撃入力用プロパティ
    public bool LightAttackInput { get; private set; }         // 通常攻撃の入力があったか
    public bool HeavyAttackInput { get; private set; }         // 溜め攻撃の入力があったか
    public bool IsAttackHolding { get; private set; }          // ボタンを押し続けているか
    public bool HeavyAttackReleasedInput { get; private set; } // 溜め後にボタンを離した瞬間か
    public bool IsAttackCanceled { get; private set; }         //溜め中にしきい値未満で離されたか
    public bool RollInput { get; private set; }                // ロール入力があったか
    public RollType CurrentRollType { get; private set; } = RollType.None; // 現在のロールの種類

    private float lastForwardTapTime = 0f;   // 前方へのダブルタップの最後の時間
    private float lastBackwardTapTime = 0f;  // 後方へのダブルタップの最後の時間
    private float attackPressStartTime = 0f; // 攻撃ボタンが押された時間を記録する変数
    private float lastHoldDuration = 0f;     // 攻撃ボタンが離されたときの最後の保持時間を記録する変数

    private void Awake()
    {
        playerInputActions = new InputSystem_Actions();

        // Move入力のイベント登録
        playerInputActions.Player.Move.started += OnMoveInput;
        playerInputActions.Player.Move.performed += OnMoveInput;
        playerInputActions.Player.Move.canceled += OnMoveInput;

        // Jump入力のイベント登録
        playerInputActions.Player.Jump.started += context => JumpInput = true;
        playerInputActions.Player.Jump.canceled += context => JumpInput = false;

        // Sprint入力のイベント登録
        playerInputActions.Player.Sprint.started += context => IsSprinting = true;
        playerInputActions.Player.Sprint.canceled += context => IsSprinting = false;

        // 攻撃入力のイベント登録
        playerInputActions.Player.Attack.started += OnAttackStarted;
        playerInputActions.Player.Attack.canceled += OnAttackCanceled;

        // Look入力のイベント登録
        playerInputActions.Player.Look.started += OnLookInput;
        playerInputActions.Player.Look.performed += OnLookInput;
        playerInputActions.Player.Look.canceled += OnLookInput;

        // スキル入力のイベント登録
        playerInputActions.Player.Skill.started += context => IsSkillInput = true;
        playerInputActions.Player.Skill.canceled += context => IsSkillInput = false;

        // スキル2入力のイベント登録
        playerInputActions.Player.Skill2.started += context => IsSkillInput2 = true;
        playerInputActions.Player.Skill2.canceled += context => IsSkillInput2 = false;

        // スキル3入力のイベント登録
        playerInputActions.Player.Skill3.started += context => IsSkillInput3 = true;
        playerInputActions.Player.Skill3.canceled += context => IsSkillInput3 = false;
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
    /// 攻撃ボタンが押されたときの処理。長押し判定の開始を記録する。
    /// </summary>
    /// <param name="context"></param>
    private void OnAttackStarted(InputAction.CallbackContext context)
    {
        attackPressStartTime = Time.time;
        IsAttackHolding = true;
        IsAttackCanceled = false; // フラグ初期化
    }

    /// <summary>
    /// 攻撃ボタンが離されたときの処理。長押し判定を行い、攻撃タイプを決定する。
    /// </summary>
    /// <param name="context"></param>
    private void OnAttackCanceled(InputAction.CallbackContext context)
    {
        lastHoldDuration = Time.time - attackPressStartTime;
        IsAttackHolding = false;

        float holdDuration = Time.time - attackPressStartTime;
        if (holdDuration >= heavyAttackHoldThreshold)
        {
            HeavyAttackInput = true;
        }
        else
        {
            LightAttackInput = true; // 短いタップは通常攻撃へ
        }
    }

    /// <summary>
    /// 攻撃ボタンが押されている時間を取得するメソッド
    /// </summary>
    /// <returns></returns>
    public float GetAttackHoldDuration()
    {
        // 押しっぱなし中なら現在の継続時間、離された直後なら最後に保持した時間を返す
        return IsAttackHolding ? (Time.time - attackPressStartTime) : lastHoldDuration;
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 newInput = context.ReadValue<Vector2>();

        if (context.started)
        {
            if (newInput.y > 0.5f)
            {
                if (Time.time - lastForwardTapTime <= doubleTapThreshold)
                {
                    RollInput = true;
                    CurrentRollType = RollType.Forward;
                }
                lastForwardTapTime = Time.time;
            }
            else if (newInput.y < -0.5f)
            {
                if (Time.time - lastBackwardTapTime <= doubleTapThreshold)
                {
                    RollInput = true;
                    CurrentRollType = RollType.Backward;
                }
                lastBackwardTapTime = Time.time;
            }
        }

        MoveInput = newInput; // Update MoveInput with the new value
    }

    /// <summary>
    /// プレイヤーの視点入力を取得するメソッド
    /// </summary>
    /// <param name="context"></param>
    private void OnLookInput(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// プレイヤーのロール入力をリセットするメソッド
    /// </summary>
    public void ResetRollInput()
    {
        RollInput = false;
        CurrentRollType = RollType.None;
    }

    /// <summary>
    /// プレイヤーの攻撃入力をリセットするメソッド
    /// </summary>
    public void ResetAttackInput()
    {
        LightAttackInput = false;
        HeavyAttackInput = false;
        HeavyAttackReleasedInput = false;
        IsAttackCanceled = false; //キャンセルフラグもリセット
    }

    /// <summary>
    /// プレイヤーのスキル入力をリセットするメソッド
    /// </summary>
    public void ResetSkillInput()
    {
        IsSkillInput = false;
        IsSkillInput2 = false;
        IsSkillInput3 = false;
    }
}