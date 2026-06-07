using UnityEngine;

public class Wolf_Boss_Reflection : StateMachineBehaviour
{
    [SerializeField] float reflectionSpeed = 5;
    private Enemy enemy;
    private Vector3 dir;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();

        enemy.LookRotaionSpeed = reflectionSpeed;
        enemy.IsLookPlayer = true;

        //dir = enemy.Target.position - enemy.transform.position;
        //dir.y = 0;
    }

    //public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    if (dir != Vector3.zero)
    //    {
    //        enemy.transform.rotation = Quaternion.Slerp(
    //            enemy.transform.rotation,
    //            Quaternion.LookRotation(dir),
    //            Time.deltaTime * reflectionSpeed
    //            );
    //    }
    //}
}
