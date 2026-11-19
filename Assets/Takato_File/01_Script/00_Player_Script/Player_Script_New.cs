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

    // 体力・死亡・状態関連プロパティ
    public float CurrentHealth { get; private set; }
    public float CurrentStamina { get; private set; }
    public bool IsDead => isDead;
    public bool IsBlocking => isBlocking; // 外部からブロック中か参照可能にする

    // イベント定義
    public event Action<float, float> OnHealthChanged;  // (現在体力, 最大体力)
    public event Action<float, float> OnStaminaChanged; // (現在スタミナ, 最大スタミナ)
    public event Action OnPlayerDied;                   // 死亡時イベント

    // 他スクリプトの参照
    private Player_Input_New playerInput;
    private Player_Animation_New playerAnimation;
    private Player_WeaponController_New playerWeaponController;
    private Player_SkillController_New playerSkillController;

    // コンポーネントの参照
    private CharacterController characterController;

    // 内部状態
    private bool isGrounded;
    private Vector3 velocity;
    private Vector3 currentMoveVelocity;
    private float staminaRegenTimer;
    private float chargeStartTime;

    // フラグ
    private bool isRolling = false;
    private bool isAttacking = false;
    private bool isDead = false;
    private bool isCharging = false;
    private bool isBlocking = false; // ブロック状態フラグ
    private Vector3 rollDirection;

    private const float GroundedDownwardForce = -2f; // 地面に接地しているときの下方向の力

    private float defaultHeight; // CharacterControllerのデフォルトの高さ
    private Vector3 defaultCenter;// CharacterControllerのデフォルトの中心位置

    private void Awake()
    {
        playerInput = GetComponent<Player_Input_New>();
        playerAnimation = GetComponent<Player_Animation_New>();
        playerWeaponController = GetComponent<Player_WeaponController_New>();
        characterController = GetComponent<CharacterController>();
        playerSkillController = GetComponent<Player_SkillController_New>();

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

        // ブロック状態の更新
        UpdateBlockState();

        // ブロック中は常にカメラ方向を向き、通常時は移動中のみカメラ方向を向く
        if (isBlocking)
        {
            RotatePlayerToCamera();
        }
        else if (!isRolling && !isAttacking && !isCharging && playerInput != null && playerInput.MoveInput.sqrMagnitude > 0.01f)
        {
            RotatePlayerToCamera();
        }

        MovePlayer(); // プレイヤーの移動処理を呼び出す

        // スキル入力の処理（ブロック中は発動不可）
        if (!isRolling && !isAttacking && !isCharging && !isBlocking && playerInput != null && playerInput.IsSkillInput)
        {
            if (playerSkillController != null)
            {
                playerSkillController.UseSkill(0);
            }
            playerInput.ResetSkillInput();
        }

        // スキル2の入力処理
        if (!isRolling && !isAttacking && !isCharging && !isBlocking && playerInput != null && playerInput.IsSkillInput2)
        {
            if (playerSkillController != null)
            {
                playerSkillController.UseSkill(1);
            }
            playerInput.ResetSkillInput();
        }

        // スキル3の入力処理
        if (!isRolling && !isAttacking && !isCharging && !isBlocking && playerInput != null && playerInput.IsSkillInput3)
        {
            if (playerSkillController != null)
            {
                playerSkillController.UseSkill(2);
            }
            playerInput.ResetSkillInput();
        }

        // スキル4の入力処理
        if (!isRolling && !isAttacking && !isCharging && !isBlocking && playerInput != null && playerInput.IsSkillInput4)
        {
            if (playerSkillController != null)
            {
                playerSkillController.UseSkill(3);
            }
            playerInput.ResetSkillInput();
        }

        // 溜め状態の分岐
        if (isCharging)
        {
            RotatePlayerToCamera(); // 溜め中は常にカメラの方向に向く

            if (playerInput != null)
            {
                if (!playerInput.IsAttackHolding)
                {
                    ReleaseChargeAttack();
                }
            }
            ApplyGravity();
            return;
        }

        Vector2 moveInput = playerInput != null ? playerInput.MoveInput : Vector2.zero;
        bool isSprinting = !isBlocking && playerInput != null && playerInput.IsSprinting && moveInput.y > 0f && CurrentStamina > 0f;

        if (!isRolling && !isBlocking && isGrounded && playerInput != null)
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
            TakeDamage(10f); // デバッグ用にTキーで10のダメージを受ける
        }
#endif
    }

    /// <summary>
    /// ブロック入力と現在のアクション状態からブロックアニメーションを切り替えるメソッド
    /// </summary>
    private void UpdateBlockState()
    {
        bool canBlock = isGrounded && !isRolling && !isAttacking && !isCharging;
        bool shouldBlock = canBlock && playerInput != null && playerInput.IsBlockInput;

        if (isBlocking != shouldBlock)
        {
            isBlocking = shouldBlock;

            if (playerAnimation != null)
            {
                playerAnimation.PlayBlock(isBlocking);
            }
        }
    }

    /// <summary>
    /// ダメージを受ける処理を行うメソッド
    /// </summary>
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
        isBlocking = false;

        if (playerAnimation != null)
        {
            playerAnimation.PlayBlock(false);
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

        // プレイヤーの移動入力を取得
        Vector2 moveInput = playerInput.MoveInput;

        // カメラの前方向と右方向を取得し、Y軸を無視して正規化
        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 cameraRight = Vector3.Scale(cameraTransform.right, new Vector3(1, 0, 1)).normalized;

        // カメラの方向に基づいて移動方向を計算
        Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

        // スプリントが要求されているかどうかを判定（ブロック中はスプリント不可）
        bool isSprintingRequested = !isBlocking && playerInput.IsSprinting && moveInput.y > 0f && CurrentStamina > 0f;

        // 現在の移動速度倍率を初期化
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
            // スタミナ回復処理
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

        // スキルによる移動速度の倍率を取得
        float speedMultiplierFromSkill = playerSkillController != null ? playerSkillController.CurrentSpeedMultiplier : 1f;

        // 最終的な移動速度を計算
        Vector3 targetVelocity = moveDirection * (playerSO.MoveSpeed * currentSpeedMultiplier * speedMultiplierFromSkill);

        // 移動速度の変化率を決定するための加速・減速の係数を設定
        float rate = moveInput.sqrMagnitude > 0f ? playerSO.AccelerationMultiplier : playerSO.DecelerationMultiplier;

        // 現在の移動速度を目標速度に向かって滑らかに変化させる
        currentMoveVelocity = Vector3.MoveTowards(currentMoveVelocity, targetVelocity, rate * Time.deltaTime);

        // CharacterControllerを使用してプレイヤーを移動させる
        characterController.Move(currentMoveVelocity * Time.deltaTime);

        // アニメーションの更新
        if (playerAnimation != null)
        {
            Vector3 relativeVelocity = transform.InverseTransformDirection(currentMoveVelocity);
            playerAnimation.UpdateMoveAnimation(relativeVelocity, playerSO.MoveSpeed);
        }
    }

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

    private void StartCharging()
    {
        isCharging = true;
        isAttacking = true;
        chargeStartTime = Time.time;

        RotatePlayerToCamera();

        Debug.Log("<color=cyan>[Charge Attack] 溜めを開始しました。</color>");

        if (playerAnimation != null)
        {
            playerAnimation.PlayChargeAttack(true);
        }
    }

    private void ReleaseChargeAttack()
    {
        isCharging = false;

        Weapon_SO_New currentWeaponSO = playerWeaponController.CurrentWeaponData;

        if (currentWeaponSO != null && currentWeaponSO.MaxChargeTime > 0f)
        {
            float chargeTime = Time.time - chargeStartTime;
            float chargeRatio = chargeTime / currentWeaponSO.MaxChargeTime;
            chargeRatio = Mathf.Clamp01(chargeRatio);

            Debug.Log($"<color=yellow>[Charge Attack] 溜め解放！ 純粋溜め時間: {chargeTime:F2}秒 | 溜め率: {chargeRatio * 100:F0}%</color>");

            playerWeaponController.SetChargeRatio(chargeRatio);
        }

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

    private void OnAttackEndHandler()
    {
        isAttacking = false;
        isCharging = false;
    }

    private void Jump()
    {
        CurrentStamina = Mathf.Max(0f, CurrentStamina - playerSO.JumpStaminaCost);
        staminaRegenTimer = playerSO.StaminaRegenDelay;
        OnStaminaChanged?.Invoke(CurrentStamina, playerSO.MaxStamina);

        velocity.y = Mathf.Sqrt(playerSO.JumpHeight * 2f * playerSO.GravityScale);

        if (playerAnimation != null)
        {
            playerAnimation.PlayJump();
        }
    }
}