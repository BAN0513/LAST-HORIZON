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
        [Header("プレイヤーのHP")]
        [SerializeField] private int hp;
        [Header("プレイヤーの移動速度")]
        [SerializeField] private float moveSpeed;
        [Header("プレイヤーのジャンプ力")]
        [SerializeField] private float jumpForce;
        [Header("プレイヤーの重力")]
        [SerializeField] private float gravity;

        private PlayerInputController inputController;         // 入力管理
        private CharacterController characterController;       // 移動管理
        private PlayerAnimationController animationController; // アニメーション管理
        private PlayerWeaponController weaponController;       // 武器管理

        private float verticalVelocity;     // 垂直方向の速度

        private void Awake()
        {
            inputController = GetComponent<PlayerInputController>();
            characterController = GetComponent<CharacterController>();
            animationController = GetComponent<PlayerAnimationController>();
            weaponController = GetComponentInChildren<PlayerWeaponController>();
        }

        private void Update()
        {
            hp = Mathf.Max(hp, 0); // HPが0未満にならないようにする
            Move(); // 移動とジャンプの処理
            Block();// 防御処理
            Attack();// 攻撃処理
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
                    animationController.SetJump(true);
                    Debug.Log("ジャンプしました！");
                }
                else
                {
                    animationController.SetJump(false);
                }
            }
            else
            {
                verticalVelocity -= gravity * Time.deltaTime;
                animationController.SetJump(true);
            }

            Vector2 moveDirection = inputController.MoveInput;
            Vector3 movement = new Vector3(moveDirection.x, 0, moveDirection.y) * moveSpeed;
            movement.y = verticalVelocity;

            characterController.Move(movement * Time.deltaTime);
            animationController.UpdateAnimation(moveDirection);
        }

        /// <summary>
        /// 攻撃処理（後から拡張可能）
        /// </summary>
        private void Attack()
        {
            if (inputController.IsAttackInput)
            {
                animationController.SetAttack(true);
                weaponController?.EnableWeaponCollider(); // 攻撃入力がある場合、武器のコライダーを有効化
                // 今後、攻撃処理を追加していきます
            }
            else
            {
                animationController.SetAttack(false);
                weaponController?.DisableWeaponCollider(); // 攻撃入力がない場合、武器のコライダーを無効化
            }
        }

        /// <summary>
        /// 防御処理（後から拡張可能）
        /// </summary>
        private void Block()
        {
            if (inputController.BlockInput)
            {
                animationController.SetBlock(true);
                // 今後、防御処理を追加していきます。
            }
            else
            {
                animationController.SetBlock(false);
            }
        }
    }
} 
