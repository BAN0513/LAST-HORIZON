using System;
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
    private static readonly int RollHash = Animator.StringToHash("Roll");
    private static readonly int BackRollHash = Animator.StringToHash("BackRoll");
    private static readonly int LightAttackHash = Animator.StringToHash("LightAttack");
    private static readonly int HeavyAttackHash = Animator.StringToHash("HeavyAttack");
    private static readonly int TakeDamageHash = Animator.StringToHash("TakeDamage");
    private static readonly int DieHash = Animator.StringToHash("Die");

    [Header("アニメーション設定")]
    [SerializeField] private float dampTime;

    // イベント定義
    public event Action OnRollEnd;
    public event Action OnAttackEnd;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

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

    public void PlayJump()
    {
        if (animator == null) return;
        animator.SetTrigger(JumpHash);
    }

    public void PlayRoll()
    {
        if (animator == null) return;
        animator.SetTrigger(RollHash);
    }

    public void PlayBackRoll()
    {
        if (animator == null) return;
        animator.SetTrigger(BackRollHash);
    }

    /// <summary>
    /// プレイヤーの軽攻撃アニメーションを再生する
    /// </summary>
    public void PlayLightAttack()
    {
        if (animator == null) return;
        animator.SetTrigger(LightAttackHash);
    }

    /// <summary>
    /// プレイヤーの重攻撃アニメーションを再生する
    /// </summary>
    public void PlayHeavyAttack()
    {
        if (animator == null) return;
        animator.SetTrigger(HeavyAttackHash);
    }

    /// <summary>
    /// プレイヤーがダメージを受けたときのアニメーションを再生する
    /// </summary>
    public void PlayTakeDamage()
    {
        if (animator == null) return;
        animator.SetTrigger(TakeDamageHash);
    }

    /// <summary>
    /// プレイヤーが死亡したときのアニメーションを再生する
    /// </summary>
    public void PlayDie()
    {
        if (animator == null) return;
        animator.SetTrigger(DieHash);
    }

    /// <summary>
    /// プレイヤーが地面に接地しているかどうかの状態を更新する
    /// </summary>
    /// <param name="isGrounded"></param>
    public void UpdateGroundedState(bool isGrounded)
    {
        if (animator == null) return;
        animator.SetBool(IsGroundedHash, isGrounded);
    }

    /// <summary>
    /// Animation Event から呼び出すメソッド
    /// </summary>
    public void OnRollCompleted()
    {
        OnRollEnd?.Invoke();
    }

    /// <summary>
    /// Animation Event から呼び出すメソッド
    /// </summary>
    public void OnBackRollCompleted()
    {
        OnRollEnd?.Invoke();
    }

    /// <summary>
    /// Animation Event から呼び出すメソッド
    /// </summary>
    public void OnAttackCompleted()
    {
        OnAttackEnd?.Invoke();
    }
}