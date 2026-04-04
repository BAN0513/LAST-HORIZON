using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class TitleManager : MonoBehaviour
{
    private SaveData saveData;

    [SerializeField] private CanvasGroup groupStart;
    [SerializeField] private CanvasGroup groupLoad;
    [SerializeField] private CanvasGroup groupSystem;

    [SerializeField] private Button startButton;
    [SerializeField] private Button loadButton;

    [SerializeField] private Slider sliderSE;
    [SerializeField] private TMP_InputField textSE;

    [SerializeField] private Slider sliderBGM;
    [SerializeField] private TMP_InputField textBGM;

    [SerializeField] private Slider sliderLight;
    [SerializeField] private TMP_InputField textLight;

    [SerializeField] private Text[] playTimeText;
    private SystemManager system;
    private bool isMouseControl = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        //EventSystem.current.firstSelectedGameObject = startButton.gameObject;
        EventSystem.current.SetSelectedGameObject(null);

        system = SystemManager.instance;
        groupStart.alpha = 1.0f;
        ChangeInteractable(groupLoad, false);
        groupLoad.alpha = 0;
        ChangeInteractable(groupSystem, false);
        groupSystem.alpha = 0;

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
    }

    private void SystemUISetting(Slider slider, TMP_InputField text, float value)
    {
        slider.minValue = 0;
        slider.maxValue = 10;
        slider.value = value * 10 - 10;
        text.text = slider.value.ToString();
    }

    public void OnNewGameButton()
    {
        SaveManager.Instance.NewGame();
    }

    public void OnContinueButton()
    {
        ChangeInteractable(groupStart, false);
        groupLoad.transform.SetAsLastSibling();
        StartCoroutine(FadeInOutControl(groupLoad, groupStart, loadButton.gameObject));
    }

    public void OnSlotButton(int slot)
    {
        SaveManager.Instance.LoadGame(slot);
    }

    public void OnLoadBackButton()
    {
        ChangeInteractable(groupLoad, false);

        StartCoroutine(FadeInOutControl(groupStart, groupLoad, startButton.gameObject));
    }

    public void OnSystemButton()
    {
        ChangeInteractable(groupStart, false);
        groupSystem.transform.SetAsLastSibling();
        StartCoroutine(FadeInOutControl(groupSystem, groupStart, sliderSE.gameObject));
    }

    public void OnSystemBackButton()
    {
        ChangeInteractable(groupSystem, false);

        StartCoroutine(FadeInOutControl(groupStart, groupSystem, startButton.gameObject));
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
        yield return StartCoroutine(Fade(1, 0, outGroup));
        yield return StartCoroutine(Fade(0, 1, inGroup));
        ChangeInteractable(inGroup, true);


        EventSystem.current.SetSelectedGameObject(null);

    }

    private void ChangeInteractable(CanvasGroup canvasGroup, bool active)
    {
        canvasGroup.interactable = active;
    }

    IEnumerator Fade(float start, float end, CanvasGroup group)
    {
        Time.timeScale = 1.0f;
        float fadeDuration = 1;

        float t = 0f;
        Color cl;
        cl.a = start;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            cl.a = Mathf.Lerp(start, end, t);

            group.alpha = cl.a;

            yield return null;
        }

        cl.a = end;

        group.alpha = cl.a;
    }

    private void OnEsc(InputValue value)
    {
        if (groupLoad.alpha == 1)
        {
            OnLoadBackButton();
        }
        else if (groupSystem.alpha == 1)
        {
            OnSystemBackButton();
        }
    }

    private void OnPoint(InputValue value)
    {
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(null);
        GroupBlockRayCast(true);
        isMouseControl = true;
    }

    private void OnNavigate(InputValue value)
    {
        if (isMouseControl)
        {
            Cursor.visible = false;
            GroupBlockRayCast(false);
            isMouseControl = false;
        }
        else if (EventSystem.current.currentSelectedGameObject == null)
        {
            StartCoroutine(Navigate());
        }
    }

    IEnumerator Navigate()
    {
        EventSystem.current.sendNavigationEvents = false;
        if (groupStart.alpha == 1)
        {
            EventSystem.current.SetSelectedGameObject(startButton.gameObject);
        }
        else if (groupLoad.alpha == 1)
        {
            EventSystem.current.SetSelectedGameObject(loadButton.gameObject);
        }
        else if (groupSystem.alpha == 1)
        {
            EventSystem.current.SetSelectedGameObject(sliderSE.gameObject);
        }
        yield return new WaitForSeconds(0.1f);
        EventSystem.current.sendNavigationEvents = true;
        isMouseControl = false;
    }

    private void ChangeblocksRaycasts(CanvasGroup canvasGroup, bool active)
    {
        canvasGroup.blocksRaycasts = active;
    }

    private void GroupBlockRayCast(bool active)
    {
        groupStart.blocksRaycasts = active;
        groupLoad.blocksRaycasts = active;
        groupSystem.blocksRaycasts = active;
    }
}
