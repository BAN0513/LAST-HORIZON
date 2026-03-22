using Takato;
using UnityEngine;

/// <summary>
/// プレイヤーのシールドを制御するクラス
/// </summary>
public class PlayerShieldContoroller : MonoBehaviour
{
    [Header("シールドのステータス")]
    [Space(10)]
    [Header("シールドのダメージカット率")]
    [SerializeField] private float damageCutRate;

    private Collider shieldCollider; // シールドのコライダー

    private void Start()
    {
        shieldCollider = GetComponent<Collider>(); // シールドのコライダーを取得
        shieldCollider.enabled = false;            // 初期状態ではシールドのコライダーを無効化
    }

    /// <summary>
    /// シールドコライダーを有効化
    /// </summary>
    public void EnableShieldCollider()
    {
        shieldCollider.enabled = true; // シールドのコライダーを有効化
        ReceiveAttack(0, null);        // ダメージカット率を適用するためにダミーの攻撃を受ける
        Debug.Log("シールドのコライダーを有効化");
    }

    /// <summary>
    /// シールドコライダーを無効化
    /// </summary>
    public void DisableShieldCollider()
    {
        shieldCollider.enabled = false; // シールドのコライダーを無効化
        Debug.Log("シールドのコライダーを無効化");
    }

    /// <summary>
    /// シールドが敵の攻撃に当たった時の処理
    /// </summary>
    public void ReceiveAttack(int damage, PlayerController player)
    {
        int reducedDamage = Mathf.RoundToInt(damage * (1f - damageCutRate));
        player.TakeDamage(reducedDamage);
        Debug.Log($"シールドでダメージカット: {damage} → {reducedDamage}");
    }
}
