using UnityEngine;

namespace Takato
{
    /// <summary>
    /// プレイヤーを管理するクラス
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        // --- Inspector で設定するフィールド ---
        [Header("プレイヤーのInspecterで設定フィールド")]
        [Space(10)]
        [Header("プレイヤーのSOを入れる")]
        [SerializeField] private PlayerSO playerSO;
        [Header("プレイヤーのHPバーのスクリプトが入ってる物を入れる")]
        [SerializeField] private PlayerHPBar hpBar;
        [Header("プレイヤーの現在のコストを見るためのText")]
        [SerializeField] private TMPro.TextMeshProUGUI costText;


        /// --- プレイヤーのステータス値の変数 ---
        private int maxHp;                 //プレイヤーの最大HPを格納する変数
        private int currentCost;            //プレイヤーの現在のスキルコストを格納する変数 
        private float moveSpeed;            //プレイヤーの移動速度を格納する変数
        private float jumpForce;            //プレイヤーのジャンプ力を格納する変数
        private float gravity;              //プレイヤーの重力を格納する変数
        private float damageCutRate;        //プレイヤーのダメージカット率を格納する変数（0～1の範囲で、例えば0.2なら20%カットとかです）
        private SkillSelectUI skillSelectUI;//スキル選択UIのスクリプトを格納する変数

        //バフ・デバフの合計値を管理する変数
        private float moveSpeedBuffTotal = 0f;
        private float damageCutBuffTotal = 0f;

        // --- キャッシュされたコンポーネント／参照 ---
        private PlayerInputController inputController;
        private CharacterController characterController;
        private PlayerAnimationController animationController;
        private PlayerWeaponController weaponController;
        private PlayerShieldContoroller shieldController;
        private PlayerSkill playerSkill;
        private CharacterChangeController characterChangeController;
        private Transform cameraTransform;

        // --- 状態 ---
        private float verticalVelocity; //ジャンプと重力の処理のための垂直速度を格納する変数
        private int hp;                 //プレイヤーの現在のHPを格納する変数
        private bool isBlocking;        //プレイヤーが現在防御中かどうかを格納する変数
        private bool isDead;            //プレイヤーが死亡しているかどうかを格納する変数
        private bool isSkillUIOpen;     //スキルUIが開いているかどうかを格納する変数
        private bool prevInventoryOpen; //インベントリが前フレームで開いていたかどうかを格納する変数

        // スキル発動に関する前フレーム入力
        private bool prevSkillInput;

        // キャラ切替のエッジ判定用フラグ
        private bool prevCharChange;

        //外部公開用プロパティ（バフを含めた最終数値を返す）
        public float MoveSpeed => Mathf.Max(0f, moveSpeed + moveSpeedBuffTotal);
        public float DamageCutRate => Mathf.Clamp(damageCutRate + damageCutBuffTotal, 0f, 1f);
        public int CurrentCost => currentCost;

        // --- Unity ライフサイクル ---
        private void Awake()
        {
            CacheComponentsOnAwake();
            AcquireCameraTransform();
            AcquireCharacterChangeController();

            // PlayerSO が割り当てられていれば値で上書きしておく
            ApplyPlayerSOValues();
        }

        private void Start()
        {
            hp = maxHp;
            EnsureHPBarInitialized();
            LockCursor();

            if (costText != null)
            {
                costText.text = $"Cost: {currentCost}";
            }
        }

        // --- PlayerSO からの適用 ---
        private void ApplyPlayerSOValues()
        {
            if (playerSO == null) return;

            // PlayerSO の値で上書き
            moveSpeed = playerSO.MoveSpeed;
            jumpForce = playerSO.JumpForce;
            gravity = playerSO.Gravity;
            damageCutRate = Mathf.Clamp01(playerSO.DamageCutRate);
            maxHp = playerSO.MaxHP;
            // SO に開始コストを追加している場合、それで初期化する
            currentCost = playerSO.StartingCost;

            // キャラクターが切り替わったらバフの合計をリセット
            moveSpeedBuffTotal = 0f;
            damageCutBuffTotal = 0f;
        }

        private void Update()
        {
            hp = Mathf.Max(hp, 0);

            // 実行時に参照が外れている可能性があるものは必要に応じて補完
            TryRecoverMissingReferences();

            UpdateHUDIfNeeded();

            HandleInventoryToggle();

            UpdateLowHPMusic();

            if (hp <= 0)
            {
                Die();
                return;
            }

            if (isSkillUIOpen)
            {
                ApplySkillUIState();
                return;
            }

            // 通常プレイ時の処理を分離
            Move();
            Block();
            Attack();
            HandleSkillInputs();
            HandleCharacterChangeEdge();
            PreviousCharacter();

            // 前フレーム情報を保存
            prevSkillInput = inputController.IsSkillInput || inputController.IsSkill2Input ||
                             inputController.IsSkill3Input || inputController.IsSkill4Input;
            prevCharChange = inputController.IsCharChange;
        }

        // --- 初期化ヘルパー ---
        private void CacheComponentsOnAwake()
        {
            inputController = GetComponent<PlayerInputController>();
            characterController = GetComponent<CharacterController>();
            animationController = GetComponent<PlayerAnimationController>();
            weaponController = GetComponentInChildren<PlayerWeaponController>();
            shieldController = GetComponentInChildren<PlayerShieldContoroller>();
            playerSkill = GetComponent<PlayerSkill>();
        }
        /// <summary>
        /// キャラクター切り替えののコントローラーを取得する。
        /// </summary>
        private void AcquireCharacterChangeController()
        {
            characterChangeController = CharacterChangeController.Instance;
            if (characterChangeController == null)
            {
                characterChangeController = FindAnyObjectByType<CharacterChangeController>();
            }
        }

        /// <summary>
        /// プレイヤーのカメラを取得する。通常は MainCamera タグのカメラを探すが、見つからない場合はシーン内の任意のカメラを探す。
        /// </summary>
        private void AcquireCameraTransform()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }
        }

        /// <summary>
        /// HPバーの初期化を行う。Inspector で割り当てられていない場合は、シーン内の任意の PlayerHPBar を探して割り当てる。
        /// </summary>
        private void EnsureHPBarInitialized()
        {
            if (hpBar == null)
            {
                hpBar = FindAnyObjectByType<PlayerHPBar>();
            }

            if (hpBar != null)
            {
                hpBar.SetHP(hp, maxHp);
            }
            else
            {
                Debug.LogWarning($"[{nameof(PlayerController)}] hpBar が割り当てられていません。Inspector にセットするか、実行時に PlayerHPBar を配置してください。");
            }
        }

        /// <summary>
        /// カーソルをロックして非表示にする。
        /// </summary>
        private void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // --- 毎フレーム補完 ---
        private void TryRecoverMissingReferences()
        {
            if (hpBar == null)
            {
                hpBar = FindAnyObjectByType<PlayerHPBar>();
                hpBar?.SetHP(hp, maxHp);
            }

            if (costText == null)
            {
                costText = FindAnyObjectByType<TMPro.TextMeshProUGUI>();
                if (costText != null)
                {
                    costText.text = $"Cost: {currentCost}";
                }
            }

            if (skillSelectUI == null)
            {
                skillSelectUI = FindAnyObjectByType<SkillSelectUI>();
            }
        }

        /// <summary>
        /// Hpが変化したときにHUDを更新する。
        /// </summary>
        private void UpdateHUDIfNeeded()
        {
            hpBar?.SetHP(hp, maxHp); // HPバーの更新
        }

        private void UpdateLowHPMusic()
        {
            if (hp < maxHp / 3)
            {
                SoundManager soundManager = FindAnyObjectByType<SoundManager>();
                soundManager?.PlaySE(3);
            }
        }

        // --- 入力関連 ---
        private void HandleInventoryToggle()
        {
            if (inputController.IsInventoryInput && !prevInventoryOpen)
            {
                ToggleSkillUI();
            }
            prevInventoryOpen = inputController.IsInventoryInput;
        }

        /// <summary>
        /// プレイヤーのスキル入力を処理する。スキルUIが開いているときはスキル入力を無視する。
        /// </summary>
        private void HandleSkillInputs()
        {
            if (playerSkill == null) return;

            if (inputController.IsSkillInput && prevSkillInput)
            {
                playerSkill.ActivateSkill(0, this);
            }
            if (inputController.IsSkill2Input && prevSkillInput)
            {
                playerSkill.ActivateSkill(1, this);
            }
            if (inputController.IsSkill3Input && prevSkillInput)
            {
                playerSkill.ActivateSkill(2, this);
            }
            if (inputController.IsSkill4Input && prevSkillInput)
            {
                playerSkill.ActivateSkill(3, this);
            }
        }

        /// <summary>
        /// キャラクターを次のキャラクターに切り替える処理を実行
        /// </summary>
        private void HandleCharacterChangeEdge()
        {
            if (inputController.IsCharChange && !prevCharChange)
            {
                characterChangeController?.NextCharacter();
            }
        }

        /// <summary>
        /// キャラクターが前のキャラクターに切り替わる処理を実行
        /// </summary>
        private void PreviousCharacter()
        {
            if (inputController.IsCharChangeDown && !prevCharChange)
            {
                characterChangeController?.PreviousCharacter();
            }
        }

        // --- UI 操作 ---
        private void ToggleSkillUI()
        {
            isSkillUIOpen = !isSkillUIOpen;
            skillSelectUI?.ShowUI(isSkillUIOpen);

            inputController.SetGameplayEnabled(!isSkillUIOpen);
            inputController.SetUIEnabled(isSkillUIOpen);
            inputController.IsLookEnabled = !isSkillUIOpen;

            if (isSkillUIOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        /// <summary>
        /// スキルのUiが開いているときの状態を適用する。攻撃、防御、ジャンプのアニメーションをリセットし、武器と盾のコライダーを無効化する。
        /// </summary>
        private void ApplySkillUIState()
        {
            animationController.SetAttack(false);
            animationController.SetBlock(false);
            animationController.SetJump(false);
            weaponController?.DisableWeaponCollider();
            shieldController?.DisableShieldCollider();
            isBlocking = false;
        }

        // --- 移動／攻撃／防御 ---
        /// <summary> カメラ基準の移動とジャンプ処理 </summary>
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

            //バフ込みの最終速度（MoveSpeed）をベースに計算するように変更
            float currentMoveSpeed = isBlocking ? MoveSpeed * 0.75f : MoveSpeed;

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
                Vector3 forward = transform.forward;
                Vector3 right = transform.right;
                movement = (forward * moveDirection.y + right * moveDirection.x) * currentMoveSpeed;
            }

            movement.y = verticalVelocity;
            characterController.Move(movement * Time.deltaTime);
            animationController.UpdateAnimation(moveDirection);
        }

        /// <summary>
        /// プレイヤーの攻撃処理を実行するためのメソッドです。
        /// </summary>
        private void Attack()
        {
            if (inputController.IsAttackInput)
            {
                animationController.SetAttack(true);
                weaponController?.EnableWeaponCollider();
            }
            else
            {
                animationController.SetAttack(false);
                weaponController?.DisableWeaponCollider();
            }
        }

        /// <summary>
        ///プレイヤーの防御処理を実行するためのメソッドです。
        /// </summary>
        private void Block()
        {
            if (inputController.BlockInput)
            {
                animationController.SetBlock(true);
                shieldController?.EnableShieldCollider();
                isBlocking = true;
            }
            else
            {
                animationController.SetBlock(false);
                shieldController?.DisableShieldCollider();
                isBlocking = false;
            }
        }

        // --- 公開 API ---

        public float GetMoveSpeed() => MoveSpeed;
        public void SetMoveSpeed(float value) => moveSpeed = value; //プレイヤーの移動速度を設定するためのメソッド

        public int GetHP() => hp;                                   //プレイヤーの現在のHPを取得するためのメソッド
        public int GetMaxHP() => maxHp;                             //プレイヤーの最大HPを取得するためのメソッド

        //プレイヤーのHPを設定するためのメソッド
        public void SetHP(int value)
        {
            hp = Mathf.Clamp(value, 0, maxHp);
            hpBar?.SetHP(hp, maxHp);
        }

        /// <summary>
        /// HPを切り替え先でも同じ割合を維持する形で転送する。preservePercent が false の場合は、現在のHPをそのまま転送する。
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

        //バフ込みの現在の最終ダメージカット率を返すように変更（UltSkillなどの互換性維持）
        public float GetDamageCutRate() => DamageCutRate;
        public void SetDamageCutRate(float value) => damageCutRate = Mathf.Clamp01(value); //プレイヤーのダメージカット率を設定するためのメソッド

       
        public void AddMoveSpeedBuff(float amount) { moveSpeedBuffTotal += amount; }
        public void RemoveMoveSpeedBuff(float amount) { moveSpeedBuffTotal -= amount; }
        public void AddDamageCutBuff(float amount) { damageCutBuffTotal += amount; }
        public void RemoveDamageCutBuff(float amount) { damageCutBuffTotal -= amount; }

        /// <summary>
        /// 一時的な移動速度バフをかけるメソッド（MGCSkillなどのコルーチン簡略化用）
        /// </summary>
        public void ApplyTemporaryMoveSpeedBuff(float buffValue, float duration)
        {
            StartCoroutine(TemporaryMoveSpeedBuffCoroutine(buffValue, duration));
        }
        private System.Collections.IEnumerator TemporaryMoveSpeedBuffCoroutine(float buffValue, float duration)
        {
            AddMoveSpeedBuff(buffValue);
            yield return new WaitForSeconds(duration);
            RemoveMoveSpeedBuff(buffValue);
        }
        // -------------------------------------------------------------------

        /// <summary>
        /// 外部から PlayerSO を割り当てる。プレイヤーのmaxHpを合わせる。
        /// </summary>
        public void SetPlayerSO(PlayerSO so, bool preserveHPPercent = true)
        {
            if (so == null) return;

            // ★修正：maxHpが0（ゲーム開始時の最初のスポーン）なら、割合保持を強制的にオフにする
            if (maxHp <= 0)
            {
                preserveHPPercent = false;
            }

            // 現在のHP割合を保持するための比率を取得
            float prevRatio = (maxHp > 0) ? (float)hp / maxHp : 1f;

            playerSO = so;
            ApplyPlayerSOValues(); // SO の値で上書き

            // HP を適切に調整
            if (preserveHPPercent)
            {
                hp = Mathf.RoundToInt(Mathf.Clamp01(prevRatio) * maxHp);
            }
            else
            {
                hp = maxHp; // 最初や、リセット時は満タンにする
            }

            // スキルコストは SO の開始値で初期化
            currentCost = playerSO.StartingCost;

            EnsureHPBarInitialized();
            hpBar?.SetHP(hp, maxHp);

            // コストテキストなど UI を更新
            if (costText != null)
            {
                costText.text = $"Cost: {currentCost}";
            }
        }

        /// <summary>
        /// プレイヤーがダメージを受けるメソッド
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (isDead) return; // 死亡している場合はダメージを受けない

            SoundManager soundManager = FindAnyObjectByType<SoundManager>();

            //音の多重再生防止のためのチェック
            if (!soundManager.SESource.isPlaying)
            {
                soundManager?.PlaySE(1); // ダメージを受けたときのSEを再生
            }
            //バフ込みの最終ダメージカット率（DamageCutRate)
            int finalDamage = Mathf.RoundToInt(damage * (1f - DamageCutRate));

            // 最低でも 1 ダメージは受けるようにする安全策
            if (finalDamage <= 0 && damage > 0) finalDamage = 1;

            hp -= finalDamage;

            // HPが0以下にならないようにクランプ
            hp = Mathf.Max(0, hp);

            // UIの更新
            hpBar?.SetHP(hp, maxHp);

            Debug.Log($"プレイヤーは {finalDamage} のダメージを受けました。現在のHP: {hp}/{maxHp}");

            if (hp <= 0)
            {
                Die();
            }
        }

        public int GetCurrentCost() => currentCost; //プレイヤーの現在のスキルコストを取得するためのメソッド

        public void ConsumeCost(int value)
        {
            currentCost = Mathf.Max(0, currentCost - value);
            if (costText != null)
            {
                costText.text = $"Cost: {currentCost}";
            }
        }

        public void RecoverCost(int value)
        {
            currentCost += value;
            if (costText != null)
            {
                costText.text = $"Cost: {currentCost}";
            }
        }

        // --- 死亡処理 ---
        private void Die()
        {
            if (isDead) return;

            isDead = true;

            inputController.enabled = false;
            animationController.enabled = false;
            weaponController?.DisableWeaponCollider();
            shieldController?.DisableShieldCollider();
            characterController.enabled = false;

            Debug.Log("プレイヤーは死亡しました");
            PlayerRagdollController ragdollController = GetComponent<PlayerRagdollController>();
            if (ragdollController != null)
            {
                ragdollController.ActivateRagdoll();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}