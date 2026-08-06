using UnityEngine;

public class Enemy_1ActionSO : EnemyActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        return base.ScoreCalculation(dis, dot);
    }

    public virtual void Execute(Enemy_1AnimatorController enemy_1Anim) { }
}
