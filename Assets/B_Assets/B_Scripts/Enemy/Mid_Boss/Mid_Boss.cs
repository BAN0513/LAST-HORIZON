using System.Collections;
using TMPro;
using UnityEngine;

public class Mid_Boss : Enemy
{

    [Header("二段目の攻撃に派生する確率")]
    [Range(0, 100)][SerializeField] private float melee2Probability = 50;

    private EnemyShieldController _shieldController;

    //アニメーションで使うやつ
    public bool isMelee1 { get; private set; }
    public bool isMelee2 { get; private set; }
    public bool isBlock { get; private set; }
    public bool is360Attack {  get; private set; }

    protected override void Start()
    {
        base.Start();
        attackProbability = enemySO.attackInitProbability;

        _shieldController = GetComponentInChildren<EnemyShieldController>();
        _shieldController.Enemy = this;
    }

    protected override void Update()
    {
        if (isDeath || isHit) { return; }
        base.Update();
        CheckPlayerAttack();
    }

    private void CheckPlayerAttack()
    {
        float blockDistance = 3;  //この値より距離が遠いとブロックしない
        float blockDot = 0.5f;    //この値よりDotが低いとブロックしない

        if (isAction) 
        {
            isBlock = false;
        }
        else if (playerAnimationController.IsAttackMove && distance <= blockDistance && dot > blockDot)
        {

            isBlock = true;
            agent.isStopped = true;
            isLookPlayer = false;
            _shieldController.SetColliderActive(true);

        }
    }

    protected override void EngageMoveControl()
    {
        if (distance <= enemySO.engageDis)
        {
            //敵のスピードを少しだけ遅くする
            agent.speed = enemySO.walkMoveSpeed - DebufDEX;

            if (!isAction && !isBlock)
            {
                lotteryTime -= Time.deltaTime;
                if (lotteryTime <= 0)
                {
                    //行動の抽選で使う
                    rand = Random.Range(1, 101);

                    //次の抽選に必要な時間をランダムで決める
                    lotteryTime = Random.Range(lotteryMinTime, lotteryMaxTime);
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

        //一定距離近づくと止まるのでstoppingDistanceを0にする
        agent.stoppingDistance = 0;

        //一定距離近づくと攻撃する
        if (distance <= attackDis)
        {
            agent.isStopped = true;
            isLookPlayer = false;
            rand = 0;

            float attackDot = 0.4f;

            if (dot >= attackDot && !is360Attack)
            {
                isMelee1 = true;
            }
            else if (dot < attackDot && !isMelee1)
            {
                is360Attack = true;
            }
        }
    }

    protected override void Death()
    {
        base.Death();

        AttackJudgmentEnd();
    }

    //ここから下はAnimator関連の関数
    public void Melee2()
    {
        int rand = Random.Range(1, 101);
        float secondAttackDis = 3;     //この値より距離が遠いと二段目の攻撃に派生しない
        float secondAttackDot = 0.4f;  //この値よりDotが高いと前方攻撃、低いと360度攻撃

        //一定確率で二段目の攻撃に派生する
        if (rand <= melee2Probability && distance <= secondAttackDis)
        {
            if (dot >= secondAttackDot)
            {
                isMelee2 = true;
            }
            else
            {
                is360Attack = true;
            }
        }
        else
        {
            InitAnim();
        }
    }

    //攻撃のアニメーションが終わったら全部初期化する
    protected override void InitAnim()
    {
        base.InitAnim();
        isMelee1 = false;
        isMelee2 = false;
        is360Attack = false;
        isBlock = false;
    }

    public void BlockEnd()
    {
        agent.isStopped = false;
        isBlock = false;
        isLookPlayer = true;
        _shieldController.SetColliderActive(false);
    }
}
