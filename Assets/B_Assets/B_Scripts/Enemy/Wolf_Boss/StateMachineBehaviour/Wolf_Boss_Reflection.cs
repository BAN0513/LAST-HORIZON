using UnityEngine;

public class Wolf_Boss_Reflection : StateMachineBehaviour
{
    [SerializeField] float reflectionSpeed = 5;
    private Enemy enemy;
    private Vector3 dir;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();

        dir = enemy.target.position - enemy.transform.position;
        dir.y = 0;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (dir != Vector3.zero)
        {
            enemy.transform.rotation = Quaternion.Slerp(
                enemy.transform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * reflectionSpeed
                );
        }
    }
}
