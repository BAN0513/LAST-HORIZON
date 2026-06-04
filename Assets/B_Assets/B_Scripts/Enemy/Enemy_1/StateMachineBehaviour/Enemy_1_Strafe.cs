using UnityEngine;

public class Enemy_1_Strafe : StateMachineBehaviour
{
    [SerializeField] private float walkSpeed = 1.0f;
    private Enemy enemy;

    enum Dir
    {
        Left,
        Right
    };
    [SerializeField] Dir dir = Dir.Left;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (dir == Dir.Left)
        {
            enemy.transform.position += Vector3.Normalize(-enemy.transform.right) * walkSpeed * Time.deltaTime;
        }
        else
        {
            enemy.transform.position += Vector3.Normalize(enemy.transform.right) * walkSpeed * Time.deltaTime;
        }
    }
}
