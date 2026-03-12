using UnityEngine;

public class Mid_Boss : Enemy
{
    [Header("武器のスクリプト")]
    [SerializeField] private WeaponController _weaponController;

    //攻撃時にこの値の距離まで近づく
    private float attackDis = 1.5f;

    [Header("攻撃の確率")]
    [SerializeField] private float attackInitProbability = 20;

    [Header("抽選で攻撃以外になった時に攻撃確率を上げるための値")]
    [SerializeField] private float attackUpProbability = 20;

    //攻撃確率の保存用変数
    private float attackProbability = 0;

    [Header("二段目の攻撃に派生する確率")]
    [Range(0, 100)][SerializeField] private float melee2Probability = 50;

    [Header("攻撃後、この値の分の秒数は攻撃の抽選は行わない")]
    [SerializeField] private float attackCoolDown = 3;

    //アニメーションで使うやつ
    public bool isMelee1 { get; private set; }
    public bool isMelee2 { get; private set; }
    public bool isBlock { get; private set; }

    protected override void Start()
    {
        base.Start();
        attackProbability = attackInitProbability;
    }

    protected override void Update()
    {
        if (isDeath || isHit) { return; }
        base.Update();
        CheckPlayerAttack();
        Debug.Log(dot);
    }

    private void CheckPlayerAttack()
    {
        if (isAction) 
        {
            isBlock = false;
            return;
        }
        if (playerAnimationController.IsAttackMove && distance <= 3 && dot > 0.5f)
        {

            isBlock = true;
            agent.isStopped = true;

        }
    }

    protected override void EngageMoveControl()
    {
        if (distance <= engageDis)
        {
            //敵のスピードを少しだけ遅くする
            agent.speed = engageMoveSpeed;

            if (!isAction && !isBlock)
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
                default:
                    //攻撃じゃなかったら攻撃の確率を上げる
                    if (isAction)
                    {
                        isAction = false;
                        attackProbability += attackUpProbability;
                    }
                    break;
            }
        }
        else
        {
            agent.speed = enemySO.moveSpeed;
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
            isMelee1 = true;
            isAtack = true;
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

        //一定確率で二段目の攻撃に派生する
        if (rand <= melee2Probability && distance <= 3 && dot > 0.5f)
        {
            isMelee2 = true;
        }
        else
        {
            InitAnim();
        }
    }

    //攻撃のアニメーションが終わったら全部初期化する
    protected override void InitAnim()
    {
        isAtack = false;
        agent.stoppingDistance = enemySO.stoopingDis;
        lotteryTime = attackCoolDown;
        attackProbability = attackInitProbability;
        rand = 0;
        isAction = false;
        agent.isStopped = false;
        isMelee1 = false;
        isMelee2 = false;
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

    public void BlockEnd()
    {
        agent.isStopped = false;
        isBlock = false;
    }
}
