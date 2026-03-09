using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyHPBarDirection : MonoBehaviour
{
    private Transform target;
    private Canvas canvas;

    private void Start()
    {
        canvas = GetComponent<Canvas>();

        target = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        canvas.transform.LookAt(target);
    }
}
