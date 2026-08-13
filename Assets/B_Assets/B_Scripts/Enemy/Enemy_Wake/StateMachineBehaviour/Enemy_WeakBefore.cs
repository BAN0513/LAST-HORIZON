using UnityEngine;
using UnityEngine.Animations;

public class Enemy_WeakBefore : StateMachineBehaviour
{
    Enemy_Weak enemy;

    [SerializeField] private float stopDis = 1.0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy_Weak>();
        enemy.Agent.isStopped = false;
        enemy.Agent.stoppingDistance = stopDis;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.Agent.SetDestination(enemy.Target.transform.position);

        if (enemy.Distance <= stopDis)
        {
            enemy.SetLookPlayerAndEnemyStop(false, true);
            enemy.enemy_WeakAnimator.SetBoolAnim(Enemy_WeakAnimatorController.Enemy_WeakAnimation.Melee, false);
        }
    }
}
