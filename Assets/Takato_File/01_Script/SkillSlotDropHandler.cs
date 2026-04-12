using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// スキルスロットにスキルアイコンがドロップされたときの処理を行うクラス
/// </summary>
public class SkillSlotDropHandler : MonoBehaviour, IDropHandler
{
    [Header("プレイヤー側のスキルスロットのインデックス")]
    public int slotIndex;
    [Header("スキル選択UIの参照")]
    public SkillSelectUI skillSelectUI;

    /// <summary>
    /// スキルアイコンがドロップされたときに呼び出されるメソッド
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag;
        if (dragged == null) return;
        var iconUI = dragged.GetComponent<SkillIconUI>();
        if (iconUI != null)
        {
            skillSelectUI.OnSkillIconDropped(iconUI.Skill, slotIndex);
        }
    }
}
