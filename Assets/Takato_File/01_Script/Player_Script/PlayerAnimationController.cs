using UnityEngine;

/// <summary>
/// プレイヤーのアニメーションを管理するクラス
/// </summary>
namespace Takato
{
    public class PlayerAnimationController : MonoBehaviour
    {
        private Animator animator; // アニメーターコンポーネント

        public bool IsFrontMove { get; private set; } // 前進アニメーションの状態
        public bool IsLeftMove { get; private set; } // 左移動アニメーションの状態
        public bool IsRightMove { get; private set; } // 右移動アニメーションの状態
        public bool IsBackMove { get; private set; } // 後退アニメーションの状態
        public bool IsJumpMove { get; private set; } // ジャンプアニメーションの状態
        public bool IsBlockMove { get; private set; } // ガードアニメーションの状態
        public bool IsAttackMove { get; private set; } // 攻撃アニメーションの状態

        private void Awake()
        {
            animator = GetComponent<Animator>(); // アニメーターコンポーネントを取得
        }

        /// <summary>
        /// 移動アニメーションの状態を更新
        /// </summary>
        public void UpdateAnimation(Vector2 moveInput)
        {
            // 入力に基づいてアニメーションの状態を更新
            IsFrontMove = moveInput.y > 0;
            IsLeftMove = moveInput.x < 0;
            IsRightMove = moveInput.x > 0;
            IsBackMove = moveInput.y < 0;

            // アニメーターのパラメーターを更新
            animator.SetBool("IsFrontMove", IsFrontMove); 
            animator.SetBool("IsLeftMove", IsLeftMove);  
            animator.SetBool("IsRightMove", IsRightMove);
            animator.SetBool("IsBackMove", IsBackMove);
        }

        /// <summary>
        /// ジャンプアニメーション
        /// </summary>
        public void SetJump(bool isJump)
        {
            IsJumpMove = isJump;
            animator.SetBool("IsJumpMove", IsJumpMove);
        }

        /// <summary>
        /// ガードアニメーション
        /// </summary>
        public void SetBlock(bool isBlock)
        {
            IsBlockMove = isBlock;
            animator.SetBool("IsBlockMove", IsBlockMove);
        }

        /// <summary>
        /// 攻撃アニメーション
        /// </summary>
        public void SetAttack(bool isAttack)
        {
            IsAttackMove = isAttack;
            animator.SetBool("IsAttackMove", IsAttackMove);
        }
    }
}
