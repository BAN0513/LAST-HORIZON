using Unity.VisualScripting;
using UnityEngine;

public class Enemy_WeakBlock : StateMachineBehaviour
{
    Enemy_Weak enemy;

    [SerializeField] float blockTime = 2.0f;
    float blockTimer = 0.0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy_Weak>();
        enemy.SetLookPlayerAndEnemyStop(false, true);
        enemy.enemy_WeakAnimator.ResetTriggerAnim(EnemyAnimatorController.AnimationBase.Weak_BlockReaction);
        blockTimer = blockTime;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        blockTimer -= Time.deltaTime;
        if (blockTimer <= 0.0f && !enemy.IsBlockingReaction)
        {
            enemy.IsBlocking = false;
            enemy.enemy_WeakAnimator.SetBoolAnim(EnemyAnimatorController.AnimationBase.Weak_Block, false);
        }

        if (enemy.IsBlockingReaction)
        {
            enemy.enemy_WeakAnimator.SetTriggerAnim(EnemyAnimatorController.AnimationBase.Weak_BlockReaction);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.SetLookPlayerAndEnemyStop(true, false);
    }
}
