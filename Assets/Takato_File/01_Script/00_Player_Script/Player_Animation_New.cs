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
    private static readonly int LightAttackHash = Animator.StringToHash("LightAttack"); // 通常攻撃トリガー
    private static readonly int HeavyAttackHash = Animator.StringToHash("HeavyAttack"); // 強攻撃トリガー

    [Header("アニメーション設定")]
    [SerializeField] private float dampTime;

    // イベント定義
    public event Action OnRollEnd;
    public event Action OnAttackEnd; // 攻撃終了時イベント（通常攻撃・強攻撃共通）

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
    /// 通常攻撃アニメーションを再生するメソッド
    /// </summary>
    public void PlayLightAttack()
    {
        if (animator == null) return;
        animator.SetTrigger(LightAttackHash);
    }

    /// <summary>
    /// 強攻撃アニメーションを再生するメソッド
    /// </summary>
    public void PlayHeavyAttack()
    {
        if (animator == null) return;
        animator.SetTrigger(HeavyAttackHash);
    }

    /// <summary>
    /// 地面に接地しているかどうかの状態を更新するメソッド
    /// </summary>
    public void UpdateGroundedState(bool isGrounded)
    {
        if (animator == null) return;
        animator.SetBool(IsGroundedHash, isGrounded);
    }

    /// <summary>
    /// Animation Event から呼び出すメソッド（回避用）
    /// </summary>
    public void OnRollCompleted()
    {
        OnRollEnd?.Invoke();
    }

    /// <summary>
    /// Animation Event から呼び出すメソッド（攻撃用：通常攻撃・強攻撃共通）
    /// </summary>
    public void OnAttackCompleted()
    {
        OnAttackEnd?.Invoke();
    }
}