using UnityEngine;

/// <summary>
/// プレイヤーの複数スキルを管理するクラス(New)
/// </summary>
public class Player_SkillController_New : MonoBehaviour
{
    [Header("装着するスキルのSO(スロットに対応するキーで発動します。)")]
    [SerializeField] private Skill_SO_New[] activeSkillSlots;

    [Header("テレポートの壁判定レイヤー")]
    [SerializeField] private LayerMask obstacleLayer;

    private CharacterController characterController;

    // 現在移動速度にかかっているスキル倍率（デフォルト1.0倍）
    public float CurrentSpeedMultiplier { get; private set; } = 1.0f;

    //現在のスキルによる攻撃力加算値（デフォルト0）
    public int CurrentAttackBonus { get; private set; } = 0;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
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
        HandleTimers();
    }

    private void HandleTimers()
    {
        if (activeSkillSlots == null) return;

        foreach (var skillSO in activeSkillSlots)
        {
            if (skillSO == null) continue;

            skillSO.UpdateTimers(Time.deltaTime, () =>
            {
                if (skillSO.SkillType == SkillType.SpeedBoost)
                {
                    CurrentSpeedMultiplier = 1.0f;
                }
                //攻撃力バフ終了時に加算値を0に戻す
                else if (skillSO.SkillType == SkillType.AttackBoost)
                {
                    CurrentAttackBonus = 0;
                }
            });
        }
    }

    /// <summary>
    /// スキルの使用を試みる
    /// </summary>
    /// <param name="slotIndex">スキルスロットのインデックス</param>
    public void UseSkill(int slotIndex = 0)
    {
        if (activeSkillSlots == null || slotIndex < 0 || slotIndex >= activeSkillSlots.Length)
        {
            Debug.LogWarning($"[Skill] 指定されたスキルスロットが無効です (Index: {slotIndex})");
            return;
        }

        Skill_SO_New skillSO = activeSkillSlots[slotIndex]; // スロットに対応するスキルSOを取得

        if (skillSO == null)
        {
            Debug.LogWarning($"[Skill] スロット {slotIndex} にスキルSOがセットされていません。");
            return;
        }

        // スキルの使用を試みる
        if (skillSO.TryUseSkill())
        {
            switch (skillSO.SkillType)
            {
                case SkillType.SpeedBoost:
                    CurrentSpeedMultiplier = skillSO.SpeedMultiplier;
                    break;

                case SkillType.Teleport:
                    ExecuteTeleport(skillSO.TeleportDistance);
                    break;

                //攻撃力固定加算値を適用
                case SkillType.AttackBoost:
                    CurrentAttackBonus = skillSO.AttackBonus;
                    break;

                case SkillType.None:
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 瞬間移動スキルの実行処理
    /// </summary>
    /// <param name="distance"></param>
    private void ExecuteTeleport(float distance)
    {
        Vector3 targetPosition = transform.position + transform.forward * distance;

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out RaycastHit hit, distance, obstacleLayer))
        {
            targetPosition = hit.point - transform.forward * 0.5f;
            targetPosition.y = transform.position.y;
        }

        if (characterController != null)
        {
            characterController.enabled = false;
            transform.position = targetPosition;
            characterController.enabled = true;
        }
        else
        {
            transform.position = targetPosition;
        }
    }

    /// <summary>
    /// スキルのクールタイム残り割合を取得する
    /// </summary>
    /// <param name="slotIndex">スキルスロットのインデックス</param>
    /// <returns>クールタイム残り割合 (0〜1)</returns>
    public float GetCooldownRatio(int slotIndex)
    {
        if (activeSkillSlots != null && slotIndex >= 0 && slotIndex < activeSkillSlots.Length && activeSkillSlots[slotIndex] != null)
        {
            return activeSkillSlots[slotIndex].CooldownRemainingRatio;
        }
        return 0f;
    }
}