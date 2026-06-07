using UnityEngine;

public class Enemy_1_BackMove : StateMachineBehaviour
{
    [SerializeField] private float walkSpeed = 1.0f;
    [SerializeField] private float rotationSpeed = 3.0f;
    private Enemy enemy;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();
        enemy.LookRotaionSpeed = rotationSpeed;
        enemy.IsLookPlayer = true;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.transform.position += -enemy.transform.forward * walkSpeed * Time.deltaTime;
    }
}
