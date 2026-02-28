using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Timeline;

public class Enemy : MonoBehaviour
{
    protected Transform target;
    protected NavMeshAgent agent;
    protected EnemyAnimatorController animatorContoller;

    [SerializeField] protected float searchDistance;  //探知範囲
    [SerializeField] protected float contactDistance; //追跡範囲

    //攻撃などのアクションを起こしているかどうか
    protected bool isAction = false;

    //プレイヤーと自身の距離
    protected float distance = 0;

    //接敵中か（今後常に敵対状態になるかも）
    public bool isContact = false;

    //プレイヤーを見る動作のスピード
    private float lookRotationSpeed = 5;

    //NavMeshの初期値
    protected float initStoopingDis = 3;
    protected float initMoveSpeed = 3.5f;

    //アニメーション用
    public bool isWalking = false;

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
            Time.deltaTime * lookRotationSpeed
            );

        if (target == null) { return; }


        if (!isContact)
        {
            if (distance <= searchDistance)
            {
                isContact = true;
                isWalking = true;
                agent.isStopped = false;
            }
        }
        else
        {
            agent.SetDestination(target.position);
            if (contactDistance <= distance)
            {
                isContact = false;
                isWalking = false;
                agent.isStopped = true;
            }

        }
    }
}
