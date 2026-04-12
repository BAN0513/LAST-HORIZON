using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Takato;

/// <summary>
/// スキル選択UI（ドラッグ＆ドロップ専用）
/// </summary>
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

    [Header("Ui Canvas")]
    [SerializeField] private Canvas skillSelectCanvas;

    private List<Image> skillSlotImages = new List<Image>(); //スロットのImageコンポーネント（アイコン表示用）

    void Start()
    {
        GenerateSkillSlots(); //スロットを生成
        RefreshOwnedSkills(); //所持スキルを表示
        RefreshSkillSlots();  //スロットの表示を更新
        ShowUI(false);        //最初は非表示
    }

    /// <summary>
    /// スキル選択UIの表示切替
    /// </summary>
    /// <param name="show"></param>
    public void ShowUI(bool show)
    {
        if (skillSelectCanvas != null)
        {
            skillSelectCanvas.enabled = show;
        }
        else
        {
            gameObject.SetActive(show);
        }
    }

    // スロットを所持スキル数分生成
    void GenerateSkillSlots()
    {
        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

        skillSlotImages.Clear();

        int slotCount = playerSkill.SkillSlotCount;
        for (int i = 0; i < slotCount; i++)
        {
            var go = Instantiate(slotPrefab, slotParent);
            var img = go.GetComponent<Image>();
            skillSlotImages.Add(img);

            // ドロップハンドラ追加
            var dropHandler = go.GetComponent<SkillSlotDropHandler>();
            if (dropHandler == null)
                dropHandler = go.AddComponent<SkillSlotDropHandler>();
            dropHandler.slotIndex = i;
            dropHandler.skillSelectUI = this;
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
            iconUI.Setup(skill); // コールバック不要
        }
    }

    // スロットUIの表示更新
    void RefreshSkillSlots()
    {
        int slotCount = playerSkill.SkillSlotCount;
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

    /// <summary>
    /// ドラッグ＆ドロップでスキルがスロットにドロップされた時の処理
    /// </summary>
    public void OnSkillIconDropped(SkillBase skill, int slotIndex)
    {
        int ownedIndex = bringSkill.GetOwnedSkills().IndexOf(skill);
        if (ownedIndex < 0) return;
        bringSkill.SwapSkillWithPlayerSkill(ownedIndex, slotIndex, playerSkill);
        RefreshOwnedSkills();
        RefreshSkillSlots();
    }
}
