using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_WolfBoss_DashAttack_FormOneSO", menuName = "EnemyActionSO/Enemy_WolfBossActionSO/Enemy_WolfBoss_DashAttack_FormOneSO")]
public class Enemy_WolfBoss_DashAttack_FormOneSO : Enemy_WolfBossActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        if (!enemy.IsAction) { return 0.0f; }
        return base.ScoreCalculation(dis, dot, enemy);
    }

    public override void Execute(EnemyAnimatorController animator)
    {
        base.Execute(animator);
        animator.SetTriggerAnim(EnemyAnimatorController.AnimationBase.WolfBoss_DashAttackBefore_FormOne);
    }
}
