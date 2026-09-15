using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_WolfBoss_DashAttackSO", menuName = "EnemyActionSO/Wolf_BossActionSO/Enemy_WolfBoss_DashAttackSO")]
public class Enemy_WolfBoss_DashAttackSO : Enemy_WolfBossActionSO
{
    public override float ScoreCalculation(float dis, float dot)
    {
        return base.ScoreCalculation(dis, dot);
    }

    public override void Execute(Enemy_WolfBossAnimatorController wolf_Anim)
    {
        wolf_Anim.SetTriggerAnim(EnemyAnimatorController.AnimationBase.WolfBoss_DashAttackBefore);
    }
}
