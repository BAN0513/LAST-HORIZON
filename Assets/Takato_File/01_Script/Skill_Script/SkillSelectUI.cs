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

    [Header("Ui Canvas")]
    [SerializeField] private Canvas skillSelectCanvas;

    private List<Button> skillSlotButtons = new List<Button>(); //スロットのButtonコンポーネント（クリック判定用）
    private List<Image> skillSlotImages = new List<Image>(); //スロットのImageコンポーネント（アイコン表示用）
    private SkillBase selectedSkill;    //選択中のスキル（UI上でハイライト等したい場合用）
    private int selectedSlotIndex = 0; //選択中のスロット番号

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

        skillSlotButtons.Clear();
        skillSlotImages.Clear();

        int slotCount = playerSkill.SkillSlotCount;
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

        // ここで必ず bringSkill.GetOwnedSkills() の要素をそのまま渡す
        foreach (var skill in bringSkill.GetOwnedSkills())
        {
            var go = Instantiate(skillIconPrefab, ownedSkillsPanel);
            var iconUI = go.GetComponent<SkillIconUI>();
            iconUI.Setup(skill, OnSkillIconSelected);
        }
    }


    /// <summary>
    /// スキルアイコンが選択されたときの処理
    /// </summary>
    void OnSkillIconSelected(SkillBase skill)
    {
        if (selectedSlotIndex >= 0 && selectedSlotIndex < playerSkill.SkillSlotCount)
        {
            int ownedIndex = bringSkill.GetOwnedSkills().IndexOf(skill);
            if (ownedIndex < 0)
            {
                Debug.LogWarning($"選択されたSkillBase({skill?.skillName})はownedSkillsリストに存在しません。参照が一致していない可能性があります。");
                // ここで bringSkill.GetOwnedSkills() の全要素と skill を比較してみる
                int i = 0;
                foreach (var s in bringSkill.GetOwnedSkills())
                {
                    Debug.Log($"ownedSkills[{i}]: {s?.skillName} ({s?.GetInstanceID()}) 選択: {skill?.skillName} ({skill?.GetInstanceID()}) 一致: {ReferenceEquals(s, skill)}");
                    i++;
                }
                return;
            }
            bringSkill.SwapSkillWithPlayerSkill(ownedIndex, selectedSlotIndex, playerSkill);
            RefreshOwnedSkills();
            RefreshSkillSlots();
        }
        selectedSkill = skill;
    }

    // スロットが押された時
    void OnSkillSlotClicked(int slotIndex)
    {
        selectedSlotIndex = slotIndex; // 選択中スロットを記憶
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
    /// 所持スキルとプレイヤースキルのスロットを入れ替えるメソッド
    /// </summary>
    public void SwapSkillWithPlayerSkill(int ownedSkillIndex, int playerSkillSlot, PlayerSkill playerSkill)
    {
       
    }
}
