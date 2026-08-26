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

    [Header("アニメーション設定")]
    [SerializeField] private float dampTime;

    // 前転アニメーション終了時に発火するイベント
    public event Action OnRollEnd;

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

    /// <summary>
    /// プレイヤーの前転アニメーションを再生するメソッド
    /// </summary>
    public void PlayRoll()
    {
        if (animator == null) return;
        animator.SetTrigger(RollHash);
    }

    /// <summary>
    /// プレイヤーの接地状態を更新するメソッド
    /// </summary>
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
        OnRollEnd?.Invoke();// 前転アニメーション終了時にイベントを発火
    }
}