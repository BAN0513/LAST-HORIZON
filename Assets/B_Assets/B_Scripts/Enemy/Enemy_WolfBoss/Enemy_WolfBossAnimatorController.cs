using System.Collections.Generic;
using UnityEngine;

public class Enemy_WolfBossAnimatorController : EnemyAnimatorController
{
    private Enemy_WolfBoss wolf_Boss;

    private Dictionary<AnimationBase, int> wolfBossAnims;

    protected override void Start()
    {
        base.Start();
        wolf_Boss = GetComponent<Enemy_WolfBoss>();

        wolfBossAnims = new Dictionary<AnimationBase, int>
        {
            {AnimationBase.WolfBoss_Tearing_FormOne,          Animator.StringToHash("isTearing_FormOne")          },
            {AnimationBase.WolfBoss_Tearing_FormTwo,          Animator.StringToHash("isTearing_FormTwo")          },
            {AnimationBase.WolfBoss_LeapAndSlash,             Animator.StringToHash("isLerpAndSlash")             },
            {AnimationBase.WolfBoss_RotationAttack,           Animator.StringToHash("isRotationAttack")           },
            {AnimationBase.WolfBoss_TailAttack,               Animator.StringToHash("isTailAttack")               },
            {AnimationBase.WolfBoss_DashAttackBefore_FormOne, Animator.StringToHash("isDashAttackBefore_FormOne") },
            {AnimationBase.WolfBoss_DashAttackBefore_FormTwo, Animator.StringToHash("isDashAttackBefore_FormTwo") },
            {AnimationBase.WolfBoss_DashAttack,               Animator.StringToHash("isDashAttack")               },
            {AnimationBase.WolfBoss_DownBefore,               Animator.StringToHash("isDownBefore")               },
            {AnimationBase.WolfBoss_BackStep,                 Animator.StringToHash("isBackStep")                 },
            {AnimationBase.WolfBoss_Contact,                  Animator.StringToHash("isContactRoar")              },
            {AnimationBase.WolfBoss_FormChange,               Animator.StringToHash("isFormChangeRoar")           },
            {AnimationBase.WolfBoss_Guard,                    Animator.StringToHash("isGuard")                    },
            {AnimationBase.WolfBoss_Jump,                     Animator.StringToHash("isJump")                     },
            {AnimationBase.WolfBoss_Laser,                    Animator.StringToHash("isLaser")                    }
        };
    }

    public override void SetBoolAnim(AnimationBase animation, bool isAnim)
    {
        if (wolfBossAnims.ContainsKey(animation))
        {
            animator.SetBool(wolfBossAnims[animation], isAnim);
        }
        else
        {
            base.SetBoolAnim(animation, isAnim);
        }
    }

    public override void SetTriggerAnim(AnimationBase animation)
    {
        if (wolfBossAnims.ContainsKey(animation))
        {
            animator.SetTrigger(wolfBossAnims[animation]);
        }
        else
        {
            base.SetTriggerAnim(animation);
        }
    }

    public override void ResetTriggerAnim(AnimationBase animation)
    {
        if (wolfBossAnims.ContainsKey(animation))
        {
            animator.ResetTrigger(wolfBossAnims[animation]);
        }
        else
        {
            base.ResetTriggerAnim(animation);
        }
    }

    public override void ResetAllAnim()
    {
        base.ResetAllAnim();
        foreach (var a in wolfBossAnims.Values)
        {
            animator.ResetTrigger(a);
            animator.SetBool(a, false);
        }
    }
}
