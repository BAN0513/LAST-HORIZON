using UnityEngine;

/// <summary>
/// ホーミング弾の挙動を管理するクラス
/// </summary>
public class HomingProjectile : MonoBehaviour
{
    [HideInInspector]
    public float speed;         // 弾の速度
    [HideInInspector]
    public float rotationSpeed; // ホーミングの回転速度
    [HideInInspector]
    public float damage;        // 弾のダメージ量


    private Transform target;   // ターゲットの位置
  
    /// <summary>
    /// ターゲットを設定するメソッド
    /// </summary>
    public void SetTarget(Transform targetTransform)
    {
        this.target = targetTransform;
    }

    private void Update()
    {
        if(target == null)
        {
            Destroy(gameObject); // ターゲットが存在しない場合は弾を破壊
            return;
        }
        // ターゲットの方向を計算
        Vector3 direction = target.position - transform.position;
        direction.Normalize();
        // 現在の向きをターゲットの方向に回転させる
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, rotationSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
        // 弾を前進させる
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    // ターゲットに衝突したときの処理
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) // 敵に衝突した場合
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(Mathf.RoundToInt(damage));
            }
            Destroy(gameObject); // 弾を破壊
        }
    }
}
