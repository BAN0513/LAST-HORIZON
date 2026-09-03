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

    [Header("前転・後転時のCharacterController設定")]
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

    // ロール状態のフラグと前進/後退方向の保持
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

        if (characterController != null)
        {
            defaultHeight = characterController.height;
            defaultCenter = characterController.center;
        }
    }

    private void OnEnable()
    {
        if (playerAnimation != null)
        {
            playerAnimation.OnRollEnd += OnRollEndHandler;
        }
    }

    private void OnDisable()
    {
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

        // 移動入力がある時のみ体をカメラの向きに合わせる（ロール中は向き固定）
        if (!isRolling && playerInput != null && playerInput.MoveInput.sqrMagnitude > 0.01f)
        {
            RotatePlayerToCamera();
        }

        MovePlayer();

        if (!isRolling)
        {
            if (playerInput != null && playerInput.RollInput && isGrounded)
            {
                Roll();
            }

            if (playerInput != null && playerInput.JumpInput && isGrounded)
            {
                Jump();
            }
        }

        ApplyGravity();
    }

    private void RotatePlayerToCamera()
    {
        if (cameraTransform == null) return;

        float targetYaw = cameraTransform.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0f, targetYaw, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothness * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        velocity.y -= playerSO.GravityScale * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void MovePlayer()
    {
        if (playerInput == null) return;

        if (isRolling)
        {
            currentMoveVelocity = rollDirection * (playerSO.MoveSpeed * playerSO.RollSpeedMultiplier);
            characterController.Move(currentMoveVelocity * Time.deltaTime);
            return;
        }

        Vector2 moveInput = playerInput.MoveInput;

        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 cameraRight = Vector3.Scale(cameraTransform.right, new Vector3(1, 0, 1)).normalized;

        Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

        bool isForwardSprinting = playerInput.IsSprinting && moveInput.y > 0f;
        float currentSpeedMultiplier = isForwardSprinting ? playerSO.SpeedMultiplier : 1f;

        Vector3 targetVelocity = moveDirection * (playerSO.MoveSpeed * currentSpeedMultiplier);

        float rate = moveInput.sqrMagnitude > 0f ? playerSO.AccelerationMultiplier : playerSO.DecelerationMultiplier;

        currentMoveVelocity = Vector3.MoveTowards(currentMoveVelocity, targetVelocity, rate * Time.deltaTime);

        characterController.Move(currentMoveVelocity * Time.deltaTime);

        if (playerAnimation != null)
        {
            Vector3 relativeVelocity = transform.InverseTransformDirection(currentMoveVelocity);
            playerAnimation.UpdateMoveAnimation(relativeVelocity, playerSO.MoveSpeed);
        }
    }

    /// <summary>
    /// 前転・後転処理を行うメソッド
    /// </summary>
    private void Roll()
    {
        if (playerInput == null) return;

        isRolling = true;

        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;

        // ダブルタップの方向によって移動ベクトルと再生アニメーションを分岐
        if (playerInput.CurrentRollType == RollType.Backward)
        {
            rollDirection = -cameraForward; // カメラの後方へ退避

            if (playerAnimation != null)
            {
                playerAnimation.PlayBackRoll();
            }
        }
        else // RollType.Forward
        {
            rollDirection = cameraForward; // カメラの前方へ回避

            if (playerAnimation != null)
            {
                playerAnimation.PlayRoll();
            }
        }

        // CharacterControllerのサイズを回避用に縮小
        characterController.height = rollHeight;
        characterController.center = new Vector3(defaultCenter.x, rollCenterY, defaultCenter.z);

        playerInput.ResetRollInput();
    }

    private void OnRollEndHandler()
    {
        isRolling = false;

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