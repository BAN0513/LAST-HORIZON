using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_WeakChargeAttackSO", menuName = "EnemyActionSO/Enemy_WeakActionSO/Enemy_WeakChargeAttackSO")]
public class Enemy_WeakChargeAttackSO : Enemy_WeakActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        if (!enemy.IsAction) { return 0.0f; }
        return base.ScoreCalculation(dis, dot);
    }

    public override void Execute(Enemy_WeakAnimatorController animator)
    {
        base.Execute(animator);
        animator.SetTriggerAnim(Enemy_WeakAnimatorController.Enemy_WeakAnimation.ChargeAttack);
    }
}
