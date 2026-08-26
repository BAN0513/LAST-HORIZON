using UnityEngine;

/// <summary>
/// プレイヤーの移動等のステータスを管理するクラス(New)
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class Player_Script_New : MonoBehaviour
{
    [Header("プレイヤーデータ参照")]
    [SerializeField] private PlayerSO_New playerSO;
    [Space(10)]

    [Header("前転時のCharacterController設定")]
    [SerializeField] private float rollHeight; // ロール中の高さ
    [SerializeField] private float rollCenterY; // ロール中の中心Y座標

    // 他スクリプトの参照
    private Player_Input_New playerInput;
    private Player_Animation_New playerAnimation;

    // コンポーネントの参照
    private CharacterController characterController;

    // 内部状態
    private bool isGrounded;
    private Vector3 velocity;
    private Vector3 currentMoveVelocity;

    // ロール状態のフラグと前進方向の保持
    private bool isRolling = false;
    private Vector3 rollDirection;

    // 変数の初期化
    private const float GroundedDownwardForce = -2f;

    // デフォルトのCharacterControllerサイズ設定値
    private float defaultHeight;
    private Vector3 defaultCenter;

    private void Awake()
    {
        playerInput = GetComponent<Player_Input_New>();
        playerAnimation = GetComponent<Player_Animation_New>();
        characterController = GetComponent<CharacterController>();

        // デフォルトの高さと中心位置を記録
        if (characterController != null)
        {
            defaultHeight = characterController.height;
            defaultCenter = characterController.center;
        }
    }

    private void OnEnable()
    {
        // アニメーション終了イベントの登録
        if (playerAnimation != null)
        {
            playerAnimation.OnRollEnd += OnRollEndHandler;
        }
    }

    private void OnDisable()
    {
        // イベント解除
        if (playerAnimation != null)
        {
            playerAnimation.OnRollEnd -= OnRollEndHandler;
        }
    }

    private void Update()
    {
        if (characterController == null || playerSO == null) return;

        isGrounded = characterController.isGrounded;

        if (playerAnimation != null)
        {
            playerAnimation.UpdateGroundedState(isGrounded);
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = GroundedDownwardForce;
        }

        MovePlayer(); // プレイヤーの移動処理を呼び出す

        // ロール中ではない場合のみ新規ロールやジャンプを受け付ける
        if (!isRolling)
        {
            if (playerInput != null && playerInput.RollInput && isGrounded)
            {
                Roll(); // 前転処理を呼び出す
            }

            if (playerInput != null && playerInput.JumpInput && isGrounded)
            {
                Jump(); // ジャンプ処理を呼び出す
            }
        }

        ApplyGravity(); // 重力処理を呼び出す
    }

    /// <summary>
    /// プレイヤーの重力処理を行うメソッド
    /// </summary>
    private void ApplyGravity()
    {
        velocity.y -= playerSO.GravityScale * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// プレイヤーの移動処理を行うメソッド
    /// </summary>
    private void MovePlayer()
    {
        if (playerInput == null) return;

        // ロール中の場合は強力な前方推進力を適用する
        if (isRolling)
        {
            currentMoveVelocity = rollDirection * (playerSO.MoveSpeed * playerSO.RollSpeedMultiplier);
            characterController.Move(currentMoveVelocity * Time.deltaTime);
            return;
        }

        Vector2 moveInput = playerInput.MoveInput;

        bool isForwardSprinting = playerInput.IsSprinting && moveInput.y > 0f;
        float currentSpeedMultiplier = isForwardSprinting ? playerSO.SpeedMultiplier : 1f;

        Vector3 targetVelocity = new Vector3(moveInput.x, 0f, moveInput.y).normalized * (playerSO.MoveSpeed * currentSpeedMultiplier);

        float rate = moveInput.sqrMagnitude > 0f ? playerSO.AccelerationMultiplier : playerSO.DecelerationMultiplier;

        currentMoveVelocity = Vector3.MoveTowards(currentMoveVelocity, targetVelocity, rate * Time.deltaTime);

        characterController.Move(currentMoveVelocity * Time.deltaTime);

        if (playerAnimation != null)
        {
            playerAnimation.UpdateMoveAnimation(currentMoveVelocity, playerSO.MoveSpeed);
        }
    }

    /// <summary>
    /// 前転処理を行うメソッド
    /// </summary>
    private void Roll()
    {
        if (playerInput == null) return;

        isRolling = true; // ロール状態開始

        // 入力方向があればその方向、なければプレイヤーの前方を推進方向に設定
        Vector3 inputDir = new Vector3(playerInput.MoveInput.x, 0f, playerInput.MoveInput.y).normalized;
        rollDirection = inputDir.sqrMagnitude > 0.01f ? inputDir : transform.forward;

        // CharacterControllerのサイズをロール用に縮小
        characterController.height = rollHeight;
        characterController.center = new Vector3(defaultCenter.x, rollCenterY, defaultCenter.z);

        if (playerAnimation != null)
        {
            playerAnimation.PlayRoll();
        }

        playerInput.ResetRollInput(); // 入力消費後にフラグをリセット
    }

    /// <summary>
    /// アニメーション終了イベントから呼び出される処理
    /// </summary>
    private void OnRollEndHandler()
    {
        // ロール状態解除
        isRolling = false;

        // CharacterControllerを元のサイズに戻す
        if (characterController != null)
        {
            characterController.height = defaultHeight;
            characterController.center = defaultCenter;
        }
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(playerSO.JumpHeight * 2f * playerSO.GravityScale);

        if (playerAnimation != null)
        {
            playerAnimation.PlayJump();
        }
    }
}