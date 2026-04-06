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
        /// 所持スキルから選択してPlayerSkillにセット
        /// </summary>
        public void SetSkillToPlayerSkill(int ownedSkillIndex, int playerSkillSlot, PlayerSkill playerSkill)
        {
            if (ownedSkillIndex < 0 || ownedSkillIndex >= ownedSkills.Count) return;
            if (playerSkill == null) return;

            SkillBase skill = ownedSkills[ownedSkillIndex];
            playerSkill.SetSkill(playerSkillSlot, skill);
        }
    }
}
