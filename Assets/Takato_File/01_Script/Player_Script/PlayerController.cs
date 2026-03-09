using UnityEngine;

/// <summary>
/// プレイヤーの動きを管理するクラス
/// </summary>
namespace Takato
{
    public class PlayerController : MonoBehaviour
    {
        [Header("(プレイヤー関連のステータス)")]
        [Space(10)]
        [Header("プレイヤーの移動速度")]
        [SerializeField] private float moveSpeed;
        [Header("プレイヤーのジャンプ力")]
        [SerializeField] private float jumpForce;
        [Header("プレイヤーの重力")]
        [SerializeField] private float gravity;

        private PlayerInputController inputController;         // プレイヤーの入力を管理するコンポーネント
        private CharacterController characterController;       // プレイヤーの移動を管理するコンポーネント
        private PlayerAnimationController animationController; // プレイヤーのアニメーションを管理するコンポーネント

        private float verticalVelocity; // プレイヤーの垂直方向の速度

        private void Awake()
        {
            inputController = GetComponent<PlayerInputController>();   // プレイヤーの入力を管理するコンポーネントを取得
            characterController = GetComponent<CharacterController>(); // プレイヤーの移動を管理するコンポーネントを取得
            animationController = GetComponent<PlayerAnimationController>(); // プレイヤーのアニメーションを管理するコンポーネントを取得
        }

        private void Update()
        {
            Move(); // プレイヤーの移動とジャンプを処理
        }

        /// <summary>
        /// プレイヤーの移動とジャンプを処理するメソッド
        /// </summary>
        private void Move()
        {
            if (characterController.isGrounded)
            {
                verticalVelocity = -1f;
                if (inputController.JumpInput)
                {
                    verticalVelocity = jumpForce;
                    Debug.Log("ジャンプしました！");
                }
            }
            else
            {
                verticalVelocity -= gravity * Time.deltaTime;
            }

            Vector2 moveDirection = inputController.MoveInput;
            Vector3 movement = new Vector3(moveDirection.x, 0, moveDirection.y) * moveSpeed;
            movement.y = verticalVelocity;

            characterController.Move(movement * Time.deltaTime);

            // アニメーションの更新
            animationController.UpdateAnimation(moveDirection);
        }
    }
}
