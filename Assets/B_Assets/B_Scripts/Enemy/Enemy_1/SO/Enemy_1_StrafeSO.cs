using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_1_Strafe", menuName = "EnemyActionSO/Enemy_1ActionSO/Enemy_1_Strafe")]
public class Enemy_1_StrafeSO : Enemy_1ActionSO
{
    public override float ScoreCalculation(float dis, float dot)
    {
        return base.ScoreCalculation(dis, dot);
    }

    public override void Execute(Enemy_1AnimatorController enemy_1Anim)
    {
        int rand = Random.Range(1, 101);

        if (rand <= 50)
        {
            enemy_1Anim.SetTriggerAnim(Enemy_1AnimatorController.Enemy_1Animation.Strafe_Left);
        }
        else
        {
            enemy_1Anim.SetTriggerAnim(Enemy_1AnimatorController.Enemy_1Animation.Strafe_Right);
        }
    }
}
