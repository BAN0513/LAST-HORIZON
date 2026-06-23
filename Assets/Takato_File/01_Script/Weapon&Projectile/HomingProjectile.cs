using UnityEngine;

namespace Takato
{
    /// <summary>
    /// ホーミング弾の基本実装
    /// ターゲットが設定されていれば追尾、衝突時に Enemy.TakeDamage(int) を呼ぶ
    /// </summary>
    public class HomingProjectile : MonoBehaviour
    {
        [Tooltip("移動速度")]
        public float speed;
        [Tooltip("回転速度（追尾の滑らかさ）")]
        public float rotationSpeed;
        [Tooltip("与えるダメージ")]
        public float damage;

        private Transform target; // 追尾対象の Transform

        private void Update()
        {
            // ターゲットがいればそちらを向く
            if (target != null)
            {
                Vector3 dir = (target.position - transform.position).normalized;
                if (dir.sqrMagnitude > 0.0001f)
                {
                    Quaternion lookRot = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
                }
            }

            // 前方へ移動（Transformによる移動）
            transform.position += transform.forward * speed * Time.deltaTime;
        }

        /// <summary>
        /// ターゲットを設定する
        /// </summary>
        public void SetTarget(Transform t)
        {
            target = t;
        }

        /// <summary>
        /// 何かに接触したときの判定（Is Trigger用）
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (other == null) return;

            // 接触したオブジェクトに "Enemy" タグがついているかチェック
            if (other.CompareTag("Enemy"))
            {
                // Enemyコンポーネントがあればダメージを与える
                if (other.TryGetComponent<Enemy>(out var enemy))
                {
                    enemy.TakeDamage((int)damage);
                }

                // Enemyタグのオブジェクトに当たったので、自身（弾）を削除
                Destroy(gameObject);
            }
        }
    }
}