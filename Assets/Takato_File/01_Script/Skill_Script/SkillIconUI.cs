using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Takato;
using UnityEngine.EventSystems;

/// <summary>
/// スキルアイコンUI（ドラッグ方式）
/// </summary>
public class SkillIconUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("スキルアイコンUI")]
    [SerializeField] private Image iconImage;
    [Header("スキル表示テキスト")]
    [SerializeField] private TMP_Text nameText;

    private SkillBase skill;

    // 所持スキル参照
    public SkillBase Skill => skill;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector2 originalPosition;

    /// <summary>
    /// UIにスキル情報をセットする
    /// </summary>
    public void Setup(SkillBase skill)
    {
        this.skill = skill; // 引数のスキルをフィールドに保存

        if (iconImage != null && skill != null)
        {
            // ScriptableObjectのskillIconをUIに反映
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
        transform.SetParent(transform.root); // ルートへ移動して描画優先
    }

    /// <summary>
    /// ドラッグ中
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position; // ドラッグ中はマウス位置に追従
    }

    /// <summary>
    /// ドラッグ終了
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(originalParent);
        transform.localPosition = originalPosition; // ドロップ先で位置が変わる可能性があるから、元の位置に戻す
    }
}
