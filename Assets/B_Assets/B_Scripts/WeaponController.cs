using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;
    }

    public void SetColliderActive(bool active)
    {
        boxCollider.enabled = active;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("UŒ‚‚ª“–‚½‚Á‚½");
        }
    }
}
