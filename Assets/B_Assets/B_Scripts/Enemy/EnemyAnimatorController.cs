using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    protected Enemy enemy;
    protected Animator animator;

    int isWalkingHash;
    int isDeathHash;
    int isHitHash;
    int isBackMoveHash;
    int isDashHash;

    protected virtual void Start()
    {
        enemy = GetComponent<Enemy>();
        animator = GetComponent<Animator>();

        //ƒnƒbƒVƒ…‰»
        isWalkingHash  = Animator.StringToHash("isWalking");
        isDeathHash    = Animator.StringToHash("isDeath");
        isHitHash      = Animator.StringToHash("isHit");
        isBackMoveHash = Animator.StringToHash("isBackMove");
        isDashHash     = Animator.StringToHash("isDash");
    }

    protected virtual void Update()
    {
        bool isWalking  = animator.GetBool(isWalkingHash);
        bool isDeath    = animator.GetBool(isDeathHash);
        bool isHit      = animator.GetBool(isHitHash);
        bool isBackMove = animator.GetBool(isBackMoveHash);
        bool isDash     = animator.GetBool(isDashHash);

        if (enemy.isWalking != isWalking)   animator.SetBool(isWalkingHash, enemy.isWalking);
        if (enemy.isDeath != isDeath)       animator.SetBool(isDeathHash, enemy.isDeath);
        if (enemy.isHit != isHit)           animator.SetBool(isHitHash,enemy.isHit);
        if (enemy.isBackMove != isBackMove) animator.SetBool(isBackMoveHash, enemy.isBackMove);
        if (enemy.isDash != isDash)         animator.SetBool(isDashHash, enemy.isDash);
    }


}
