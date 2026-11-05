using System.Collections;
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

    // 現在のスキルによる攻撃力加算値（デフォルト0）
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
                    skillSO.Initialize(); // スキルSOの初期化
                }
            }
        }
    }

    private void Update()
    {
        HandleTimers(); // スキルのクールタイムや効果時間の更新
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
                else if (skillSO.SkillType == SkillType.AttackBoost)
                {
                    CurrentAttackBonus = 0;
                }
            });
        }
    }

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

                case SkillType.AttackBoost:
                    CurrentAttackBonus = skillSO.AttackBonus;
                    break;

                // ファンネル攻撃の発動
                case SkillType.FunnelAttack:
                    StartCoroutine(ExecuteFunnelRoutine(skillSO));
                    break;

                case SkillType.None:
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// ファンネルの生成と射出を行うコルーチン
    /// </summary>
    private IEnumerator ExecuteFunnelRoutine(Skill_SO_New skillSO)
    {
        if (skillSO.FunnelPrefab == null) yield break;

        // 最も近い敵を検索
        Transform nearestEnemy = FindNearestEnemy();

        int count = skillSO.FunnelCount;
        Funnel_Bullet[] funnels = new Funnel_Bullet[count];

        for (int i = 0; i < count; i++)
        {
            float angle = i * (360f / count);
            // プレイヤー中心からの相対オフセット位置を計算
            Vector3 relativeOffset = Vector3.up * skillSO.SpawnHeight + Quaternion.Euler(0, angle, 0) * Vector3.forward * skillSO.SpawnRadius;
            Vector3 spawnPos = transform.position + relativeOffset;

            GameObject obj = Instantiate(skillSO.FunnelPrefab, spawnPos, Quaternion.identity);
            Funnel_Bullet bullet = obj.GetComponent<Funnel_Bullet>();
            if (bullet == null)
            {
                bullet = obj.AddComponent<Funnel_Bullet>();
            }

            //追従用に transform (プレイヤー) と relativeOffset を渡す
            bullet.Initialize(transform, relativeOffset, nearestEnemy, skillSO.FunnelSpeed, skillSO.FunnelDamage);
            funnels[i] = bullet;

            // 生成間隔（spawnInterval）を適用
            if (skillSO.SpawnInterval > 0f)
            {
                yield return new WaitForSeconds(skillSO.SpawnInterval);
            }
        }

        // 生成完了後の発射遅延を適用
        if (skillSO.LaunchDelay > 0f)
        {
            yield return new WaitForSeconds(skillSO.LaunchDelay);
        }

        // 順次敵へ射出
        for (int i = 0; i < count; i++)
        {
            if (funnels[i] != null)
            {
                funnels[i].Launch();
            }

            // 連射時のテンポ感も生成間隔を基準に少し挟む
            if (skillSO.SpawnInterval > 0f)
            {
                yield return new WaitForSeconds(skillSO.SpawnInterval);
            }
        }
    }

    /// <summary>
    /// 一番近い敵を探すメソッド
    /// </summary>
    private Transform FindNearestEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Transform nearest = null;
        float minDistance = float.MaxValue;

        foreach (var enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = enemy.transform;
            }
        }

        return nearest;
    }

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

    public float GetCooldownRatio(int slotIndex)
    {
        if (activeSkillSlots != null && slotIndex >= 0 && slotIndex < activeSkillSlots.Length && activeSkillSlots[slotIndex] != null)
        {
            return activeSkillSlots[slotIndex].CooldownRemainingRatio;
        }
        return 0f;
    }
}