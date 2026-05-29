using Takato;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    protected Transform target;
    protected NavMeshAgent agent;
    public NavMeshAgent Agent
    {
        get { return agent; }
        set { agent = value; }
    }
    protected PlayerController playerController;
    protected CharacterController playerCharacterController;
    public CharacterController PlayerCharacterController
    {
        get { return playerCharacterController; }
    }
    protected PlayerAnimationController playerAnimationController;
    protected EnemyAnimatorController enemyAnimatorController;

    protected int rand;
    protected float lotteryTime;
    protected float lotteryMinTime = 0.5f;
    protected float lotteryMaxTime = 2.0f;
    protected float dot;

    [Header("敵のScriptable Object")]
    [SerializeField] protected EnemySO enemySO;

    [Header("transform.forwardが正常に取れないから\n前方に空のオブジェクトを置いておく")]
    [SerializeField] protected Transform forward;

    [Header("0～この値までは近距離攻撃")]
    [SerializeField] protected float shortDis = 3;
    [Header("shortDisからこの値までを中距離攻撃")]
    [SerializeField] protected float mediumDis = 6;
    //mediumDis～engageDisまでは遠距離攻撃

    //攻撃時にこの値の距離まで近づく
    [SerializeField] protected float attackDis = 1.5f;

    //攻撃確率の保存用変数
    protected float attackProbability = 0;

    protected Coroutine backMoveCor = null;

    //敵のHP
    public int hp { get; private set; }

    //敵のスピードにかかるデバフ
    private float debufDEX = 1.0f;
    public float DebufDEX
    {
        get
        {
            return debufDEX;
        }
        set
        {
            debufDEX = value;
        }
    }

    //敵の防御力にかかるデバフ
    private int debufDEF;
    public int DebufDEF
    {
        get
        {
            return debufDEF;
        }
        set
        {
            debufDEF = value;
        }
    }

    //何かしらアクションが抽選されているかどうか
    protected bool isAction = false;

    //アニメーションが再生されているかどうか
    protected bool isAnimation = false;

    //プレイヤーを見続けるかどうか
    protected bool isLookPlayer = true;

    //プレイヤーと自身の距離
    protected float distance = 0;

    //接敵中か（今後常に敵対状態になるかも）
    public bool isContact {  get; protected set; }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyAnimatorController = GetComponent<EnemyAnimatorController>();

        target = GameObject.FindWithTag("Player").transform;
        playerController = target.GetComponent<PlayerController>();
        playerCharacterController = target.GetComponent<CharacterController>();
        playerAnimationController = target.GetComponent<PlayerAnimationController>();
    }

    protected virtual void Start()
    {
        distance = Vector3.Distance(transform.position, target.position);

        agent.updateRotation = false;

        hp = enemySO.maxHP;

        agent.stoppingDistance = enemySO.stoopingDis;

        lotteryTime = enemySO.attackCoolDown;

        attackProbability = enemySO.attackInitProbability;


        //画面の明るさ変更（後で別のとこに書く）
        //RenderSettings.ambientIntensity = SystemManager.instance.valueLight;
    }

    protected virtual void Update()
    {
       
        if (enemyAnimatorController.CheckCurrentAnim("Death") || enemyAnimatorController.CheckCurrentAnim("Hit")) { return; }
        //プレイヤーと自身の距離計算
        distance = Vector3.Distance(transform.position, target.position);

        //自身からプレイヤーのDotを取る
        DotPlayer();

        //常にプレイヤーの方向を見るようにする
        LookPlayer();

        if (isAnimation) { return; }
        //交戦時の処理
        EngageMoveControl();

        //追跡するかしないかを調整する関数
        AgentContact();
    }

    private void DotPlayer()
    {
        //自身からプレイヤーの方向を取る
        Vector3 toTarget = (target.position - transform.position).normalized;
        toTarget.y = 0;
        //自身の前方方向を取る
        Vector3 forwardDir = (forward.position - transform.position).normalized;
        forwardDir.y = 0;

        //内積で方向の一致度を取る
        dot = Vector3.Dot(toTarget, forwardDir);
    }

    private void LookPlayer()
    {
        if (isLookPlayer)
        {
            Vector3 dir = target.position - transform.position;
            dir.y = 0;
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(dir),
                    Time.deltaTime * enemySO.lookRotationSpeed
                    );
            }
        }
    }

    protected virtual void LookPlayerChange(bool isLook)
    {
        isLookPlayer = isLook;
        isAnimation = true;
        agent.isStopped = true;
    }

    private void AgentContact()
    {
        if (!isContact)
        {
            if (distance <= enemySO.searchDistance)
            {
                isContact = true;
                agent.isStopped = false;
            }
        }
        else
        {
            agent.SetDestination(target.position);
            if (enemySO.contactDistance <= distance)
            {
                isContact = false;
                agent.isStopped = true;
            }

        }
    }

    protected virtual void EngageMoveControl()
    {
        if (distance <= enemySO.engageDis)
        {
            //敵のスピードを少しだけ遅くする
            agent.speed = enemySO.walkMoveSpeed * DebufDEX;

            if (!isAction)
            {
                lotteryTime -= Time.deltaTime;
                if (lotteryTime <= 0)
                {

                    //行動の抽選で使う
                    rand = Random.Range(1, 101);

                    isAction = true;

                    //次の抽選に必要な時間をランダムで決める
                    lotteryTime = Random.Range(lotteryMinTime, lotteryMaxTime);
                }
            }
            else if (!isAnimation)
            {
                switch (rand)
                {
                    case int r when (r > 0 && r <= attackProbability):
                        ShortDistanceAction();
                        break;
                    default:
                        DoNotAttack();
                        break;
                }
            }
        }
        else
        {
            agent.speed = enemySO.dashMoveSpeed - DebufDEX;
            isAction = false;
            rand = 0;
        }
    }

    protected virtual void DoNotAttack()
    {
        //攻撃じゃなかったら攻撃の確率を上げる
        isAction = false;
        attackProbability += enemySO.attackUpProbability;
    }


    protected virtual void ShortDistanceAction() { }
    protected virtual void MediumDistanceAction() { }
    protected virtual void LongDistanceAction() { }

    public virtual void TakeDamage(int damage)
    {
        damage -= (enemySO.def - debufDEF);
        if (damage <= 0) { return; }

        hp -= damage;

        if (hp <= 0)
        {
            Init();
            Death();
        }
        else
        {
            if (!enemyAnimatorController.CheckCurrentAnim("Hit"))
            {
                enemyAnimatorController.SetTriggerAnim(EnemyAnimatorController.AnimationBase.Hit);
            }

            Init();
            agent.isStopped = true;
        }
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
        if (agent != null && enemySO != null)
            debufDEX = moveSpeedMultiplier;
    }

    // NavMeshAgentの速度を更新するメソッド
    protected void SetWalkSpeed()
    {
        if (agent != null && enemySO != null)
            debufDEX = moveSpeedMultiplier;
    }


    protected virtual void Death()
    {
        agent.isStopped = true;
        InitAnim();
        enemyAnimatorController.SetTriggerAnim(EnemyAnimatorController.AnimationBase.Death);
    }

    public virtual void Init()
    {
        agent.stoppingDistance = enemySO.stoopingDis;
        lotteryTime = enemySO.attackCoolDown;
        attackProbability = enemySO.attackInitProbability;
        rand = 0;
        agent.isStopped = false;
        isAction = false;
        isAnimation = false;
    }

    public virtual void InitAnim()
    {
        enemyAnimatorController.ResetTriggerAnim(EnemyAnimatorController.AnimationBase.Hit);
    }

    public virtual void InitAll()
    {
        Init();
        InitAnim();
    }

    //攻撃判定の出現
    public void AttackJudgmentActive(EnemyAttackRollController _weaponController)
    {
        _weaponController.SetColliderActive(true);
    }

    //攻撃判定の終了
    public void AttackJudgmentEnd(EnemyAttackRollController _weaponController)
    {
        _weaponController.SetColliderActive(false);
    }

    public void DeathAnimEnd()
    {
        Destroy(gameObject);
    }

    //デバッグ用
    [ContextMenu("Damage")]
    public void Damage()
    {
        TakeDamage(600);
    }
}
