using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_WolfBoss_DashAttackSO", menuName = "EnemyActionSO/Enemy_WolfBossActionSO/Enemy_WolfBoss_DashAttackSO")]
public class Enemy_WolfBoss_DashAttackSO : Enemy_WolfBossActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        if (!enemy.IsAction) { return 0.0f; }
        return base.ScoreCalculation(dis, dot, enemy);
    }

    public override void Execute(EnemyAnimatorController animator)
    {
        base.Execute(animator);
        animator.Enemy.SetLookPlayerAndEnemyStop(false, false);
        animator.SetTriggerAnim(EnemyAnimatorController.AnimationBase.WolfBoss_DashAttackBefore);
    }
}
