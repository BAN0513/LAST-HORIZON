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
    [Header("プレイヤーのスキル管理クラス")]
    [SerializeField] private PlayerSkill playerSkill;
    [Header("所持スキルの親オブジェクト")]
    [SerializeField] private Transform ownedSkillsPanel;
    [Header("スキルアイコンのプレハブ")]
    [SerializeField] private GameObject skillIconPrefab;

    [Header("装備スロットUI")]
    [Header("スキルスロットの親オブジェクト")]
    [SerializeField] private Transform slotParent;
    [Header("スキルスロットのプレハブ")]
    [SerializeField] private GameObject slotPrefab;

    [Header("Ui Canvas")]
    [SerializeField] private Canvas skillSelectCanvas;

    private List<Image> skillSlotImages = new List<Image>(); // スロットのImageコンポーネントを保持するリスト
    private List<SkillSlotDropHandler> slotHandlers = new List<SkillSlotDropHandler>(); // 各スロットのハンドラ参照

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

        // 表示するタイミングで必ず最新の状態に描画する
        if (show)
        {
            RefreshSkillSlots(); //スロットの表示更新
        }
    }

    // スロットを生成
    void GenerateSkillSlots()
    {
        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

        skillSlotImages.Clear(); // 既存のスロットイメージリストをクリア
        slotHandlers.Clear();

        int slotCount = playerSkill.SkillSlotCount;
        for (int i = 0; i < slotCount; i++)
        {
            var go = Instantiate(slotPrefab, slotParent);
            go.name = $"SkillSlot_{i}";

            // スロットのImageコンポーネントを取得
            var img = go.GetComponent<Image>() ?? go.GetComponentInChildren<Image>(true);

            //nullチェックと警告ログ
            if (img == null)
            {
                Debug.LogWarning($"Slot prefab に Image コンポーネントが見つかりません: {go.name}");
                skillSlotImages.Add(null);
            }
            else
            {
                skillSlotImages.Add(img);
            }

            // ドロップハンドラ設定
            var dropHandler = go.GetComponent<SkillSlotDropHandler>();
            if (dropHandler == null)
                dropHandler = go.AddComponent<SkillSlotDropHandler>();
            dropHandler.slotIndex = i;
            dropHandler.skillSelectUI = this;

            // ハンドラにも同じ Image を渡して参照を揃える
            if (img != null)
            {
                dropHandler.SetCachedImage(img);
            }

            slotHandlers.Add(dropHandler);

            // ここで初期表示をセット
            var initialSkill = (playerSkill != null) ? playerSkill.GetSkill(i) : null;
            SetSlotSpriteImmediate(i, initialSkill);
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
            if (iconUI != null)
            {
                iconUI.Setup(skill); // 表示セット
                iconUI.SetSkillSelectUI(this); // ドロップ後のUI更新のためにSkillSelectUIをセット
            }
        }
    }

    // 装備スロットの表示更新
    public void RefreshSkillSlots()
    {
        int slotCount = playerSkill.SkillSlotCount;
        for (int i = 0; i < slotCount; i++)
        {
            Image slotImg = (i < skillSlotImages.Count) ? skillSlotImages[i] : null;
            if (slotImg == null) continue;

            var handler = (i < slotHandlers.Count) ? slotHandlers[i] : null;
            var skill = playerSkill.GetSkill(i);

            if (skill != null)
            {
                // スロットにスキルが入っている場合はスロット固有のSpriteを優先
                if (handler != null && handler.skillSlotSprite != null)
                {
                    slotImg.sprite = handler.skillSlotSprite;
                    slotImg.color = Color.white;
                }
                else if (skill.skillIcon != null)
                {
                    slotImg.sprite = skill.skillIcon;
                    slotImg.color = Color.white;
                }
                else
                {
                    // フォールバック：ハンドラの empty があれば使う、なければ透過
                    if (handler != null && handler.emptySlotSprite != null)
                    {
                        slotImg.sprite = handler.emptySlotSprite;
                        slotImg.color = Color.gray;
                    }
                    else
                    {
                        slotImg.sprite = null;
                        slotImg.color = Color.clear;
                    }
                }
            }
            else
            {
                // スロットが空ならスロット固有の emptySprite を優先、無ければ透過
                if (handler != null && handler.emptySlotSprite != null)
                {
                    slotImg.sprite = handler.emptySlotSprite;
                    slotImg.color = Color.gray;
                }
                else
                {
                    slotImg.sprite = null;
                    slotImg.color = Color.clear;
                }
            }
        }
    }

    /// <summary>
    /// ドラッグでスキルがスロットにドロップされた時の処理
    /// </summary>
    public void OnSkillIconDropped(SkillIconUI iconUI, int slotIndex)
    {
        int ownedIndex = bringSkill.GetOwnedSkills().IndexOf(iconUI.Skill);
        if (ownedIndex < 0) return;
        bringSkill.SwapSkillWithPlayerSkill(ownedIndex, slotIndex, playerSkill);
        Destroy(iconUI.gameObject); // ドロップされたアイコンは消す

        RefreshOwnedSkills(); // 所持スキルUI更新
        RefreshSkillSlots();  // スロットUI更新
    }

    /// <summary>
    /// スロットの画像を即時に設定（プレビュー／即時反映用）
    /// </summary>
    public void SetSlotSpriteImmediate(int slotIndex, SkillBase skill)
    {
        if (slotIndex < 0 || slotIndex >= skillSlotImages.Count) return;
        var img = skillSlotImages[slotIndex];
        if (img == null) return;

        var handler = (slotIndex < slotHandlers.Count) ? slotHandlers[slotIndex] : null;

        if (skill != null)
        {
            // プレビューでもハンドラのスロット用Spriteを優先
            if (handler != null && handler.skillSlotSprite != null)
            {
                img.sprite = handler.skillSlotSprite;
                img.color = Color.white;
            }
            else if (skill.skillIcon != null)
            {
                img.sprite = skill.skillIcon;
                img.color = Color.white;
            }
            else if (handler != null && handler.emptySlotSprite != null)
            {
                img.sprite = handler.emptySlotSprite;
                img.color = Color.gray;
            }
            else
            {
                img.sprite = null;
                img.color = Color.clear;
            }
        }
        else
        {
            // null が渡されたら空状態表示
            if (handler != null && handler.emptySlotSprite != null)
            {
                img.sprite = handler.emptySlotSprite;
                img.color = Color.gray;
            }
            else
            {
                img.sprite = null;
                img.color = Color.clear;
            }
        }
    }

    /// <summary>
    /// 指定スロットに現在セットされているSkillBaseを取得
    /// </summary>
    public SkillBase GetSlotSkill(int slotIndex)
    {
        if (playerSkill == null) return null;
        if (slotIndex < 0 || slotIndex >= playerSkill.SkillSlotCount) return null;
        return playerSkill.GetSkill(slotIndex);
    }
}
