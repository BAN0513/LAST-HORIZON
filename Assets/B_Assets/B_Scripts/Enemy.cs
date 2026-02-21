using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    protected Transform target;
    protected NavMeshAgent agent;
    protected EnemyAnimatorController animatorContoller;

    [SerializeField] protected float searchDistance;  //íTímîÕàÕ
    [SerializeField] protected float contactDistance; //í«ê’îÕàÕ

    protected float distance = 0;
    public bool isContact = false;
    protected bool isAnimation = false;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        target = GameObject.FindWithTag("Player").transform;
    }

    protected virtual void Update()
    {
        if (target == null) { return; }

        distance = Vector3.Distance(transform.position, target.position);

        if (!isContact)
        {
            if (distance <= searchDistance)
            {
                isContact = true;
                agent.isStopped = false;
            }
        }
        else
        {
            //à⁄ìÆêÊÇê›íËÇ∑ÇÈ
            agent.SetDestination(target.position);
            if (contactDistance <= distance)
            {
                isContact = false;
                agent.isStopped = true;
            }
        }
    }
}
