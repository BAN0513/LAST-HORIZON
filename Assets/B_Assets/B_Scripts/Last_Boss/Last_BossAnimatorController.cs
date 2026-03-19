using UnityEngine;

public class Last_BossAnimatorController : EnemyAnimatorController
{
    private Last_Boss last_Boss;

    int isChantHash;
    int isMagicHash;
    int isFireHash;

    protected override void Start()
    {
        base.Start();

        last_Boss = GetComponent<Last_Boss>();

        isChantHash   = Animator.StringToHash("isChant");
        isMagicHash   = Animator.StringToHash("isMagic");
        isFireHash    = Animator.StringToHash("isFire");
    }

    protected override void Update()
    {
        base.Update();

        bool isChant   = animator.GetBool(isChantHash);
        bool isMagic   = animator.GetBool(isMagicHash);
        bool isFire    = animator.GetBool(isFireHash);

        if (last_Boss.isChant != isChant)     animator.SetBool(isChantHash, last_Boss.isChant);
        if (last_Boss.isMagic != isMagic)     animator.SetBool(isMagicHash, last_Boss.isMagic);
        if (last_Boss.isFire  != isFire)      animator.SetBool(isFireHash,  last_Boss.isFire); 
    }
}
