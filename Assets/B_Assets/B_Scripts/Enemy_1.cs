using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_1 : Enemy
{
    [Header("武器のスクリプト")]
    [SerializeField] private WeaponController _weaponController;

    private Coroutine backMoveCor = null;
    private int rand;
    private float lotteryTime;

    [Header("プレイヤーとの距離が値以下になると下がる行動をする")]
    [SerializeField] private float backActionDis = 2;

    [Header("下がる行動の時に下がる距離")]
    [SerializeField] private float backMoveDis = 3;

    [Header("接敵距離（この値以下になると攻撃の抽選を開始する）")]
    [SerializeField] private float engageDis = 5;

    [Header("接敵状態時の動くスピード")]
    [SerializeField] private float engageMoveSpeed = 1;

    //攻撃時にこの値の距離まで近づく
    private float attackDis = 2f;

    [Header("攻撃の確率")]
    [SerializeField] private float attackInitProbability = 20;

    [Header("抽選で攻撃以外になった時に攻撃確率を上げるための値")]
    [SerializeField] private float attackUpProbability = 20;

    //攻撃確率の保存用変数
    private float attackProbability = 0;

    [Header("二段目の攻撃に派生する確率")]
    [Range(0,100)] [SerializeField] private float melee2Probability = 50;

    [Header("攻撃後、この値の分の秒数は攻撃の抽選は行わない")]
    [SerializeField] private float attackCoolDown = 3;

    //アニメーションで使うやつ
    public bool isMelee1 { get; private set; }
    public bool isMelee2 { get; private set; }
    public bool isBackMove { get; private set; }
    public bool isDash { get; private set; }

    protected override void Start()
    {
        base.Start();
        attackProbability = attackInitProbability;
    }

    protected override void Update()
    {
        if (isDeath) { return; }
        base.Update();

        //移動アニメーションの変更処理
        MoveAnimControl();

        //後ろに下がる行動の処理
        BackMoveControl();

        //交戦時の処理
        EngageMoveControl();
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

    private void EngageMoveControl()
    {
        if (distance <= engageDis)
        {
            //敵のスピードを少しだけ遅くする
            agent.speed = engageMoveSpeed;

            if (!isAction)
            {
                lotteryTime -= Time.deltaTime;
                if (lotteryTime <= 0)
                {
                    //行動の抽選で使う
                    rand = Random.Range(1, 101);

                    //次の抽選に必要な時間をランダムで決める
                    lotteryTime = Random.Range(0.5f, 2.1f);
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
                        rand = 0;
                    }
                    break;
            }
        }
        else
        {
            agent.speed = enemySO.moveSpeed;
            isAction = false;
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
        }
    }

    //ここから下はAnimator関連の関数
    public void Melee2()
    {
        int rand = Random.Range(1, 101);

        //一定確率で二段目の攻撃に派生する
        if (rand <= melee2Probability)
        {
            isMelee2 = true;
        }
        else
        {
            AnimEnd();
        }
    }

    //攻撃のアニメーションが終わったら全部初期化する
    public void AnimEnd()
    {
        isAction = false;
        agent.stoppingDistance = enemySO.stoopingDis;
        agent.isStopped = false;
        isMelee1 = false;
        isMelee2 = false;
        rand = 0;
        lotteryTime = attackCoolDown;
        attackProbability = attackInitProbability;
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
}
