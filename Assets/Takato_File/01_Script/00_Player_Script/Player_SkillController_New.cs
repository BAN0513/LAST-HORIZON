using UnityEngine;

/// <summary>
/// プレイヤーのスキルを管理するクラス(New)
/// </summary>
public class Player_SkillController_New : MonoBehaviour
{
    [Header("装着するスキル")]
    [SerializeField] private Skill_SO_New activeSkill;

    // タイマー管理
    private float currentCooldownTimer = 0f;
    private float currentDurationTimer = 0f;
    private bool isSkillActive = false;

    // 現在移動速度にかかっているスキル倍率
    public float CurrentSpeedMultiplier { get; private set; }= 1.0f; // デフォルトは1.0倍

    // 外部からクールタイム情報や発動状態を取得するプロパティ
    public bool IsSkillActive => isSkillActive;// スキルが発動中かどうか

    public bool IsOnCooldown => currentCooldownTimer > 0f; // スキルがクールタイム中かどうか

    // クールタイムの残り時間の割合を取得するプロパティ
    public float CooldownRemainingRatio => (activeSkill != null && activeSkill.Cooldown > 0f) ?
        Mathf.Clamp01(currentCooldownTimer / activeSkill.Cooldown) : 0f;

    private void Update()
    {
        HandleTimers(); // タイマー更新処理
    }

    /// <summary>
    /// タイマー更新処理
    /// </summary>
    private void HandleTimers()
    {
        // クールタイム計算
        if (currentCooldownTimer > 0f)
        {
            currentCooldownTimer -= Time.deltaTime;
        }

        // 効果時間計算
        if (isSkillActive)
        {
            currentDurationTimer -= Time.deltaTime;
            if (currentDurationTimer <= 0f)
            {
                EndSkill(); // スキル効果終了処理を呼び出す
            }
        }
    }

    /// <summary>
    /// スキルを発動するメソッド（キー入力などから呼び出す）
    /// </summary>
    public void UseSkill()
    {
        if (activeSkill == null)
        {
            Debug.LogWarning("[Skill] スキルがセットされていません。");
            return;
        }

        if (IsOnCooldown || isSkillActive)
        {
            Debug.Log($"[Skill] スキル使用不可 (クールタイム中: {currentCooldownTimer:F1}秒)");
            return;
        }

        // スキルの種類に応じた発動処理
        switch (activeSkill.SkillType)
        {
            //スキルタイプがSpeedBoostの場合の処理
            case SkillType.SpeedBoost:
                ActivateSpeedBoost();
                break;

            //スキルがNoneまたはその他のタイプの場合の処理
            case SkillType.None:
            default:
                break;
        }
    }

    /// <summary>
    /// スピードブーストの発動処理
    /// </summary>
    private void ActivateSpeedBoost()
    {
        // スキル発動中フラグを立て、タイマーをリセット
        isSkillActive = true;

        // 効果時間とクールタイムを設定
        currentDurationTimer = activeSkill.Duration;
        currentCooldownTimer = activeSkill.Cooldown;

        CurrentSpeedMultiplier = activeSkill.SpeedMultiplier;

        Debug.Log($"<color=green>[Skill Activated] {activeSkill.SkillName} 発動！ 速度倍率: {CurrentSpeedMultiplier}倍 (効果時間: {activeSkill.Duration}秒)</color>");
    }

    /// <summary>
    /// スキル効果終了処理
    /// </summary>
    private void EndSkill()
    {
        isSkillActive = false;
        CurrentSpeedMultiplier = 1.0f; // 倍率を戻す

        Debug.Log($"<color=white>[Skill Ended] {activeSkill.SkillName} の効果が終了しました。</color>");
    }
}