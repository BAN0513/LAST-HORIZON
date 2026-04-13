using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup groupSceneFade;

    public static FadeManager instance {  get; private set; }

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

    private void Start()
    {
        //シーンが遷移されたときにSceneFadeOutを呼ぶ
        SceneManager.sceneLoaded += SceneFadeOut;
    }

    public IEnumerator SceneFadeIn(string sceneName)
    {
        yield return StartCoroutine(Fade(0, 1, groupSceneFade));
        SceneManager.LoadScene(sceneName);
    }

    void SceneFadeOut(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Sceneの遷移を検知！！");
        StartCoroutine(Fade(1, 0, groupSceneFade));
    }

    public IEnumerator Fade(float start, float end, CanvasGroup group)
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
}
