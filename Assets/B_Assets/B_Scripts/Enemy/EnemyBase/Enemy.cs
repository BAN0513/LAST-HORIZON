using System.Collections.Generic;
using Takato;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    [Header("敵のScriptable Object")]
    [SerializeField] protected EnemySO enemySO;

    [Header("HPのスライダー")]
    [SerializeField] protected Slider hpSliider;

    [Header("ほぼ中ボス用。\nNav Mesh Obstacleの影響を受けなくなる")]
    [SerializeField] private bool isNoObstacleAvoidance = false;

    protected PlayerController playerController;
    protected CharacterController playerCharacterController;
    protected EnemyAnimatorController enemyAnimatorController;
    protected EventProgress eventProgress;

    protected float dot;

    //敵のHP
    protected int hp;

    //プレイヤーを見続けるかどうか
    protected bool isLookPlayer = true;

    //歩いているか
    protected bool isWalk = true;

    //敵が攻撃された後に連続で攻撃が当たらないようにするための変数
    protected float invincibilityTime  = 0.5f;
    protected float invincibilityTimer = 0;

    private float lotteryTime;
    private float lotteryMinTime = 0.5f;
    private float lotteryMaxTime = 3.0f;

    private float currentStoppingDistance = 0.0f;

    //敵がプレイヤーを発見しているかどうか
    private float contactDis;
    private float contactDot;
    private float searchDis;

    //プレイヤーと自身の距離
    public float Distance { get; set; }

    public Transform Target { get; private set; }

    public NavMeshAgent Agent { get; private set; }

    //敵のスピードにかかるデバフ
    public float DebufDEX { get; set; } = 1.0f;

    //敵の防御力にかかるデバフ
    public int DebufDEF { get; set; }

    //何かしらアクションが抽選されているかどうか
    public bool IsAction { get; set; }

    //アニメーションが再生されているかどうか
    public bool IsActionAnimation { get; set; }

    //振り向きのスピード
    public float LookRotationSpeed { get; set; }


    protected enum EnemyBaseState
    {
        Search,
        Contact,
        Dead

    }
    protected EnemyBaseState enemyBaseState = EnemyBaseState.Search;

    protected EnemyActionSO currentAction;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();

        if (Agent == null)
        {
            Debug.LogError("navMeshが見つからない");
        }
        enemyAnimatorController = GetComponent<EnemyAnimatorController>();
        eventProgress = GetComponentInParent<EventProgress>();

        // 開始時にすでにシーンにプレイヤーがいる場合のみ実行
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            UpdateTarget(playerObj);
        }
    }

    protected virtual void Start()
    {
        IsAction = false;

        if (hpSliider != null)
        {
            hpSliider.maxValue = enemySO.maxHP;
            hpSliider.minValue = 0;
            hpSliider.value = enemySO.maxHP;
        }

        Distance = Vector3.Distance(transform.position, Target.position);

        hp = enemySO.maxHP;

        Agent.updateRotation = false;

        Agent.stoppingDistance = enemySO.stoopingDis;

        if (isNoObstacleAvoidance)
        {
            Agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        }

        LookRotationSpeed = enemySO.lookRotationSpeed;

        IsActionAnimation = false;

        contactDis = enemySO.contactDis;
        contactDot = enemySO.contactDot;
        searchDis = enemySO.searchDis;

        //画面の明るさ変更（後で別のとこに書く）
        //RenderSettings.ambientIntensity = SystemManager.instance.valueLight;
    }

    protected virtual void Update()
    {
        if (enemyBaseState == EnemyBaseState.Dead) { return; }

        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }

        //playerタグのついたオブジェクトを見つける(Awakeで取得失敗した際の保険です。)
        if(Target == null)
        {
            Target = GameObject.FindWithTag("Player").transform;
            playerController = Target.GetComponent<PlayerController>();
            playerCharacterController = Target.GetComponent<CharacterController>();
        }

        //プレイヤーと自身の距離計算
        Distance = Vector3.Distance(transform.position, Target.position);

        //自身からプレイヤーのDotを取る
        DotPlayer();

        //追跡するかしないかを調整する関数
        AgentContact();

        if (enemyBaseState != EnemyBaseState.Contact) { return; }

        //常にプレイヤーの方向を見るようにする
        LookPlayer();

        //交戦時の処理
        EngageMoveControl();

        //行動の計算
        CalcActionUpdate();
    }

    private void DotPlayer()
    {
        //自身からプレイヤーの方向を取る
        Vector3 toTarget = (Target.position - transform.position).normalized;
        toTarget.y = 0;
        //自身の前方方向を取る
        Vector3 forwardDir = (transform.position + transform.forward - transform.position).normalized;
        forwardDir.y = 0;

        //内積で方向の一致度を取る
        dot = Vector3.Dot(toTarget, forwardDir);
    }

    private void LookPlayer()
    {
        if (isLookPlayer)
        {
            Vector3 dir;
            dir.x = 0;
            dir.z = 0;

            if (Agent.speed == enemySO.walkMoveSpeed)
            {
                dir = Target.position - transform.position;

            }
            else
            {
                dir = (transform.position + Agent.velocity) - transform.position;
            }
            dir.y = 0;
            SetRotation(dir);
        }
    }
    private void SetRotation(Vector3 dir)
    {
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * LookRotationSpeed
                );
        }
    }

    private void AgentContact()
    {
        if (Agent.isOnNavMesh)
        {
            if (Distance <= contactDis && dot >= contactDot && enemyBaseState != EnemyBaseState.Contact)
            {
                ContactAnimation();
            }
            else if (Distance >= searchDis && enemyBaseState == EnemyBaseState.Contact)
            {
                enemyBaseState = EnemyBaseState.Search;
                InitAll();
                Agent.isStopped = true;
                enemyAnimatorController.ForcedQuitAnimation();
            }
        }

        //agent.stoppingDistanceの値の付近を行ったり来たりするとアニメーションがガタガタするのでそれ対策
        if (Distance <= Agent.stoppingDistance && isWalk)
        {
            isWalk = false;
            Agent.stoppingDistance = enemySO.stoopingDis + 1;
            currentStoppingDistance = Agent.stoppingDistance;
        }
        else if (Distance >= Agent.stoppingDistance && !isWalk)
        {
            isWalk = true;
            Agent.stoppingDistance = enemySO.stoopingDis - 1;
            currentStoppingDistance = Agent.stoppingDistance;
        }

        if (IsActionAnimation) { return; }

        if (enemyBaseState == EnemyBaseState.Contact)
        {
            Agent.SetDestination(Target.position);
        }
    }

    protected virtual void ContactAnimation() { enemyBaseState = EnemyBaseState.Contact; }

    private void EngageMoveControl()
    {
        if (Distance <= enemySO.engageDis && Mathf.Abs(Target.position.y - transform.position.y) < 0.5f)
        {
            //敵のスピードを少しだけ遅くする
            Agent.speed = enemySO.walkMoveSpeed * DebufDEX;

            if (!IsAction)
            {
                lotteryTime -= Time.deltaTime;      

                if (lotteryTime <= 0)
                {
                    IsAction = true;

                    //次の抽選に必要な時間をランダムで決める
                    lotteryTime = Random.Range(lotteryMinTime, lotteryMaxTime);
                }
            }
        }
        else
        {
            Agent.speed = enemySO.dashMoveSpeed - DebufDEX;
            //isAction = false;
        }
    }

    protected void CalcActionUpdate()
    {
        if (IsActionAnimation || enemyBaseState == EnemyBaseState.Dead) { return; }

        //SOは並びかえ手はいけないから変数に代入する
        List<EnemyActionSO> action = new List<EnemyActionSO>(enemySO.action);

        EnemyActionSO executeAction = CalcAction(action);

        if (executeAction != null)
        {
            executeAction.Execute(enemyAnimatorController);

            if (executeAction != currentAction)
            {
                CurrentActionEnd();
            }

            currentAction = executeAction;
        }
        else
        {
            CurrentActionEnd();
        }
    }

    protected void CurrentActionEnd()
    {
        if (currentAction == null) { return; }
        currentAction.ActionEnd(enemyAnimatorController);
        currentAction = null;
    }

    EnemyActionSO CalcAction(List<EnemyActionSO> action)
    {
        List<float> scores = new List<float>();

        //各行動のスコアを計算して、0.0fの物はListから削除する
        for (int i = 0; i < action.Count; i++)
        {
            float score = action[i].ScoreCalculation(Distance, dot, this);

            if (score == Mathf.Infinity) { return action[i]; }

            if (score == 0.0f)
            {
                action.RemoveAt(i);
                i--;
            }
            else
            {
                scores.Add(score);
            }
        }

        //アクションが空の場合はreturnする
        if (action.Count <= 0) { return null; }

        //スコアの大きい順にアクションを並び替える
        for (int i = 0; i < scores.Count; i++)
        {
            for (int j = 1; j < scores.Count; j++)
            {
                if (scores[i] <= scores[j])
                {
                    float saveScore = scores[i];
                    EnemyActionSO saveAction = action[i];

                    scores[i] = scores[j];
                    action[i] = action[j];

                    scores[j] = saveScore;
                    action[j] = saveAction;
                    break;
                }
            }
        }

        //各行動の確率をリストに入れて、すべての確率の合計を取っておく
        List<float> actionProbability = new List<float>();
        float totalProbability = 0.0f;
        for (int i = 0; i < action.Count; i++)
        {
            actionProbability.Add(action[i].baseProbability / (i + 1));

            totalProbability += actionProbability[i];
        }

        //余った確率をアクションの数で割る
        float equal = (100 - totalProbability) / action.Count;

        float rand = Random.Range(0, 101);
        float save = 0.0f;

        Debug.Log("<color=yellow> Random : " + rand + "</color>");
        //Debugですべての行動の確率を確認するために一時的に分けて書いてます。
        for (int i = 0; i < action.Count; i++)
        {
            actionProbability[i] += equal;
            Debug.Log("<color=yellow>" + action[i].actionName + " : " + actionProbability[i] + "%</color>");
        }

        //ランダムの数値と確率を使ってアクションを決める
        for (int i = 0; i < action.Count; i++)
        {
            save += actionProbability[i];

            if (save >= rand)
            {
                return action[i];
            }
        }
        return null;
    }

    public virtual void TakeDamage(int damage, SoundManager sound = null, int seNumber = -1)
    {
        if (enemyBaseState == EnemyBaseState.Dead) { return; }
        if (invincibilityTimer > 0) { return; }

        if (sound != null) { sound.PlaySE(seNumber); }
        if (enemyBaseState != EnemyBaseState.Contact) { ContactAnimation(); }  //未発見状態の場合は強制的に発見状態に変更する

        damage -= (enemySO.def - DebufDEF);
        if (damage <= 0) { return; }

        hp -= damage;

        hpSliider.value = hp;

        Debug.Log("Enemy_HP" + hp);

        if (hp <= 0)
        {
            Death();
        }

        invincibilityTimer = invincibilityTime;
    }

    //移動速度の倍率
    protected float moveSpeedMultiplier = 1.0f;
    public float MoveSpeedMultiplier
    {
        get => moveSpeedMultiplier;
        set
        {
            moveSpeedMultiplier = value;

            // 必ず現在の速度を再設定
            if (enemyAnimatorController.CheckCurrentAnim("Running"))
                SetDashSpeed();
            else if (enemyAnimatorController.CheckCurrentAnim("Walking") || enemyAnimatorController.CheckCurrentAnim("Walk Back"))
                SetWalkSpeed();
            else
                SetWalkSpeed(); // 停止時もwalk速度
        }
    }

    // NavMeshAgentの速度を更新するメソッド
    protected void SetDashSpeed()
    {
        if (Agent != null && enemySO != null)
            DebufDEX = moveSpeedMultiplier;
    }

    // NavMeshAgentの速度を更新するメソッド
    protected void SetWalkSpeed()
    {
        if (Agent != null && enemySO != null)
            DebufDEX = moveSpeedMultiplier;
    }

    protected virtual void Death()
    {
        enemyBaseState = EnemyBaseState.Dead;
        InitAll();
        hpSliider.gameObject.SetActive(false);
        Agent.isStopped = true;
        enemyAnimatorController.SetTriggerAnim(EnemyAnimatorController.AnimationBase.Death);
    }

    /// <summary>
    /// 第一引数はtrueの場合プレイヤーを見る。 第二引数はtrueの場合移動を止める
    /// </summary>
    /// <param name="isLook"></param>
    /// <param name="isStop"></param>
    public void SetLookPlayerAndEnemyStop(bool isLook, bool isStop)
    {
        isLookPlayer = isLook;
        Agent.isStopped = isStop;
    }


    public virtual void Init()
    {
        Agent.stoppingDistance = currentStoppingDistance;
        LookRotationSpeed = enemySO.lookRotationSpeed;
        isLookPlayer = true;
        Agent.isStopped = false;
        IsAction = false;
        IsActionAnimation = false;
    }

    public virtual void InitAnim()
    {
        //全アニメーションのリセット
        enemyAnimatorController.ResetTriggerAnim(EnemyAnimatorController.AnimationBase.Hit);
        enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Walk, false);
        enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Dash, false);
        enemyAnimatorController.ForcedQuitAnimation();
    }

    public virtual void InitAll()
    {
        Init();
        InitAnim();
    }

    //攻撃判定の出現
    public void AttackJudgmentActive(EnemyAttackRollController _weaponController)
    {
        _weaponController.Damage = currentAction.damage;
        _weaponController.SetColliderActive(true);
    }

    //攻撃判定の終了
    public void AttackJudgmentEnd(EnemyAttackRollController _weaponController)
    {
        _weaponController.SetColliderActive(false);
    }

    public void DeathAnimEnd()
    {
        if (eventProgress != null)
        {
            eventProgress.DestroyEnemy(this);
        }
        else
        {
            Debug.Log("EventProgressが見つかりません。");
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// キャラクター切り替え時やスポーン時に、新しいプレイヤーの参照を設定し直す
    /// </summary>
    public void UpdateTarget(GameObject newPlayer)
    {
        if (newPlayer == null) return;

        Target = newPlayer.transform;

        //ここで各コンポーネントを安全に取得
        playerController = Target.GetComponent<Takato.PlayerController>();
        playerCharacterController = Target.GetComponent<CharacterController>();

        // デバッグ用ログ
        if (playerController == null)
            Debug.LogWarning($"[{gameObject.name}] PlayerController の取得に失敗しました。Playerプレハブにスクリプトが付いているか確認してください。");
    }
}
