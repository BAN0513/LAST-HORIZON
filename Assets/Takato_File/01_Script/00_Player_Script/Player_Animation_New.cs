using UnityEngine;

/// <summary>
/// プレイヤーのアニメーションを管理するクラス(New)
/// </summary>
public class Player_Animation_New : MonoBehaviour
{
    private Animator animator;

    // 2D Blend Tree 用のパラメータハッシュ化
    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");

    [Header("アニメーション設定")]
    [SerializeField] private float dampTime = 0.1f; // アニメーション遷移の滑らかさ

    private void Awake()
    {
        // 子オブジェクトを含めて Animator を取得
        animator = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// 2D Blend Tree 用の移動パラメータ(MoveX, MoveY)を更新する
    /// </summary>
    public void UpdateMoveAnimation(Vector3 moveVelocity, float baseMoveSpeed)
    {
        if (animator == null || baseMoveSpeed <= 0f) return;

        // 微小な慣性残りをカットして Idle ポーズへ強制移行させる
        if (moveVelocity.magnitude < 0.01f)
        {
            moveVelocity = Vector3.zero;
        }

        // MoveX と MoveY の値を正規化して 2D Blend Tree に渡す
        float normalizedX = moveVelocity.x / baseMoveSpeed;
        float normalizedY = moveVelocity.z / baseMoveSpeed;

        // Animator の 2D Blend Tree パラメータを更新
        animator.SetFloat(MoveXHash, normalizedX, dampTime, Time.deltaTime);
        animator.SetFloat(MoveYHash, normalizedY, dampTime, Time.deltaTime);
    }
}