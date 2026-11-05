using UnityEngine;

/// <summary>
/// ファンネルの弾丸クラス
/// </summary>
public class Funnel_Bullet : MonoBehaviour
{
    //SOから受け取るパラメータ
    private Transform ownerTransform; // 追従対象（プレイヤー）
    private Vector3 localOffset;      // プレイヤーからの相対位置オフセット
    private Transform targetEnemy;    // 追尾対象の敵
    private float moveSpeed;          // 弾丸の移動速度
    private int damage;               // 弾丸のダメージ
    private bool isLaunched = false;  // 発射済みかどうかのフラグ
    private bool hasHit = false;      // 多重ヒット防止フラグ

    //コンポーネントの参照
    private TrailRenderer trailRenderer;
    private void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    /// <summary>
    /// ファンネルの初期化
    /// </summary>
    public void Initialize(Transform owner, Vector3 offset, Transform target, float speed, int dmg)
    {
        trailRenderer.Clear(); // トレイルをクリアして残像を消す
        trailRenderer.enabled = false; // 初期状態ではトレイルを無効にする

        ownerTransform = owner;
        localOffset = offset;
        targetEnemy = target;
        moveSpeed = speed;
        damage = dmg;
    }

    /// <summary>
    /// 発射指示に加えてトレイルを有効化する
    /// </summary>
    public void Launch()
    {
        isLaunched = true;
        trailRenderer.enabled = true;
    }

    private void Update()
    {
        // 未発射状態のときはプレイヤーに追従する
        if (!isLaunched)
        {
            if (ownerTransform != null)
            {
                // プレイヤーの位置＋相対オフセット位置へ移動
                transform.position = ownerTransform.position + localOffset;
            }
            return;
        }

        // 発射後の追尾処理
        if (targetEnemy != null)
        {
            // 敵の中心に向けて移動
            Vector3 direction = (targetEnemy.position + Vector3.up * 1.0f - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
        else
        {
            // 敵がすでに死亡・消滅している場合は前方に直進して消滅
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
            Destroy(gameObject, 2.0f);
        }
    }

    /// <summary>
    /// ファンネル弾が敵に当たったときの処理
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (!isLaunched || hasHit) return;

        if (other.CompareTag("Player") || other.GetComponent<Funnel_Bullet>() != null) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null)
        {
            enemy = other.GetComponentInParent<Enemy>();
        }

        if (enemy != null)
        {
            hasHit = true;
            enemy.TakeDamageIgnoreInvincibility(damage);
            Debug.Log($"<color=cyan>[Funnel Hit] {enemy.name} に {damage} のダメージ！</color>");

            Destroy(gameObject);
        }
    }
}