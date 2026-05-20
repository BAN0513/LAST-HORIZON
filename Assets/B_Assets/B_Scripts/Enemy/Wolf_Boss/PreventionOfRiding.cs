using Takato;
using UnityEngine;

public class PreventionOfRiding : MonoBehaviour
{
    Enemy enemy;
    CharacterController playerController;
    BoxCollider boxCollider;
    Vector3 dir;

    private void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        boxCollider = GetComponent<BoxCollider>();

        playerController = enemy.PlayerCharacterController;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dir = Vector3.Normalize(playerController.transform.position - transform.position);
            dir.y = 0;
            dir *= 0.1f;
            playerController.Move(dir);
        }
    }
}
