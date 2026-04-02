using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using NUnit.Framework.Interfaces;

public class TitleManager : MonoBehaviour
{
    private SaveData saveData;

    [SerializeField] private CanvasGroup groupStart;
    [SerializeField] private CanvasGroup groupLoad;
    [SerializeField] private CanvasGroup groupSystem;

    [SerializeField] private Button startButton;
    [SerializeField] private Button loadButton;

    [SerializeField] private Slider sliderSE;
    [SerializeField] private Slider sliderBGM;
    [SerializeField] private Slider sliderLight;
    [SerializeField] private Text[] playTimeText;
    private SystemManager system;
    private bool isMouseControl = false;

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
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

        sliderSE.minValue = 0;
        sliderBGM.minValue = 0;
        sliderLight.minValue = 0;

        sliderSE.maxValue = 10;
        sliderBGM.maxValue = 10;
        sliderLight.maxValue = 10;

        sliderSE.value = system.volueSE * 10 - 10;
        sliderBGM.value = system.volueBGM * 10 - 10;
        sliderLight.value = system.valueLight * 10 - 10;

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

    private void Update()
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject);
    }

    public void OnNewGameButton()
    {
        SaveManager.Instance.LoadButton(1,true);
    }

    public void OnContinueButton()
    {
        ChangeInteractable(groupStart, false);
        groupLoad.transform.SetAsLastSibling();
        StartCoroutine(FadeInOutControl(groupLoad, groupStart, loadButton.gameObject));
    }

    public void OnSlotButton(int slot)
    {
        SaveManager.Instance.LoadButton(slot,false);
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
    }

    public void BGMChange()
    {
        float bgmValue = sliderBGM.value / 10 + 1;
        system.volueBGM = bgmValue;
    }

    public void LightChange()
    {
        float lightValue = sliderLight.value / 10 + 1;
        RenderSettings.ambientIntensity = lightValue;
        system.valueLight = lightValue;
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
