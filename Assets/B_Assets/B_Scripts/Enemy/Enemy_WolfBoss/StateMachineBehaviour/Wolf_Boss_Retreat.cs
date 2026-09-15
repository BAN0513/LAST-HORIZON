using UnityEngine;

public class Wolf_Boss_Retreat : StateMachineBehaviour
{
    [SerializeField] private float dashSpeed = 5.0f;
    private Enemy enemy;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.transform.position += Vector3.Normalize(-enemy.transform.forward) * dashSpeed * Time.deltaTime;
    }
}
