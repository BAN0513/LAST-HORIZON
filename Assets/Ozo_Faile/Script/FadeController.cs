using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// フェードイン・フェードアウト用の管理用クラス
/// </summary>
public class FadeController : MonoBehaviour
{
    [Header("フェード用画像")]
    public Image FadeImage;
    //フェードインさせる画像(UI)

    [Header("フェードさせる速度 ( 1 ～ 0.01 )")]
    public float FadeSpeed = 0.01f;
    //フェードの速度（1から引いていくので0.～で書く必要あり）

    [Header("フェード中か")]
    public bool Fade = false;
    //現在フェード中か

    private float a;
    //透明度

    void Start()
    {
        a = FadeImage.GetComponent<Image>().color.a;
    }

    private void Update()
    {
        Fade = StageChage.IsChage;

        if (Fade) //Fadeがtrueならフェードアウトを開始する。
        {
            StartCoroutine(FadeOutPanel());
        }
        if(!Fade) //Fadeがfalseならフェードインを開始する。
        {
            StartCoroutine(FadeInPanel()); 
        }
    }

    /// <summary>
    /// フェードアウト関数（IEnumerator型）
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeOutPanel()
    {
        while (a < 1) 
        {
            FadeImage.GetComponent<Image>().color += new Color(0, 0, 0, FadeSpeed);
            a += FadeSpeed;
            yield return null;
        }
    }

    /// <summary>
    /// フェードイン関数（IEnumerator型）
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeInPanel()
    {
        while (a > 0) 
        {
            FadeImage.GetComponent<Image>().color -= new Color(0, 0, 0, FadeSpeed);
            a -= FadeSpeed;
            yield return null;
        }
    }
}
