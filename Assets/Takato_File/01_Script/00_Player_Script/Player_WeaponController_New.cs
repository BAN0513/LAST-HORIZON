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
    private Player_SkillController_New playerSkillController; //スキルコントローラーの参照
    private List<Enemy> hitEnemies = new List<Enemy>(); // 攻撃判定中にヒットした敵を管理するリスト
    private float currentChargeRatio = 0f;
    private bool isChargedAttack = false;

    private void Awake()
    {
        //コンポーネントの取得
        playerSkillController = GetComponent<Player_SkillController_New>();
        if (playerSkillController == null)
        {
            playerSkillController = GetComponentInParent<Player_SkillController_New>();
        }
    }

    private void Start()
    {
        // 初期装備があれば生成
        if (weaponData != null)
        {
            EquipWeapon(weaponData);
        }
    }

    public Weapon_SO_New CurrentWeaponData => weaponData;

    public void EquipWeapon(Weapon_SO_New newWeaponSO)
    {
        weaponData = newWeaponSO;

        if (weaponHolderpoint == null) return;

        foreach (Transform child in weaponHolderpoint)
        {
            Destroy(child.gameObject);
        }

        if (weaponData != null && weaponData.WeaponPrefab != null)
        {
            GameObject weaponInstance = Instantiate(weaponData.WeaponPrefab, weaponHolderpoint.position, weaponHolderpoint.rotation, weaponHolderpoint);

            weaponCollider = weaponInstance.GetComponentInChildren<Collider>();

            if (weaponCollider != null)
            {
                weaponCollider.isTrigger = true;
                weaponCollider.enabled = false;

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

    public void SetChargeRatio(float ratio)
    {
        if (weaponData == null) return;
        isChargedAttack = true;
        currentChargeRatio = Mathf.Clamp01(ratio);
    }

    public void EnableAttackCollider()
    {
        hitEnemies.Clear();
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }
    }

    public void DisableAttackCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }

        hitEnemies.Clear();

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
            playerAnimation.OnAttackHitStart += EnableAttackCollider;
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

    public void OnWeaponTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.GetComponent<Enemy>() != null)
        {
            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy != null && !hitEnemies.Contains(enemy))
            {
                hitEnemies.Add(enemy);

                int finalDamage = CalculateDamage(out bool isCritical);

                enemy.TakeDamage(finalDamage);

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

        if (weaponData == null) return 10;

        float finalDamage = weaponData.AttackPower;

        //スキルによる攻撃力補正を取得
        int skillAttackBonus = (playerSkillController != null) ? playerSkillController.CurrentAttackBonus : 0;
        finalDamage += skillAttackBonus;

        // 溜め攻撃の場合の倍率補正
        if (isChargedAttack)
        {
            float chargeMultiplier = Mathf.Lerp(weaponData.MinChargeDamageMultiplier, weaponData.MaxChargeDamageMultiplier, currentChargeRatio);
            finalDamage *= chargeMultiplier;
            Debug.Log($"<color=orange>[Hit Info] 溜め率: {currentChargeRatio * 100:F0}% | 溜め倍率: {chargeMultiplier:F2}倍 | スキル加算値: +{skillAttackBonus} | 計算値: {finalDamage:F1}</color>");

            if (currentChargeRatio >= 1.0f)
            {
                Debug.Log("<color=orange>【最大溜め攻撃！】</color>");
            }
        }
        else if (skillAttackBonus > 0)
        {
            Debug.Log($"<color=red>[Hit Info] 攻撃バフ適用中！ 加算値: +{skillAttackBonus} | 基礎計算値: {finalDamage:F1}</color>");
        }

        // クリティカル判定
        if (Random.value < weaponData.CriticalRate)
        {
            isCritical = true;
            finalDamage *= weaponData.CriticalDamageMultiplier;
        }

        int resultDamage = Mathf.RoundToInt(finalDamage);

        Debug.Log($"<color=green>[Hit Info] 最終ダメージ確定: {resultDamage}</color>");
        return resultDamage;
    }
}