using System.Collections;
using UnityEngine;

public class Wolf_Boss : Enemy_FourLegs
{
    public Wolf_BossAnimatorController wolf_Anim;

    private Wolf_BossActionSO lastAction;

    protected override void Start()
    {
        base.Start();
        attackProbability = enemySO.attackInitProbability;
    }

    protected override void Update()
    {        
        if (wolf_Anim.CheckCurrentAnim("DownBefore") || wolf_Anim.CheckCurrentAnim("Down")) { return; }

        base.Update();
    }

    protected override void AttackAction()
    {
        float curScore = 0;
        Wolf_BossActionSO firtstActionSO = null;
        Wolf_BossActionSO secondActionSO = null;
        foreach (var a in enemySO.action)
        {
            float lastScore = a.ScoreCalculation(distance, dot);

            if (curScore < lastScore)
            {             
                curScore = lastScore;
                secondActionSO = firtstActionSO;
                firtstActionSO = (Wolf_BossActionSO)a;
            }
        }
        if (firtstActionSO != null)
        {
            LookPlayerChange(false);
            if (lastAction == firtstActionSO)
            {
                if (secondActionSO != null)
                {
                    secondActionSO.Execute(wolf_Anim);
                    lastAction = secondActionSO;
                }
                else
                {
                    lastAction = null;
                    Init();
                }
            }
            else
            {
                firtstActionSO.Execute(wolf_Anim);
                lastAction = firtstActionSO;
            }
        }


    }

    protected override void DoNotAttack()
    {
        LookPlayerChange(false);

        float curScore = 0;
        Wolf_BossActionSO firstActionSO = null;

        foreach(var a in enemySO.doNotAttack_Action)
        {
            float lastScore = a.ScoreCalculation(distance, dot);

            if (curScore < lastScore)
            {
                curScore = lastScore;
                firstActionSO = (Wolf_BossActionSO)a;
            }
        }
        
        if (firstActionSO != null)
        {
            firstActionSO.Execute(wolf_Anim);
        }
        else
        {
            Init();
        }
        base.DoNotAttack();
    }

    //ここから下はAnimator関連の関数

    //攻撃のアニメーションが終わったら全部初期化する
    public override void Init()
    {
        agent.enabled = true;
        base.Init();
    }

    public override void InitAnim()
    {
        base.InitAnim();
    }

    public override void InitAll()
    {
        base.InitAll();
    }

    public void DashAttak()
    {
        wolf_Anim.SetBoolAnim(EnemyAnimatorController.AnimationBase.Dash, true);
        AttackJudgmentActive(BodyPart.AllBody);

        agent.enabled = false;
    }
}

