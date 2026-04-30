using UnityEngine;

namespace Takato
{
    public class PlayerController : MonoBehaviour
    {
        [Header("(プレイヤー関連のステータス)")]
        [Space(10)]
        [Header("プレイヤーのHP")]
        [SerializeField] private int maxHp;
        [Header("現在のスキルコスト")]
        [SerializeField] private int currentCost;
        [Header("プレイヤーのHPバーのスクリプトが入ってる物を入れる")]
        [SerializeField] private PlayerHPBar hpBar;
        [Header("プレイヤーの現在のコストを見るためのText")]
        [SerializeField] private TMPro.TextMeshProUGUI costText;
        [Header("プレイヤーの移動速度")]
        [SerializeField] private float moveSpeed;
        [Header("プレイヤーのジャンプ力")]
        [SerializeField] private float jumpForce;
        [Header("プレイヤーの重力")]
        [SerializeField] private float gravity;
        [Header("プレイヤーのダメージカット率")]
        [SerializeField] private float damageCutRate;
        [Header("Skill Select UI")]
        [SerializeField] private SkillSelectUI skillSelectUI;
        [Header("Look Orbit Controller(Cinemachineカメラ入れてください)")]
        [SerializeField] private Behaviour lookOrbitController; 

        private PlayerInputController inputController;
        private CharacterController characterController;
        private PlayerAnimationController animationController;
        private PlayerWeaponController weaponController;
        private PlayerShieldContoroller shieldController;
        private PlayerSkill playerSkill;

        private float verticalVelocity; // ジャンプと重力の処理に使用する垂直速度
        private int hp;                 // プレイヤーの現在のHP
        private bool isBlocking = false;// 防御中かどうかを管理するフラグ
        private bool isDead = false;    // 死亡フラグ
        private bool isSkillUIOpen = false;// スキル選択UIが開いているかどうかを管理するフラグ
        private bool prevInventoryOpen = false;// 前フレームのインベントリの開閉状態を管理するフラグ

        private void Awake()
        {
            inputController = GetComponent<PlayerInputController>();
            characterController = GetComponent<CharacterController>();
            animationController = GetComponent<PlayerAnimationController>();
            weaponController = GetComponentInChildren<PlayerWeaponController>();
            shieldController = GetComponentInChildren<PlayerShieldContoroller>();
            playerSkill= GetComponent<PlayerSkill>();
        }

        private void Start()
        {
            hp = maxHp;             // HPを最大値で初期化
            hpBar.SetHP(hp, maxHp); // HPバーを初期化

            //カーソルをロックして非表示にする
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (costText != null)
            {
                costText.text = $"Cost: {currentCost}";
            }
        }

        private void Update()
        {
            hp = Mathf.Max(hp, 0);

            if (inputController.IsInventoryInput && !prevInventoryOpen)
            {
                ToggleSkillUI(); // インベントリ入力が押されたときにスキルUIの表示切替を行う
            }
            prevInventoryOpen = inputController.IsInventoryInput;

            if (hp <= 0)
            {
                Die(); // HPが0以下になったら死亡処理を行う
                return;
            }

            if (isSkillUIOpen)
            {
                animationController.SetAttack(false); // スキルUIが開いているときは攻撃アニメーションをオフ
                animationController.SetBlock(false); // スキルUIが開いているときは防御アニメーションをオフ
                animationController.SetJump(false);  // スキルUIが開いているときはジャンプアニメーションをオフ
                weaponController?.DisableWeaponCollider(); // スキルUIが開いているときは武器のコライダーを無効化
                shieldController?.DisableShieldCollider(); // スキルUIが開いているときはシールドのコライダーを無効化
                isBlocking = false;
                return;
            }

            Move(); // 移動処理は常に行う
            Block();// 防御処理は常に行う
            Attack();// 攻撃処理は常に行う

            if (inputController.IsSkillInput && playerSkill != null)
            {
                playerSkill.ActivateSkill(0, this);
            }
            if (inputController.IsSkill2Input && playerSkill != null)
            {
                playerSkill.ActivateSkill(1, this);
            }
            if (inputController.IsSkill3Input && playerSkill != null)
            {
                playerSkill.ActivateSkill(2, this);
            }
            if(inputController.IsSkill4Input && playerSkill != null)
            {
                playerSkill.ActivateSkill(3, this);
            }
        }

        /// <summary>
        /// スキル選択UIの表示切替と、それに伴う入力制御やカーソルの状態を管理するメソッド
        /// </summary>
        private void ToggleSkillUI()
        {
            isSkillUIOpen = !isSkillUIOpen;
            skillSelectUI.ShowUI(isSkillUIOpen);

            // ゲーム入力を止めて、UI マップを有効化／無効化
            inputController.SetGameplayEnabled(!isSkillUIOpen);
            inputController.SetUIEnabled(isSkillUIOpen);

            // Look アクションも確実に切る
            inputController.IsLookEnabled = !isSkillUIOpen;

            // Cinemachine 側の Look を無効化
            if (lookOrbitController != null)
            {
                lookOrbitController.enabled = !isSkillUIOpen;
            }

            // カーソルのロックと表示を切り替える
            if (isSkillUIOpen)
            {
                Cursor.lockState = CursorLockMode.None; // カーソルのロックを解除して表示する
                Cursor.visible = true;                  // カーソルを表示する
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked; // カーソルをロックして非表示にする
                Cursor.visible = false;                   // カーソルを非表示にする
            }
        }

        /// <summary>
        /// LateUpdateは、Updateの後に呼び出されるため、プレイヤーの移動やアニメーションが更新された後にカメラの向きを調整することができる。
        /// </summary>
        private void LateUpdate()
        {
            // 死亡またはスキル選択UIが開いている場合は視点操作をスキップ
            if (isDead || isSkillUIOpen) return;

            if (Camera.main == null) return;
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0; // 水平面上の方向に制限
            if (cameraForward.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(cameraForward), Time.deltaTime * 10f);
            }
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
                    animationController.SetJump(false); // 地面にいるときはジャンプアニメーションをオフ
                }
            }
            else
            {
                verticalVelocity -= gravity * Time.deltaTime;
                animationController.SetJump(true);
            }

            Vector2 moveDirection = inputController.MoveInput;
            float currentMoveSpeed = isBlocking ? moveSpeed * 0.75f : moveSpeed; // 防御中は移動速度を低下

            // プレイヤーの向いている方向に移動
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;
            Vector3 movement = (forward * moveDirection.y + right * moveDirection.x) * currentMoveSpeed;
            movement.y = verticalVelocity;

            characterController.Move(movement * Time.deltaTime);
            animationController.UpdateAnimation(moveDirection);
        }

        /// <summary>
        /// プレイヤーの移動速度を取得するメソッド
        /// </summary>
        public float GetMoveSpeed()
        {
            return moveSpeed; // プレイヤーの移動速度を返す
        }
        public void SetMoveSpeed(float value)
        {
            moveSpeed = value; // プレイヤーの移動速度を設定する
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
        /// ダメージカット率を取得するメソッド
        /// </summary>
        public float GetDamageCutRate()
        {
            return damageCutRate; // ダメージカット率を返す
        }

        /// <summary>
        /// ダメージカット率を設定するメソッド
        /// </summary>
        public void SetDamageCutRate(float value)
        {
            damageCutRate = Mathf.Clamp01(value);
        }

        /// <summary>
        /// プレイヤーがダメージを受ける処理
        /// </summary>
        public void TakeDamage(int damage)
        {
            int finalDamage = Mathf.RoundToInt(damage * (1f - damageCutRate)); // ダメージカット率を適用
            hp -= finalDamage;
            hpBar.SetHP(hp, maxHp); // HPバーを更新
            Debug.Log($"プレイヤーは{finalDamage}のダメージを受けました。現在のHP: {hp}/{maxHp}");
        }

        /// <summary>
        /// 現在のスキルコストを取得するメソッド
        /// </summary>
        public int GetCurrentCost()
        {
            return currentCost;
        }

        /// <summary>
        /// スキルコストを消費する処理
        /// </summary>
        public void ConsumeCost(int value)
        {
            currentCost = Mathf.Max(0, currentCost - value); // コストが0未満にならないようにする
            costText.text = $"Cost: {currentCost}"; // コストの表示を更新
        }

        // 必要に応じてコスト回復メソッドも追加する予定
        public void RecoverCost(int value)
        {
            currentCost += value;
        }


        /// <summary>
        /// プレイヤーが死亡したときの処理
        /// </summary>
        private void Die()
        {
            isDead = true; // 死亡フラグを立てる

            //操作を無効化
            inputController.enabled = false;
            animationController.enabled = false;
            weaponController?.DisableWeaponCollider(); // 武器のコライダーを無効化
            shieldController?.DisableShieldCollider(); // シールドのコライダーを無効化
            

            Debug.Log("プレイヤーは死亡しました");
            PlayerRagdollController ragdollController = GetComponent<PlayerRagdollController>();
            if (ragdollController != null)
            {
                ragdollController.ActivateRagdoll(); // ラグドールを有効化して死亡表現
            }
            else
            {
                // ラグドールがない場合は、単純にオブジェクトを非表示にする
                gameObject.SetActive(false);
            }
        }
    }
}
