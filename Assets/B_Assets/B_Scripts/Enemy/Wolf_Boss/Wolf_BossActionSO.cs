using UnityEngine;

public class Wolf_BossActionSO : EnemyActionSO
{
    public override float ScoreCalculation(float dis, float dot)
    {
        return base.ScoreCalculation(dis, dot);
    }

    //アニメーションの実行

    public virtual void Execute(Wolf_BossAnimatorController wolf_BossAnim) { }  
}
