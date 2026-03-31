using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.EventSystems;

public class TitleManager : MonoBehaviour
{
    private SaveData saveData;

    [SerializeField] private Image[] startImages;
    [SerializeField] private Image[] loadImages;
    [SerializeField] private Button[] startButtons;
    [SerializeField] private Button[] loadButtons;
    private Coroutine cor;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        EventSystem.current.firstSelectedGameObject = startButtons[0].gameObject;
        EventSystem.current.SetSelectedGameObject(startButtons[0].gameObject);
        
        foreach (Button button in loadButtons)
        {
            button.interactable = false;
        }
    }


    public void OnNewGameButton()
    {
        SaveManager.Instance.LoadButton(1,true);
    }

    public void OnContinueButton()
    {
        foreach (var button in startButtons)
        {
            button.interactable = false;
        }
        StartCoroutine(StartImageOutLoadImageIn());
        //SaveManager.Instance.LoadButton(1,false);
    }

    IEnumerator StartImageOutLoadImageIn()
    {
        yield return cor = StartCoroutine(Fade(1, 0, startImages));
        yield return cor = StartCoroutine(Fade(0, 1, loadImages));
        cor = null;
        foreach (var button in loadButtons)
        {
            button.interactable = true;
        }
        EventSystem.current.SetSelectedGameObject(loadImages[1].gameObject);
    }

    IEnumerator Fade(float start, float end, Image[] fadeImages)
    {
        Time.timeScale = 1.0f;
        float fadeDuration = 1;

        float t = 0f;
        Color cl = fadeImages[0].color;

        cl.a = start;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            cl.a = Mathf.Lerp(start, end, t);

            foreach (var image in fadeImages)
            {
                image.color = cl;
            }
            yield return null;
        }

        cl.a = end;

        foreach (var image in fadeImages)
        {
            image.color = cl;
        }
    }

    public void OnSlotButton(int slot)
    {
        SaveManager.Instance.LoadButton(slot,false);
    }
}
