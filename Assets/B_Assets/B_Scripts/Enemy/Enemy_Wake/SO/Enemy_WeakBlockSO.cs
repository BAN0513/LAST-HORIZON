using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_WeakBlockSO", menuName = "EnemyActionSO/Enemy_WeakActionSO/Enemy_BlockSO")]
public class Enemy_WeakBlockSO : Enemy_WeakActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        return base.ScoreCalculation(dis, dot);
    }

    public override void Execute(Enemy_WeakAnimatorController animator)
    {
        animator.SetBoolAnim(Enemy_WeakAnimatorController.Enemy_WeakAnimation.Block, true);
    }
}
