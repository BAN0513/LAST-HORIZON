using UnityEngine;

public class Wolf_Boss_ForwardStep : StateMachineBehaviour
{
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float rotationSpeed = 5.0f;

    private Enemy enemy;
    private Vector3 movePos;
    private Vector3 dir;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();

        movePos = enemy.transform.forward;

        enemy.LookRotaionSpeed = rotationSpeed;
        enemy.IsLookPlayer = true;

        //dir = enemy.Target.position - enemy.transform.position;
        //dir.y = 0;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        enemy.transform.position += movePos * walkSpeed * Time.deltaTime;

        //if (dir != Vector3.zero)
        //{
        //    enemy.transform.rotation = Quaternion.Slerp(
        //        enemy.transform.rotation,
        //        Quaternion.LookRotation(dir),
        //        Time.deltaTime * rotationSpeed
        //        );
        //}
    }
}
