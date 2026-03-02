using UnityEngine;

/// <summary>
/// プレイヤーの動きを管理するクラス
/// </summary>
namespace Takato
{
    public class PlayerController : MonoBehaviour
    {
        [Header("プレイヤーの移動速度")]
        [SerializeField] private float moveSpeed; // プレイヤーの移動速度

        private PlayerInputController inputController; // 入力管理クラスへの参照
        private void Awake()
        {
            inputController = GetComponent<PlayerInputController>();
        }
        private void Update()
        {
            Move();
        }
        private void Move()
        {
            // 入力から移動方向を取得
            Vector2 moveDirection = inputController.MoveInput;
            // 移動方向に速度を掛けて移動量を計算
            Vector3 movement = new Vector3(moveDirection.x, 0, moveDirection.y) * moveSpeed * Time.deltaTime;
            // プレイヤーを移動させる
            transform.Translate(movement, Space.World);
        }
    }
}
