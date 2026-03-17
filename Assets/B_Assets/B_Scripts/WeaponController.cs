using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private BoxCollider boxCollider; // 武器のコライダー

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
        // 敵のタグに当たった時の処理
        if (other.CompareTag("Player"))
        {
            Debug.Log("攻撃が当たった");
        }
    }
}
