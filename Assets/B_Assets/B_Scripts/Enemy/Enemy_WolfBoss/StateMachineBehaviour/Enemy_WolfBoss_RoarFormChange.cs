using UnityEngine;

public class Enemy_WolfBoss_RoarFormChange : StateMachineBehaviour
{
    [SerializeField] private float knockBackPower = 5.0f;
    [SerializeField] private float distance = 30.0f;
    private Enemy enemy;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemy.Distance > distance) { return; }
        enemy.Target.transform.position += (enemy.Target.transform.position - enemy.transform.position).normalized * knockBackPower * Time.deltaTime;
    }
}
