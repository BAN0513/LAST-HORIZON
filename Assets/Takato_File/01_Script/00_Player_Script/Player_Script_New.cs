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

    // イベント定義
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
    private bool isCharging = false;
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

        if (!isRolling && !isAttacking && !isCharging && playerInput != null && playerInput.MoveInput.sqrMagnitude > 0.01f)
        {
            RotatePlayerToCamera(); // 移動中は常にカメラの方向に向く
        }

        MovePlayer();

        // 溜め状態の分岐
        if (isCharging)
        {
            RotatePlayerToCamera(); // 溜め中は常にカメラの方向に向く

            if (playerInput != null)
            {
                // ★十分な溜め時間クリア後に離された場合 -> 溜め強攻撃を発動
                if (playerInput.HeavyAttackReleasedInput)
                {
                    ReleaseChargeAttack(); // 溜め攻撃の発動処理を呼び出す
                }
                // ★溜め時間未満で離された（キャンセルされた）場合 -> 何もしないで元に戻る
                else if (playerInput.IsAttackCanceled || !playerInput.IsAttackHolding)
                {
                    CancelChargeAttack(); // 溜め攻撃のキャンセル処理を呼び出す
                }
            }
            ApplyGravity(); // 重力の適用
            return;
        }

        Vector2 moveInput = playerInput != null ? playerInput.MoveInput : Vector2.zero;
        bool isSprinting = playerInput != null && playerInput.IsSprinting && moveInput.y > 0f && CurrentStamina > 0f;

        if (!isRolling && isGrounded && playerInput != null)
        {
            if (isSprinting)
            {
                if (playerInput.LightAttackInput || playerInput.HeavyAttackInput)
                {
                    playerInput.ResetAttackInput(); // スプリント中は攻撃入力を無効化
                }
            }
            else if (!isAttacking)
            {
                if (playerInput.IsAttackHolding && playerInput.GetAttackHoldDuration() >= 0.2f)
                {
                    StartCharging(); // 溜め攻撃処理を呼び出す
                }
                else if (playerInput.LightAttackInput)
                {
                    LightAttack(); // 軽攻撃処理を呼び出す
                }
            }

            if (!isAttacking && !isCharging)
            {
                if (playerInput.RollInput)
                {
                    Roll(); // 前転・後転処理を呼び出す
                }
                else if (playerInput.JumpInput && CurrentStamina >= playerSO.JumpStaminaCost)
                {
                    Jump(); // ジャンプ処理を呼び出す
                }
            }
        }

        ApplyGravity(); // 重力の適用

#if UNITY_EDITOR
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            TakeDamage(10f);
        }
#endif
    }

    /// <summary>
    /// ダメージを受ける処理を行うメソッド
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage)
    {
        if (isDead || damage <= 0f) return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);
        OnHealthChanged?.Invoke(CurrentHealth, playerSO.MaxHealth);

        if (CurrentHealth <= 0f)
        {
            Die(); // 死亡処理を呼び出す
        }
        else
        {
            if (playerAnimation != null)
            {
                playerAnimation.PlayTakeDamage(); // ダメージを受けたアニメーションを再生
            }
        }
    }

    /// <summary>
    /// プレイヤーが回復を行う処理を行うメソッド
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(float amount)
    {
        if (isDead || amount <= 0f) return;

        CurrentHealth = Mathf.Min(playerSO.MaxHealth, CurrentHealth + amount);
        OnHealthChanged?.Invoke(CurrentHealth, playerSO.MaxHealth);
    }

    /// <summary>
    /// プレイヤーが死亡した際の処理を行うメソッド
    /// </summary>
    private void Die()
    {
        isDead = true;
        isAttacking = false;
        isRolling = false;
        isCharging = false;

        if (playerAnimation != null)
        {
            playerAnimation.PlayDie();
        }

        OnPlayerDied?.Invoke();
    }

    /// <summary>
    /// プレイヤーがカメラの向いている方向に滑らかに回転する処理を行うメソッド
    /// </summary>
    private void RotatePlayerToCamera()
    {
        if (cameraTransform == null) return;

        float targetYaw = cameraTransform.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0f, targetYaw, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothness * Time.deltaTime);
    }

    /// <summary>
    /// プレイヤーが重力の影響を受ける処理を行うメソッド
    /// </summary>
    private void ApplyGravity()
    {
        velocity.y -= playerSO.GravityScale * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// プレイヤーが移動する処理を行うメソッド
    /// </summary>
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

        bool isSprintingRequested = playerInput.IsSprinting && moveInput.y > 0f && CurrentStamina > 0f;

        float currentSpeedMultiplier = 1f;

        if (isSprintingRequested && moveDirection.sqrMagnitude > 0.01f)
        {
            currentSpeedMultiplier = playerSO.SpeedMultiplier;

            CurrentStamina = Mathf.Max(0f, CurrentStamina - playerSO.StaminaDrainRate * Time.deltaTime);
            staminaRegenTimer = playerSO.StaminaRegenDelay;
            OnStaminaChanged?.Invoke(CurrentStamina, playerSO.MaxStamina);
        }
        else
        {
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
    /// 軽攻撃の処理を行うメソッド
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
    /// プレイヤーが溜め攻撃を開始する処理
    /// </summary>
    private void StartCharging()
    {
        isCharging = true;
        isAttacking = true;

        RotatePlayerToCamera();

        if (playerAnimation != null)
        {
            playerAnimation.PlayChargeAttack(true);
        }
    }

    private void ReleaseChargeAttack()
    {
        isCharging = false;

        if (playerAnimation != null)
        {
            playerAnimation.PlayChargeAttack(false);
            playerAnimation.PlayHeavyAttack();
        }

        if (playerInput != null)
        {
            playerInput.ResetAttackInput();
        }
    }

    /// <summary>
    ///溜め攻撃のキャンセル（しきい値未満で離された場合）
    /// </summary>
    private void CancelChargeAttack()
    {
        isCharging = false;
        isAttacking = false; // 攻撃ステートを解除

        if (playerAnimation != null)
        {
            playerAnimation.PlayChargeAttack(false); // 溜めBoolを解除して待機モーションへ戻す
        }

        if (playerInput != null)
        {
            playerInput.ResetAttackInput(); // 攻撃入力をリセット
        }
    }

    /// <summary>
    /// 回避（前転・後転）の処理を行うメソッド
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
                playerAnimation.PlayBackRoll(); // 後転アニメーションを再生
            }
        }
        else
        {
            rollDirection = cameraForward;

            if (playerAnimation != null)
            {
                playerAnimation.PlayRoll(); // 前転アニメーションを再生
            }
        }

        characterController.height = rollHeight;
        characterController.center = new Vector3(defaultCenter.x, rollCenterY, defaultCenter.z);

        playerInput.ResetRollInput(); // ロール入力をリセット
    }

    /// <summary>
    /// プレイヤーがロールアニメーションを終了した際の処理を行うメソッド
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
    /// プレイヤーが攻撃アニメーションを終了した際の処理を行うメソッド
    /// </summary>
    private void OnAttackEndHandler()
    {
        isAttacking = false;
        isCharging = false;
    }

    /// <summary>
    /// プレイヤーがジャンプする処理を行うメソッド
    /// </summary>
    private void Jump()
    {
        CurrentStamina = Mathf.Max(0f, CurrentStamina - playerSO.JumpStaminaCost);
        staminaRegenTimer = playerSO.StaminaRegenDelay;
        OnStaminaChanged?.Invoke(CurrentStamina, playerSO.MaxStamina);

        velocity.y = Mathf.Sqrt(playerSO.JumpHeight * 2f * playerSO.GravityScale);

        if (playerAnimation != null)
        {
            playerAnimation.PlayJump(); // ジャンプアニメーションを再生
        }
    }
}