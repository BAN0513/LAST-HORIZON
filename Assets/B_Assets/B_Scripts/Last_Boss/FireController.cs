using System.Collections;
using Takato;
using UnityEngine;

public class FireController : MonoBehaviour
{
    public int damage;
    private void Start()
    {
        StartCoroutine(DestroyCnt());
    }

    IEnumerator DestroyCnt()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
