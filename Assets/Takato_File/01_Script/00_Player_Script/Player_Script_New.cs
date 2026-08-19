using UnityEngine;

/// <summary>
/// プレイヤーの移動等のステータスを管理するクラス(New)
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class Player_Script_New : MonoBehaviour
{
    [Header("プレイヤーデータ参照")]
    [SerializeField] private PlayerSO_New playerSO;

    // 他スクリプトの参照
    private Player_Input_New playerInput;
    private Player_Animation_New playerAnimation;

    // コンポーネントの参照
    private CharacterController characterController;

    // 内部状態
    private bool isGrounded;
    private Vector3 velocity;
    private Vector3 currentMoveVelocity;

    private void Awake()
    {
        playerInput = GetComponent<Player_Input_New>();
        playerAnimation = GetComponent<Player_Animation_New>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (characterController == null || playerSO == null) return;

        // 接地判定の更新
        isGrounded = characterController.isGrounded;

        // アニメーション側へ接地状態を通知
        if (playerAnimation != null)
        {
            playerAnimation.UpdateGroundedState(isGrounded);
        }

        // 地面にいる間はわずかに下向きの力を与えて着地状態を安定させる
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 水平移動の処理
        MovePlayer();

        // ジャンプ処理
        if (playerInput != null && playerInput.JumpInput && isGrounded)
        {
            Jump();
        }

        ApplyGravity();
    }

    /// <summary>
    /// 重力を下向きに計算し、垂直移動を適用するメソッド
    /// </summary>
    private void ApplyGravity()
    {
        velocity.y -= playerSO.GravityScale * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// プレイヤーの水平移動を行うメソッド
    /// </summary>
    private void MovePlayer()
    {
        if (playerInput == null) return;

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
    /// ジャンプ初速をセットし、アニメーションを再生するメソッド
    /// </summary>
    private void Jump()
    {
        velocity.y = Mathf.Sqrt(playerSO.JumpHeight * 2f * playerSO.GravityScale);  // ジャンプ初速の計算

        // ジャンプアニメーションの再生
        if (playerAnimation != null)
        {
            playerAnimation.PlayJump();
        }
    }
}