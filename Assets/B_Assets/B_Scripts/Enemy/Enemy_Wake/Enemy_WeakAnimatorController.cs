using System.Collections.Generic;
using UnityEngine;

public class Enemy_WeakAnimatorController : EnemyAnimatorController
{
    private Enemy_Weak enemy_Weak;

    public enum Enemy_WeakAnimation
    {
        Melee,
        ChargeAttack,
        Block,
        BlockReaction
    }

    private Dictionary<Enemy_WeakAnimation, int> enemy_WeakAnims;

    protected override void Start()
    {
        base.Start();
        enemy_Weak = GetComponent<Enemy_Weak>();

        enemy_WeakAnims = new Dictionary<Enemy_WeakAnimation, int>
        {
            {Enemy_WeakAnimation.Melee, Animator.StringToHash("isMeleeBefore") },
            {Enemy_WeakAnimation.ChargeAttack, Animator.StringToHash("isChargeAttack") },
            {Enemy_WeakAnimation.Block, Animator.StringToHash("isBlock") },
            {Enemy_WeakAnimation.BlockReaction, Animator.StringToHash("isBlockReaction") }
        };
    }

    public void SetBoolAnim(Enemy_WeakAnimation animation, bool isAnim)
    {
        animator.SetBool(enemy_WeakAnims[animation], isAnim);
    }

    public void SetTriggerAnim(Enemy_WeakAnimation animation)
    {
        animator.SetTrigger(enemy_WeakAnims[animation]);
    }

    public void ResetTriggerAnim(Enemy_WeakAnimation animation)
    {
        animator.ResetTrigger(enemy_WeakAnims[animation]);
    }

    public override bool CheckCurrentAnim(string name)
    {
        return base.CheckCurrentAnim(name);
    }
}
