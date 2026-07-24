using UnityEngine;

/// <summary>
/// プレイヤーのアニメーションを管理するクラス
/// </summary>
namespace Takato
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [Header("アニメーションのステータス")]
        [Space(10)]
        [Header("アニメーションの補完速度")]
        [SerializeField] private float animationSmoothTime; // デフォルト値を設定

        private Animator animator;

        // 補完用の現在値・速度
        private float currentMoveX;
        private float currentMoveY;
        private float moveXVelocity;
        private float moveYVelocity;

        public bool IsFrontMove { get; private set; } // 前方移動の状態を表すプロパティ
        public bool IsLeftMove { get; private set; }  // 左移動の状態を表すプロパティ
        public bool IsRightMove { get; private set; } // 右移動の状態を表すプロパティ
        public bool IsBackMove { get; private set; } // 後方移動の状態を表すプロパティ
        public bool IsJumpMove { get; private set; } // ジャンプの状態を表すプロパティ
        public bool IsBlockMove { get; private set; }// 防御の状態を表すプロパティ
        public bool IsAttackMove { get; private set; }// 攻撃の状態を表すプロパティ

        private void Awake()
        {
            animator = GetComponent<Animator>(); // Animatorコンポーネントを取得
        }

        /// <summary>
        /// 移動アニメーションの状態を更新
        /// </summary>
        public void UpdateAnimation(Vector2 moveInput)
        {
            // 補完してBlendTree用パラメータをセット
            currentMoveX = Mathf.SmoothDamp(currentMoveX, moveInput.x, ref moveXVelocity, animationSmoothTime);
            currentMoveY = Mathf.SmoothDamp(currentMoveY, moveInput.y, ref moveYVelocity, animationSmoothTime);

            animator.SetFloat("MoveX", currentMoveX); // BlendTreeのX軸パラメータに補完した値をセット
            animator.SetFloat("MoveY", currentMoveY); // BlendTreeのY軸パラメータに補完した値をセット

            // 状態プロパティも更新
            IsFrontMove = moveInput.y > 0;
            IsLeftMove = moveInput.x < 0;
            IsRightMove = moveInput.x > 0;
            IsBackMove = moveInput.y < 0;
        }

        /// <summary>
        /// ジャンプアニメーションの状態を更新
        /// </summary>
        public void SetJump(bool isJump)
        {
            IsJumpMove = isJump;
            animator.SetBool("IsJumpMove", IsJumpMove);
        }

        /// <summary>
        /// 防御アニメーションの状態を更新
        /// </summary>
        public void SetBlock(bool isBlock)
        {
            IsBlockMove = isBlock;
            animator.SetBool("IsBlockMove", IsBlockMove);
        }

        /// <summary>
        /// 攻撃アニメーションの状態を更新
        /// </summary>
        public void SetAttack(bool isAttack)
        {
            IsAttackMove = isAttack;
            animator.SetBool("IsAttackMove", IsAttackMove);
        }
    }
}
