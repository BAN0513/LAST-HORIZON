using Takato;
using UnityEngine;

public class PreventionOfRiding : MonoBehaviour
{
    Enemy enemy;
    CharacterController playerController;
    Vector3 dir;

    private void Start()
    {
        enemy = GetComponentInParent<Enemy>();

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerController == null)
            {
                playerController = enemy.PlayerCharacterController;
            }

            dir = Vector3.Normalize(playerController.transform.position - transform.position);
            dir.y = 0;
            dir *= 0.1f;
            playerController.Move(dir);
        }
    }
}
