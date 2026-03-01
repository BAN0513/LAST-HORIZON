using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Timeline;

public class Enemy : MonoBehaviour
{
    protected Transform target;
    protected NavMeshAgent agent;
    protected EnemyAnimatorController animatorContoller;

    [Header("敵のScriptable Object")]
    [SerializeField] protected EnemySO enemySO;

    //攻撃などのアクションを起こしているかどうか
    protected bool isAction = false;

    //プレイヤーと自身の距離
    protected float distance = 0;

    //接敵中か（今後常に敵対状態になるかも）
    public bool isContact {  get; protected set; }

    //アニメーション用
    public bool isWalking {  get; protected set; }

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        target = GameObject.FindWithTag("Player").transform;

        distance = Vector3.Distance(transform.position, target.position);

        agent.updateRotation = false;
    }

    protected virtual void Update()
    {
        //プレイヤーと自身の距離計算
        distance = Vector3.Distance(transform.position, target.position);

        //常にプレイヤーの方向を見るようにする
        Vector3 dir = target.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * enemySO.lookRotationSpeed
            );

        if (target == null) { return; }


        if (!isContact)
        {
            if (distance <= enemySO.searchDistance)
            {
                isContact = true;
                agent.isStopped = false;
            }
        }
        else
        {
            agent.SetDestination(target.position);
            if (enemySO.contactDistance <= distance)
            {
                isContact = false;
                agent.isStopped = true;
            }

        }
    }
}
