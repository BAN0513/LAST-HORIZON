using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Takato;
using UnityEngine.EventSystems;

/// <summary>
/// スキルアイコンUI（ドラッグ＆ドロップ専用）
/// </summary>
public class SkillIconUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("スキルアイコンUI")]
    [SerializeField] private Image iconImage;
    [Header("スキル名テキスト")]
    [SerializeField] private TMP_Text nameText;

    private SkillBase skill;

    // ドラッグ用
    public SkillBase Skill => skill;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector2 originalPosition;

    /// <summary>
    /// スキルアイコンUIをセットアップする
    /// </summary>
    public void Setup(SkillBase skill)
    {
        this.skill = skill;

        if (iconImage != null && skill != null)
        {
            // 必要に応じてアイコン画像をセット
            // iconImage.sprite = skill.icon;
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
    /// ドラッグ開始時にアイコンをUIの最前面に移動させ、元の位置を保存する
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = transform.localPosition;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(transform.root); // UIの最前面に
    }

    /// <summary>
    /// ドラッグ中はアイコンをマウス位置に追従させる
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    /// <summary>
    /// ドラッグ終了時に元の位置に戻す
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(originalParent);
        transform.localPosition = originalPosition;
    }
}
