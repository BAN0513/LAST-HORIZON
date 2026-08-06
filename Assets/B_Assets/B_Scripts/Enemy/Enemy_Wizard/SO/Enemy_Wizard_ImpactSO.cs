using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_Wizard_ImpactSO", menuName = "EnemyActionSO/Enemy_WizardActionSO/Enemy_Wizard_ImpactSO")]
public class Enemy_Wizard_ImpactSO : Enemy_WizardActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        if (!enemy.isAction) { return 0.0f; }
        return base.ScoreCalculation(dis, dot);
    }

    public override void Execute(Enemy_WizardAnimatorController enemy_WizardAnim)
    {
        enemy_WizardAnim.enemy.AttackAnimStart();
        enemy_WizardAnim.SetTriggerAnim(Enemy_WizardAnimatorController.Enemy_1Animation.Impact);
    }
}
