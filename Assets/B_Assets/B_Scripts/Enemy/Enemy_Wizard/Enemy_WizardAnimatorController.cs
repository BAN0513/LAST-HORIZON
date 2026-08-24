using System.Collections.Generic;
using UnityEngine;

public class Enemy_WizardAnimatorController : EnemyAnimatorController
{
    private Enemy_Wizard enemy_Wizard;

    private Dictionary<AnimationBase, int> wizardAnims;

    protected override void Start()
    {
        base.Start();
        enemy_Wizard = GetComponent<Enemy_Wizard>();

        wizardAnims = new Dictionary<AnimationBase, int>
        {
            {AnimationBase.Wizard_Mera,      Animator.StringToHash("isMera")      },
            {AnimationBase.Wizard_MeraZoma,  Animator.StringToHash("isMeraZoma")  },
            {AnimationBase.Wizard_MeraStorm, Animator.StringToHash("isMeraStorm") },
            {AnimationBase.Wizard_Shield,    Animator.StringToHash("isShield")    },
            {AnimationBase.Wizard_SummonEnemyWeak, Animator.StringToHash("isSummonEnemyWeak") },
            {AnimationBase.Wizard_Teleportation, Animator.StringToHash("isTeleport") }
        };
    }

    public override void SetBoolAnim(AnimationBase animation, bool isAnim)
    {
        if (wizardAnims.ContainsKey(animation))
        {
            animator.SetBool(wizardAnims[animation], isAnim);
        }
        else
        {
            base.SetBoolAnim(animation, isAnim);
        }
    }

    public override void SetTriggerAnim(AnimationBase animation)
    {
        if (wizardAnims.ContainsKey(animation))
        {
            animator.SetTrigger(wizardAnims[animation]);
        }
        else
        {
            base.SetTriggerAnim(animation);
        }
    }

    public override void ResetTriggerAnim(AnimationBase animation)
    {
        if (wizardAnims.ContainsKey(animation))
        {
            animator.ResetTrigger(wizardAnims[animation]);
        }
        else
        {
            base.ResetTriggerAnim(animation);
        }
    }
}
