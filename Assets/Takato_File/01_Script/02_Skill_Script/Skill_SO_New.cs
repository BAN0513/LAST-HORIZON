using UnityEngine;

/// <summary>
/// スキルの種類を表す列挙型
/// </summary>
public enum SkillType
{
    None,
    SpeedBoost,  // 移動速度上昇スキル
    Teleport,    // 瞬間移動スキル
    AttackBoost, // 攻撃力上昇スキル
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

    [Header("瞬間移動設定（Teleport用）")]
    [Header("瞬間移動距離（メートル）")]
    [SerializeField] private float teleportDistance;

    [Header("攻撃力上昇設定（AttackBoost用）")]
    [Tooltip("加算する攻撃力")]
    [SerializeField] private int attackBonus;

    // --- 実行時のタイマー管理 ---
    private float currentCooldownTimer = 0f;
    private float currentDurationTimer = 0f;
    private bool isSkillActive = false;

    // プロパティ
    public string SkillName => skillName;
    public SkillType SkillType => skillType;
    public float Duration => duration;
    public float Cooldown => cooldown;
    public float SpeedMultiplier => speedMultiplier;
    public float TeleportDistance => teleportDistance;
    public int AttackBonus => attackBonus;

    // スキルの状態を外部から参照するプロパティ
    public bool IsSkillActive => isSkillActive;
    public bool IsOnCooldown => currentCooldownTimer > 0f;

    // クールタイム残り割合
    public float CooldownRemainingRatio => cooldown > 0f ? Mathf.Clamp01(currentCooldownTimer / cooldown) : 0f;

    /// <summary>
    /// ゲーム開始時にタイマーを初期化する
    /// </summary>
    public void Initialize()
    {
        currentCooldownTimer = 0f;
        currentDurationTimer = 0f;
        isSkillActive = false;
    }

    /// <summary>
    /// タイマー更新処理
    /// </summary>
    public void UpdateTimers(float deltaTime, System.Action onSkillEnd)
    {
        // スキル効果中の処理
        if (isSkillActive)
        {
            currentDurationTimer -= deltaTime;

            if (currentDurationTimer <= 0f)
            {
                isSkillActive = false;
                currentCooldownTimer = cooldown;
                onSkillEnd?.Invoke();
                Debug.Log($"<color=white>[Skill Ended] {skillName} の効果が終了しました。クールタイム開始: {cooldown}秒</color>");
            }
        }
        else if (currentCooldownTimer > 0f)
        {
            currentCooldownTimer -= deltaTime;
        }
    }

    /// <summary>
    /// スキル発動チェックとタイマーセット
    /// </summary>
    public bool TryUseSkill()
    {
        if (IsOnCooldown || isSkillActive)
        {
            Debug.Log($"[Skill] {skillName} は使用不可 (クールタイム中: {currentCooldownTimer:F1}秒)");
            return false;
        }

        if (skillType == SkillType.Teleport)
        {
            currentCooldownTimer = cooldown;
            Debug.Log($"<color=green>[Skill Activated] {skillName} 発動！ (瞬間移動 {teleportDistance}m | CD: {cooldown}秒)</color>");
            return true;
        }

        isSkillActive = true;
        currentDurationTimer = duration;

        Debug.Log($"<color=green>[Skill Activated] {skillName} 発動！ (効果時間: {duration}秒 | 効果終了後にCD: {cooldown}秒)</color>");
        return true;
    }
}