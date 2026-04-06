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
    [SerializeField] private Transform slotParent; // スロットを並べる親
    [SerializeField] private GameObject slotPrefab; // スロット用Prefab
    [SerializeField] private Sprite emptySlotSprite; // 空スロット用画像

    private List<Button> skillSlotButtons = new List<Button>();
    private List<Image> skillSlotImages = new List<Image>();
    private SkillBase selectedSkill;

    void Start()
    {
        GenerateSkillSlots();
        RefreshOwnedSkills();
        RefreshSkillSlots();
    }

    // スロットを所持スキル数分生成
    void GenerateSkillSlots()
    {
        // 既存のスロットを削除
        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

        skillSlotButtons.Clear();
        skillSlotImages.Clear();

        int slotCount = Mathf.Min(skillSlotButtons.Count, skillSlotImages.Count, playerSkill.SkillSlotCount);
        for (int i = 0; i < slotCount; i++)
        {
            var go = Instantiate(slotPrefab, slotParent);
            var btn = go.GetComponent<Button>();
            var img = go.GetComponent<Image>();
            int slot = i;
            btn.onClick.AddListener(() => OnSkillSlotClicked(slot));
            skillSlotButtons.Add(btn);
            skillSlotImages.Add(img);
        }
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
        int slotCount = Mathf.Min(skillSlotButtons.Count, skillSlotImages.Count, playerSkill.SkillSlotCount);
        for (int i = 0; i < slotCount; i++)
        {
            var skill = playerSkill.GetSkill(i);
            if (skill != null)
            {
                // skillSlotImages[i].sprite = skill.icon;
                skillSlotImages[i].color = Color.white;
            }
            else
            {
                skillSlotImages[i].sprite = emptySlotSprite;
                skillSlotImages[i].color = Color.gray;
            }
        }
    }
}
