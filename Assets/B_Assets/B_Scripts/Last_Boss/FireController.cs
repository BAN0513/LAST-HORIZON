using System.Collections;
using Takato;
using UnityEngine;

public class FireController : MonoBehaviour
{
    private int damage;
    public int Damage
    {
        set
        {
            damage = value;
        }
    }
    private PlayerController player;
    public PlayerController Player
    {
        set
        {
            player = value;
        }
    }

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
        if (other.CompareTag("Player") || other.CompareTag("Shield"))
        {
            PlayerShieldContoroller shield = other.GetComponent<PlayerShieldContoroller>();

            if (shield != null)
            {
                shield.ReceiveAttack(damage, player);
            }
            else
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
