using System.Collections;
using UnityEngine;
using static Enemy_FourLegs;

public class ChargeAttackCheckHit : MonoBehaviour
{
    [SerializeField] private BoxCollider boxCollider;

    Enemy_WolfBoss wolf;

    private void Start()
    {
        wolf = GetComponentInParent<Enemy_WolfBoss>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!boxCollider.enabled) { return; }
        if (other.CompareTag("Wall"))
        {
            wolf.Init();
        }
        else if (other.CompareTag("Pillar"))
        {
            wolf.wolf_Anim.SetTriggerAnim(EnemyAnimatorController.AnimationBase.WolfBoss_DownBefore);
        }

        wolf.wolf_Anim.SetBoolAnim(EnemyAnimatorController.AnimationBase.WolfBoss_DashAttack, false);
        wolf.Agent.enabled = true;
        wolf.AttackJudgmentEnd(BodyPart.AllBody);
    }
}
