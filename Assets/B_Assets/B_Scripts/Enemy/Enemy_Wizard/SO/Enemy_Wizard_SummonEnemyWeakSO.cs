using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_Wizard_SummonEnemyWeakSO", menuName = "EnemyActionSO/Enemy_WizardActionSO/Enemy_Wizard_SummonEnemyWeakSO")]
public class Enemy_Wizard_SummonEnemyWeakSO : Enemy_WizardActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        if (!enemy.IsAction) { return 0.0f; }

        if (enemy is Enemy_Wizard wizard)
        {
            //テレポートが発動中は雑魚敵召喚は後回しにする
            if (wizard.IsTeleport) { return 0.0f; }
            switch(wizard.Enemy_WeakSpawnState)
            {
                case Enemy_Wizard.SummonEnemyWeakState.NotSpawn:
                    wizard.Enemy_WeakSpawnState = Enemy_Wizard.SummonEnemyWeakState.Spawn;
                    break;

                case Enemy_Wizard.SummonEnemyWeakState.FiftyPercentSpawn:
                    wizard.Enemy_WeakSpawnState = Enemy_Wizard.SummonEnemyWeakState.SpawnEnd;
                    break;

                case Enemy_Wizard.SummonEnemyWeakState.Spawn:
                case Enemy_Wizard.SummonEnemyWeakState.SpawnEnd:
                    return 0.0f;
            }
        }

        return Mathf.Infinity;
    }

    public override void Execute(EnemyAnimatorController animator)
    {
        base.Execute(animator);
        animator.enemy.SetLookPlayerAndEnemyStop(false, true);
        animator.SetTriggerAnim(EnemyAnimatorController.AnimationBase.Wizard_SummonEnemyWeak);
    }
}
