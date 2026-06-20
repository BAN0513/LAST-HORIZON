using Takato;
using UnityEngine;

public class EnemyAttackRollController : MonoBehaviour
{
    [Header("攻撃のステータス")]
    [Space(10)]

    [Header("武器のコライダー")]
    [SerializeField] private BoxCollider boxCollider; // 武器のコライダー
    [Header("攻撃のダメージ")]
    [SerializeField] private int damage;

    // ※この変数やプロパティは使わなくてもよくなるため、残しておいても削除しても大丈夫です
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
        boxCollider.enabled = false;// 初期状態では武器のコライダーを無効化
    }

    /// <summary>
    /// 武器のコライダーを有効化または無効化するメソッド
    /// </summary>
    public void SetColliderActive(bool active)
    {
        boxCollider.enabled = active; // 武器のコライダーを有効化または無効化
    }

    private void OnTriggerEnter(Collider other)
    {
        if (boxCollider.enabled == false) return;

        if (other.CompareTag("Player") || other.CompareTag("Shield"))
        {
            // プレイヤーに当たった場合の処理
            PlayerController targetPlayer = other.GetComponent<PlayerController>() ?? other.GetComponentInParent<PlayerController>();

            if (targetPlayer == null)
            {
                Debug.LogWarning("Playerタグのオブジェクトに当たりましたが、PlayerControllerが見つかりません。");
                return;
            }

            // シールドに当たったかどうかの判定
            PlayerShieldContoroller shield = other.GetComponent<PlayerShieldContoroller>();

            if (shield != null)
            {
                // シールドがある場合はガード処理（見つかった targetPlayer を渡す）
                shield.ReceiveAttack(damage, targetPlayer);
            }
            else
            {
                // 【修正】見つかった targetPlayer に対してダメージを与える
                targetPlayer.TakeDamage(damage);
                Debug.Log($"[Hit] 敵の攻撃がプレイヤーに命中！ ダメージ: {damage}");
            }
        }
    }
}