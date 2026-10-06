using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの武器制御を管理するクラス
/// </summary>
public class Player_WeaponController_New : MonoBehaviour
{
    [Header("武器のデータの参照")]
    [SerializeField] private Weapon_SO_New weaponData;
    [Header("プレイヤーのアニメーションを参照する")]
    [SerializeField] private Player_Animation_New playerAnimation;
    [Header("武器のTransform")]
    [SerializeField] private Transform weaponHolderpoint;

    private Collider weaponCollider; // 生成した武器のコライダー
    private List<Enemy> hitEnemies = new List<Enemy>(); // 攻撃判定中にヒットした敵を管理するリスト
    private float currentChargeRatio = 0f;
    private bool isChargedAttack = false;

    private void Start()
    {
        // 初期装備があれば生成
        if (weaponData != null)
        {
            EquipWeapon(weaponData);
        }
    }

    public Weapon_SO_New CurrentWeaponData => weaponData; // 現在装備している武器のデータを外部から取得可能にする
    /// <summary>
    /// 武器を装備・生成するメソッド
    /// </summary>
    public void EquipWeapon(Weapon_SO_New newWeaponSO)
    {
        weaponData = newWeaponSO;

        if (weaponHolderpoint == null) return;

        // 既存の武器モデルを削除
        foreach (Transform child in weaponHolderpoint)
        {
            Destroy(child.gameObject);
        }

        // 新しい武器のPrefabを生成して配置
        if (weaponData != null && weaponData.WeaponPrefab != null)
        {
            GameObject weaponInstance = Instantiate(weaponData.WeaponPrefab, weaponHolderpoint.position, weaponHolderpoint.rotation, weaponHolderpoint);

            // 生成した武器オブジェクト（またはその子）からColliderを取得
            weaponCollider = weaponInstance.GetComponentInChildren<Collider>();

            if (weaponCollider != null)
            {
                weaponCollider.isTrigger = true;
                weaponCollider.enabled = false; // 初期状態は攻撃判定オフ

                // Colliderが付いているオブジェクトに WeaponHitbox を取り付けて初期化
                Weapon_HitBox hitbox = weaponCollider.gameObject.GetComponent<Weapon_HitBox>();
                if (hitbox == null)
                {
                    hitbox = weaponCollider.gameObject.AddComponent<Weapon_HitBox>();
                }
                hitbox.Initialize(this);
            }
            else
            {
                Debug.LogWarning($"{weaponData.WeaponPrefab.name} に Collider が設定されていません。");
            }
        }
    }

    /// <summary>
    /// 溜め攻撃の比率を設定するメソッド
    /// </summary>
    /// <param name="ratio"></param>
    public void SetChargeRatio(float ratio)
    {
        if (weaponData == null) return;
        isChargedAttack = true;
        currentChargeRatio = Mathf.Clamp01(ratio);
    }

    /// <summary>
    /// 武器の攻撃判定を有効化する
    /// </summary>
    public void EnableAttackCollider()
    {
        hitEnemies.Clear();
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }
    }

    /// <summary>
    /// 武器の攻撃判定を無効化する
    /// </summary>
    public void DisableAttackCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
        hitEnemies.Clear(); // 攻撃判定を無効化した際にヒットした敵のリストをクリア

        //溜め攻撃のフラグをリセット
        isChargedAttack = false;
        currentChargeRatio = 0f;
    }

    private void OnEnable()
    {
        if (playerAnimation == null)
        {
            playerAnimation = GetComponentInParent<Player_Animation_New>();
        }

        if (playerAnimation != null)
        {
            // 攻撃ヒット開始の購読
            playerAnimation.OnAttackHitStart += EnableAttackCollider;

            // 攻撃終了イベントでコライダーをオフにする
            playerAnimation.OnAttackEnd += DisableAttackCollider;
        }
    }

    private void OnDisable()
    {
        if (playerAnimation != null)
        {
            playerAnimation.OnAttackHitStart -= EnableAttackCollider;
            playerAnimation.OnAttackEnd -= DisableAttackCollider;
        }
    }

    /// <summary>
    /// WeaponHitbox から呼び出される武器用の衝突検知メソッド
    /// </summary>
    public void OnWeaponTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.GetComponent<Enemy>() != null)
        {
            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy != null && !hitEnemies.Contains(enemy))
            {
                hitEnemies.Add(enemy);

                // ダメージ計算処理
                int finalDamage = CalculateDamage(out bool isCritical);

                enemy.TakeDamage(finalDamage); // 敵にダメージを与える

                if (isCritical)
                {
                    Debug.Log($"<color=yellow>【クリティカル攻撃】</color> {enemy.name} に {finalDamage} のダメージを与えました！");
                }
                else
                {
                    Debug.Log($"{enemy.name} に武器で {finalDamage} のダメージを与えました！");
                }
            }
        }
    }


    /// <summary>
    /// クリティカル判定を含めたダメージを計算するメソッド
    /// </summary>
    private int CalculateDamage(out bool isCritical)
    {
        isCritical = false;

        if (weaponData == null) return 10; // 武器データがない場合のデフォルト

        float finalDamage = weaponData.AttackPower;

        //溜め攻撃の場合の倍率補正
        if (isChargedAttack)
        {
            float chargeMultiplier = Mathf.Lerp(weaponData.MinChargeDamageMultiplier, weaponData.MaxChargeDamageMultiplier, currentChargeRatio);
            finalDamage *= chargeMultiplier;
            Debug.Log($"<color=orange>[Hit Info] 溜め率: {currentChargeRatio * 100:F0}% | 倍率: {chargeMultiplier:F2}倍 | 基礎威力を計算: {finalDamage:F1}</color>");

            if (currentChargeRatio >= 1.0f)
            {
                Debug.Log("<color=orange>【最大溜め攻撃！】</color>");
            }
        }

        // クリティカル判定
        if (Random.value < weaponData.CriticalRate)
        {
            isCritical = true;
            finalDamage *= weaponData.CriticalDamageMultiplier;
        }
        int resultDamage = Mathf.RoundToInt(finalDamage);

        //デバッグログ: 最終出力ダメージ
        Debug.Log($"<color=green>[Hit Info] 最終ダメージ確定: {resultDamage}</color>");
        return resultDamage;
    }
}