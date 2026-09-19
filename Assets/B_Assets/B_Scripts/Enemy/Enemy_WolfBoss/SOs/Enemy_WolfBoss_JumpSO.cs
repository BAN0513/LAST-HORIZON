using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_WolfBoss_JumpSO", menuName = "EnemyActionSO/Enemy_WolfBossActionSO/Enemy_WolfBoss_JumpSO")]
public class Enemy_WolfBoss_JumpSO : Enemy_WolfBossActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        if (!enemy.IsAction || enemy.Distance < 30.0f) { return 0.0f; }
        return Mathf.Infinity;
    }

    public override void Execute(EnemyAnimatorController animator)
    {
        base.Execute(animator);
        animator.SetTriggerAnim(EnemyAnimatorController.AnimationBase.WolfBoss_Jump);
    }
}
