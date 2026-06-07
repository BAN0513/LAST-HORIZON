using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_1_BackMove", menuName = "EnemyActionSO/Enemy_1ActionSO/Enemy_1_BackMove")]
public class Enemy_1_BackMoveSO : Enemy_1ActionSO
{
    public override float ScoreCalculation(float dis, float dot)
    {
        return base.ScoreCalculation(dis, dot);
    }

    public override void Execute(Enemy_1AnimatorController enemy_1Anim)
    {
        enemy_1Anim.SetTriggerAnim(Enemy_1AnimatorController.Enemy_1Animation.BackMove);
    }
}
