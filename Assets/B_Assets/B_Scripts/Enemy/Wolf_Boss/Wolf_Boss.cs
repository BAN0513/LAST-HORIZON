using System.Collections;
using UnityEngine;

public class Wolf_Boss : Enemy_FourLegs
{
    public Wolf_BossAnimatorController wolf_Anim;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (wolf_Anim.CheckCurrentAnim("DownBefore") || wolf_Anim.CheckCurrentAnim("Down")) { return; }

        base.Update();

        if (isActionAnimation) { return; }
        Wolf_BossActionSO action = (Wolf_BossActionSO)CalcAction(enemySO.action);

        if (action != null)
        {
            SetLookPlayerAndEnemyStop(false, true);
            action.Execute(wolf_Anim);
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

