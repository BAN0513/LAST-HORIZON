using System.Collections;
using UnityEngine;

public class Enemy_1 : Enemy_Humanoid
{
    [Header("二段目の攻撃に派生する確率")]
    [Range(0,100)] [SerializeField] private float melee2Probability = 50;

    //アニメーションで使うやつ
    public bool isMelee1 { get; private set; }
    public bool isMelee2 { get; private set; }

    protected override void Start()
    {
        base.Start();
        attackProbability = enemySO.attackInitProbability;
    }

    protected override void Update()
    {
        if (isDeath || isHit) { return; }
        base.Update();
    }

    protected override void ShortDistanceAction()
    {
        AttackMove();
    }

    protected override void MediumDistanceAction()
    {
        AttackMove();
    }

    protected override void LongDistanceAction()
    {
        AttackMove();
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
            isLookPlayer = false;
            agent.isStopped = true;
            isMelee1 = true;
        }
    }

    //ここから下はAnimator関連の関数
    public void Melee2()
    {
        int rand = Random.Range(1, 101);

        //一定確率で二段目の攻撃に派生する
        if (rand <= melee2Probability && distance <= 3 && dot > 0.3f)
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
        base.InitAnim();
        isMelee1 = false;
        isMelee2 = false;
    }
}
