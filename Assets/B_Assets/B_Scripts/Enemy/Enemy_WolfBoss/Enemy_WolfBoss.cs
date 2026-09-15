using System.Collections;
using UnityEngine;

public class Enemy_WolfBoss : Enemy_FourLegs
{
    public Enemy_WolfBossAnimatorController wolf_Anim;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        //if (wolf_Anim.CheckCurrentAnim("DownBefore") || wolf_Anim.CheckCurrentAnim("Down")) { return; }

        //base.Update();

        //if (isActionAnimation) { return; }
        //Wolf_BossActionSO action = (Wolf_BossActionSO)CalcAction(enemySO.action);

        //if (action != null)
        //{
        //    SetLookPlayerAndEnemyStop(false, true);
        //    action.Execute(wolf_Anim);
        //}
    }

    //ここから下はAnimator関連の関数

    //攻撃のアニメーションが終わったら全部初期化する
    public override void Init()
    {
        Agent.enabled = true;
        base.Init();
    }

    public override void InitAnim()
    {
        base.InitAnim();
    }

    public override void InitAll()
    {
        base.InitAll();

        wolf_Anim.ResetTriggerAnim(EnemyAnimatorController.AnimationBase.WolfBoss_Tearing);
        wolf_Anim.ResetTriggerAnim(EnemyAnimatorController.AnimationBase.WolfBoss_LeapAndSlash);
        wolf_Anim.ResetTriggerAnim(EnemyAnimatorController.AnimationBase.WolfBoss_RotationAttack);
        wolf_Anim.ResetTriggerAnim(EnemyAnimatorController.AnimationBase.WolfBoss_TailAttack);
        wolf_Anim.ResetTriggerAnim(EnemyAnimatorController.AnimationBase.WolfBoss_BackStep);
        wolf_Anim.ResetTriggerAnim(EnemyAnimatorController.AnimationBase.WolfBoss_DownBefore);
        wolf_Anim.ResetTriggerAnim(EnemyAnimatorController.AnimationBase.WolfBoss_DashAttackBefore);
    }

    public void DashAttak()
    {
        wolf_Anim.SetBoolAnim(EnemyAnimatorController.AnimationBase.Dash, true);
        AttackJudgmentActive(BodyPart.AllBody);

        Agent.enabled = false;
    }
}

