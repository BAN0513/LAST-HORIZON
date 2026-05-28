using UnityEngine;

namespace Takato
{
    /// <summary>
    ///プレイヤーを管理するクラス
    /// </summary>
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


        private PlayerInputController inputController;　// プレイヤーの入力を管理するクラスのインスタンス
        private CharacterController characterController; // プレイヤーの移動を管理するCharacterControllerコンポーネントのインスタンス
        private PlayerAnimationController animationController;// プレイヤーのアニメーションを管理するクラスのインスタンス
        private PlayerWeaponController weaponController; // プレイヤーの攻撃を管理するクラスのインスタンス
        private PlayerShieldContoroller shieldController; // プレイヤーの防御を管理するクラスのインスタンス
        private PlayerSkill playerSkill;                  // プレイヤーのスキルを管理するクラスのインスタンス
        private CharacterChangeController characterChangeController; // キャラクター切り替えを管理するクラスのインスタンス
        private Transform cameraTransform; // カメラのTransform

        private float verticalVelocity; // ジャンプと重力の処理に使用する垂直速度
        private int hp;                 // プレイヤーの現在のHP
        private bool isBlocking = false;// 防御中かどうかを管理するフラグ
        private bool isDead = false;    // 死亡フラグ
        private bool isSkillUIOpen = false;// スキル選択UIが開いているかどうかを管理するフラグ
        private bool prevInventoryOpen = false;// 前フレームのインベントリの開閉状態を管理するフラグ

        //スキル発動ボタンを押された時の判定をするための前フレームのスキル入力状態を管理するフラグ
        private bool prevSkillInput = false;

        // キャラ切替のエッジ判定用フラグ
        private bool prevCharChange = false;

        private void Awake()
        {
            inputController = GetComponent<PlayerInputController>();
            characterController = GetComponent<CharacterController>();
            animationController = GetComponent<PlayerAnimationController>();
            weaponController = GetComponentInChildren<PlayerWeaponController>();
            shieldController = GetComponentInChildren<PlayerShieldContoroller>();
            playerSkill= GetComponent<PlayerSkill>();

            // マネージャー化に合わせてインスタンス参照をシングルトンから取得する
            characterChangeController = CharacterChangeController.Instance;
            if (characterChangeController == null)
            {
                // Awake のタイミングで未登録の場合はフォールバックで検索
                characterChangeController = FindAnyObjectByType<CharacterChangeController>();
            }

            // カメラのTransformを取得
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }
        }

        private void Start()
        {
            hp = maxHp;// HPを最大値で初期化

            // hpBar が割当てられていない場合はフォールバックで検索してから SetHP を呼ぶ
            if (hpBar == null)
            {
                hpBar = FindAnyObjectByType<PlayerHPBar>();
            }

            if (hpBar != null)
            {
                hpBar.SetHP(hp, maxHp); // HPバーを初期化
            }
            else
            {
                Debug.LogWarning($"[{nameof(PlayerController)}] hpBar が割り当てられていません。Inspector にセットするか、実行時に PlayerHPBar を配置してください。");
            }

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
            hp = Mathf.Max(hp, 0); // HPが0未満にならないようにする

            //HPBarのコンポーネントを取得し続ける
            if(hpBar == null)
            {
                hpBar = FindAnyObjectByType<PlayerHPBar>();
                if (hpBar != null)
                {
                    hpBar.SetHP(hp, maxHp); // HPバーを更新
                }
            }

            //コストのTextコンポーネントを取得し続ける
            if(costText == null)
            {
                costText = FindAnyObjectByType<TMPro.TextMeshProUGUI>();
                if (costText != null)
                {
                    costText.text = $"Cost: {currentCost}"; // コストの表示を更新
                }
            }

            //SelectUIのコンポーネントを取得し続ける
            if(skillSelectUI == null)
            {
                skillSelectUI = FindAnyObjectByType<SkillSelectUI>();
            }

            if (inputController.IsInventoryInput && !prevInventoryOpen)
            {
                ToggleSkillUI(); // インベントリ入力が押されたときにスキルUIの表示切替を行う
            }
            prevInventoryOpen = inputController.IsInventoryInput;


            if(hp < maxHp/3)
            {
                SoundManager soundManager = FindAnyObjectByType<SoundManager>();
                if (soundManager != null)
                {
                    soundManager.PlaySE(3); // HPが1/3以下になったらBGMを切り替える
                }
            }
            
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

            // スキル入力の判定
            if (inputController.IsSkillInput&&prevSkillInput && playerSkill != null)
            {
                playerSkill.ActivateSkill(0, this);
            }
            if (inputController.IsSkill2Input&&prevSkillInput && playerSkill != null)
            {
                playerSkill.ActivateSkill(1, this);
            }
            if (inputController.IsSkill3Input && prevSkillInput && playerSkill != null)
            {
                playerSkill.ActivateSkill(2, this);
            }
            if(inputController.IsSkill4Input && prevSkillInput && playerSkill != null)
            {
                playerSkill.ActivateSkill(3, this);
            }

            // 操作するキャラクターを入れ替える処理（キー押下の瞬間のみ実行）
            if (inputController.IsCharChange && !prevCharChange)
            {
                characterChangeController?.NextCharacter(); // NextCharacter を呼ぶ（インデックスを進める）
            }

            //現フレームのスキル入力状態を保存
            prevSkillInput = inputController.IsSkillInput || inputController.IsSkill2Input || 
                inputController.IsSkill3Input || inputController.IsSkill4Input;

            // キャラ切替の前フレーム状態を保存
            prevCharChange = inputController.IsCharChange;
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
        /// プレイヤーの移動とジャンプを処理するメソッド
        /// （カメラ基準の移動）
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

            // カメラ基準の移動方向を計算
            Vector3 movement;
            if (cameraTransform != null)
            {
                Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
                Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
                Vector3 desiredMove = camForward * moveDirection.y + camRight * moveDirection.x;

                movement = desiredMove * currentMoveSpeed;
            }
            else
            {
                // カメラが設定されていない場合は従来通りプレイヤー基準で移動
                Vector3 forward = transform.forward;
                Vector3 right = transform.right;
                movement = (forward * moveDirection.y + right * moveDirection.x) * currentMoveSpeed;
            }

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
        /// HP 関係の公開ユーティリティ
        /// </summary>
        public int GetHP()
        {
            return hp;
        }

        public int GetMaxHP()
        {
            return maxHp;
        }

        public void SetHP(int value)
        {
            hp = Mathf.Clamp(value, 0, maxHp);
            hpBar?.SetHP(hp, maxHp);
        }

        /// <summary>
        /// HPを別のプレイヤーに転送するメソッド
        /// </summary>
        public void TransferHPTo(PlayerController target, bool preservePercent = true)
        {
            if (target == null) return;

            if (preservePercent)
            {
                float ratio = (maxHp > 0) ? (float)hp / maxHp : 0f;
                int newHp = Mathf.RoundToInt(ratio * target.GetMaxHP());
                target.SetHP(newHp);
            }
            else
            {
                target.SetHP(hp);
            }
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
            SoundManager soundManager = FindAnyObjectByType<SoundManager>();
            if (soundManager != null)
            {
                soundManager.PlaySE(1); // ダメージを受けたときのSEを再生
            }

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
