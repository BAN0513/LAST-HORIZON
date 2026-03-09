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
        public bool IsJumpMove { get; set; }      // プレイヤーがジャンプしているかどうかを示すフラグ

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
    }
}
