using UnityEngine;

public class Enemy_WolfBoss_DashAttack : StateMachineBehaviour
{
    [SerializeField] private float dashSpeed = 10.0f;
    private Enemy enemy;
    Vector3 dir;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();
        dir = Vector3.Normalize(enemy.transform.forward);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.transform.position += dir * dashSpeed * Time.deltaTime;
    }
}
