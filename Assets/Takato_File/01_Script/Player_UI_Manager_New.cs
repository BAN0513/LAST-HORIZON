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
    [Space(10)]
    [Header("スタミナUI")]
    [SerializeField] private Image staminaFillImage;
    [SerializeField] private GameObject staminaUIRoot;
    [Header("体力UI")]
    [SerializeField] private Image healthFillImage;

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

        // 初期状態で体力を満タン表示にしておく
        if (playerScript != null)
        {
            UpdateHealthUI(playerScript.CurrentHealth, playerScript.CurrentHealth);
        }
    }

    private void OnEnable()
    {
        // プレイヤーのスタミナが変化したときにUIを更新するイベントを登録
        if (playerScript != null)
        {
            playerScript.OnStaminaChanged += UpdateStaminaUI;
        }

        // プレイヤーの体力が変化したときにUIを更新するイベントを登録
        if (playerScript != null)
        {
            playerScript.OnHealthChanged += UpdateHealthUI;
        }
    }

    private void OnDisable()
    {
        // プレイヤーのスタミナが変化したときにUIを更新するイベントの登録を解除
        if (playerScript != null)
        {
            playerScript.OnStaminaChanged -= UpdateStaminaUI;
        }

        // プレイヤーの体力が変化したときにUIを更新するイベントの登録を解除
        if (playerScript != null)
        {
            playerScript.OnHealthChanged -= UpdateHealthUI;
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

    /// <summary>
    ///プレイヤーの体力UIの表示を更新する
    ///</summary>
    private void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        if (healthFillImage == null || maxHealth <= 0f) return;
        // 割合を計算
        float fillValue = Mathf.Clamp01(currentHealth / maxHealth);
        healthFillImage.fillAmount = fillValue;
    }   
}