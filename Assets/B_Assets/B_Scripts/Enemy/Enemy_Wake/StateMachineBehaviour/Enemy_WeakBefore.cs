using UnityEngine;

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
        if (enemy.distance <= stopDis)
        {
            enemy.NotLoopAnimStart();
            enemy.enemy_WeakAnimator.SetBoolAnim(Enemy_WeakAnimatorController.Enemy_WeakAnimation.Melee1, false);
        }
    }
}
