using UnityEngine;

public class Enemy_WolfBoss_Guard : StateMachineBehaviour
{
    Enemy_WolfBoss enemy;

    [SerializeField] float blockTime = 2.0f;
    float blockTimer = 0.0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy_WolfBoss>();
        enemy.SetLookPlayerAndEnemyStop(false, true);
        blockTimer = blockTime;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        blockTimer -= Time.deltaTime;
        if (blockTimer <= 0.0f)
        {
            enemy.IsGuard = false;
            enemy.wolf_Anim.SetBoolAnim(EnemyAnimatorController.AnimationBase.WolfBoss_Guard, false);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.SetLookPlayerAndEnemyStop(true, false);
    }
}
