using System.Collections.Generic;
using UnityEngine;

public class Enemy_1AnimatorController : EnemyAnimatorController
{
    private Enemy_1 enemy_1;

    public enum Enemy_1Animation
    {
        Melee1,
        Melee2
    }

    private Dictionary<Enemy_1Animation, int> enemy_1Anims;

    protected override void Start()
    {
        base.Start();
        enemy_1 = GetComponent<Enemy_1>();

        enemy_1Anims = new Dictionary<Enemy_1Animation, int>
        {
            {Enemy_1Animation.Melee1, Animator.StringToHash("isMelee1") },
            {Enemy_1Animation.Melee2, Animator.StringToHash("isMelee2") }
        };
    }

    public override void SetBoolAnim(AnimationBase animation, bool isAnim)
    {
        base.SetBoolAnim(animation, isAnim);
    }

    public void SetBoolAnim(Enemy_1Animation animation, bool isAnim)
    {
        animator.SetBool(enemy_1Anims[animation], isAnim);
    }

    public override void SetTriggerAnim(AnimationBase animation)
    {
        base.SetTriggerAnim(animation);
    }

    public void SetTriggerAnim(Enemy_1Animation animation)
    {
        animator.SetTrigger(enemy_1Anims[animation]);
    }

    public void ResetTriggerAnim(Enemy_1Animation animation)
    {
        animator.ResetTrigger(enemy_1Anims[animation]);
    }

    public override bool CheckCurrentAnim(string name)
    {
        return base.CheckCurrentAnim(name);
    }
}
