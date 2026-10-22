using UnityEngine;
using UnityEngine.Animations;

public class Enemy_WeakBefore : StateMachineBehaviour
{
    Enemy_Weak enemy;

    [Header("どの距離で止まるか")]
    [SerializeField] private float stopDis = 1.0f;

    [Header("敵が振り向くかどうか")]
    [SerializeField] private bool isLookPlayer = false;

    [Header("どのアニメーションの前か")]
    [SerializeField] private EnemyAnimatorController.AnimationBase animation;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy_Weak>();
        enemy.Agent.stoppingDistance = stopDis;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.Agent.SetDestination(enemy.Target.transform.position);

        if (enemy.Distance <= stopDis)
        {
            enemy.SetLookPlayerAndEnemyStop(isLookPlayer, true);
            enemy.Enemy_WeakAnimator.SetBoolAnim(animation, false);
        }
    }
}
