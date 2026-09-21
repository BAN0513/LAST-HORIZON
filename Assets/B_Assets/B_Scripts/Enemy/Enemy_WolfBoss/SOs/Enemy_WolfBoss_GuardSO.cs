using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_WolfBoss_GuardSO", menuName = "EnemyActionSO/Enemy_WolfBossActionSO/Enemy_WolfBoss_GuardSO")]
public class Enemy_WolfBoss_GuardSO : Enemy_WolfBossActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        if (enemy is Enemy_WolfBoss wolf)
        {
            if (wolf.IsGuard) { return Mathf.Infinity; }
        }

        return 0.0f;
    }

    public override void Execute(EnemyAnimatorController animator)
    {
        base.Execute(animator);
        animator.SetTriggerAnim(EnemyAnimatorController.AnimationBase.WolfBoss_Guard);
    }
}
