using System.Collections;
using Takato;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    protected Transform target;
    protected NavMeshAgent agent;
    protected PlayerController playerController;
    protected PlayerAnimationController playerAnimationController;

    protected int rand;
    protected float lotteryTime;
    protected float dot;

    [Header("敵のScriptable Object")]
    [SerializeField] protected EnemySO enemySO;

    [Header("敵のAnimatorController")]
    [SerializeField] protected EnemyAnimatorController enemyAnimatorController;

    [Header("HPのスライダー")]
    [SerializeField] protected Slider hpSliider;

    [Header("武器のスクリプト")]
    [SerializeField] protected WeaponController _weaponController;

    [Header("transform.forwardが正常に取れないから\n前方に空のオブジェクトを置いておく")]
    [SerializeField] protected Transform forward;

    //攻撃時にこの値の距離まで近づく
    [SerializeField] protected float attackDis = 1.5f;

    //攻撃確率の保存用変数
    protected float attackProbability = 0;

    protected Coroutine backMoveCor = null;

    //敵のHP
    public int hp { get; private set; }

    //敵のスピードにかかるデバフ
    private float debufDEX;
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

    //プレイヤーを見続けるかどうか
    protected bool isLookPlayer = true;

    //プレイヤーと自身の距離
    protected float distance = 0;

    //接敵中か（今後常に敵対状態になるかも）
    public bool isContact {  get; protected set; }

    //アニメーション用
    public bool isWalking {  get; protected set; }
    public bool isDeath { get; protected set; }
    public bool isHit { get; protected set; }
    public bool isBackMove { get; protected set; }
    public bool isDash { get; protected set; }

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        target = GameObject.FindWithTag("Player").transform;
        playerController = target.GetComponent<PlayerController>();
        playerAnimationController = target.GetComponent<PlayerAnimationController>();
        

        if (hpSliider != null)
        {
            hpSliider.maxValue = enemySO.maxHP;
            hpSliider.minValue = 0;
            hpSliider.value = enemySO.maxHP;
        }

        distance = Vector3.Distance(transform.position, target.position);

        agent.updateRotation = false;

        hp = enemySO.maxHP;

        agent.speed = enemySO.dashMoveSpeed;

        agent.stoppingDistance = enemySO.stoopingDis;
    }

    protected virtual void Update()
    {
        //プレイヤーと自身の距離計算
        distance = Vector3.Distance(transform.position, target.position);

        //自身からプレイヤーのDotを取る
        DotPlayer();

        //常にプレイヤーの方向を見るようにする
        LookPlayer();

        //移動アニメーションの変更処理
        MoveAnimControl();

        //後ろに下がる行動の処理
        BackMoveControl();

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

    private void MoveAnimControl()
    {
        //自身からプレイヤーの方向を取る
        Vector3 toTarget = (target.position - transform.position).normalized;

        //自身の動く方向を取る
        Vector3 moveDir = agent.velocity.normalized;

        //内積で方向の一致度を取る
        float moveDot = Vector3.Dot(toTarget, moveDir);

        //magunitudeでvelocityの長さを取る（0.1fより下だと動いていない）
        if (agent.velocity.magnitude < 0.1f)
        {
            isWalking = false;
            isBackMove = false;
        }
        //dotが0より高いと前に進んでいるので前に進むアニメーションを動かす
        else if (moveDot > 0)
        {
            //接敵距離より遠いとダッシュをして、近いと歩く
            if (distance >= enemySO.engageDis)
            {
                isDash = true;
                isWalking = false;
            }
            else
            {
                isWalking = true;
                isDash = false;
            }
            isBackMove = false;
        }
        //dotが0より低いと後ろに進むアニメーションを動かす
        else
        {
            isWalking = false;
            isBackMove = true;
        }
    }

    private void BackMoveControl()
    {
        //距離がbackActionDisより小さかったり、攻撃をしていない場合に下がる動作をする
        if (distance <= enemySO.backActionDis && !isAction)
        {
            if (isBackMove) { return; }
            backMoveCor = StartCoroutine(BackMove());
        }
    }

    IEnumerator BackMove()
    {
        agent.stoppingDistance = 0;

        while (distance <= enemySO.backMoveDis)
        {
            //敵の方向を取る
            Vector3 toTarget = (target.position - transform.position).normalized;
            toTarget.y = 0;

            //敵の方向と反対方向を取る
            Vector3 pos = transform.position + -toTarget * enemySO.backMoveDis;

            //キャラクターの後ろを目的地として設定する
            agent.SetDestination(pos);
            yield return null;
        }
        agent.stoppingDistance = enemySO.stoopingDis;
        backMoveCor = null;
    }

    protected abstract void EngageMoveControl();

    public void TakeDamage(int damage)
    {
        damage -= enemySO.def - debufDEF;
        if (damage <= 0) { return; }

        hp -= damage;
        hpSliider.value = hp;

        if (hp <= 0)
        {
            hpSliider.gameObject.SetActive(false);
            Death();
        }
        else
        {
            isHit = true;
            InitAnim();
            agent.isStopped = true;
        }
    }

    protected virtual void Death()
    {
        agent.isStopped = true;
        isDeath = true;
    }

    protected virtual void InitAnim()
    {
        isLookPlayer = true;
        agent.stoppingDistance = enemySO.stoopingDis;
        lotteryTime = enemySO.attackCoolDown;
        attackProbability = enemySO.attackInitProbability;
        rand = 0;
        agent.isStopped = false;
        isAction = false;
    }

    //向き補正
    public void DirCorrection()
    {
        isLookPlayer = true;
    }

    public void DirCorrectionEnd()
    {
        isLookPlayer = false;
    }

    //攻撃判定の出現
    public void AttackJudgmentActive()
    {
        _weaponController.SetColliderActive(true);
    }

    //攻撃判定の終了
    public void AttackJudgmentEnd()
    {
        _weaponController.SetColliderActive(false);
    }

    public void IsHitAnimEnd()
    {
        isHit = false;
        agent.isStopped = false;
    }

    public void DeathAnimEnd()
    {
        Destroy(gameObject);
    }

    //デバッグ用
    [ContextMenu("Damage")]
    public void Damage()
    {
        TakeDamage(60);
    }

    //デバッグ用
    [ContextMenu("1Damage")]
    public void OneDamage()
    {
        TakeDamage(1);
    }
}
