using System.Collections;
using UnityEngine;

public class Wolf_Boss : Enemy
{

    [Header("0～この値までは近距離攻撃")]
    [SerializeField] private float shortDis = 3;
    [Header("shortDisからこの値までを中距離攻撃")]
    [SerializeField] private float mediumDis = 6;
    //mediumDis～engageDisまでは遠距離攻撃

    //アニメーションで使うやつ
    public bool isAttack_1 { get; private set; }
    public bool isAttack_2 { get; private set; }
    public bool isDashAttacKBefore { get; private set; }
    public bool isRotationAttack { get; private set; }
    public bool isTailAttack { get; private set; }

    protected override void Start()
    {
        base.Start();
        attackProbability = enemySO.attackInitProbability;
    }

    protected override void Update()
    {
        Debug.Log(agent.isStopped);
        base.Update();
    }

    protected override void EngageMoveControl()
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

                    switch (distance)
                    {
                        case float dis when (dis >= 0 && dis <= shortDis):
                            ShortDistanceAction();
                            break;
                        case float dis when (dis > shortDis && dis <= mediumDis):
                            MediumDistanceAction();
                            break;
                        case float dis when (dis > mediumDis && dis <= enemySO.engageDis):
                            LongDistanceAction();
                            break;
                    }


                    //次の抽選に必要な時間をランダムで決める
                    lotteryTime = Random.Range(lotteryMinTime, lotteryMaxTime);
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
            //case int r when (r > 0 && r <= attackProbability):
            //    StartCoroutine(Tornado());
            //    break;
            case int r when (r > 0 && r <= attackProbability):

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

    private void LookPlayerChange(bool isLook)
    {
        StopBackMoveCor();

        isLookPlayer = isLook;
        agent.isStopped = true;
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


    //攻撃のアニメーションが終わったら全部初期化する
    protected override void InitAnim()
    {
        base.InitAnim();
        isAttack_1 = false;
        isAttack_2 = false;
        isDashAttacKBefore = false;
        isRotationAttack = false;
        isTailAttack = false;

    }
}

