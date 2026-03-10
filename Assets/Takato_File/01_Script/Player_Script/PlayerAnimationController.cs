using UnityEngine;

/// <summary>
/// プレイヤーのアニメーションを管理するクラス
/// </summary>
namespace Takato
{
    public class PlayerAnimationController : MonoBehaviour
    {
        private Animator animator;

        public bool IsFrontMove { get; private set; }  // プレイヤーが前方に移動しているかどうかを示すフラグ
        public bool IsLeftMove { get; private set; }  // プレイヤーが左に移動しているかどうかを示すフラグ
        public bool IsRightMove { get; private set; } // プレイヤーが右に移動しているかどうかを示すフラグ
        public bool IsBackMove { get; private set; }  // プレイヤーが後方に移動しているかどうかを示すフラグ
        public bool IsJumpMove { get; set; }          // プレイヤーがジャンプしているかどうかを示すフラグ
        public bool IsBlockMove { get; set; }         // プレイヤーがガードしているかどうかを示すフラグ
        public bool IsAttackMove { get; set; }         // プレイヤーが攻撃しているかどうかを示すフラグ

        private void Awake()
        {
            animator = GetComponent<Animator>(); // Animator コンポーネントを取得
        }

        /// <summary>
        /// アニメーションの状態を更新する
        /// </summary>
        public void UpdateAnimation(Vector2 moveInput)
        {
            // 入力方向に応じてフラグを設定
            IsFrontMove = moveInput.y > 0;
            IsLeftMove = moveInput.x < 0;
            IsRightMove = moveInput.x > 0;
            IsBackMove = moveInput.y < 0;

            // Animator に値を反映
            animator.SetBool("IsFrontMove", IsFrontMove);
            animator.SetBool("IsLeftMove", IsLeftMove);
            animator.SetBool("IsRightMove", IsRightMove);
            animator.SetBool("IsBackMove", IsBackMove);
        }

        public void SetJump(bool isJump)
        {
            IsJumpMove = isJump;
            animator.SetBool("IsJumpMove", IsJumpMove);
        }

        /// <summary>
        /// ガードアニメーション
        /// </summary>
        /// <param name="isBlock"></param>
        public void SetBlock(bool isBlock)
        {
            IsBlockMove = isBlock;
            animator.SetBool("IsBlockMove", IsBlockMove);
        }

        /// <summary>
        /// 攻撃アニメーション
        /// </summary>
        /// <param name="isAttack"></param>
        public void SetAttack(bool isAttack)
        {
            IsAttackMove = isAttack;
            animator.SetBool("IsAttackMove", IsAttackMove);
        }
    }
}
