using Takato;
using UnityEngine;

public class PreventionOfRiding : MonoBehaviour
{
    Enemy enemy;
    CharacterController playerController;
    Vector3 dir;
    [SerializeField] private float power_PushPlayer = 0.1f;
    [SerializeField] private float power_PushEnemy = 2.0f;

    private void Start()
    {
        enemy = GetComponentInParent<Enemy>();

        if (enemy == null)
        {
            playerController = GameObject.FindWithTag("Player").GetComponent<CharacterController>();
        }
        else
        {
            playerController = enemy.Target.GetComponent<CharacterController>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dir = Vector3.Normalize(playerController.transform.position - transform.position);
            dir *= power_PushPlayer;
            if (!playerController.enabled) { return; }
            playerController.Move(dir);
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Pillar"))
        {
            dir = Vector3.Normalize(enemy.transform.position - other.gameObject.transform.position);
            dir *= power_PushEnemy;
            enemy.transform.position += dir * Time.deltaTime;
        }
    }
}
