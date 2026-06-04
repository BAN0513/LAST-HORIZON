using UnityEngine;

public class Enemy_1_SwordCombo : StateMachineBehaviour
{
    [SerializeField] private float walkSpeed = 5.0f;
    private Enemy enemy;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.transform.position += Vector3.Normalize(enemy.transform.forward) * walkSpeed * Time.deltaTime;
    }
}
