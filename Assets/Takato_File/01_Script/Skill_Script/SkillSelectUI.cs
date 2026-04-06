using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Takato;

public class SkillSelectUI : MonoBehaviour
{
    [Header("所持スキルUI")]
    [SerializeField] private PlayerBringSkill bringSkill;
    [SerializeField] private PlayerSkill playerSkill;
    [SerializeField] private Transform ownedSkillsPanel; // ScrollViewのContent
    [SerializeField] private GameObject skillIconPrefab;

    [Header("装備スロットUI")]
    [SerializeField] private List<Button> skillSlotButtons; // 左側のスロット
    [SerializeField] private List<Image> skillSlotImages;   // スロットに表示するアイコン
    [SerializeField] private Sprite emptySlotSprite;        // 空スロット用画像

    private SkillBase selectedSkill;

    void Start()
    {
        RefreshOwnedSkills();
        for (int i = 0; i < skillSlotButtons.Count; i++)
        {
            int slot = i;
            skillSlotButtons[i].onClick.AddListener(() => OnSkillSlotClicked(slot));
        }
        RefreshSkillSlots();
    }

    // 所持スキル一覧をUIに表示
    void RefreshOwnedSkills()
    {
        foreach (Transform child in ownedSkillsPanel)
            Destroy(child.gameObject);

        foreach (var skill in bringSkill.GetOwnedSkills())
        {
            var go = Instantiate(skillIconPrefab, ownedSkillsPanel);
            var iconUI = go.GetComponent<SkillIconUI>();
            iconUI.Setup(skill, OnSkillIconSelected);
        }
    }

    // スキルアイコンが押された時
    void OnSkillIconSelected(SkillBase skill)
    {
        selectedSkill = skill;
        // ここで選択中のスキルをハイライト表示などしても良い
    }

    // スロットが押された時
    void OnSkillSlotClicked(int slotIndex)
    {
        if (selectedSkill == null) return;
        int ownedIndex = bringSkill.GetOwnedSkills().IndexOf(selectedSkill);
        if (ownedIndex >= 0)
        {
            bringSkill.SetSkillToPlayerSkill(ownedIndex, slotIndex, playerSkill);
            RefreshSkillSlots();
        }
    }

    // スロットUIの表示更新
    void RefreshSkillSlots()
    {
        for (int i = 0; i < skillSlotButtons.Count; i++)
        {
            var skill = playerSkill.GetSkill(i);
            if (skill != null)
            {
                // skillSlotImages[i].sprite = skill.icon; // アイコンを使う場合
                skillSlotImages[i].color = Color.white;
                // スキル名表示が不要なら下記を削除
                // skillSlotImages[i].GetComponentInChildren<Text>().text = skill.skillName;
            }
            else
            {
                skillSlotImages[i].sprite = emptySlotSprite;
                skillSlotImages[i].color = Color.gray;
                // skillSlotImages[i].GetComponentInChildren<Text>().text = "";
            }
        }
    }
}
