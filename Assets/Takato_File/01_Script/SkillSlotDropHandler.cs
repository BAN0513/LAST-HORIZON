using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// スキルスロットにスキルアイコンがドロップされたときの処理を行うクラス
/// （プレビュー機能：ドラッグ中にスロット上でSOのSpriteを表示）
/// </summary>
public class SkillSlotDropHandler : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("プレイヤー側のスキルスロットのインデックス")]
    public int slotIndex;
    [Header("スキルが入っている時のSprite")]
    public Sprite skillSlotSprite;
    [Header("スキルが入っていない時のSprite")]
    public Sprite emptySlotSprite;
    [Header("スキル選択UIの参照")]
    public SkillSelectUI skillSelectUI;

    // このスロットの表示Image参照
    private Image cachedImage;

    public void Start()
    {
        // 最初にスロットにスキルが入っている場合の表示を初期化
        var img = GetSlotImage();
        if (img == null) return;

        // skillSelectUI が設定されていればプレイヤーの現在のスロット内容を参照して表示を決定する
        if (skillSelectUI != null)
        {
            var slotSkill = skillSelectUI.GetSlotSkill(slotIndex);
            if (slotSkill != null)
            {
                // スキルがセットされている場合はハンドラの skillSlotSprite を優先、なければ SO のアイコンを使用
                if (skillSlotSprite != null)
                {
                    img.sprite = skillSlotSprite;
                    img.color = Color.white;
                }
                else if (slotSkill.skillIcon != null)
                {
                    img.sprite = slotSkill.skillIcon;
                    img.color = Color.white;
                }
                else if (emptySlotSprite != null)
                {
                    img.sprite = emptySlotSprite;
                    img.color = Color.gray;
                }
                else
                {
                    img.sprite = null;
                    img.color = Color.clear;
                }

                return;
            }
            else
            {
                // スロットが空なら emptySlotSprite を優先して表示
                if (emptySlotSprite != null)
                {
                    img.sprite = emptySlotSprite;
                    img.color = Color.gray;
                }
                else
                {
                    img.sprite = null;
                    img.color = Color.clear;
                }

                return;
            }
        }

        // skillSelectUI が設定されていない場合は、ハンドラの skillSlotSprite を優先して表示
        if (skillSlotSprite != null)
        {
            img.sprite = skillSlotSprite;
            img.color = Color.white;
        }
        else if (emptySlotSprite != null)
        {
            img.sprite = emptySlotSprite;
            img.color = Color.gray;
        }
        else
        {
            img.sprite = null;
            img.color = Color.clear;
        }
    }
    // SkillSelectUIから明示的に共有するためのSetterを用意
    public void SetCachedImage(Image image)
    {
        cachedImage = image; // これでSkillSelectUIと同じImage参照を共有できる
    }

    /// <summary>
    /// スロットのImageコンポーネントをキャッシュして取得する
    /// </summary>
    private Image GetSlotImage()
    {
        if (cachedImage != null) return cachedImage;
        // 非アクティブな子も含めて検索する
        cachedImage = GetComponent<Image>() ?? GetComponentInChildren<Image>(true);
        return cachedImage;
    }

    /// <summary>
    /// スキルアイコンがドロップされたときに呼び出されるメソッド
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag;
        if (dragged == null || skillSelectUI == null) return;
        var iconUI = dragged.GetComponent<SkillIconUI>();
        if (iconUI != null)
        {
            // ドロップ直後は必ずスキルが入っている時のSpriteを優先表示
            var img = GetSlotImage();
            if (img != null)
            {
                if (skillSlotSprite != null)
                {
                    img.sprite = skillSlotSprite;
                    img.color = Color.white;
                }
                else if (iconUI.Skill != null && iconUI.Skill.skillIcon != null)
                {
                    img.sprite = iconUI.Skill.skillIcon;
                    img.color = Color.white;
                }
                else if (emptySlotSprite != null)
                {
                    img.sprite = emptySlotSprite;
                    img.color = Color.gray;
                }
                else
                {
                    img.sprite = null;
                    img.color = Color.clear;
                }
            }

            // データのスワップと最終的なUI更新はSkillSelectUI側で行う
            skillSelectUI.OnSkillIconDropped(iconUI, slotIndex);
        }
    }

    /// <summary>
    /// ドラッグ中にスロット上に入ったらプレビュー表示
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (skillSelectUI == null) return;

        var img = GetSlotImage();
        if (img == null) return;

        // ドラッグしていないときは何もしない（プレビューのみ）
        var dragged = eventData.pointerDrag;
        if (dragged == null) return;

        // 現在スロットにセットされているスキルを取得
        var slotSkill = skillSelectUI.GetSlotSkill(slotIndex);

        Sprite targetSprite = null;
        Color targetColor = Color.clear;

        if (slotSkill != null)
        {
            // スロットに既にスキルがある場合はスロット表示
            if (skillSlotSprite != null)
            {
                targetSprite = skillSlotSprite;
                targetColor = Color.white;
            }
            else if (slotSkill.skillIcon != null)
            {
                targetSprite = slotSkill.skillIcon;
                targetColor = Color.white;
            }
            else if (emptySlotSprite != null)
            {
                targetSprite = emptySlotSprite;
                targetColor = Color.gray;
            }
        }
        else
        {
            // スロットが空ならドラッグ中のアイコンを優先表示
            var iconUI = dragged.GetComponent<SkillIconUI>();
            if (iconUI != null && iconUI.Skill != null)
            {
                if (skillSlotSprite != null)
                {
                    targetSprite = skillSlotSprite;
                    targetColor = Color.white;
                }
                else if (iconUI.Skill.skillIcon != null)
                {
                    targetSprite = iconUI.Skill.skillIcon;
                    targetColor = Color.white;
                }
                else if (emptySlotSprite != null)
                {
                    targetSprite = emptySlotSprite;
                    targetColor = Color.gray;
                }
            }
        }

      img.sprite = targetSprite;
      img.color = targetColor != null ? targetColor : Color.clear;
    }

    /// <summary>
    /// ドラッグが離れたらスロットを元の状態に戻す
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        var img = GetSlotImage();
        if (img == null) return;

        // ドラッグしていないときは何もしない
        if (skillSelectUI == null)
        {
            // SkillSelectUI が無いなら既定の empty 表示へ
            if (emptySlotSprite != null)
            {
                img.sprite = emptySlotSprite;
                img.color = Color.gray;
            }
            else
            {
                img.sprite = null;
                img.color = Color.clear;
            }
            return;
        }

        var slotSkill = skillSelectUI.GetSlotSkill(slotIndex);
        if (slotSkill != null)
        {
            // スロットにスキルがある場合はハンドラ優先で表示
            if (skillSlotSprite != null)
            {
                img.sprite = skillSlotSprite;
                img.color = Color.white;
            }
            else if (slotSkill.skillIcon != null)
            {
                img.sprite = slotSkill.skillIcon;
                img.color = Color.white;
            }
            else if (emptySlotSprite != null)
            {
                img.sprite = emptySlotSprite;
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
            // スロットが空なら必ず emptySlotSpriteに戻す
            if (emptySlotSprite != null)
            {
                img.sprite = emptySlotSprite;
                img.color = Color.gray;
            }
            else
            {
                img.sprite = null;
                img.color = Color.clear;
            }
        }
    }
}