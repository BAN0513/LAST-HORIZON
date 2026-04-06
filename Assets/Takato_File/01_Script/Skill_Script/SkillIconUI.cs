using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Takato;

public class SkillIconUI : MonoBehaviour
{
    [Header("スキルアイコンUI")]
    [SerializeField] private Image iconImage;
    [Header("スキル名テキスト")]
    [SerializeField] private TMP_Text nameText;

    private SkillBase skill; // 表示するスキルのデータ
    private System.Action<SkillBase> onClick; // アイコンがクリックされたときのコールバック

    /// <summary>
    /// スキルアイコンUIをセットアップするメソッド
    /// </summary>
    public void Setup(SkillBase skill, System.Action<SkillBase> onClick)
    {
        this.skill = skill;
        this.onClick = onClick;
        //nameText.text = skill.skillName;
        // iconImage.sprite = skill.icon; // SkillBaseにiconがあれば
    }

    /// <summary>
    /// アイコンがクリックされたときに呼ばれるメソッド
    /// </summary>
    public void OnClick()
    {
        onClick?.Invoke(skill);
    }
}
