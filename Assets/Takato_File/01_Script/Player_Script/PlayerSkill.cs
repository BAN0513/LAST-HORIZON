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
            // スロット数分の要素がなければnullで埋める
            int slotCount = Mathf.Max(skills.Count, 4); // 最低4スロット確保
            while (skills.Count < slotCount)
            {
                skills.Add(null);
            }
            skillCooldownTimers = new float[skills.Count];
        }

        private void Update()
        {
            for (int i = 0; i < skillCooldownTimers.Length; i++)
            {
                if (skillCooldownTimers[i] > 0)
                {
                    skillCooldownTimers[i] -= Time.deltaTime; // クールダウンタイマーを減少させる
                }     
            }
        }

        /// <summary>
        /// スキルを発動するメソッド
        /// </summary>
        public void ActivateSkill(int index, PlayerController player)
        {
            if (index < 0 || index >= skills.Count) return;
            if (skillCooldownTimers[index] > 0) return;
            if (skills[index] == null) return;

            skills[index].Activate(player);
            skillCooldownTimers[index] = skills[index].cooldown;
        }

        /// <summary>
        /// スキルスロットの数を取得するプロパティ
        /// </summary>
        public int SkillSlotCount
        {
            get { return skills.Count; } // スキルスロットの数を返す
        }

        /// <summary>
        /// スキルをセットするメソッド
        /// </summary>
        public void SetSkill(int slotindex, SkillBase skill)
        {
            if (slotindex < 0 || slotindex >= skills.Count)
            {
                Debug.LogWarning($"SetSkill: slotindex {slotindex} が無効です（skills.Count={skills.Count}）");
                return;
            }
            skills[slotindex] = skill;
            Debug.Log($"SetSkill: スロット{slotindex}に「{(skill != null ? skill.skillName : "なし")}」をセットしました。");
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
