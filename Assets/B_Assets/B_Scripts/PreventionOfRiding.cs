using Takato;
using UnityEngine;

public class PreventionOfRiding : MonoBehaviour
{
    Enemy enemy;
    CharacterController playerController;
    Vector3 dir;
    [SerializeField] private float power = 0.1f;

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
            dir *= power;
            playerController.Move(dir);
        }
    }
}
