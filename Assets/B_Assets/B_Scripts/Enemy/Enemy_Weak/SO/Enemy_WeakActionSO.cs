using UnityEngine;

public class Enemy_WeakActionSO : EnemyActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        return base.ScoreCalculation(dis, dot, enemy);
    }

    public override void Execute(EnemyAnimatorController animator)
    {
        base.Execute(animator);
    }

    public override void ActionEnd(EnemyAnimatorController animator)
    {
        base.ActionEnd(animator);
    }
}
