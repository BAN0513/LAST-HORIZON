using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_WolfBoss_BackStepSO", menuName = "EnemyActionSO/Enemy_WolfBossActionSO/Enemy_WolfBoss_BackStepSO")]
public class Enemy_WolfBoss_BackStepSO : Enemy_WolfBossActionSO
{
    public override float ScoreCalculation(float dis, float dot)
    {
        return base.ScoreCalculation(dis, dot);
    }

    public override void Execute(EnemyAnimatorController animator)
    {
        base.Execute(animator);
        animator.SetTriggerAnim(EnemyAnimatorController.AnimationBase.WolfBoss_BackStep);
    }
}
