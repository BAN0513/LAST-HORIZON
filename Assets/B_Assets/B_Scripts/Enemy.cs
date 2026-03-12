using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    protected Transform target;
    protected NavMeshAgent agent;

    protected int rand;
    protected float lotteryTime;

    [Header("敵のScriptable Object")]
    [SerializeField] protected EnemySO enemySO;

    [Header("敵のAnimatorController")]
    [SerializeField] protected EnemyAnimatorController enemyAnimatorController;

    [Header("HPのスライダー")]
    [SerializeField] protected Slider hpSliider;

    [Header("接敵距離（この値以下になると攻撃の抽選を開始する）")]
    [SerializeField] protected float engageDis = 5;

    [Header("接敵状態時の動くスピード")]
    [SerializeField] protected float engageMoveSpeed = 1;

    [Header("プレイヤーとの距離が値以下になると下がる行動をする")]
    [SerializeField] private float backActionDis = 2;

    [Header("下がる行動の時に下がる距離")]
    [SerializeField] private float backMoveDis = 3;

    protected Coroutine backMoveCor = null;

    //敵のHP
    public int hp { get; private set; }

    //攻撃などのアクションを起こしているかどうか
    protected bool isAction = false;

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

        hpSliider.maxValue = enemySO.maxHP;
        hpSliider.minValue = 0;
        hpSliider.value = enemySO.maxHP;

        distance = Vector3.Distance(transform.position, target.position);

        agent.updateRotation = false;

        hp = enemySO.maxHP;

        agent.speed = enemySO.moveSpeed;

        agent.stoppingDistance = enemySO.stoopingDis;
    }

    protected virtual void Update()
    {
        //プレイヤーと自身の距離計算
        distance = Vector3.Distance(transform.position, target.position);

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

    private void LookPlayer()
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
        float dot = Vector3.Dot(toTarget, moveDir);

        //magunitudeでvelocityの長さを取る（0.1fより下だと動いていない）
        if (agent.velocity.magnitude < 0.1f)
        {
            isWalking = false;
            isBackMove = false;
        }
        //dotが0より高いと前に進んでいるので前に進むアニメーションを動かす
        else if (dot > 0)
        {
            //接敵距離より遠いとダッシュをして、近いと歩く
            if (distance >= engageDis)
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
        if (distance <= backActionDis && !isAction)
        {
            if (isBackMove) { return; }
            backMoveCor = StartCoroutine(BackMove());
        }
    }

    IEnumerator BackMove()
    {
        agent.stoppingDistance = 0;

        while (distance <= backMoveDis)
        {
            //敵の方向を取る
            Vector3 toTarget = (target.position - transform.position).normalized;
            toTarget.y = 0;

            //敵の方向と反対方向を取る
            Vector3 pos = transform.position + -toTarget * backMoveDis;

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
        }
    }

    protected virtual void Death()
    {
        agent.isStopped = true;
        isDeath = true;
    }

    protected abstract void InitAnim();

    public void IsHitAnimEnd()
    {
        isHit = false;
    }

    public void DeathAnimEnd()
    {
        Destroy(gameObject);
    }

    //デバッグ用
    [ContextMenu("Damage")]
    public void Damage()
    {
        TakeDamage(100);
    }

    //デバッグ用
    [ContextMenu("1Damage")]
    public void OneDamage()
    {
        TakeDamage(1);
    }
}
