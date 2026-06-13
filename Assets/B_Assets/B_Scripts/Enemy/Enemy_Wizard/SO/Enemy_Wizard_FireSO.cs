using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_Wizard_FireSO", menuName = "EnemyActionSO/Enemy_WizardActionSO/Enemy_Wizard_FireSO")]
public class Enemy_Wizard_FireSO : Enemy_WizardActionSO
{
    public override float ScoreCalculation(float dis, float dot)
    {
        return base.ScoreCalculation(dis, dot);
    }

    public override void Execute(Enemy_WizardAnimatorController enemy_WizardAnim)
    {
        enemy_WizardAnim.SetTriggerAnim(Enemy_WizardAnimatorController.Enemy_1Animation.Fire);
    }
}
