using UnityEngine;

/// <summary>
/// スキルの種類を表す列挙型
/// </summary>
public enum SkillType
{
    None,
    SpeedBoost, // 移動速度上昇スキル
}

/// <summary>
/// スキルのScriptableObjectクラス(New)
/// </summary>
[CreateAssetMenu(fileName = "Skill_SO_New", menuName = "ScriptableObjects/Skill_SO_New", order = 3)]
public class Skill_SO_New : ScriptableObject
{
    [Header("基本情報")]
    [Space(10)]

    [Header("スキル名・スキルタイプ設定")]
    [SerializeField] private string skillName;

    [SerializeField] private SkillType skillType;

    [Header("クールタイム・効果時間設定")]

    [Header("スキルの効果時間(秒)")]
    [SerializeField] private float duration;

    [Header("再使用までのクールタイム(秒)")]
    [SerializeField] private float cooldown;

    [Header("バフ効果設定（SpeedBoost用）")]
    [SerializeField] private float speedMultiplier;


    // プロパティ
    public string SkillName => skillName;    // スキル名を取得するプロパティ
    public SkillType SkillType => skillType; // スキルタイプを取得するプロパティ
    public float Duration => duration;       // スキルの効果時間を取得するプロパティ
    public float Cooldown => cooldown;       // スキルのクールタイムを取得するプロパティ
    public float SpeedMultiplier => speedMultiplier; // スピード倍率を取得するプロパティ
}