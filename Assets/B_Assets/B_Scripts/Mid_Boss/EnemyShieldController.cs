using UnityEngine;

public class EnemyShieldController : MonoBehaviour
{
    private Enemy enemy;
    public Enemy Enemy
    {
        set
        {
            enemy = value;
        }
    }
    private BoxCollider boxCollider;

    [Header("ダメージのカット率")]
    [SerializeField] private float damageCutRate;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public void SetColliderActive(bool active)
    {
        boxCollider.enabled = active; // 盾のコライダーを有効化または無効化
    }

    public void ReceiveAttack(int damage)
    {
        Debug.Log("防御成功");
        int reducedDamage = Mathf.RoundToInt(damage * (1f - damageCutRate));
        enemy.TakeDamage(reducedDamage);
    }

}
