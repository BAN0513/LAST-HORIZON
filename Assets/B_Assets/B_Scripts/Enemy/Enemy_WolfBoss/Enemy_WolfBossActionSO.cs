using UnityEngine;

public class Enemy_WolfBossActionSO : EnemyActionSO
{
    [Header("この技が使用可能な形態")]
    [SerializeField] private Enemy_WolfBoss.WolfBoss_Form availableForms;

    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        if (enemy is Enemy_WolfBoss wolf)
        {
            if (availableForms != wolf.Form) { return 0.0f; }
        }
        return base.ScoreCalculation(dis, dot);
    }

    //アニメーションの実行
    public override void Execute(EnemyAnimatorController animator)
    {
        animator.Enemy.SetLookPlayerAndEnemyStop(false, true);
        base.Execute(animator);
    }

    public override void ActionEnd(EnemyAnimatorController animator)
    {
        base.ActionEnd(animator);
    }
}
