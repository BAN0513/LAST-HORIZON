using UnityEngine;

public class Enemy_1AnimatorController : EnemyAnimatorController
{
    private Enemy_1 enemy_1;

    int isMelee1Hash;
    int isMelee2Hash;

    protected override void Start()
    {
        base.Start();

        enemy_1 = GetComponent<Enemy_1>();

        isMelee1Hash   = Animator.StringToHash("isMelee1");
        isMelee2Hash   = Animator.StringToHash("isMelee2");
    }

    protected override void Update()
    {
        base.Update();

        bool isMelee1   = animator.GetBool(isMelee1Hash);
        bool isMelee2   = animator.GetBool(isMelee2Hash);

        if (enemy_1.isMelee1 != isMelee1)     animator.SetBool(isMelee1Hash, enemy_1.isMelee1);
        if (enemy_1.isMelee2 != isMelee2)     animator.SetBool(isMelee2Hash, enemy_1.isMelee2);
    }
}
