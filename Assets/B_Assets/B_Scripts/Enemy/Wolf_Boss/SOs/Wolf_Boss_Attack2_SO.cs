using UnityEngine;

[CreateAssetMenu(fileName = "Wolf_Boss_Attack2_SO", menuName = "EnemyActionSO/Wolf_BossActionSO/Wolf_Boss_Attack2_SO")]
public class Wolf_Boss_Attack2_SO : Wolf_BossActionSO
{
    public override float ScoreCalculation(float dis, float dot)
    {
        return base.ScoreCalculation(dis, dot);
    }

    public override void Execute(Wolf_BossAnimatorController wolf_Anim)
    {
        wolf_Anim.SetTriggerAnim(Wolf_BossAnimatorController.WolfAnimation.Attack_2);
    }
}
