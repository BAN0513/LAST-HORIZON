using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Takato;

/// <summary>
/// スキル選択UI全体制御
/// </summary>
public class SkillSelectUI : MonoBehaviour
{
    [Header("持っているスキルUI")]
    [SerializeField] private PlayerBringSkill bringSkill;
    [SerializeField] private PlayerSkill playerSkill;
    [SerializeField] private Transform ownedSkillsPanel;
    [SerializeField] private GameObject skillIconPrefab;

    [Header("装備スロットUI")]
    [SerializeField] private Transform slotParent;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Sprite emptySlotSprite;

    [Header("Ui Canvas")]
    [SerializeField] private Canvas skillSelectCanvas;

    private List<Image> skillSlotImages = new List<Image>();

    void Start()
    {
        GenerateSkillSlots(); // スロット生成
        RefreshOwnedSkills(); // 所持スキル表示
        RefreshSkillSlots();  // スロット表示更新
        ShowUI(false);        // 最初は非表示
    }

    /// <summary>
    /// UI表示切替
    /// </summary>
    public void ShowUI(bool show)
    {
        if (skillSelectCanvas != null)
        {
            skillSelectCanvas.enabled = show; // Canvasの有効/無効でUI全体の表示を切り替え
        }
        else
        {
            gameObject.SetActive(show); // Canvasがない場合はGameObjectのActiveで切り替え
        }
    }

    // スロットを生成
    void GenerateSkillSlots()
    {
        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

        skillSlotImages.Clear(); // 既存のスロットイメージリストをクリア

        int slotCount = playerSkill.SkillSlotCount;
        for (int i = 0; i < slotCount; i++)
        {
            var go = Instantiate(slotPrefab, slotParent);
            var img = go.GetComponent<Image>();
            skillSlotImages.Add(img);

            // ドロップハンドラ設定
            var dropHandler = go.GetComponent<SkillSlotDropHandler>();
            if (dropHandler == null)
                dropHandler = go.AddComponent<SkillSlotDropHandler>();
            dropHandler.slotIndex = i;
            dropHandler.skillSelectUI = this;
        }
    }

    // 所持スキル一覧を表示
    void RefreshOwnedSkills()
    {
        foreach (Transform child in ownedSkillsPanel)
            Destroy(child.gameObject);

        foreach (var skill in bringSkill.GetOwnedSkills())
        {
            var go = Instantiate(skillIconPrefab, ownedSkillsPanel);
            var iconUI = go.GetComponent<SkillIconUI>();
            iconUI.Setup(skill); // 表示セット
        }
    }

    // 装備スロットの表示更新
    void RefreshSkillSlots()
    {
        int slotCount = playerSkill.SkillSlotCount;
        for (int i = 0; i < slotCount; i++)
        {
            var skill = playerSkill.GetSkill(i);
            if (skill != null)
            {
                skillSlotImages[i].sprite = skill.skillIcon;
                skillSlotImages[i].color = skill.skillIcon != null ? Color.white : Color.clear;
            }
            else
            {
                skillSlotImages[i].sprite = emptySlotSprite;
                skillSlotImages[i].color = Color.gray;
            }
        }
    }

    /// <summary>
    /// ドラッグでスキルがスロットにドロップされた時の処理
    /// </summary>
    public void OnSkillIconDropped(SkillBase skill, int slotIndex)
    {
        int ownedIndex = bringSkill.GetOwnedSkills().IndexOf(skill);
        if (ownedIndex < 0) return;
        bringSkill.SwapSkillWithPlayerSkill(ownedIndex, slotIndex, playerSkill);
        RefreshOwnedSkills(); // 所持スキルUI更新
        RefreshSkillSlots();  // スロットUI更新
    }
}
