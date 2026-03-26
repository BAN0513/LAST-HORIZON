using Takato;
using UnityEngine;

public class EnemyWeaponController : MonoBehaviour
{
    private BoxCollider boxCollider; // 武器のコライダー
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
        boxCollider = GetComponent<BoxCollider>(); // 武器のコライダーを取得
        boxCollider.enabled = false;               // 初期状態では武器のコライダーを無効化
    }

    public void SetColliderActive(bool active)
    {
        boxCollider.enabled = active; // 武器のコライダーを有効化または無効化
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Shield"))
        {
            PlayerShieldContoroller shield = other.GetComponent<PlayerShieldContoroller>();

            if (shield != null)
            {
                shield.ReceiveAttack(damage,player);
            }
            else
            {
                player.TakeDamage(damage);
            }
        }
    }
}
