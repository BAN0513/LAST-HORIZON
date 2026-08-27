using System.Collections.Generic;
using UnityEngine;

public class Enemy_WeakAnimatorController : EnemyAnimatorController
{
    private Enemy_Weak enemy_Weak;

    private Dictionary<AnimationBase, int> weakAnims;

    protected override void Start()
    {
        base.Start();
        enemy_Weak = GetComponent<Enemy_Weak>();

        weakAnims = new Dictionary<AnimationBase, int>()
        {
            {AnimationBase.Weak_Melee,         Animator.StringToHash("isMeleeBefore")   },
            {AnimationBase.Weak_ChargeAttack,  Animator.StringToHash("isChargeAttack")  },
            {AnimationBase.Weak_Block,         Animator.StringToHash("isBlock")         },
            {AnimationBase.Weak_BlockReaction, Animator.StringToHash("isBlockReaction") },
            {AnimationBase.Weak_Contact,       Animator.StringToHash("isContact")       },
            {AnimationBase.Weak_Down,          Animator.StringToHash("isDown")          }
        };   
    }

    public override void SetBoolAnim(AnimationBase animation, bool isAnim)
    {
        if (weakAnims.ContainsKey(animation))
        {
            animator.SetBool(weakAnims[animation], isAnim);
        }
        else
        {
            base.SetBoolAnim(animation, isAnim);
        }
    }

    public override void SetTriggerAnim(AnimationBase animation)
    {
        if (weakAnims.ContainsKey(animation))
        {
            animator.SetTrigger(weakAnims[animation]);
        }
        else
        {
            base.SetTriggerAnim(animation);
        }
    }

    public override void ResetTriggerAnim(AnimationBase animation)
    {
        if (weakAnims.ContainsKey(animation))
        {
            animator.ResetTrigger(weakAnims[animation]);
        }
        else
        {
            base.ResetTriggerAnim(animation);
        }
    }
}
