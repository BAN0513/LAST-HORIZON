using UnityEngine;

/// <summary>
/// プレイヤーの複数スキルを管理するクラス(New)
/// </summary>
public class Player_SkillController_New : MonoBehaviour
{
    [Header("装着するスキルのSO(スロットに対応するキーで発動します。)")]
    [SerializeField] private Skill_SO_New[] activeSkillSlots;

    // 現在移動速度にかかっているスキル倍率（デフォルト1.0倍）
    public float CurrentSpeedMultiplier { get; private set; } = 1.0f;

    private void Start()
    {
        // 各SOのタイマー状態をリセット
        if (activeSkillSlots != null)
        {
            foreach (var skillSO in activeSkillSlots)
            {
                if (skillSO != null)
                {
                    skillSO.Initialize();
                }
            }
        }
    }

    private void Update()
    {
        HandleTimers(); // タイマー更新処理
    }

    /// <summary>
    /// 各スキルのタイマー更新処理
    /// </summary>
    private void HandleTimers()
    {
        if (activeSkillSlots == null) return;

        foreach (var skillSO in activeSkillSlots)
        {
            if (skillSO == null) continue;

            // 各SOのタイマーを更新
            skillSO.UpdateTimers(Time.deltaTime, () =>
            {
                // 効果終了時のバフ解除処理
                if (skillSO.SkillType == SkillType.SpeedBoost)
                {
                    CurrentSpeedMultiplier = 1.0f; // 移動速度倍率を元に戻す
                }
            });
        }
    }

    /// <summary>
    /// スロットインデックス（0番目、1番目...）のスキルのSOを発動する
    /// </summary>
    /// <param name="slotIndex">発動したいスキルスロットの番号</param>
    public void UseSkill(int slotIndex = 0)
    {
        if (activeSkillSlots == null || slotIndex < 0 || slotIndex >= activeSkillSlots.Length)
        {
            Debug.LogWarning($"[Skill] 指定されたスキルスロットが無効です (Index: {slotIndex})");
            return;
        }

        Skill_SO_New skillSO = activeSkillSlots[slotIndex];

        if (skillSO == null)
        {
            Debug.LogWarning($"[Skill] スロット {slotIndex} にスキルSOがセットされていません。");
            return;
        }

        // SO側で発動可能かチェックして発動
        if (skillSO.TryUseSkill())
        {
            // スキルの種類に応じた効果を反映
            switch (skillSO.SkillType)
            {
                case SkillType.SpeedBoost:
                    CurrentSpeedMultiplier = skillSO.SpeedMultiplier;
                    break;

                case SkillType.None:
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 指定スロットに入っているスキルのクールタイム割合を取得する
    /// </summary>
    public float GetCooldownRatio(int slotIndex)
    {
        if (activeSkillSlots != null && slotIndex >= 0 && slotIndex < activeSkillSlots.Length && activeSkillSlots[slotIndex] != null)
        {
            return activeSkillSlots[slotIndex].CooldownRemainingRatio;
        }
        return 0f;
    }
}