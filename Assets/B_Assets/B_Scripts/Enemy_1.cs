using System.Collections;
using UnityEngine;

public class Enemy_1 : Enemy
{
    [SerializeField] private WeaponController _weaponController;

    private float actionCoolDown = 3;
    private float timeCnt = 0;
    private Coroutine backMoveCor = null;
    private int rand;
    private float randTime;

    //下がる行動に移る距離
    private float backDis = 2;
    //下がる距離
    private float backMoveDis = 3;

    private float engageDis = 10;
    private float attackDis = 2f;

    private float engageMoveSpeed = 1;

    private float attackInitProbability = 20;
    private float attackProbability = 20;
    private float attackUpProbability = 20;

    //アニメーションで使うやつ
    public bool isMelee1;
    public bool isMelee2;
    public bool isBackMove;

    protected override void Start()
    {
        base.Start();
        attackProbability = attackInitProbability;
        timeCnt = actionCoolDown;
    }

    protected override void Update()
    {
        base.Update();

        MoveAnimControl();
        
        if (!isAction)
        {
            timeCnt += Time.deltaTime;
        }

        if (distance <= backDis && !isAction)
        {
            if (isBackMove) { return; }
            backMoveCor = StartCoroutine(BackMove());
        }

        if (distance <= engageDis)
        {
            agent.speed = engageMoveSpeed;
            if (timeCnt < actionCoolDown) { return; }

            if (!isAction)
            {
                randTime -= Time.deltaTime;
                if (randTime <= 0)
                {
                    //行動の抽選で使う
                    rand = Random.Range(1, 101);

                    //次の抽選に必要な時間をランダムで決める
                    randTime = Random.Range(0.5f, 2.1f);
                    isAction = true;
                }
            }

            //確率で行動を決める
            switch (rand)
            {
                case int r when (r > 0 && r <= attackProbability):
                    AttackMove();
                    break;
                default:
                    if (isAction)
                    {
                        isAction = false;
                        attackProbability += attackUpProbability;
                        rand = 0;
                    }
                    break;
            }
        }
        else
        {
            agent.speed = initMoveSpeed;
            isAction = false;
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
            isWalking = true;
            isBackMove = false;
        }
        //dotが0より低いと後ろに進むアニメーションを動かす
        else
        {
            isWalking = false;
            isBackMove = true;
        }
    }

    IEnumerator BackMove()
    { 
        agent.stoppingDistance = 0;

        while (distance <= backMoveDis)
        {
            //Debug.Log("Back");

            //敵の方向を取る
            Vector3 toTarget = (target.position - transform.position).normalized;
            toTarget.y = 0;

            //敵の方向と反対方向を取る
            Vector3 pos = transform.position + -toTarget * backMoveDis;

            agent.SetDestination(pos);
            yield return null;
        }
        isBackMove = false;
        agent.stoppingDistance = initStoopingDis;
    }


    private void AttackMove()
    {
        //Debug.Log("攻撃");

        //もし後退中なら後退を止める
        if (backMoveCor != null)
        {
            isBackMove = false;
            StopCoroutine(backMoveCor);
        }

        //敵の止まる場所を攻撃する位置より少し低く設定する
        agent.stoppingDistance = attackDis - 1f;

        //一定距離近づくと攻撃する
        if (distance <= attackDis)
        {
            timeCnt = 0;
            agent.isStopped = true;
            isMelee1 = true;
        }
    }

    //ここから下はAnimator関連の関数

    public void Melee2()
    {
        int rand = Random.Range(1, 101);

        if (rand <= 50)
        {
            isMelee2 = true;
        }
        else
        {
            AnimEnd();
        }
    }

    public void AnimEnd()
    {
        isAction = false;
        agent.stoppingDistance = initStoopingDis;
        agent.isStopped = false;
        isMelee1 = false;
        isMelee2 = false;
        timeCnt = 0;
        rand = 0;
        attackProbability = attackInitProbability;
    }

    public void AttackJudgmentActive()
    {
        _weaponController.SetColliderActive(true);
    }

    public void AttackJudgmentEnd()
    {
        _weaponController.SetColliderActive(false);
    }
}
