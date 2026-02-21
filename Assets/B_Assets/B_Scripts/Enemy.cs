using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    protected Transform target;
    protected NavMeshAgent agent;

    [SerializeField] protected float searchDistance;  //’T’m”ÍˆÍ
    [SerializeField] protected float contactDistance; //’ÇÕ”ÍˆÍ
    protected float distance = 0;
    public bool isContact = false;

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
            //ˆÚ“®æ‚ğİ’è‚·‚é
            agent.SetDestination(target.position);
            if (contactDistance <= distance)
            {
                isContact = false;
                agent.isStopped = true;
            }
        }
    }
}
