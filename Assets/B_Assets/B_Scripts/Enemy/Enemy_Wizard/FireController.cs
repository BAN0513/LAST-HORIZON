using System.Collections;
using Takato;
using UnityEngine;

public class FireController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f; 
    private Rigidbody rb;

    public Vector3 Direction { private get; set; }

    private int damage;
    public int Damage { set { damage = value; } }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(DestroyCnt());
    }

    private void Update()
    {
        rb.linearVelocity = Direction * moveSpeed;
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
            PlayerController player = other.GetComponent<PlayerController>();

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
