using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_WeakMeleeSO", menuName = "EnemyActionSO/Enemy_WeakActionSO/Enemy_WeakMeleeSO")]
public class Enemy_WeakMeleeSO : Enemy_WeakActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        if (!enemy.IsAction) { return 0.0f; }
        return base.ScoreCalculation(dis, dot);
    }

    public override void Execute(EnemyAnimatorController animator)
    {
        base.Execute(animator);
        animator.SetBoolAnim(EnemyAnimatorController.AnimationBase.Weak_Melee, true);
    }
}
