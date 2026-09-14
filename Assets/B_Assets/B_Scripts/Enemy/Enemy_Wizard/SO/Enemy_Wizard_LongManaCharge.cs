using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_Wizard_LongManaCharge", menuName = "EnemyActionSO/Enemy_WizardActionSO/Enemy_Wizard_LongManaCharge")]
public class Enemy_Wizard_LongManaCharge : Enemy_WizardActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        if (!enemy.IsAction) { return 0.0f; }

        if (enemy is Enemy_Wizard wizard)
        {
            if (wizard.LongManaCharge) { return Mathf.Infinity; }
        }

        return 0.0f;
    }

    public override void Execute(EnemyAnimatorController animator)
    {
        base.Execute(animator);
        animator.Enemy.SetLookPlayerAndEnemyStop(false, true);
        animator.SetTriggerAnim(EnemyAnimatorController.AnimationBase.Wizard_LongManaCharge);
    }
}
