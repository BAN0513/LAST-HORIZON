using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "Enemy_WolfBoss_TearingSO", menuName = "EnemyActionSO/Enemy_WolfBossActionSO/Enemy_WolfBoss_TearingSO")]
public class Enemy_WolfBoss_TearingSO : Enemy_WolfBossActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        if (!enemy.IsAction) { return 0.0f; }
        return base.ScoreCalculation(dis, dot);
    }

    public override void Execute(EnemyAnimatorController animator)
    {
        base.Execute(animator);
        animator.SetTriggerAnim(EnemyAnimatorController.AnimationBase.WolfBoss_Tearing);
    }
}
