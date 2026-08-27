using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_WeakBlockSO", menuName = "EnemyActionSO/Enemy_WeakActionSO/Enemy_BlockSO")]
public class Enemy_WeakBlockSO : Enemy_WeakActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        if (enemy is Enemy_Weak weak)
        {
            if (weak.IsBlocking) { return Mathf.Infinity; }
        }
        return 0.0f;
        //return base.ScoreCalculation(dis, dot);
    }

    public override void Execute(EnemyAnimatorController animator)
    {
        animator.enemy.SetLookPlayerAndEnemyStop(false, true);
        animator.SetBoolAnim(EnemyAnimatorController.AnimationBase.Weak_Block, true);
    }

    public override void ActionEnd(EnemyAnimatorController animator)
    {
        animator.SetBoolAnim(EnemyAnimatorController.AnimationBase.Weak_Block, false);

        if (animator.enemy is Enemy_Weak weak)
        {
            weak.IsBlocking = false;
            weak.IsBlockingReaction = false;
        }
    }
}
