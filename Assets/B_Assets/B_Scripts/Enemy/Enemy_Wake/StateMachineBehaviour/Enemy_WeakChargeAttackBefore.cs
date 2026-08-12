using UnityEngine;

public class Enemy_WeakChargeAttackBefore : StateMachineBehaviour
{
    Enemy_Weak enemy;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy_Weak>();
        enemy.SetLookPlayerAndEnemyStop(true, true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.SetLookPlayerAndEnemyStop(false, true);
    }
}
