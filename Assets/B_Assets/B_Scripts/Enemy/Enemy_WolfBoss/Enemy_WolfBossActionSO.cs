using UnityEngine;

public class Enemy_WolfBossActionSO : EnemyActionSO
{
    public override float ScoreCalculation(float dis, float dot)
    {
        return base.ScoreCalculation(dis, dot);
    }

    //アニメーションの実行
    public override void Execute(EnemyAnimatorController animator)
    {
        base.Execute(animator);
    }

    public override void ActionEnd(EnemyAnimatorController animator)
    {
        base.ActionEnd(animator);
    }
}
