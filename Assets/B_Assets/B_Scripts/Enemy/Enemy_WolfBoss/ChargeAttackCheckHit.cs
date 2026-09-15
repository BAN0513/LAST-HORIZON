using System.Collections;
using UnityEngine;
using static Enemy_FourLegs;

public class ChargeAttackCheckHit : MonoBehaviour
{
    Enemy_WolfBoss wolf;

    private void Start()
    {
        wolf = GetComponentInParent<Enemy_WolfBoss>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!wolf.wolf_Anim.CheckCurrentAnim("DashAttack")) { return; }
        Debug.Log("Hit");
        if (other.CompareTag("Wall"))
        {
            wolf.Init();
        }
        else if (other.CompareTag("Pillar"))
        {
            wolf.wolf_Anim.SetTriggerAnim(EnemyAnimatorController.AnimationBase.WolfBoss_DownBefore);
        }

        wolf.wolf_Anim.SetBoolAnim(EnemyAnimatorController.AnimationBase.Dash, false);
        wolf.Agent.enabled = true;
        wolf.AttackJudgmentEnd(BodyPart.AllBody);
    }
}
