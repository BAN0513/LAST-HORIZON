using UnityEngine;

public class Last_BossAnimatorController : EnemyAnimatorController
{
    private Last_Boss last_Boss;

    int isChantHash;
    int isMagicHash;
    int isFireHash;
    int isSlashHash;
    int isRunJumpAttackHash;

    protected override void Start()
    {
        base.Start();

        last_Boss = GetComponent<Last_Boss>();

        isChantHash   = Animator.StringToHash("isChant");
        isMagicHash   = Animator.StringToHash("isMagic");
        isFireHash    = Animator.StringToHash("isFire");
        isSlashHash = Animator.StringToHash("isSlash");
        isRunJumpAttackHash = Animator.StringToHash("isRunJumpAttack");
    }

    protected override void Update()
    {
        base.Update();

        bool isChant   = animator.GetBool(isChantHash);
        bool isMagic   = animator.GetBool(isMagicHash);
        bool isFire    = animator.GetBool(isFireHash);
        bool isSlash   = animator.GetBool(isSlashHash);
        bool isRunJumpAttack = animator.GetBool(isRunJumpAttackHash);

        if (last_Boss.isChant != isChant)     animator.SetBool(isChantHash, last_Boss.isChant);
        if (last_Boss.isMagic != isMagic)     animator.SetBool(isMagicHash, last_Boss.isMagic);
        if (last_Boss.isFire  != isFire)      animator.SetBool(isFireHash,  last_Boss.isFire); 
        if (last_Boss.isSlash != isSlash)     animator.SetBool(isSlashHash, last_Boss.isSlash);
        if (last_Boss.isRunJumpAttack != isRunJumpAttack) animator.SetBool(isRunJumpAttackHash, last_Boss.isRunJumpAttack);

        //åªç›ÇÃStateÇ™âΩÇ©Çí≤Ç◊ÇÈÅBÇ‡ÇµSlashÇ©RunJumpAttackÇ»ÇÁAnimatorÇÃìÆÇ´ÇÇªÇÃÇ‹Ç‹égÇ§ÇΩÇﬂapplyRootMotionÇtrueÇ…Ç∑ÇÈÅB
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Slash") || animator.GetCurrentAnimatorStateInfo(0).IsName("RunJumpAttack"))
        {
            animator.applyRootMotion = true;
        }
        else
        {
            animator.applyRootMotion = false;
        }
    }
}
