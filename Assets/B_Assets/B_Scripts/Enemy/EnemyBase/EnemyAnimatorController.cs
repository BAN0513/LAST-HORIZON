using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    public Enemy enemy { get; private set; }
    protected Animator animator;

    public enum AnimationBase
    {
        Walk,
        Dash,
        Death,
        Hit,

        Weak_Melee,
        Weak_ChargeAttack,
        Weak_Block,
        Weak_BlockReaction,
        Weak_Contact,
        Weak_Down,

        Wizard_Mera,
        Wizard_MeraZoma,
        Wizard_MeraStorm,
        Wizard_Shield,
        Wizard_SummonEnemyWeak
    }

    protected Dictionary<AnimationBase, int> anims;

    protected virtual void Start()
    {
        enemy = GetComponent<Enemy>();
        animator = GetComponent<Animator>();

        anims = new Dictionary<AnimationBase, int>()
        {
            { AnimationBase.Walk, Animator.StringToHash("isWalking") },
            { AnimationBase.Dash, Animator.StringToHash("isDash") },
            { AnimationBase.Death, Animator.StringToHash("isDeath") },
            { AnimationBase.Hit,  Animator.StringToHash("isHit") },
        };
    }

    public virtual void SetBoolAnim(AnimationBase animation, bool isAnim)
    {
        animator.SetBool(anims[animation], isAnim);
    }

    public virtual void SetTriggerAnim(AnimationBase animation)
    {
        animator.SetTrigger(anims[animation]);
    }

    public virtual void ResetTriggerAnim(AnimationBase animation)
    {
        animator.ResetTrigger(anims[animation]);
    }

    public virtual bool CheckCurrentAnim(string name)
    {
        if (animator == null) { return false; }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(name))
        {
            return true;
        }
        return false;
    }

    public void ForcedQuitAnimation()
    {
        animator.Play("Idle", 0, 0.0f);
    }
}
