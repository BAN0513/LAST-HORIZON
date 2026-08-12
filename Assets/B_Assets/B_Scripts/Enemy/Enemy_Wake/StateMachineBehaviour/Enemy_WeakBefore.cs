using UnityEngine;
using UnityEngine.Animations;

public class Enemy_WeakBefore : StateMachineBehaviour
{
    Enemy_Weak enemy;

    [SerializeField] private float stopDis = 1.0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy_Weak>();
        enemy.agent.isStopped = false;
        enemy.agent.stoppingDistance = stopDis;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.agent.SetDestination(enemy.target.transform.position);

        if (enemy.distance <= stopDis)
        {
            enemy.SetLookPlayerAndEnemyStop(false, true);
            enemy.enemy_WeakAnimator.SetBoolAnim(Enemy_WeakAnimatorController.Enemy_WeakAnimation.Melee, false);
        }
    }
}
