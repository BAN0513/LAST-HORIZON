using UnityEngine;

/// <summary>
/// 武器のScriptableObjectクラス
/// </summary>
[CreateAssetMenu(fileName = "Weapon_SO", menuName = "ScriptableObjects/Weapon_SO_New", order = 2)]
public class Weapon_SO_New : ScriptableObject
{
    [Header("武器の基本情報")]
    [Space(10)]

    [Header("武器の名前")]
    [SerializeField] private string weaponName;
    [Header("武器のPrefab")]
    [SerializeField] private GameObject weaponPrefab;
    [Header("武器の攻撃力")]
    [SerializeField] private int attackPower;

    public string WeaponName => weaponName; // 武器の名前
    public GameObject WeaponPrefab => weaponPrefab; // 武器のPrefab
    public int AttackPower => attackPower;  // 武器の攻撃力
}