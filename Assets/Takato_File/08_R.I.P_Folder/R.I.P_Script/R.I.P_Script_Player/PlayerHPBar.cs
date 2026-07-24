using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレイヤーのHPバーを制御するクラス
/// </summary>
public class PlayerHPBar : MonoBehaviour
{
   
    [Header("HPバーのスライダー")]
    [SerializeField] private Slider hpSlider; // HPバーのスライダー

    private void Start()
    {
        if (hpSlider == null)
        {
            Debug.LogError("HPスライダーが設定されていません。");
        }

        // Valueの初期値を1に設定
        if (hpSlider != null)
        {
            hpSlider.value = 1f;
        }

    }

    /// <summary>
    /// HPバーの値を更新するメソッド
    /// </summary>
    public void SetHP(int current, int max)
    {
        if (hpSlider != null)
        {
            hpSlider.value = (max > 0) ? (float)current / max : 0f; // HPが0のときは0に設定
        }
    }
}
