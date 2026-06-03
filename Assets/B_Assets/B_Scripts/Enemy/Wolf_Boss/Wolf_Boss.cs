using System.Collections;
using UnityEngine;

public class Wolf_Boss : Enemy_FourLegs
{
    public Wolf_BossAnimatorController wolf_Anim;

    private Wolf_BossActionSO lastAction;

    public GameObject neck;

    protected override void Start()
    {
        base.Start();
        attackProbability = enemySO.attackInitProbability;
    }

    protected override void Update()
    {
        Debug.Log(attackProbability);
        if (wolf_Anim.CheckCurrentAnim("DownBefore") || wolf_Anim.CheckCurrentAnim("Down")) { return; }

        base.Update();
    }

    protected override void AttackAction()
    {
        float curScore = 0;
        Wolf_BossActionSO firstActionSO = null;
        Wolf_BossActionSO secondActionSO = null;
        foreach (var a in enemySO.action)
        {
            float lastScore = a.ScoreCalculation(distance, dot);

            if (curScore < lastScore)
            {             
                curScore = lastScore;
                secondActionSO = firstActionSO;
                firstActionSO = (Wolf_BossActionSO)a;
            }
        }
        if (firstActionSO != null)
        {
            LookPlayerChange(false);
            if (lastAction == firstActionSO)
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
                firstActionSO.Execute(wolf_Anim);
                lastAction = firstActionSO;
            }
        }
        else
        {
            DoNotAttack();
        }
    }

    protected override void DoNotAttack()
    {
        LookPlayerChange(false);

        float curScore = 0;
        Wolf_BossActionSO firstActionSO = null;
        Wolf_BossActionSO secondActionSO = null;

        foreach (var a in enemySO.doNotAttack_Action)
        {
            float lastScore = a.ScoreCalculation(distance, dot);

            if (curScore < lastScore)
            {
                curScore = lastScore;
                secondActionSO = firstActionSO;
                firstActionSO = (Wolf_BossActionSO)a;
            }
        }

        if (firstActionSO != null)
        {
            if (lastAction == firstActionSO)
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
                firstActionSO.Execute(wolf_Anim);
                lastAction = firstActionSO;
            }
        }
        else
        {
            Init();
        }
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

        wolf_Anim.ResetTriggerAnim(Wolf_BossAnimatorController.WolfAnimation.Attack_1);
        wolf_Anim.ResetTriggerAnim(Wolf_BossAnimatorController.WolfAnimation.Attack_2);
        wolf_Anim.ResetTriggerAnim(Wolf_BossAnimatorController.WolfAnimation.DashAttackBefore);
        wolf_Anim.ResetTriggerAnim(Wolf_BossAnimatorController.WolfAnimation.TailAttack);
        wolf_Anim.ResetTriggerAnim(Wolf_BossAnimatorController.WolfAnimation.RotationAttack);
        wolf_Anim.ResetTriggerAnim(Wolf_BossAnimatorController.WolfAnimation.DownBefore);
        wolf_Anim.ResetTriggerAnim(Wolf_BossAnimatorController.WolfAnimation.Reflection);
        wolf_Anim.ResetTriggerAnim(Wolf_BossAnimatorController.WolfAnimation.ShortReflection);
        wolf_Anim.ResetTriggerAnim(Wolf_BossAnimatorController.WolfAnimation.Retreat);
        wolf_Anim.ResetTriggerAnim(Wolf_BossAnimatorController.WolfAnimation.ForwardStep);
    }

    public void DashAttak()
    {
        wolf_Anim.SetBoolAnim(EnemyAnimatorController.AnimationBase.Dash, true);
        AttackJudgmentActive(BodyPart.AllBody);

        agent.enabled = false;
    }
}

