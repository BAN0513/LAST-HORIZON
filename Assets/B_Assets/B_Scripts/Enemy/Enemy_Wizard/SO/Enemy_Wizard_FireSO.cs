using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_Wizard_FireSO", menuName = "EnemyActionSO/Enemy_WizardActionSO/Enemy_Wizard_FireSO")]
public class Enemy_Wizard_FireSO : Enemy_WizardActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        if (!enemy.IsAction) { return 0.0f; }
        return base.ScoreCalculation(dis, dot);
    }

    public override void Execute(EnemyAnimatorController animator)
    {
        base.Execute(animator);
        animator.enemy.SetLookPlayerAndEnemyStop(false, true);
        animator.SetTriggerAnim(EnemyAnimatorController.AnimationBase.Wizard_Mera);
    }
}
