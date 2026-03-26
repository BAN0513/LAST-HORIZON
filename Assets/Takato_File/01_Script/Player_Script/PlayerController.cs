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
        [SerializeField] private int maxHp;
        [Header("プレイヤーのHPバーのスクリプトが入ってる物を入れる")]
        [SerializeField] private PlayerHPBar hpBar;       
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
        private PlayerShieldContoroller shieldController;     // シールド管理

        private float verticalVelocity;                       // 垂直方向の速度
        private int hp;                                       // プレイヤーの現在のHP
        private bool isBlocking = false;                      // 防御中かどうか

        private void Awake()
        {
            inputController = GetComponent<PlayerInputController>();
            characterController = GetComponent<CharacterController>();
            animationController = GetComponent<PlayerAnimationController>();
            weaponController = GetComponentInChildren<PlayerWeaponController>();
            shieldController = GetComponentInChildren<PlayerShieldContoroller>();
        }

        private void Start()
        {
            hp = maxHp;
            hpBar.SetHP(hp, maxHp); // 初期値を反映
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
            float currentMoveSpeed = isBlocking ? moveSpeed * 0.75f : moveSpeed; // 防御中は移動速度を低下
            Vector3 movement = new Vector3(moveDirection.x, 0, moveDirection.y) * currentMoveSpeed;
            movement.y = verticalVelocity;

            characterController.Move(movement * Time.deltaTime);
            animationController.UpdateAnimation(moveDirection);
        }

        /// <summary>
        /// 攻撃処理
        /// </summary>
        private void Attack()
        {
            // 攻撃入力がある場合、攻撃アニメーションを再生して、武器のコライダーを有効化
            if (inputController.IsAttackInput)
            {
                animationController.SetAttack(true);
                weaponController?.EnableWeaponCollider(); // 攻撃入力がある場合、武器のコライダーを有効化
            }
            else
            {
                animationController.SetAttack(false);
                weaponController?.DisableWeaponCollider(); // 攻撃入力がない場合、武器のコライダーを無効化
            }
        }

        /// <summary>
        /// 防御処理
        /// </summary>
        private void Block()
        {
            // 防御入力がある場合、防御アニメーションを再生して、シールドのコライダーを有効化
            if (inputController.BlockInput)
            {
                animationController.SetBlock(true);
                shieldController?.EnableShieldCollider(); // 防御入力がある場合、シールドのコライダーを有効化
                isBlocking = true; // 防御中フラグを立てる
            }
            else
            {
                animationController.SetBlock(false);
                shieldController?.DisableShieldCollider(); // 防御入力がない場合、シールドのコライダーを無効化
                isBlocking = false; // 防御中フラグを下げる
            }
        }

        /// <summary>
        /// プレイヤーがダメージを受ける処理
        /// </summary>
        public void TakeDamage(int damage)
        {
            hp -= damage;
            hp = Mathf.Max(hp, 0); // HPが0未満にならないようにする
            hpBar.SetHP(hp, maxHp); // HPバーを更新
            Debug.Log($"プレイヤーは{damage}のダメージを受けました！残りHP: {hp}");
        }
    }
} 
