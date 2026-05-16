using System.Collections.Generic;
using UnityEngine;

public class Wolf_BossAnimatorController : EnemyAnimatorController
{
    private Wolf_Boss wolf_Boss;

    public enum WolfAnimation
    {
        Attack_1,
        DashAttackBefore,
        RotationAttack,
        TailAttack,
        DownBefore
    }
    private Dictionary<WolfAnimation, int> wolfAnims;

    protected override void Start()
    {
        base.Start();

        wolf_Boss = GetComponent<Wolf_Boss>();

        wolfAnims = new System.Collections.Generic.Dictionary<WolfAnimation, int>
        {
            {WolfAnimation.Attack_1,  Animator.StringToHash("isAttack_1") },
            {WolfAnimation.DashAttackBefore, Animator.StringToHash("isDashAttackBefore") },
            {WolfAnimation.RotationAttack, Animator.StringToHash("isRotationAttack") },
            {WolfAnimation.TailAttack, Animator.StringToHash("isTailAttack") },
            {WolfAnimation.DownBefore, Animator.StringToHash("isDownBefore") }
        };
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void SetBoolAnim(AnimationBase animation, bool isAnim)
    {
        base.SetBoolAnim(animation, isAnim);
    }
    public void SetBoolAnim(WolfAnimation animation, bool isAnim)
    {
        animator.SetBool(wolfAnims[animation], isAnim);
    }

    public override void SetTriggerAnim(AnimationBase animation)
    {
        base.SetTriggerAnim(animation);
    }

    public void SetTriggerAnim(WolfAnimation animation)
    {
        animator.SetTrigger(wolfAnims[animation]);
    }

    public override bool CheckCurrentAnim(string name)
    {
        return base.CheckCurrentAnim(name);
    }
}
