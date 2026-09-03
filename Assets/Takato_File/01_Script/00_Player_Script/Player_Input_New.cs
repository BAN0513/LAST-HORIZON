using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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

    // 攻撃入力用プロパティ（通常攻撃と強攻撃に分離）
    public bool LightAttackInput { get; private set; }
    public bool HeavyAttackInput { get; private set; }

    [Header("長押し判定設定")]
    [SerializeField] private float heavyAttackHoldThreshold;
    [Header("ダブルタップ判定設定")]
    [SerializeField] private float doubleTapThreshold;


    // 回避用のプロパティと内部変数
    public bool RollInput { get; private set; }
    public RollType CurrentRollType { get; private set; } = RollType.None;

    private float lastForwardTapTime = 0f;
    private float lastBackwardTapTime = 0f;
    private float attackPressStartTime = 0f;


    private void Awake()
    {
        playerInputActions = new InputSystem_Actions();

        // Move入力イベント登録
        playerInputActions.Player.Move.started += OnMoveInput;
        playerInputActions.Player.Move.performed += OnMoveInput;
        playerInputActions.Player.Move.canceled += OnMoveInput;

        // Jump入力イベント登録
        playerInputActions.Player.Jump.started += context => JumpInput = true;
        playerInputActions.Player.Jump.canceled += context => JumpInput = false;

        // Sprint入力イベント登録
        playerInputActions.Player.Sprint.started += context => IsSprinting = true;
        playerInputActions.Player.Sprint.canceled += context => IsSprinting = false;

        // Attack入力イベント（タップ/長押し判定）
        playerInputActions.Player.Attack.started += OnAttackStarted;
        playerInputActions.Player.Attack.canceled += OnAttackCanceled;

        // Look入力イベント登録
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
    }

    private void OnAttackCanceled(InputAction.CallbackContext context)
    {
        float holdDuration = Time.time - attackPressStartTime;

        if (holdDuration >= heavyAttackHoldThreshold)
        {
            HeavyAttackInput = true;
        }
        else
        {
            LightAttackInput = true;
        }
    }

    /// <summary>
    /// プレイヤーの移動入力を処理するメソッド
    /// </summary>
    private void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 newInput = context.ReadValue<Vector2>();

        if (context.started)
        {
            // Wキーまたは上矢印キーのダブルタップ判定 (前転)
            if (newInput.y > 0.5f)
            {
                if (Time.time - lastForwardTapTime <= doubleTapThreshold)
                {
                    RollInput = true;
                    CurrentRollType = RollType.Forward;
                }
                lastForwardTapTime = Time.time;
            }
            // Sキーまたは下矢印キーのダブルタップ判定 (後転)
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

        MoveInput = newInput;
    }

    /// <summary>
    /// 視点移動の入力を処理するメソッド
    /// </summary>
    private void OnLookInput(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// 回避入力消費後にフラグをリセットするメソッド
    /// </summary>
    public void ResetRollInput()
    {
        RollInput = false;
        CurrentRollType = RollType.None;
    }

    /// <summary>
    /// 攻撃入力消費後にフラグをリセットするメソッド
    /// </summary>
    public void ResetAttackInput()
    {
        LightAttackInput = false;
        HeavyAttackInput = false;
    }
}