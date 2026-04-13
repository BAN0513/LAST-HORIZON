using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class TitleUIManager : MonoBehaviour
{
    public static TitleUIManager Instance { get; private set; }

    private SaveData saveData;

    [SerializeField] private CanvasGroup groupStart;
    [SerializeField] private CanvasGroup groupLoad;
    [SerializeField] private CanvasGroup groupSystem;
    [SerializeField] private CanvasGroup groupCharacterSelect;

    [SerializeField] private Button startButton;
    [SerializeField] private Button loadButton;

    [SerializeField] private Button swordButton;
    [SerializeField] private Button greatSwordButton;
    [SerializeField] private Button wizardButton;

    [SerializeField] private Slider sliderSE;
    [SerializeField] private TMP_InputField textSE;

    [SerializeField] private Slider sliderBGM;
    [SerializeField] private TMP_InputField textBGM;

    [SerializeField] private Slider sliderLight;
    [SerializeField] private TMP_InputField textLight;

    [SerializeField] private Text[] playTimeText;
    private SystemManager system;
    private bool isGamePadConnection = false;

    private FadeManager fadeManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        groupStart.transform.SetAsLastSibling();

        system = SystemManager.instance;
        groupStart.alpha = 1.0f;
        ChangeInteractable(groupLoad, false);
        groupLoad.alpha = 0;
        ChangeInteractable(groupSystem, false);
        groupSystem.alpha = 0;
        ChangeInteractable (groupCharacterSelect, false);
        groupCharacterSelect.alpha = 0;

        float lightValue = system.valueLight;
        RenderSettings.ambientIntensity = lightValue;

        SystemUISetting(sliderSE, textSE, system.volueSE);
        SystemUISetting(sliderBGM, textBGM, system.volueBGM);
        SystemUISetting(sliderLight, textLight, lightValue);

        for (int i = 0; i < playTimeText.Length; i++)
        {
            string path = Application.persistentDataPath + $"/save_{i + 1}.json";

            // ファイルからJSON文字列を読み込む
            string json = File.ReadAllText(path);

            // JSON文字列からGameDataオブジェクトに復元
            SaveData save = JsonUtility.FromJson<SaveData>(json);

            float time = save.playTime / 3600;
            float notVery = save.playTime % 3600;
            float minute = notVery / 60;
            float second = notVery % 60;

            playTimeText[i].text = $"{Mathf.FloorToInt(time)} : {Mathf.FloorToInt(minute)} : {Mathf.FloorToInt(second)}";
        }

        // デバイス一覧を取得
        foreach (var device in InputSystem.devices)
        {
            // デバイス名をログ出力
            Debug.Log(device.name);

            //GamePadに後々変更する。今はデバッグ用でKeybordを使用する
            if (device.name == "Keyboar")
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                isGamePadConnection = true;
                EventSystem.current.firstSelectedGameObject = startButton.gameObject;
                EventSystem.current.SetSelectedGameObject(startButton.gameObject);
            }
        }

        fadeManager = FadeManager.instance;
    }

    private void SystemUISetting(Slider slider, TMP_InputField text, float value)
    {
        slider.minValue = 0;
        slider.maxValue = 10;
        slider.value = value * 10 - 10;
        text.text = slider.value.ToString();
    }

    public void NewGame()
    {
        //SaveManager.Instance.NewGame();
        ChangeInteractable(groupStart, false);
        groupCharacterSelect.transform.SetAsLastSibling();
        StartCoroutine(FadeInOutControl(groupCharacterSelect, groupStart, swordButton.gameObject));
    }

    public void CharacterSelectBack()
    {
        ChangeInteractable(groupCharacterSelect, false);

        StartCoroutine(FadeInOutControl(groupStart, groupCharacterSelect, startButton.gameObject));
    }

    public void Continue()
    {
        ChangeInteractable(groupStart, false);
        groupLoad.transform.SetAsLastSibling();
        StartCoroutine(FadeInOutControl(groupLoad, groupStart, loadButton.gameObject));
    }

    public void Slot(int slot)
    {
        SaveManager.Instance.LoadGame(slot);
    }

    public void LoadBack()
    {
        ChangeInteractable(groupLoad, false);

        StartCoroutine(FadeInOutControl(groupStart, groupLoad, startButton.gameObject));
    }

    public void System()
    {
        ChangeInteractable(groupStart, false);
        groupSystem.transform.SetAsLastSibling();
        StartCoroutine(FadeInOutControl(groupSystem, groupStart, sliderSE.gameObject));
    }

    public void SystemBack()
    {
        ChangeInteractable(groupSystem, false);

        StartCoroutine(FadeInOutControl(groupStart, groupSystem, startButton.gameObject));
    }

    public void CharacterSelect(ButtonControl.ButtonType type)
    {
        switch (type)
        {
            case ButtonControl.ButtonType.Characetr_Sword:
                SaveManager.Instance.NewGame(SaveData.Character.Sword);
                break;
            case ButtonControl.ButtonType.Character_GreatSword:
                SaveManager.Instance.NewGame(SaveData.Character.GreateSword);
                break;
            case ButtonControl.ButtonType.Character_Wizard:
                SaveManager.Instance.NewGame(SaveData.Character.Wizard);
                break;
        }
    }


    public void SEChange()
    {
        float seValue = sliderSE.value / 10 + 1;
        system.volueSE = seValue;
        textSE.text = sliderSE.value.ToString("F0");
    }


    public void BGMChange()
    {
        float bgmValue = sliderBGM.value / 10 + 1;
        textBGM.text = sliderBGM.value.ToString("F0");
        system.volueBGM = bgmValue;
    }

    public void LightChange()
    {
        float lightValue = sliderLight.value / 10 + 1;
        RenderSettings.ambientIntensity = lightValue;
        textLight.text = sliderLight.value.ToString("F0");
        system.valueLight = lightValue;
    }


    private void SliderMove(TMP_InputField text, Slider slider)
    {
        if (text.text == "")
        {
            text.text = slider.value.ToString("F0");
        }
        else
        {
            float num = float.Parse(text.text);
            if (num < 0 || num > 10)
            {
                RectTransform rect = text.GetComponent<RectTransform>();
                rect.right = new Vector3(0, 0, 0);
                text.text = slider.value.ToString("F0");
            }
            else
            {
                slider.value = num;
            }
        }

    }

    public void SETextChange()
    {
        SliderMove(textSE, sliderSE);   
    }

    public void BGMTextChange()
    {
        SliderMove(textBGM, sliderBGM);
    }

    public void LightTextChange()
    {
        SliderMove(textLight, sliderLight);
    }

    IEnumerator FadeInOutControl(CanvasGroup inGroup, CanvasGroup outGroup, GameObject setSelectObj)
    {
        yield return StartCoroutine(fadeManager.Fade(1, 0, outGroup));
        yield return StartCoroutine(fadeManager.Fade(0, 1, inGroup));
        if (isGamePadConnection)
        {
            EventSystem.current.SetSelectedGameObject(setSelectObj);
        }
        ChangeInteractable(inGroup, true);
    }

    private void ChangeInteractable(CanvasGroup canvasGroup, bool active)
    {
        canvasGroup.interactable = active;
    }

    private void OnEsc(InputValue value)
    {
        if (groupLoad.alpha == 1)
        {
            LoadBack();
        }
        else if (groupSystem.alpha == 1)
        {
            SystemBack();
        }
        else if (groupCharacterSelect.alpha == 1)
        {
            CharacterSelectBack();
        }
    }
}
