using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Takato;
using UnityEngine.EventSystems;

/// <summary>
/// スキルアイコンUI（ドラッグ用）
/// </summary>
public class SkillIconUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("スキルアイコンUI")]
    [SerializeField] private Image iconImage;
    [Header("スキル表示名")]
    [SerializeField] private TMP_Text nameText;

    private SkillBase skill;

    // スキル参照プロパティ
    public SkillBase Skill => skill;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector2 originalPosition;

    // 追加: SkillSelectUI 参照（ドラッグ終了でスロットをリセットするため）
    private SkillSelectUI skillSelectUI;

    /// <summary>
    /// SkillSelectUI をセットするメソッド
    /// </summary>
    public void SetSkillSelectUI(SkillSelectUI ui)
    {
        skillSelectUI = ui;
    }

    /// <summary>
    /// UI表示セットアップ
    /// </summary>
    public void Setup(SkillBase skill)
    {
        this.skill = skill; // 所持スキルを保持

        if (iconImage != null && skill != null)
        {
            // ScriptableObject の skillIcon を UI に反映
            iconImage.sprite = skill.skillIcon;
            iconImage.enabled = skill.skillIcon != null;
            iconImage.color = skill.skillIcon != null ? Color.white : Color.clear;
        }
        if (nameText != null && skill != null)
        {
            nameText.text = skill.skillName;
        }
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    /// <summary>
    /// ドラッグ開始
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = transform.localPosition;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(transform.root); // ルートに移動して描画順を確保
    }

    /// <summary>
    /// ドラッグ中
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    /// <summary>
    /// ドラッグ終了
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(originalParent);
        transform.localPosition = originalPosition;

        // 追加: ドラッグ終了時に必ずスロット表示をリフレッシュして
        // プレビュー状態（skillSlotSprite 表示）を解除する
        skillSelectUI?.RefreshSkillSlots();
    }
}
