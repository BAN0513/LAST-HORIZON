using System.Collections.Generic;
using UnityEngine;

public class Mid_BossAnimatorController : EnemyAnimatorController
{
    private Mid_Boss mid_Boss;

    public enum Mid_BossAnimation
    {
        Melee1,
        Melee2,
        Block,
        RotationAttack
    }

    private Dictionary<Mid_BossAnimation, int> mid_BossAnims;

    protected override void Start()
    {
        base.Start();

        mid_Boss = GetComponent<Mid_Boss>();

        mid_BossAnims = new Dictionary<Mid_BossAnimation, int>
        {
            {Mid_BossAnimation.Melee1, Animator.StringToHash("isMelee1") },
            {Mid_BossAnimation.Melee2, Animator.StringToHash("isMelee2") },
            {Mid_BossAnimation.Block, Animator.StringToHash("isBlock") },
            {Mid_BossAnimation.RotationAttack, Animator.StringToHash("is360Attack") }
        };
    }

    public override void SetBoolAnim(AnimationBase animation, bool isAnim)
    {
        base.SetBoolAnim(animation, isAnim);
    }

    public void SetBoolAnim(Mid_BossAnimation animation, bool isAnim)
    {
        animator.SetBool(mid_BossAnims[animation], isAnim);
    }

    public override void SetTriggerAnim(AnimationBase animation)
    {
        base.SetTriggerAnim(animation);
    }

    public void SetTriggerAnim(Mid_BossAnimation animation)
    {
        animator.SetTrigger(mid_BossAnims[animation]);
    }

    public override void ResetTriggerAnim(AnimationBase animation)
    {
        base.ResetTriggerAnim(animation);
    }

    public void ResetTriggerAnim(Mid_BossAnimation animation)
    {
        animator.ResetTrigger(mid_BossAnims[animation]);
    }

    public override bool CheckCurrentAnim(string name)
    {
        return base.CheckCurrentAnim(name);
    }
}
