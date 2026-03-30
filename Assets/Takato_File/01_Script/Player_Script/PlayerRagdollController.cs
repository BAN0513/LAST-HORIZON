using UnityEngine;
using Takato;

/// <summary>
/// プレイヤーのラグドールを制御するクラス
/// </summary>
namespace Takato
{
    public class PlayerRagdollController : MonoBehaviour
    {
        [Header("ラグドール化対象のルート")]
        [SerializeField] private Transform ragdollRoot; // ラグドール化したいボーンの親

        private Rigidbody[] ragdollRigidbodies; // ラグドール化するRigidbodyの配列
        private Collider[] ragdollColliders;    // ラグドール化するColliderの配列
        private Animator animator;             // Animatorコンポーネント

        private void Awake()
        {
            animator = GetComponent<Animator>();

            // ragdollRootが未設定なら自身を使う
            if (ragdollRoot == null)
                ragdollRoot = transform;

            // 子階層のRigidbodyとColliderを取得
            ragdollRigidbodies = ragdollRoot.GetComponentsInChildren<Rigidbody>(true);
            ragdollColliders = ragdollRoot.GetComponentsInChildren<Collider>(true);

            // 初期状態はラグドール無効
            SetRagdollActive(false);
        }

        /// <summary>
        /// ラグドールの有効/無効を切り替える
        /// </summary>
        public void SetRagdollActive(bool active)
        {
            // Animatorの有効/無効
            if (animator != null)
                animator.enabled = !active;

            // RigidbodyとColliderの切り替え
            foreach (var rb in ragdollRigidbodies)
            {
                rb.isKinematic = !active;
            }
            foreach (var col in ragdollColliders)
            {
                // 自身のColliderは無効化しない。
                if (col.gameObject == this.gameObject) continue;
                col.enabled = active;
            }
        }

        /// <summary>
        /// 死亡時に呼び出す
        /// </summary>
        public void ActivateRagdoll()
        {
            //Animatorを無効化
            if (animator != null)
                animator.enabled = false;

            //RigidbodyとColliderを物理化
            foreach (var rb in ragdollRigidbodies)
                rb.isKinematic = false;
            foreach (var col in ragdollColliders)
                if (col.gameObject != this.gameObject) col.enabled = true;

            //速度伝播（rootボーンのみ）
            var mainRb = GetComponent<Rigidbody>();
            if (mainRb != null && ragdollRigidbodies.Length > 0)
            {
                ragdollRigidbodies[0].linearVelocity = mainRb.linearVelocity; // 速度を伝える
                ragdollRigidbodies[0].angularVelocity = mainRb.angularVelocity; // 回転速度を伝える
            }

            // 武器の暴れ対策
            var weapon = GetComponentInChildren<Weapon>();
            if (weapon != null)
            {
                var rb = weapon.GetComponent<Rigidbody>();  // 武器のRigidbodyを取得
                if (rb != null) rb.isKinematic = true;      // 武器を物理化しない
                var col = weapon.GetComponent<Collider>();  // 武器のColliderを取得
                if (col != null) col.enabled = false;       // 武器のColliderを無効化
            }
        }
    }
}
