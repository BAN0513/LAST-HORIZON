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
        /// <param name="active">trueでラグドール化</param>
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
                // 自身のCollider（例: CharacterController）は除外
                if (col.gameObject == this.gameObject) continue;
                col.enabled = active;
            }
        }

        /// <summary>
        /// 死亡時に呼び出す
        /// </summary>
        public void ActivateRagdoll()
        {
            SetRagdollActive(true); // ラグドール化

            // 物理挙動を自然にするために現在の速度をラグドールのRigidbodyに伝える
            var mainRb = GetComponent<Rigidbody>();
            if (mainRb != null)
            {
                foreach (var rb in ragdollRigidbodies)
                {
                    rb.linearVelocity = mainRb.linearVelocity;
                    rb.angularVelocity = mainRb.angularVelocity;
                }
            }

            // 武器の暴れ対策
            var weapon = GetComponentInChildren<Weapon>();
            if (weapon != null)
            {
                var rb = weapon.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;
                var col = weapon.GetComponent<Collider>();
                if (col != null) col.enabled = false;
            }
        }
    }
}
