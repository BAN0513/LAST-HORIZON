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
    [SerializeField] private Image staminaFillImage;
    [SerializeField] private GameObject staminaUIRoot; 

    private void Awake()
    {
        if (playerScript == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerScript = playerObject.GetComponent<Player_Script_New>();
            }
        }
    }

    private void Start()
    {
        // 初期状態は非表示にする
        if (staminaUIRoot != null)
        {
            staminaUIRoot.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (playerScript != null)
        {
            playerScript.OnStaminaChanged += UpdateStaminaUI;
        }
    }

    private void OnDisable()
    {
        if (playerScript != null)
        {
            playerScript.OnStaminaChanged -= UpdateStaminaUI;
        }
    }

    /// <summary>
    /// スタミナUIの表示を更新する
    /// </summary>
    private void UpdateStaminaUI(float currentStamina, float maxStamina)
    {
        if (staminaFillImage == null || maxStamina <= 0f) return;

        // 割合を計算
        float fillValue = Mathf.Clamp01(currentStamina / maxStamina);
        staminaFillImage.fillAmount = fillValue;

        if (staminaUIRoot != null)
        {
            //スタミナが最大値未満の場合にUIを表示する
            bool shouldShow = currentStamina < maxStamina;

            staminaUIRoot.SetActive(shouldShow); // UIの表示・非表示を切り替える
        }
    }
}