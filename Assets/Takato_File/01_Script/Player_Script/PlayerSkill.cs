using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// プレイヤーのスキルを管理するクラス
/// </summary>
namespace Takato
{
    public class PlayerSkill : MonoBehaviour
    {
        [Header("所持スキル（ScriptableObject）")]
        [SerializeField] private List<SkillBase> skills;

        private float[] skillCooldownTimers; // 各スキルのクールダウンタイマー

        private void Awake()
        {
            skillCooldownTimers = new float[skills.Count];
        }

        private void Update()
        {
            for (int i = 0; i < skillCooldownTimers.Length; i++)
            {
                if (skillCooldownTimers[i] > 0)
                    skillCooldownTimers[i] -= Time.deltaTime;
            }
        }

        /// <summary>
        /// スキルを発動するメソッド
        /// </summary>
        public void ActivateSkill(int index, PlayerController player)
        {
            if (index < 0 || index >= skills.Count) return;
            if (skillCooldownTimers[index] > 0) return;

            skills[index].Activate(player);
            skillCooldownTimers[index] = skills[index].cooldown;
        }

        /// <summary>
        /// スキルスロットの数を取得するプロパティ
        /// </summary>
        public int SkillSlotCount
        {
            get { return skills.Count; }
        }

        /// <summary>
        /// スキルをセットするメソッド
        /// </summary>
        public void SetSkill(int slotindex, SkillBase skill)
        {
            if (slotindex < 0 || slotindex >= skills.Count) return;
            skills[slotindex] = skill;
        }
        /// <summary>
        /// スキルを取得するメソッド
        /// </summary>
        public SkillBase GetSkill(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= skills.Count) return null;
            return skills[slotIndex];
        }

    }
}
