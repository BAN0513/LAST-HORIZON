using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレイヤーのUI（体力・スタミナ等）を管理するクラス(New)
/// </summary>
public class Player_UI_Manager_New : MonoBehaviour
{
    [Header("プレイヤーの参照")]
    [SerializeField] private Player_Script_New playerScript;

    [Header("UIコンポーネント参照")]
    [SerializeField] private Image staminaFillImage; // Circle設定にしたFill用Image

    private void Awake()
    {
        if (playerScript == null)
        {
           //プレイヤータグを持つオブジェクトを探して取得
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerScript = playerObject.GetComponent<Player_Script_New>();
                if (playerScript == null)
                {
                    Debug.LogError("Player_Script_Newが見つかりません。");
                }
            }
            else
            {
                Debug.LogError("タグ 'Player' を持つオブジェクトが見つかりません。");
            }
        }

        if (staminaFillImage == null)
        {
            Debug.LogError("スタミナ用のImageがアタッチされていません。");
        }
    }

    private void OnEnable()
    {
        if (playerScript != null)
        {
            playerScript.OnStaminaChanged += UpdateStaminaUI; // スタミナが変化したときにUIを更新するイベントを登録
        }
    }

    private void OnDisable()
    {
        if (playerScript != null)
        {
            playerScript.OnStaminaChanged -= UpdateStaminaUI; // イベントの登録を解除
        }
    }

    /// <summary>
    /// スタミナUIの表示を更新する
    /// </summary>
    /// <param name="currentStamina">現在のスタミナ</param>
    /// <param name="maxStamina">最大スタミナ</param>
    private void UpdateStaminaUI(float currentStamina, float maxStamina)
    {
        if (staminaFillImage == null || maxStamina <= 0f) return;

        // 0.0 ~ 1.0 の範囲に正規化して FillAmount に適用
        float fillValue = Mathf.Clamp01(currentStamina / maxStamina);
        staminaFillImage.fillAmount = fillValue;
    }
}