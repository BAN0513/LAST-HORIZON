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

    public Vector2 MoveInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool IsSprinting { get; private set; } = false;
    public Vector2 LookInput { get; private set; }

    // 攻撃入力用プロパティ
    public bool LightAttackInput { get; private set; }
    public bool HeavyAttackInput { get; private set; }

    public bool IsAttackHolding { get; private set; } // ボタンを押し続けているか
    public bool HeavyAttackReleasedInput { get; private set; } // 溜め後にボタンを離した瞬間か
    public bool IsAttackCanceled { get; private set; } //溜め中にしきい値未満で離されたか

    [Header("長押し判定設定")]
    [SerializeField] private float heavyAttackHoldThreshold;
    [Header("ダブルタップ判定設定")]
    [SerializeField] private float doubleTapThreshold;

    public bool RollInput { get; private set; }
    public RollType CurrentRollType { get; private set; } = RollType.None;

    private float lastForwardTapTime = 0f;
    private float lastBackwardTapTime = 0f;
    private float attackPressStartTime = 0f;

    private void Awake()
    {
        playerInputActions = new InputSystem_Actions();

        playerInputActions.Player.Move.started += OnMoveInput;
        playerInputActions.Player.Move.performed += OnMoveInput;
        playerInputActions.Player.Move.canceled += OnMoveInput;

        playerInputActions.Player.Jump.started += context => JumpInput = true;
        playerInputActions.Player.Jump.canceled += context => JumpInput = false;

        playerInputActions.Player.Sprint.started += context => IsSprinting = true;
        playerInputActions.Player.Sprint.canceled += context => IsSprinting = false;

        playerInputActions.Player.Attack.started += OnAttackStarted;
        playerInputActions.Player.Attack.canceled += OnAttackCanceled;

        playerInputActions.Player.Look.started += OnLookInput;
        playerInputActions.Player.Look.performed += OnLookInput;
        playerInputActions.Player.Look.canceled += OnLookInput;
    }

    private void OnEnable()
    {
        playerInputActions.Player.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Player.Disable();
    }

    private void OnAttackStarted(InputAction.CallbackContext context)
    {
        attackPressStartTime = Time.time;
        IsAttackHolding = true;
        IsAttackCanceled = false; // フラグ初期化
    }

    private void OnAttackCanceled(InputAction.CallbackContext context)
    {
        IsAttackHolding = false;

        float holdDuration = Time.time - attackPressStartTime;

        if (holdDuration >= heavyAttackHoldThreshold)
        {
            HeavyAttackInput = true;
            HeavyAttackReleasedInput = true;
        }
        else
        {
            //すでに溜め動作に入っている状態から短時間で離された場合はキャンセル扱いにする
            IsAttackCanceled = true;
            LightAttackInput = true; // タップ操作用の通常攻撃入力を無効化
        }
    }

    /// <summary>
    /// 攻撃ボタンが押されている時間を取得するメソッド
    /// </summary>
    /// <returns></returns>
    public float GetAttackHoldDuration()
    {
        return IsAttackHolding ? (Time.time - attackPressStartTime) : 0f;
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
        IsAttackCanceled = false; // ★追加: キャンセルフラグもリセット
    }
}