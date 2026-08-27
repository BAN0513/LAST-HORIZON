using Unity.Cinemachine;
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

    [Header("カメラの参照")]
    [SerializeField] private Transform cameraTransform; // カメラのTransform参照
    [Header("プレイヤーがカメラの向いている方向にどれくらい滑らかに向くかの度合い")]
    [Range(5f, 20f)]
    [SerializeField] private float rotationSmoothness; // プレイヤーの回転の滑らかさ

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

    /// <summary>
    /// イベントの登録処理を行うメソッド
    /// </summary>
    private void OnEnable()
    {
        // アニメーション終了イベントの登録
        if (playerAnimation != null)
        {
            playerAnimation.OnRollEnd += OnRollEndHandler;
        }
    }

    /// <summary>
    /// イベントの解除処理を行うメソッド
    /// </summary>
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

        //移動入力がある時のみ体をカメラの向きに合わせる（停止時はカメラだけ周回可能）
        if (playerInput != null && playerInput.MoveInput.sqrMagnitude > 0.01f)
        {
            RotatePlayerToCamera();// プレイヤーの体をカメラと同じ向きにする処理を呼び出す
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
    /// プレイヤーの体をカメラと同じ向きにする
    /// </summary>
    private void RotatePlayerToCamera()
    {
        if (cameraTransform == null) return;

        // Y軸の回転のみを抽出して適用
        float targetYaw = cameraTransform.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0f, targetYaw, 0f);

        // 滑らかに向かせる処理
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothness * Time.deltaTime);
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

        Vector2 moveInput = playerInput.MoveInput; // プレイヤーの移動入力を取得

        // カメラの正面・右方向を取得
        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 cameraRight = Vector3.Scale(cameraTransform.right, new Vector3(1, 0, 1)).normalized;

        // 入力値とカメラの向きを掛け合わせてワールド移動方向を算出
        Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

        bool isForwardSprinting = playerInput.IsSprinting && moveInput.y > 0f;
        float currentSpeedMultiplier = isForwardSprinting ? playerSO.SpeedMultiplier : 1f;

        // moveDirection をベースにターゲット速度を確定
        Vector3 targetVelocity = moveDirection * (playerSO.MoveSpeed * currentSpeedMultiplier);

        float rate = moveInput.sqrMagnitude > 0f ? playerSO.AccelerationMultiplier : playerSO.DecelerationMultiplier;

        currentMoveVelocity = Vector3.MoveTowards(currentMoveVelocity, targetVelocity, rate * Time.deltaTime);

        characterController.Move(currentMoveVelocity * Time.deltaTime);

        // アニメーション側にはプレイヤーから見たローカル速度を渡す
        if (playerAnimation != null)
        {
            Vector3 relativeVelocity = transform.InverseTransformDirection(currentMoveVelocity);
            playerAnimation.UpdateMoveAnimation(relativeVelocity, playerSO.MoveSpeed);
        }
    }

    /// <summary>
    /// 前転処理を行うメソッド
    /// </summary>
    private void Roll()
    {
        if (playerInput == null) return;

        isRolling = true; // ロール状態開始

        // 入力方向があればその方向（カメラ基準）、なければ体の正面方向
        Vector2 input = playerInput.MoveInput;
        if (input.sqrMagnitude > 0.01f)
        {
            Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 cameraRight = Vector3.Scale(cameraTransform.right, new Vector3(1, 0, 1)).normalized;
            rollDirection = (cameraForward * input.y + cameraRight * input.x).normalized;
        }
        else
        {
            rollDirection = transform.forward;
        }

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

    /// <summary>
    /// ジャンプ処理を行うメソッド
    /// </summary>
    private void Jump()
    {
        velocity.y = Mathf.Sqrt(playerSO.JumpHeight * 2f * playerSO.GravityScale);

        if (playerAnimation != null)
        {
            playerAnimation.PlayJump();
        }
    }
}