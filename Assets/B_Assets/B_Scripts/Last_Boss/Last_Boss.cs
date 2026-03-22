using System.Collections;
using UnityEngine;

public class Last_Boss : Enemy
{
    [SerializeField] private GameObject magicCircleEffect;
    [SerializeField] private GameObject tornadoEffect;
    [SerializeField] private GameObject fireEffect;
    [SerializeField] private GameObject impactEffect;
    private GameObject magicCircle;

    //アニメーションで使うやつ
    public bool isChant { get; private set; }
    public bool isMagic { get; private set; }
    public bool isFire { get; private set; }
    public bool isSlash { get; private set; }
    public bool isRunJumpAttack {  get; private set; }

    protected override void Start()
    {
        base.Start();
        attackProbability = enemySO.attackInitProbability;
    }

    protected override void Update()
    {
        //Debug.Log(distance);
        if (isDeath || isHit) 
        {
            Destroy(magicCircle);
            return; 
        }
        base.Update();
    }

    protected override void EngageMoveControl()
    {
        if (distance <= enemySO.engageDis)
        {
            //敵のスピードを少しだけ遅くする
            agent.speed = enemySO.walkMoveSpeed - DebufDEX;

            if (!isAction)
            {
                lotteryTime -= Time.deltaTime;
                if (lotteryTime <= 0)
                {
                    //行動の抽選で使う
                    rand = Random.Range(1, 101);

                    isAction = true;

                    switch (distance)
                    {
                        case float dis when (dis >= 0 && dis <= 3):
                            ShortDistanceAction();
                            break;
                        case float dis when (dis > 3 && dis <= 7):
                            MediumDistanceAction();
                            break;
                        case float dis when (dis > 7 && dis <= 10):
                            LongDistanceAction();
                            break;
                    }

                    //次の抽選に必要な時間をランダムで決める
                    lotteryTime = Random.Range(0.5f, 2.0f);
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

    private void ShortDistanceAction()
    {
        //確率で行動を決める
        switch (rand)
        {
            case int r when (r > 0 && r <= attackProbability):
                StartCoroutine(Slash());
                break;
            default:
                DoNotAttack();
                break;
        }
    }

    private void MediumDistanceAction()
    {
        switch (rand)
        {
            case int r when (r > 0 && r <= attackProbability):
                StartCoroutine(Fire());
                break;
            default:
                DoNotAttack();
                break;
        }
    }

    private void LongDistanceAction()
    {
        switch (rand)
        {
            case int r when (r > 0 && r <= attackProbability / 2):
                StartCoroutine(Tornado());
                break;
            case int r when (r > attackProbability / 2 && r <= attackProbability):
                StartCoroutine(DashJumpAttack());
                break;
            default:
                DoNotAttack();
                break;
        }
    }

    private void DoNotAttack()
    {
        //攻撃じゃなかったら攻撃の確率を上げる
        isAction = false;
        attackProbability += enemySO.attackUpProbability;

    }

    IEnumerator Slash()
    {
        StopBackMoveCor();

        agent.stoppingDistance = 0;

        if (distance <= attackDis)
        {
            agent.isStopped = true;
            isSlash = true;
        }

        yield return null;
    }

    IEnumerator DashJumpAttack()
    {
        StopBackMoveCor();
        isRunJumpAttack = true;
        isLookPlayer = false;
        agent.isStopped = true;
        yield return null;
    }

    IEnumerator Fire()
    {
        StopBackMoveCor();
        isLookPlayer = false;
        agent.isStopped = true;
        isFire = true;
        yield return null;
    }

    IEnumerator Tornado()
    {
        StopBackMoveCor();

        isLookPlayer = false;
        agent.isStopped = true;
        isChant = true;

        yield return null;
    }

    private void StopBackMoveCor()
    {
        //rand = 0;
        //もし後退中なら後退を止める
        if (backMoveCor != null)
        {
            isBackMove = false;
            StopCoroutine(backMoveCor);
        }
    }

    protected override void Death()
    {
        base.Death();

        AttackJudgmentEnd();
    }

    //ここから下はAnimator関連の関数
    public void ChantStart()
    {
        magicCircle = Instantiate(magicCircleEffect, transform.position, Quaternion.identity);
        Destroy(magicCircle, 10);
    }

    public void ChantEnd()
    {
        isMagic = true;
    }

    public void Magic()
    {
        GameObject tornado = Instantiate(tornadoEffect, transform.position, Quaternion.identity);
        TornadoController tornadoController = tornado.GetComponent<TornadoController>();
        tornadoController.damage = enemySO.damage;
        Destroy(tornado, 15);
    }

    public void FireSpawn()
    {
        Vector3 toTarget = target.position - forward.position;
        Vector3 nor = (toTarget).normalized;
        Quaternion quaternion = Quaternion.LookRotation(toTarget);
        GameObject fire = Instantiate(fireEffect, forward.position, quaternion);
        FireController fireController = fire.GetComponent<FireController>();
        fireController.damage = enemySO.damage;
        Rigidbody rb = fire.GetComponent<Rigidbody>();
        rb.linearVelocity = nor * 5;
    }

    public void Impact()
    {
        GameObject impact = Instantiate(impactEffect, transform.position, Quaternion.identity);
        ImpactController impactController = impact.GetComponent<ImpactController>();
        impactController.damage = enemySO.damage;
        Destroy(impact, 1);
    }

    //攻撃のアニメーションが終わったら全部初期化する
    protected override void InitAnim()
    {
        base.InitAnim();
        isChant = false;
        isMagic = false;
        isFire = false;
        isSlash = false;
        isRunJumpAttack = false;
    }
}
