using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_WolfBoss_TeleportSO", menuName = "EnemyActionSO/Enemy_WolfBossActionSO/Enemy_WolfBoss_TeleportSO")]
public class Enemy_WolfBoss_TeleportSO : Enemy_WolfBossActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        if (!enemy.IsAction) { return 0.0f; }
        return base.ScoreCalculation(dis, dot, enemy);
    }

    public override void Execute(EnemyAnimatorController animator)
    {
        base.Execute(animator);
        animator.SetBoolAnim(EnemyAnimatorController.AnimationBase.WolfBoss_Teleport, true);
    }
}
