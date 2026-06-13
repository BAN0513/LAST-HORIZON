using System.Collections.Generic;
using UnityEngine;

public class Enemy_WizardAnimatorController : EnemyAnimatorController
{
    private Enemy_Wizard enemy_Wizard;

    public enum Enemy_1Animation
    {
        Fire,
        Area,
        Impact,
    }

    private Dictionary<Enemy_1Animation, int> enemy_WizardAnims;

    protected override void Start()
    {
        base.Start();
        enemy_Wizard = GetComponent<Enemy_Wizard>();

        enemy_WizardAnims = new Dictionary<Enemy_1Animation, int>
        {
            {Enemy_1Animation.Fire, Animator.StringToHash("isFire") },
            {Enemy_1Animation.Area, Animator.StringToHash("isArea") },
            {Enemy_1Animation.Impact, Animator.StringToHash("isImpact") },
        };
    }

    public void SetBoolAnim(Enemy_1Animation animation, bool isAnim)
    {
        animator.SetBool(enemy_WizardAnims[animation], isAnim);
    }

    public void SetTriggerAnim(Enemy_1Animation animation)
    {
        animator.SetTrigger(enemy_WizardAnims[animation]);
    }

    public void ResetTriggerAnim(Enemy_1Animation animation)
    {
        animator.ResetTrigger(enemy_WizardAnims[animation]);
    }

    public override bool CheckCurrentAnim(string name)
    {
        return base.CheckCurrentAnim(name);
    }
}
