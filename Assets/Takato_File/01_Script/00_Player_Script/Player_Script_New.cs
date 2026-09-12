using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

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

    // 体力・死亡関連プロパティ
    public float CurrentHealth { get; private set; }
    public float CurrentStamina { get; private set; }
    public bool IsDead => isDead;

    // イベント定義 (UI等との通知用)
    public event Action<float, float> OnHealthChanged;  // (現在体力, 最大体力)
    public event Action<float, float> OnStaminaChanged; // (現在スタミナ, 最大スタミナ)
    public event Action OnPlayerDied;                   // 死亡時イベント

    // 他スクリプトの参照
    private Player_Input_New playerInput;
    private Player_Animation_New playerAnimation;

    // コンポーネントの参照
    private CharacterController characterController;

    // 内部状態
    private bool isGrounded;
    private Vector3 velocity;
    private Vector3 currentMoveVelocity;
    private float staminaRegenTimer;

    // フラグ
    private bool isRolling = false;
    private bool isAttacking = false;
    private bool isDead = false;
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

    private void Start()
    {
        // 体力・スタミナの初期化
        if (playerSO != null)
        {
            CurrentHealth = playerSO.MaxHealth;
            CurrentStamina = playerSO.MaxStamina;

            OnHealthChanged?.Invoke(CurrentHealth, playerSO.MaxHealth);
            OnStaminaChanged?.Invoke(CurrentStamina, playerSO.MaxStamina);
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
        // 死亡時や必要なコンポーネントがない場合は処理を行わない
        if (characterController == null || playerSO == null || isDead) return;

        isGrounded = characterController.isGrounded;

        if (playerAnimation != null)
        {
            playerAnimation.UpdateGroundedState(isGrounded);
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = GroundedDownwardForce;
        }

        // ロール中・攻撃中でなく移動入力がある場合はカメラの向きに回転
        if (!isRolling && !isAttacking && playerInput != null && playerInput.MoveInput.sqrMagnitude > 0.01f)
        {
            RotatePlayerToCamera(); // カメラの向きに回転
        }

        MovePlayer(); // 移動処理

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
                    Roll(); // ロール処理
                }
                else if (playerInput.JumpInput)
                {
                    Jump(); // ジャンプ処理
                }
            }
        }

        ApplyGravity(); // 重力処理

#if UNITY_EDITOR
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            TakeDamage(10f); // Tキーを押したときに10ダメージ
        }
#endif
    }

    /// <summary>
    /// ダメージ受傷処理
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (isDead || damage <= 0f) return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);
        OnHealthChanged?.Invoke(CurrentHealth, playerSO.MaxHealth);

        if (CurrentHealth <= 0f)
        {
            Die();
        }
        else
        {
            if (playerAnimation != null)
            {
                playerAnimation.PlayTakeDamage();
            }
        }
    }

    /// <summary>
    /// 回復処理
    /// </summary>
    public void Heal(float amount)
    {
        if (isDead || amount <= 0f) return;

        CurrentHealth = Mathf.Min(playerSO.MaxHealth, CurrentHealth + amount);
        OnHealthChanged?.Invoke(CurrentHealth, playerSO.MaxHealth);
    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    private void Die()
    {
        isDead = true;
        isAttacking = false;
        isRolling = false;

        if (playerAnimation != null)
        {
            playerAnimation.PlayDie(); // 死亡アニメーション再生
        }

        OnPlayerDied?.Invoke(); // 死亡イベント通知
    }

    /// <summary>
    /// プレイヤーをカメラの向きに回転させるメソッド
    /// </summary>
    private void RotatePlayerToCamera()
    {
        if (cameraTransform == null) return;

        float targetYaw = cameraTransform.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0f, targetYaw, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothness * Time.deltaTime);
    }

    /// <summary>
    /// プレイヤーに重力を適用するメソッド
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

        // ロール中の移動
        if (isRolling)
        {
            currentMoveVelocity = rollDirection * (playerSO.MoveSpeed * playerSO.RollSpeedMultiplier);
            characterController.Move(currentMoveVelocity * Time.deltaTime);
            return;
        }

        // 攻撃中の移動停止
        if (isAttacking)
        {
            currentMoveVelocity = Vector3.zero;
            if (playerAnimation != null)
            {
                playerAnimation.UpdateMoveAnimation(Vector3.zero, playerSO.MoveSpeed);
            }
            return;
        }

        Vector2 moveInput = playerInput.MoveInput; // 入力ベクトル (x: 水平方向, y: 前後方向)

        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 cameraRight = Vector3.Scale(cameraTransform.right, new Vector3(1, 0, 1)).normalized;

        Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

        // 前進入力かつスプリント入力があり、スタミナが残っているか判定
        bool isSprintingRequested = playerInput.IsSprinting && moveInput.y > 0f && CurrentStamina > 0f;

        float currentSpeedMultiplier = 1f;

        if (isSprintingRequested && moveDirection.sqrMagnitude > 0.01f)
        {
            currentSpeedMultiplier = playerSO.SpeedMultiplier;

            // スタミナ消費
            CurrentStamina = Mathf.Max(0f, CurrentStamina - playerSO.StaminaDrainRate * Time.deltaTime);
            staminaRegenTimer = playerSO.StaminaRegenDelay; // 回復タイマーリセット
            OnStaminaChanged?.Invoke(CurrentStamina, playerSO.MaxStamina);
        }
        else
        {
            // スタミナ自動回復
            if (staminaRegenTimer > 0f)
            {
                staminaRegenTimer -= Time.deltaTime;
            }
            else if (CurrentStamina < playerSO.MaxStamina)
            {
                CurrentStamina = Mathf.Min(playerSO.MaxStamina, CurrentStamina + playerSO.StaminaRegenRate * Time.deltaTime);
                OnStaminaChanged?.Invoke(CurrentStamina, playerSO.MaxStamina);
            }
        }

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

        playerInput.ResetAttackInput(); // 攻撃入力をリセット
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

    /// <summary>
    /// プレイヤーのロール処理を行うメソッド
    /// </summary>
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

    /// <summary>
    /// プレイヤーのロール終了時に呼ばれるハンドラー
    /// </summary>
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