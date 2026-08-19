using UnityEngine;

/// <summary>
/// プレイヤーの移動等のステータスを管理するクラス(New)
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class Player_Script_New : MonoBehaviour
{
    [Header("プレイヤーデータ参照")]
    [SerializeField] private PlayerSO_New playerSO; // ScriptableObjectの参照

    // 他スクリプトの参照
    private Player_Input_New playerInput; // プレイヤーの入力を管理するクラスの参照
    private Player_Animation_New playerAnimation; // アニメーション管理クラスの参照

    // コンポーネントの参照
    private CharacterController characterController; // CharacterControllerコンポーネントの参照

    // 内部状態
    private bool isGrounded; // 地面に接地しているか
    private Vector3 velocity; // 上下方向の速度を保持
    private Vector3 currentMoveVelocity; // 水平方向の現在の移動速度を保持

    private void Awake()
    {
        playerInput = GetComponent<Player_Input_New>();           // Player_Input_Newコンポーネントの参照を取得
        playerAnimation = GetComponent<Player_Animation_New>();   // Player_Animation_Newコンポーネントの参照を取得
        characterController = GetComponent<CharacterController>();// CharacterControllerコンポーネントの参照を取得
    }

    private void Update()
    {
        if (characterController == null || playerSO == null) return;

        // 接地判定の更新
        isGrounded = characterController.isGrounded;

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

        ApplyGravity(); // 重力の計算と垂直移動の適用
    }

    /// <summary>
    /// 重力と垂直移動を適用するメソッド
    /// </summary>
    private void ApplyGravity()
    {
        // 重力を下向きに計算
        velocity.y -= playerSO.GravityScale * Time.deltaTime;

        // 計算した垂直速度をCharacterControllerに渡して移動させる
        characterController.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// プレイヤーの水平移動（加減速付き）を行うメソッド
    /// </summary>
    private void MovePlayer()
    {
        if (playerInput == null) return;

        Vector2 moveInput = playerInput.MoveInput;

        // 前方向かつスプリント入力中の場合のみ速度倍率を適用
        bool isForwardSprinting = playerInput.IsSprinting && moveInput.y > 0f;
        float currentSpeedMultiplier = isForwardSprinting ? playerSO.SpeedMultiplier : 1f;

        // 目標となる移動方向と最高速度のベクトル
        Vector3 targetVelocity = new Vector3(moveInput.x, 0f, moveInput.y).normalized * (playerSO.MoveSpeed * currentSpeedMultiplier);

        // 入力があるか判定して加減速の倍率を切り替え
        float rate = moveInput.sqrMagnitude > 0f ? playerSO.AccelerationMultiplier : playerSO.DecelerationMultiplier;

        // 現在の速度から目標速度へ徐々に変化させる
        currentMoveVelocity = Vector3.MoveTowards(currentMoveVelocity, targetVelocity, rate * Time.deltaTime);

        // 水平移動を適用
        characterController.Move(currentMoveVelocity * Time.deltaTime);

        // アニメーション側へ現在の移動速度情報を送る
        if (playerAnimation != null)
        {
            playerAnimation.UpdateMoveAnimation(currentMoveVelocity, playerSO.MoveSpeed);
        }
    }

    /// <summary>
    /// ジャンプ初速をセットするメソッド
    /// </summary>
    private void Jump()
    {
        // ジャンプの初速を計算して垂直速度にセット（SOの値を使用）
        velocity.y = Mathf.Sqrt(playerSO.JumpHeight * 2f * playerSO.GravityScale);
    }
}