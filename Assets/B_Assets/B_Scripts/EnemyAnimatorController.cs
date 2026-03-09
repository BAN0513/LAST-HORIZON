using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    protected Enemy enemy;
    protected Animator animator;

    int isWalkingHash;
    int isDeathHash;
    int isHitHash;

    protected virtual void Start()
    {
        enemy = GetComponent<Enemy>();
        animator = GetComponent<Animator>();

        //ƒnƒbƒVƒ…‰»
        isWalkingHash  = Animator.StringToHash("isWalking");
        isDeathHash    = Animator.StringToHash("isDeath");
        isHitHash      = Animator.StringToHash("isHit");
    }

    protected virtual void Update()
    {
        bool isWalking  = animator.GetBool(isWalkingHash);
        bool isDeath    = animator.GetBool(isDeathHash);
        bool isHit      = animator.GetBool(isHitHash);

        if (enemy.isWalking != isWalking)   animator.SetBool(isWalkingHash, enemy.isWalking);
        if (enemy.isDeath != isDeath)       animator.SetBool(isDeathHash, enemy.isDeath);
        if (enemy.isHit != isHit)           animator.SetBool(isHitHash,enemy.isHit);
    }
}
