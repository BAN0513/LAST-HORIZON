using UnityEngine;

/// <summary>
/// プレイヤーのスキルを管理するクラス
/// </summary>
public class PlayerSkill : MonoBehaviour
{
    [Header("スキルのステータス")]
    [Space(10)]
    [Header("スキルのクールタイム")]
    [SerializeField] private float skillCooldown;
    [Header("攻撃スキルの上昇倍率")]
    [SerializeField] private float attackBuffMultiplier;
    [Header("防御スキルのダメージカット率")]
    [SerializeField] private float defenseBuffCutRate;
    [Header("スキルの持続時間")]
    [SerializeField] private float skillDuration;

    private float skillTimer; // スキルのクールタイム管理
    private bool isSkillActive; // スキルが発動中かどうか

    private PlayerWeaponController weaponController;
    private Takato.PlayerController playerController;

    private float originalAttackDamage;
    private float originalDamageCutRate;

    private void Start()
    {
        skillTimer = 0f; // 初期状態ではスキルは使用可能
        isSkillActive = false;
        weaponController = GetComponentInChildren<PlayerWeaponController>();
        playerController = GetComponent<Takato.PlayerController>();
    }

    private void Update()
    {
        // スキルのクールタイムを管理
        if (skillTimer > 0f)
        {
            skillTimer -= Time.deltaTime;
        }

        // スキルの効果時間を管理
        if (isSkillActive)
        {
            skillDuration -= Time.deltaTime;
            if (skillDuration <= 0f)
            {
                EndSkill();
            }
        }
    }

    /// <summary>
    /// 攻撃力アップスキル発動
    /// </summary>
    public void ActivateAttackBuff()
    {
        if (skillTimer > 0f || isSkillActive || weaponController == null) return;

        var weapon = GetEquippedWeapon();
        if (weapon == null) return;

        // 元の攻撃力を保存
        originalAttackDamage = weapon.AttackDamage;
        // 攻撃力を上昇
        weapon.SetAttackDamage(originalAttackDamage * attackBuffMultiplier);

        isSkillActive = true;
        skillDuration = Mathf.Max(skillDuration, 0.1f);
        skillTimer = skillCooldown;
    }

    /// <summary>
    /// 防御力アップスキル発動
    /// </summary>
    public void ActivateDefenseBuff()
    {
        if (skillTimer > 0f || isSkillActive || playerController == null) return;

        // 元のダメージカット率を保存
        originalDamageCutRate = playerController.GetDamageCutRate();
        // ダメージカット率を上昇
        playerController.SetDamageCutRate(originalDamageCutRate + defenseBuffCutRate);

        isSkillActive = true;
        skillDuration = Mathf.Max(skillDuration, 0.1f);
        skillTimer = skillCooldown;
    }

    /// <summary>
    /// スキル効果終了
    /// </summary>
    private void EndSkill()
    {
        var weapon = GetEquippedWeapon();
        if (weapon != null && originalAttackDamage > 0)
        {
            weapon.SetAttackDamage(originalAttackDamage);
        }
        if (playerController != null && originalDamageCutRate >= 0)
        {
            playerController.SetDamageCutRate(originalDamageCutRate);
        }
        isSkillActive = false;
        originalAttackDamage = 0;
        originalDamageCutRate = -1;
    }

    private Weapon GetEquippedWeapon()
    {
        return weaponController != null ? typeof(PlayerWeaponController)
            .GetField("equippedWeapon", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(weaponController) as Weapon : null;
    }
}
