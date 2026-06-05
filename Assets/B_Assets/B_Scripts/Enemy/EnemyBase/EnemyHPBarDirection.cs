using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyHPBarDirection : MonoBehaviour
{
    private Transform target;
    private Canvas canvas;

    private void Start()
    {
        canvas = GetComponent<Canvas>();

        target = GameObject.FindWithTag("MainCamera").transform;
    }

    private void Update()
    {
        Vector3 dir = target.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
    }
}
