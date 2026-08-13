using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "Wolf_Boss_Attack1_SO", menuName = "EnemyActionSO/Wolf_BossActionSO/Wolf_Boss_Attack1_SO")]
public class Wolf_Boss_Attack1_SO : Wolf_BossActionSO
{
    public override float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        if (!enemy.IsAction) { return 0.0f; }
        return base.ScoreCalculation(dis, dot);
    }

    public override void Execute(Wolf_BossAnimatorController wolf_Anim)
    {
        wolf_Anim.SetTriggerAnim(Wolf_BossAnimatorController.WolfAnimation.Attack_1);
    }
}
