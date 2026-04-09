using UnityEngine;
using Takato;
using System.Collections.Generic;

/// <summary>
/// プレイヤーの獲得したスキルを管理するクラス
/// </summary>
namespace Takato
{
    public class PlayerBringSkill : MonoBehaviour
    {
        [Header("所持スキルリスト")]
        [SerializeField] private List<SkillBase> ownedSkills = new List<SkillBase>();

        /// <summary>
        /// スキルを所持リストに追加
        /// </summary>
        public void AddSkill(SkillBase skill)
        {
            if (!ownedSkills.Contains(skill))
            {
                ownedSkills.Add(skill);
            }
        }

        /// <summary>
        /// スキルを所持リストから削除
        /// </summary>
        public void RemoveSkill(SkillBase skill)
        {
            ownedSkills.Remove(skill);
        }

        /// <summary>
        /// 所持スキルリストを取得
        /// </summary>
        public List<SkillBase> GetOwnedSkills()
        {
            return ownedSkills;
        }

        /// <summary>
        /// 所持スキルとプレイヤースキルのスロットを入れ替えるメソッド
        /// </summary>
        public void SwapSkillWithPlayerSkill(int ownedSkillIndex, int playerSkillSlot, PlayerSkill playerSkill)
        {
            if (ownedSkillIndex < 0 || ownedSkillIndex >= ownedSkills.Count) return;
            if (playerSkill == null) return;
            if (playerSkillSlot < 0 || playerSkillSlot >= playerSkill.SkillSlotCount) return;

            // 所持スキルとスロットスキルを取得
            SkillBase ownedSkill = ownedSkills[ownedSkillIndex];
            SkillBase slotSkill = playerSkill.GetSkill(playerSkillSlot);

            // 入れ替え
            ownedSkills[ownedSkillIndex] = slotSkill;
            playerSkill.SetSkill(playerSkillSlot, ownedSkill);

            // デバッグログ
            string ownedSkillName = ownedSkill != null ? ownedSkill.skillName : "なし";
            string slotSkillName = slotSkill != null ? slotSkill.skillName : "なし";
            Debug.Log($"インベントリー「{ownedSkillName}」とスロット{playerSkillSlot}「{slotSkillName}」を入れ替えました。");
        }
    }
}
