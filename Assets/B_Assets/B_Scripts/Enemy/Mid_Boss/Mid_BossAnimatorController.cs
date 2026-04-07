using UnityEngine;

public class Mid_BossAnimatorController : EnemyAnimatorController
{
    private Mid_Boss mid_Boss;

    int isMelee1Hash;
    int isMelee2Hash;
    int isBlockHash;
    int is360AttackHash;

    protected override void Start()
    {
        base.Start();

        mid_Boss = GetComponent<Mid_Boss>();

        isMelee1Hash = Animator.StringToHash("isMelee1");
        isMelee2Hash = Animator.StringToHash("isMelee2");
        isBlockHash = Animator.StringToHash("isBlock");
        is360AttackHash = Animator.StringToHash("is360Attack");
    }

    protected override void Update()
    {
        base.Update();

        bool isMelee1 = animator.GetBool(isMelee1Hash);
        bool isMelee2 = animator.GetBool(isMelee2Hash);
        bool isBlock  = animator.GetBool(isBlockHash);
        bool is360Attack = animator.GetBool(is360AttackHash);

        if (mid_Boss.isMelee1 != isMelee1) animator.SetBool(isMelee1Hash, mid_Boss.isMelee1);
        if (mid_Boss.isMelee2 != isMelee2) animator.SetBool(isMelee2Hash, mid_Boss.isMelee2);
        if (mid_Boss.isBlock  != isBlock)  animator.SetBool(isBlockHash,  mid_Boss.isBlock);
        if (mid_Boss.is360Attack != is360Attack) animator.SetBool(is360AttackHash,mid_Boss.is360Attack);
    }
}
