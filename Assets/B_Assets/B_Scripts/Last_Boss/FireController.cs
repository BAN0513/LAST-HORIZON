using System.Collections;
using UnityEngine;

public class FireController : MonoBehaviour
{
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
            Debug.Log("‰Î‚ª“–‚½‚Á‚½");
        }
        Destroy(gameObject);
    }
}
