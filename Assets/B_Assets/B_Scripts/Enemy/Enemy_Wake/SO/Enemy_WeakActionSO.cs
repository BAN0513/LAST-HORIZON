using UnityEngine;

public class Enemy_WeakActionSO : EnemyActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        return base.ScoreCalculation(dis, dot, enemy);
    }

    public virtual void Execute(Enemy_WeakAnimatorController animator)
    {
        animator.enemy.IsActionAnimation = true;
    }

    public virtual void ActionEnd(Enemy_WeakAnimatorController animator) { }
}
