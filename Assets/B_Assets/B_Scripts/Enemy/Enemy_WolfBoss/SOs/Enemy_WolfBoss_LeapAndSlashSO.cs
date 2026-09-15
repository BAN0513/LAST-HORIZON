using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_WolfBoss_LeapAndSlashSO", menuName = "EnemyActionSO/Wolf_BossActionSO/Enemy_WolfBoss_LeapAndSlashSO")]
public class Enemy_WolfBoss_LeapAndSlashSO : Enemy_WolfBossActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        if (!enemy.IsAction) { return 0.0f; }
        return base.ScoreCalculation(dis, dot);
    }

    public override void Execute(Enemy_WolfBossAnimatorController wolf_Anim)
    {
        wolf_Anim.SetTriggerAnim(EnemyAnimatorController.AnimationBase.WolfBoss_LeapAndSlash);
    }
}
