using UnityEngine;

/// <summary>
/// プレイヤーのアニメーションを管理するクラス(New)
/// </summary>
public class Player_Animation_New : MonoBehaviour
{
    private Animator animator;

    // パラメータハッシュ化
    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");

    [Header("アニメーション設定")]
    [SerializeField] private float dampTime;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// 2D Blend Tree 用の移動パラメータ(MoveX, MoveY)を更新する
    /// </summary>
    public void UpdateMoveAnimation(Vector3 moveVelocity, float baseMoveSpeed)
    {
        if (animator == null || baseMoveSpeed <= 0f) return;

        if (moveVelocity.magnitude < 0.01f)
        {
            moveVelocity = Vector3.zero;
        }

        float normalizedX = moveVelocity.x / baseMoveSpeed;
        float normalizedY = moveVelocity.z / baseMoveSpeed;

        animator.SetFloat(MoveXHash, normalizedX, dampTime, Time.deltaTime);
        animator.SetFloat(MoveYHash, normalizedY, dampTime, Time.deltaTime);
    }

    /// <summary>
    /// ジャンプ開始アニメーションを呼び出す
    /// </summary>
    public void PlayJump()
    {
        if (animator == null) return;
        animator.SetTrigger(JumpHash);
    }

    /// <summary>
    /// 接地状態のアニメーションパラメータを更新する
    /// </summary>
    public void UpdateGroundedState(bool isGrounded)
    {
        if (animator == null) return;
        animator.SetBool(IsGroundedHash, isGrounded);
    }
}