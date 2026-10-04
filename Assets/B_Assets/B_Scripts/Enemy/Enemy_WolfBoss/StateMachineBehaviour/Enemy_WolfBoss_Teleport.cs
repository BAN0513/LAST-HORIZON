using UnityEngine;
using UnityEngine.AI;

public class Enemy_WolfBoss_Teleport : StateMachineBehaviour
{
    [Header("何秒後にテレポートするか")]
    [SerializeField] private float teleportTime = 3;
    private float teleportTimer = 0.0f;

    [Header("テレポートする時のプレイヤーとの距離")]
    [SerializeField] private float distance = 10.0f;

    private float checkDistance = 3.0f;

    private bool isTeleport = false;
    private Vector3[] teleportPosition = new Vector3[4];
    private int[] teleportRand = new int[4] { 0, 1, 2, 3 };

    private Enemy_WolfBoss enemy;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy_WolfBoss>();

        teleportTimer = teleportTime;
        isTeleport = false;

        teleportPosition[0] = enemy.Target.transform.forward * distance;
        teleportPosition[1] = -enemy.Target.transform.forward * distance;
        teleportPosition[2] = enemy.Target.transform.right * distance;
        teleportPosition[3] = -enemy.Target.transform.right * distance;

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        teleportTimer -= Time.deltaTime;

        if (teleportTimer > 0.0f) { return; }

        if (!isTeleport)
        {
            for (int i = 0; i < teleportRand.Length; i++)
            {
                int rand = Random.Range(0, teleportRand.Length);
                int save = teleportRand[i];
                teleportRand[i] = teleportRand[rand];
                teleportRand[rand] = save;
            }

            bool hit = false;
            for (int i = 0; i < teleportRand.Length; i++)
            {
                hit = CheckTeleport(teleportRand[i]);

                if (hit)
                {
                    Debug.Log("rand" + teleportRand[i]);
                    enemy.transform.position = enemy.Target.transform.position + teleportPosition[teleportRand[i]];
                    enemy.transform.rotation = Quaternion.LookRotation((enemy.Target.position - enemy.transform.position).normalized);
                    enemy.wolf_Anim.SetBoolAnim(EnemyAnimatorController.AnimationBase.WolfBoss_Teleport, false);
                    isTeleport = true;
                    break;
                }
            }
        }
    }

    private bool CheckTeleport(int num)
    {
        NavMeshHit hit;

        if (NavMesh.SamplePosition(teleportPosition[0], out hit, checkDistance, NavMesh.AllAreas))
        {
            return true;
        }

        return false;
    }
}
