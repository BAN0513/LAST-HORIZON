using UnityEngine;

public class Enemy_WizardActionSO : EnemyActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        return base.ScoreCalculation(dis, dot);
    }

    public virtual void Execute(Enemy_WizardAnimatorController enemy_WizardAnim) { }
}
