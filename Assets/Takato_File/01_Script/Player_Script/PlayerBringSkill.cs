using UnityEngine;
using Takato;
using System.Collections.Generic;

/// <summary>
/// プレイヤーの獲得したスキルを管理するクラス(シーン上に置いて置くような構造にしてます。)
/// </summary>
namespace Takato
{
    public class PlayerBringSkill : MonoBehaviour
    {
        [Header("所持スキルリスト")]
        [SerializeField] private List<SkillBase> ownedSkills = new List<SkillBase>();

        /// <summary>
        /// 所持スキルリストを取得
        /// </summary>
        public List<SkillBase> GetOwnedSkills()
        {
            return new List<SkillBase>(ownedSkills); // 所持スキルのコピーを返す。
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

            // 所持リストからドラッグしたスキルを取り除く
            ownedSkills.RemoveAt(ownedSkillIndex);

            // スロットに元々入っていたスキルがあれば、インベントリに戻す。
            if (slotSkill != null)
            {
                if (!ownedSkills.Contains(slotSkill))
                {
                    // 取り除いた位置と同じ位置に戻す
                    int insertIndex = Mathf.Clamp(ownedSkillIndex, 0, ownedSkills.Count);
                    ownedSkills.Insert(insertIndex, slotSkill);
                }
            }

            // スロットにドラッグしてきたスキルをセット
            playerSkill.SetSkill(playerSkillSlot, ownedSkill);

            // デバッグログ
            string ownedSkillName = ownedSkill != null ? ownedSkill.skillName : "なし";
            string slotSkillName = slotSkill != null ? slotSkill.skillName : "なし";
            Debug.Log($"インベントリー「{ownedSkillName}」とスロット{playerSkillSlot}「{slotSkillName}」を入れ替えました。");
        }
    }
}
