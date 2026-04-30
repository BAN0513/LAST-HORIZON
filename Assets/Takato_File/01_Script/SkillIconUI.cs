using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Takato;
using UnityEngine.EventSystems;

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

    //SkillSelectUI 参照（ドラッグ終了でスロットをリセットするため）
    private SkillSelectUI skillSelectUI;

    // ドラッグ時に移す親Canvas参照（nullなら従来動作）
    private Canvas parentCanvas;

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

            // 明示的に RaycastTarget を有効にしておく
            iconImage.raycastTarget = true;
        }
        if (nameText != null && skill != null)
        {
            nameText.text = skill.skillName;
        }
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // 所属する Canvas をキャッシュ（ドラッグ中の親として使う）
        parentCanvas = GetComponentInParent<Canvas>();
    }

    /// <summary>
    /// ドラッグ開始
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = transform.localPosition;
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;

        // 安定的に Canvas の直下へ移す
        if (parentCanvas != null)
            transform.SetParent(parentCanvas.transform, true);
        else
            transform.SetParent(transform.root, true);
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
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;

        transform.SetParent(originalParent);
        transform.localPosition = originalPosition;

        // ドラッグ終了時にスロット表示を更新
        skillSelectUI?.RefreshSkillSlots();
    }
}
