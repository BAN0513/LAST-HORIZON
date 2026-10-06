using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// 武器のScriptableObjectクラス
/// </summary>
[CreateAssetMenu(fileName = "Weapon_SO", menuName = "ScriptableObjects/Weapon_SO_New", order = 2)]
public class Weapon_SO_New : ScriptableObject
{
    [Header("武器の基本情報")]
    [Space(10)]

    [Header("武器の基礎ステータス値")]
    [Space(10)]
    [Header("武器の名前")]
    [SerializeField] private string weaponName;
    [Header("武器のPrefab")]
    [SerializeField] private GameObject weaponPrefab;
    [Header("武器の攻撃力")]
    [SerializeField] private int attackPower;

    [Header("武器の倍率値")]
    [Space(10)]
    [Header("武器のクリティカル率")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float criticalRate;
    [Header("クリティカル時のダメージ倍率")]
    [Range(1.0f, 3.0f)]
    [SerializeField] private float criticalDamageMultiplier;

    [Header("武器の最小溜めのダメージ倍率")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float minChargeDamageMultiplier;
    [Header("武器の最大溜めのダメージ倍率")]
    [Range(0.0f, 3.0f)]
    [SerializeField] private float maxChargeDamageMultiplier;
    [Header("武器の最大溜めにかかる時間")]
    [SerializeField] private float maxChargeTime;


    public string WeaponName => weaponName; // 武器の名前
    public GameObject WeaponPrefab => weaponPrefab; // 武器のPrefab
    public int AttackPower => attackPower;  // 武器の攻撃力
    public float CriticalRate => criticalRate; // 武器のクリティカル率
    public float CriticalDamageMultiplier => criticalDamageMultiplier; // クリティカル時のダメージ倍率

    public float MinChargeDamageMultiplier => minChargeDamageMultiplier; // 武器の最小溜めのダメージ倍率
    public float MaxChargeDamageMultiplier => maxChargeDamageMultiplier; // 武器の最大溜めのダメージ倍率
    public float MaxChargeTime => maxChargeTime; // 武器の最大溜めにかかる時間

}