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
        private PlayerAnimationController playerAnimationController; // プレイヤーのアニメーションを管理するコンポーネント

        private float verticalVelocity; // プレイヤーの垂直方向の速度

        private void Awake()
        {
            inputController = GetComponent<PlayerInputController>();   // プレイヤーの入力を管理するコンポーネントを取得
            characterController = GetComponent<CharacterController>(); // プレイヤーの移動を管理するコンポーネントを取得
            animationController = GetComponent<PlayerAnimationController>(); // プレイヤーのアニメーションを管理するコンポーネントを取得
            playerAnimationController = GetComponent<PlayerAnimationController>(); // プレイヤーのアニメーションを管理するコンポーネントを取得
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
            // 地面にいるかどうかをチェックして、ジャンプや重力の処理を行う
            if (characterController.isGrounded)
            {
                verticalVelocity = -1f; // 地面にいるときは少しだけ下方向に力を加えて、地面にしっかりと接地させる

                // ジャンプ入力があればジャンプする
                if (inputController.JumpInput)
                {
                    verticalVelocity = jumpForce;
                    playerAnimationController.SetJump(true); // ジャンプアニメーションを開始
                    Debug.Log("ジャンプしました！");
                }
                else
                {
                    playerAnimationController.SetJump(false); // ジャンプアニメーションを終了
                }
            }
            else
            {
                verticalVelocity -= gravity * Time.deltaTime; // 空中にいるときは重力を適用
                playerAnimationController.SetJump(true); // 空中にいるときはジャンプアニメーションを維持
            }

            // 入力に基づいて移動方向を計算
            Vector2 moveDirection = inputController.MoveInput;
            Vector3 movement = new Vector3(moveDirection.x, 0, moveDirection.y) * moveSpeed;
            movement.y = verticalVelocity;

            characterController.Move(movement * Time.deltaTime); // プレイヤーを移動させる

            animationController.UpdateAnimation(moveDirection); // アニメーションの状態を更新
        }
    }
}
