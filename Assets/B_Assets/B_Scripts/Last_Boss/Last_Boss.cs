using UnityEngine;

public class Last_Boss : Enemy
{
    [SerializeField] private GameObject magicCircleEffect;
    [SerializeField] private GameObject tornadoEffect;
    [SerializeField] private GameObject fireEffect;
    private GameObject magicCircle;

    //アニメーションで使うやつ
    public bool isChant { get; private set; }
    public bool isMagic { get; private set; }
    public bool isFire { get; private set; }

    protected override void Start()
    {
        base.Start();
        attackProbability = enemySO.attackInitProbability;
    }

    protected override void Update()
    {
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

                    //次の抽選に必要な時間をランダムで決める
                    lotteryTime = Random.Range(0.5f, 2.0f);
                    isAction = true;
                }
            }

            //確率で行動を決める
            switch (rand)
            {
                case int r when (r > 0 && r <= attackProbability):
                    AttackMove();
                    break;
                case 0:
                    break;
                default:
                    //攻撃じゃなかったら攻撃の確率を上げる
                    if (isAction)
                    {
                        isAction = false;
                        attackProbability += enemySO.attackUpProbability;
                    }
                    break;
            }
        }
        else
        {
            agent.speed = enemySO.dashMoveSpeed - DebufDEX;
            isAction = false;
            rand = 0;
        }
    }




    private void AttackMove()
    {
        //もし後退中なら後退を止める
        if (backMoveCor != null)
        {
            isBackMove = false;
            StopCoroutine(backMoveCor);
        }
        rand = 0;
        isLookPlayer = false;
        agent.isStopped = true;

        if (distance >= 7)
        {
            isChant = true;
        }
        else
        {
            isFire = true;
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
        Destroy(tornado, 15);
    }

    public void Fire()
    {
        Vector3 toTarget = target.position - transform.position;
        Vector3 nor = (toTarget).normalized;
        Quaternion quaternion = Quaternion.LookRotation(toTarget);
        GameObject fire = Instantiate(fireEffect, forward.position, quaternion);
        Rigidbody rb = fire.GetComponent<Rigidbody>();
        rb.linearVelocity = nor * 5;
    }

    //攻撃のアニメーションが終わったら全部初期化する
    protected override void InitAnim()
    {
        base.InitAnim();
        isChant = false;
        isMagic = false;
        isFire = false;
    }
}
