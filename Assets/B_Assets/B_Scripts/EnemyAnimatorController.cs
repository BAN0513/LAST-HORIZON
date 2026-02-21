using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    private Enemy enemy;
    private Animator animator;

    int isWalkingHash;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        animator = GetComponent<Animator>();

        //ƒnƒbƒVƒ…‰»
        isWalkingHash = Animator.StringToHash("isWalking");
    }

    private void Update()
    {
        bool isWalking = animator.GetBool(isWalkingHash);

        if (enemy.isContact)
        {
            animator.SetBool(isWalkingHash, enemy.isContact);
        }
    }
}
