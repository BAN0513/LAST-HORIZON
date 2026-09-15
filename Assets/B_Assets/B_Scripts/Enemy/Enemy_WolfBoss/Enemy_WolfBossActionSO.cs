using UnityEngine;

public class Enemy_WolfBossActionSO : EnemyActionSO
{
    public override float ScoreCalculation(float dis, float dot)
    {
        return base.ScoreCalculation(dis, dot);
    }

    //アニメーションの実行

    public virtual void Execute(Enemy_WolfBossAnimatorController wolf_BossAnim) { }  
}
