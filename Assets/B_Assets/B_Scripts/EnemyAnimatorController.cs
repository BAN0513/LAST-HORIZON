using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    private Enemy_1 enemy;
    private Animator animator;

    int isWalkingHash;
    int isMelee1Hash;
    int isMelee2Hash;
    int isBackMoveHash;

    private void Start()
    {
        enemy = GetComponent<Enemy_1>();
        animator = GetComponent<Animator>();

        //ƒnƒbƒVƒ…‰»
        isWalkingHash = Animator.StringToHash("isWalking");
        isMelee1Hash  = Animator.StringToHash("isMelee1");
        isMelee2Hash  = Animator.StringToHash("isMelee2");
        isBackMoveHash = Animator.StringToHash("isBackMove");
    }

    private void Update()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isMelee1  = animator.GetBool(isMelee1Hash);
        bool isMelee2  = animator.GetBool(isMelee2Hash);
        bool isBackMove = animator.GetBool(isBackMoveHash);

        if (enemy.isWalking != isWalking) animator.SetBool(isWalkingHash, enemy.isWalking);
        if (enemy.isMelee1 != isMelee1) animator.SetBool(isMelee1Hash, enemy.isMelee1);
        if (enemy.isMelee2 != isMelee2) animator.SetBool(isMelee2Hash, enemy.isMelee2);
        if (enemy.isBackMove != isBackMove) animator.SetBool(isBackMoveHash, enemy.isBackMove);
    }
}
