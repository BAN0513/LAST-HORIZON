using UnityEngine;

public class Wolf_BossAnimatorController : EnemyAnimatorController
{
    private Wolf_Boss wolf_Boss;

    int isAttack_1Hash;
    int isDashAttackBeforeHash;
    int isRotationAttackHash;
    int isTailAttackHash;

    protected override void Start()
    {
        base.Start();

        wolf_Boss = GetComponent<Wolf_Boss>();

        isAttack_1Hash = Animator.StringToHash("isAttack_1");
        isDashAttackBeforeHash = Animator.StringToHash("isDashAttackBefore");
        isRotationAttackHash = Animator.StringToHash("isRotationAttack");
        isTailAttackHash = Animator.StringToHash("isTailAttack");
    }

    protected override void Update()
    {
        base.Update();

        bool isAttack_1 = animator.GetBool(isAttack_1Hash);
        bool isDashAttackBefore = animator.GetBool(isDashAttackBeforeHash);
        bool isRotationAttack = animator.GetBool(isRotationAttackHash);
        bool isTailAttack = animator.GetBool(isTailAttackHash);

        if (wolf_Boss.isAttack_1 != isAttack_1) animator.SetBool(isAttack_1Hash, wolf_Boss.isAttack_1);
        if (wolf_Boss.isDashAttacKBefore != isDashAttackBefore) animator.SetBool(isDashAttackBeforeHash, wolf_Boss.isDashAttacKBefore);
        if (wolf_Boss.isRotationAttack != isRotationAttack) animator.SetBool(isRotationAttackHash, wolf_Boss.isRotationAttack);
        if (wolf_Boss.isTailAttack != isTailAttack) animator.SetBool(isTailAttackHash, wolf_Boss.isTailAttack);
    }
}
