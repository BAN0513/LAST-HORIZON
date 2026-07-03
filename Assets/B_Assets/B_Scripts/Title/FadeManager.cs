using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance {  get; private set; }

    [SerializeField] private CanvasGroup groupFade;

    //画面が暗くなっているかどうか
    private bool isFade = false;
    public bool IsFade
    {
        get { return isFade; }
        set { isFade = value; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator SceneFadeIn(string sceneName)
    {
        yield return StartCoroutine(Fade(0, 1, groupFade));
        SceneManager.LoadScene(sceneName);
    }

    //Sceneのスタートで呼ぶ
    public void SceneFadeOut()
    {
        StartCoroutine(Fade(1, 0, groupFade));
    }

    //Sceneを変更しないFadeInFadeOut
    public IEnumerator FadeInOut(WarpManager warp, WarpManager.WarpPoint point)
    {
        yield return StartCoroutine(Fade(0, 1, groupFade));
        warp.StageAllNotActive();
        DestinationUI.Instance.SetDestinationText(warp.WarpAfterText[point]);
        isFade = true;
        yield return StartCoroutine(Fade(1, 0, groupFade));
    }

    public IEnumerator Fade(float start, float end, CanvasGroup group)
    {
        if (group == null) { yield break; }
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
}
