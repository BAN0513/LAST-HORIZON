using Takato;
using UnityEngine;

public class PreventionOfRiding : MonoBehaviour
{
    [Header("プレイヤーを押し出す力")]
    [SerializeField] private float power_PushPlayer = 0.1f;

    [Header("押し出しをプレイヤーだけに適応するかどうか")]
    [SerializeField] private bool pushPlayerOnly = false;

    [Header("自身を押し出す力")]
    [SerializeField] private float power_PushEnemy = 2.0f;
 
    Enemy enemy;
    CharacterController playerController;
    Vector3 dir;

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
            dir.y = 0.0f;
            dir *= power_PushPlayer;
            if (!playerController.enabled) { return; }
            playerController.Move(dir);
        }

        if (pushPlayerOnly) { return; }
        if (other.CompareTag("Wall") || other.CompareTag("Pillar"))
        {
            dir = Vector3.Normalize(enemy.transform.position - other.gameObject.transform.position);
            enemy.Agent.velocity = dir * power_PushEnemy;
        }
    }
}
