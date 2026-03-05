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
        [SerializeField] private float moveSpeed; // プレイヤーの移動速度
        [Header("プレイヤーのジャンプ力")]
        [SerializeField] private float jumpForce; // プレイヤーのジャンプ力
        [Header("プレイヤーの重力")]
        [SerializeField] private float gravity;   // プレイヤーの重力

        private PlayerInputController inputController;   // 入力管理クラスへの参照
        private CharacterController characterController; // キャラクターコントローラーへの参照

        private float verticalVelocity; // Y方向の速度を管理する変数

        private void Awake()
        {
            inputController = GetComponent<PlayerInputController>();   // 入力管理クラスを取得
            characterController = GetComponent<CharacterController>(); // キャラクターコントローラーを取得
        }
        private void Update()
        {
            Move(); // 毎フレームプレイヤーを移動させる関数を呼び出す
        }

        private void Move()
        {
            // 地面にいる場合はY速度をリセット
            if (characterController.isGrounded)
            {
                verticalVelocity = -1f; // 少し下向きにして地面に張り付く
                if (inputController.JumpInput)
                {
                    verticalVelocity = jumpForce; // ジャンプ力を加算
                    Debug.Log("ジャンプしました！");
                }
            }
            else
            {
                verticalVelocity -= gravity * Time.deltaTime; // 重力を加算
            }

            // 入力から移動方向を取得
            Vector2 moveDirection = inputController.MoveInput;
            Vector3 movement = new Vector3(moveDirection.x, 0, moveDirection.y) * moveSpeed;

            // Y方向の速度を加算
            movement.y = verticalVelocity;

            // プレイヤーを移動させる
            characterController.Move(movement * Time.deltaTime);
        }
    }
}
