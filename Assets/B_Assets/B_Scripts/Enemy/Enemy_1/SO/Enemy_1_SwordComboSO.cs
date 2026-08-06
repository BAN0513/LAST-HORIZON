using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_1_isSwordCombo", menuName = "EnemyActionSO/Enemy_1ActionSO/Enemy_1_isSwordCombo")]
public class Enemy_1_SwordComboSO : Enemy_1ActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        if (!enemy.isAction) { return 0.0f; }
        return base.ScoreCalculation(dis, dot);
    }

    public override void Execute(Enemy_1AnimatorController enemy_1Anim)
    {
        enemy_1Anim.enemy.AttackAnimStart();
        enemy_1Anim.SetTriggerAnim(Enemy_1AnimatorController.Enemy_1Animation.SwordCombo);
    }
}
