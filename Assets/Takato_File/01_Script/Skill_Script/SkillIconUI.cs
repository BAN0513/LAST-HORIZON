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
        // 必要ならUI表示もここで

        var btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClick);
        }
    }

    /// <summary>
    /// アイコンがクリックされたときに呼ばれるメソッド
    /// </summary>#
    public void OnClick()
    {
        onClick?.Invoke(skill);
    }
}
