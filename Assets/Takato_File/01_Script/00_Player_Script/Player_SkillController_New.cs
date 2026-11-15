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

    [Header("ファンネルのアーチ広がり角度")]
    [Range(30f, 180f)]
    [SerializeField] private float funnelArcAngle;

    // キャラクターコントローラーの参照
    private CharacterController characterController;

    // 現在移動速度にかかっているスキル倍率
    public float CurrentSpeedMultiplier { get; private set; } = 1.0f;

    // 現在のスキルによる攻撃力加算値
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

    /// <summary> /// 指定されたスロットのスキルを使用するメソッド /// </summary>
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

    /// <summary>/// ファンネルの生成と射出を行うコルーチン/// </summary>
    private IEnumerator ExecuteFunnelRoutine(Skill_SO_New skillSO)
    {
        if (skillSO.FunnelPrefab == null) yield break;

        // 最も近い敵を検索
        Transform nearestEnemy = FindNearestEnemy();

        int count = skillSO.FunnelCount;
        Funnel_Bullet[] funnels = new Funnel_Bullet[count];
        float halfAngle = funnelArcAngle * 0.5f;

        for (int i = 0; i < count; i++)
        {
            // 生成位置の角度を計算
            float t = (count > 1) ? (float)i / (count - 1) : 0.5f;
            float currentAngle = Mathf.Lerp(-halfAngle, halfAngle, t);
            float rad = currentAngle * Mathf.Deg2Rad;

            // 中央が一番高く両端に行くほど外側に広がりつつ少し下がるアーチ座標
            float offsetX = Mathf.Sin(rad) * skillSO.SpawnRadius;
            float offsetY = skillSO.SpawnHeight - (1f - Mathf.Cos(rad)) * skillSO.SpawnRadius;

            Vector3 localOffset = new Vector3(offsetX, offsetY, 0f);
            Vector3 spawnPos = transform.position + transform.rotation * localOffset;

            GameObject obj = Instantiate(skillSO.FunnelPrefab, spawnPos, transform.rotation);
            Funnel_Bullet bullet = obj.GetComponent<Funnel_Bullet>();
            if (bullet == null)
            {
                bullet = obj.AddComponent<Funnel_Bullet>();
            }

            bullet.Initialize(transform, localOffset, nearestEnemy, skillSO.FunnelSpeed, skillSO.FunnelDamage);
            funnels[i] = bullet;

            // 生成間隔を適用
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

    /// <summary>/// 一番近い敵を探すメソッド/// </summary>
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