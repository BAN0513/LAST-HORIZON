using Unity.VisualScripting;
using UnityEngine;

public class Enemy_WeakBlock : StateMachineBehaviour
{
    Enemy_Weak enemy;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy_Weak>();
        enemy.SetLookPlayerAndEnemyStop(false, true);
        enemy.enemy_WeakAnimator.ResetTriggerAnim(Enemy_WeakAnimatorController.Enemy_WeakAnimation.BlockReaction);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemy.isAction)
        {
            enemy.enemy_WeakAnimator.SetBoolAnim(Enemy_WeakAnimatorController.Enemy_WeakAnimation.Block, false);
        }
        else if (enemy.isBlocking)
        {
            enemy.enemy_WeakAnimator.SetBoolAnim(Enemy_WeakAnimatorController.Enemy_WeakAnimation.Block, false);
            enemy.enemy_WeakAnimator.SetTriggerAnim(Enemy_WeakAnimatorController.Enemy_WeakAnimation.BlockReaction);
            enemy.isBlocking = false;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.SetLookPlayerAndEnemyStop(true, false);
    }
}
