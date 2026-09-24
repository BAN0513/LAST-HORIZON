using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_WolfBoss_FormChangeSO", menuName = "EnemyActionSO/Enemy_WolfBossActionSO/Enemy_WolfBoss_FormChangeSO")]
public class Enemy_WolfBoss_FormChangeSO : Enemy_WolfBossActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        if ((float)enemy.HP / (float)enemy.EnemySO.maxHP <= 0.2f)
        {
            if (enemy is Enemy_WolfBoss wolf)
            {
                if (wolf.Form == Enemy_WolfBoss.WolfBoss_Form.Two) { return 0.0f; }
                return Mathf.Infinity;
            }
        }

        return 0.0f;
    }

    public override void Execute(EnemyAnimatorController animator)
    {
        base.Execute(animator);
        if (animator.Enemy is Enemy_WolfBoss wolf)
        {
            wolf.Form = Enemy_WolfBoss.WolfBoss_Form.Two;
        }
        animator.SetTriggerAnim(EnemyAnimatorController.AnimationBase.WolfBoss_FormChange);
    }
}
