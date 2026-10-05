using UnityEngine;
using UnityEngine.AI;


public class Enemy_WolfBoss_Teleport : StateMachineBehaviour
{
    [Header("何秒後にテレポートするか")]
    [SerializeField] private float teleportTime = 3;
    private float teleportTimer = 0.0f;

    [Header("テレポートする時のプレイヤーとの距離")]
    [SerializeField] private float initDistance = 10.0f;
    private float distance = 10.0f;

    [Header("障害物のレイヤー")]
    [SerializeField] private LayerMask layerMask;

    private float checkDistance = 5.0f;

    private bool isTeleport = false;
    private Vector3[] teleportPosition = new Vector3[4];
    private int[] teleportRand = new int[4] { 0, 1, 2, 3 };

    Vector3 forward = Vector3.zero;
    Vector3 right = Vector3.zero;

    private Enemy_WolfBoss enemy;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy_WolfBoss>();

        teleportTimer = teleportTime;
        isTeleport = false;

        distance = initDistance;

        TeleportPositionInit();

    }

    private void TeleportPositionInit()
    {
        forward = enemy.Target.forward.normalized;
        right = enemy.Target.right.normalized;
        teleportPosition[0] = forward * distance;
        teleportPosition[1] = -forward * distance;
        teleportPosition[2] = right * distance;
        teleportPosition[3] = -right * distance;
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
            for (int j = 0; j < teleportRand.Length; j++)
            {
                hit = CheckTeleport(teleportRand[j]);

                if (hit)
                {
                    enemy.transform.position = enemy.Target.transform.position + teleportPosition[teleportRand[j]];
                    enemy.transform.rotation = Quaternion.LookRotation((enemy.Target.position - enemy.transform.position).normalized);
                    enemy.wolf_Anim.SetBoolAnim(EnemyAnimatorController.AnimationBase.WolfBoss_Teleport, false);
                    isTeleport = true;
                    break;
                }
            }

            //テレポート失敗したので距離を離してやり直す
            distance++;
            TeleportPositionInit();
        }
    }

    private bool CheckTeleport(int num)
    {
        Vector3 checkPosition = enemy.Target.transform.position + teleportPosition[num];
        NavMeshHit hit;

        if (NavMesh.SamplePosition(checkPosition, out hit, checkDistance, NavMesh.AllAreas))
        {
            Collider[] col = Physics.OverlapSphere(checkPosition, checkDistance, layerMask);

            if (col.Length > 0)
            {
                Debug.Log("障害物に当たった");
                return false;
            }
            return true;
        }

        return false;
    }
}
