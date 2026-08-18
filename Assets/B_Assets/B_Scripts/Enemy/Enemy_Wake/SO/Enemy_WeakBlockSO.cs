using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_WeakBlockSO", menuName = "EnemyActionSO/Enemy_WeakActionSO/Enemy_BlockSO")]
public class Enemy_WeakBlockSO : Enemy_WeakActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        return base.ScoreCalculation(dis, dot);
    }

    public override void Execute(EnemyAnimatorController animator)
    {
        animator.SetBoolAnim(EnemyAnimatorController.AnimationBase.Weak_Block, true);
    }

    public override void ActionEnd(EnemyAnimatorController animator)
    {
        animator.SetBoolAnim(EnemyAnimatorController.AnimationBase.Weak_Block, false);
    }
}
