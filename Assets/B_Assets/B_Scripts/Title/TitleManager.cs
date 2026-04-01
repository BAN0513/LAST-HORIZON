using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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

    private SystemManager system;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        EventSystem.current.firstSelectedGameObject = startButton.gameObject;
        EventSystem.current.SetSelectedGameObject(startButton.gameObject);

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
    }

    public void OnNewGameButton()
    {
        SaveManager.Instance.LoadButton(1,true);
    }

    public void OnContinueButton()
    {
        ChangeInteractable(groupStart, false);
        StartCoroutine(StartImageOutLoadImageIn());
        //SaveManager.Instance.LoadButton(1,false);
    }

    IEnumerator StartImageOutLoadImageIn()
    {
        yield return StartCoroutine(Fade(1, 0, groupStart));
        yield return StartCoroutine(Fade(0, 1, groupLoad));
        ChangeInteractable(groupLoad, true);
        EventSystem.current.SetSelectedGameObject(loadButton.gameObject);
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

    public void OnSlotButton(int slot)
    {
        SaveManager.Instance.LoadButton(slot,false);
    }

    public void OnLoadBackButton()
    {
        ChangeInteractable(groupLoad, false);

        StartCoroutine(OnLoadBack());
    }

    IEnumerator OnLoadBack()
    {
        yield return StartCoroutine(Fade(1, 0, groupLoad));
        yield return StartCoroutine(Fade(0, 1, groupStart));

        ChangeInteractable(groupStart, true);
        EventSystem.current.SetSelectedGameObject(startButton.gameObject);
    }

    public void OnSystemButton()
    {
        ChangeInteractable(groupStart, false);
        StartCoroutine(StartImageOutSystemImageIn());
    }

    IEnumerator StartImageOutSystemImageIn()
    {
        yield return StartCoroutine(Fade(1, 0, groupStart));
        yield return StartCoroutine(Fade(0, 1, groupSystem));
        ChangeInteractable(groupSystem, true);
        EventSystem.current.SetSelectedGameObject(sliderSE.gameObject);
    }

    public void OnSystemBackButton()
    {
        ChangeInteractable(groupSystem, false);

        StartCoroutine(OnSystemBack());
    }

    IEnumerator OnSystemBack()
    {
        yield return StartCoroutine(Fade(1, 0, groupSystem));
        yield return StartCoroutine(Fade(0, 1, groupStart));

        ChangeInteractable(groupStart, true);
        EventSystem.current.SetSelectedGameObject(startButton.gameObject);
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

    private void ChangeInteractable(CanvasGroup canvasGroup, bool active)
    {
        canvasGroup.interactable = active;
    }
}
