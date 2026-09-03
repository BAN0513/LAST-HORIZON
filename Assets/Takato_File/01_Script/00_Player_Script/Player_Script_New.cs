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
    [SerializeField] private float rollHeight;
    [SerializeField] private float rollCenterY;

    [Header("カメラの参照")]
    [SerializeField] private Transform cameraTransform;
    [Header("プレイヤーがカメラの向いている方向にどれくらい滑らかに向くかの度合い")]
    [Range(5f, 20f)]
    [SerializeField] private float rotationSmoothness;

    // 他スクリプトの参照
    private Player_Input_New playerInput;
    private Player_Animation_New playerAnimation;

    // コンポーネントの参照
    private CharacterController characterController;

    // 内部状態
    private bool isGrounded;
    private Vector3 velocity;
    private Vector3 currentMoveVelocity;

    // ロール状態および攻撃状態のフラグ
    private bool isRolling = false;
    private bool isAttacking = false;
    private Vector3 rollDirection;

    private const float GroundedDownwardForce = -2f;

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
            playerAnimation.OnAttackEnd += OnAttackEndHandler;
        }
    }

    private void OnDisable()
    {
        if (playerAnimation != null)
        {
            playerAnimation.OnRollEnd -= OnRollEndHandler;
            playerAnimation.OnAttackEnd -= OnAttackEndHandler;
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

        // ロール中でなく移動入力がある場合はカメラの向きに回転
        if (!isRolling && playerInput != null && playerInput.MoveInput.sqrMagnitude > 0.01f)
        {
            RotatePlayerToCamera();
        }

        MovePlayer();

        // ロール中でなく接地している場合は攻撃を受け付ける
        if (!isRolling && isGrounded)
        {
            if (playerInput != null && !isAttacking)
            {
                if (playerInput.HeavyAttackInput)
                {
                    HeavyAttack(); // 強攻撃処理
                }
                else if (playerInput.LightAttackInput)
                {
                    LightAttack(); // 通常攻撃処理
                }
                else if (playerInput.RollInput)
                {
                    Roll();
                }
                else if (playerInput.JumpInput)
                {
                    Jump();
                }
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

        // ロール中の移動
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
    /// 通常攻撃処理を行うメソッド
    /// </summary>
    private void LightAttack()
    {
        if (playerInput == null) return;

        isAttacking = true;

        if (cameraTransform != null)
        {
            float targetYaw = cameraTransform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, targetYaw, 0f);
        }

        if (playerAnimation != null)
        {
            playerAnimation.PlayLightAttack();
        }

        playerInput.ResetAttackInput();
    }

    /// <summary>
    /// 強攻撃処理を行うメソッド
    /// </summary>
    private void HeavyAttack()
    {
        if (playerInput == null) return;

        isAttacking = true;

        if (cameraTransform != null)
        {
            float targetYaw = cameraTransform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, targetYaw, 0f);
        }

        if (playerAnimation != null)
        {
            playerAnimation.PlayHeavyAttack();
        }

        playerInput.ResetAttackInput();
    }

    private void Roll()
    {
        if (playerInput == null) return;

        isRolling = true;

        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;

        if (playerInput.CurrentRollType == RollType.Backward)
        {
            rollDirection = -cameraForward;

            if (playerAnimation != null)
            {
                playerAnimation.PlayBackRoll();
            }
        }
        else
        {
            rollDirection = cameraForward;

            if (playerAnimation != null)
            {
                playerAnimation.PlayRoll();
            }
        }

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

    /// <summary>
    /// アニメーションイベントから呼ばれる攻撃終了ハンドラー
    /// </summary>
    private void OnAttackEndHandler()
    {
        isAttacking = false;
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