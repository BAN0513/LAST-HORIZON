using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOverAnim : MonoBehaviour
{
    [SerializeField] private RectTransform GameOverRect;
    [SerializeField] private RectTransform TitleRect;
    [SerializeField] private RectTransform ContinueRect;

    [SerializeField] private Image GameOverImage;
    [SerializeField] private Image TitleImage;
    [SerializeField] private Image ContinueImage;

    [SerializeField] private float MoveSpeed = 10f;
    [SerializeField] private float MoveSpeed2 = 5f;
    [SerializeField] private float FadeSpeed = 2f;
    [SerializeField] private float WaitTime = 1.0f;

    public static bool IsSelect;
    //private bool AnimTrue = false;

    [SerializeField] private Vector2 StartPosG;
    [SerializeField] private Vector2 StartPosT;
    [SerializeField] private Vector2 StartPosC;

    [SerializeField] private Vector2 GameOverPos;
    [SerializeField] private Vector2 TitlePos;
    [SerializeField] private Vector2 ContinuePos;

    [SerializeField] Image FadeImage;

    [SerializeField] float speed = 0.01f;

    private float alfa;
    private float red, green, blue;

    //private bool FadeIn;
    //private bool FadeOut;


    void Start()
    {
        //FadeIn = false;
        //FadeOut = true;

        red = FadeImage.GetComponent<Image>().color.r;
        green = FadeImage.GetComponent<Image>().color.g;
        blue = FadeImage.GetComponent<Image>().color.b;

        FadeImage.GetComponent<Image>().color = new Color(red, green, blue, 0.0f);

        IsSelect = false;

        GameOverRect.anchoredPosition = StartPosG;
        TitleRect.anchoredPosition = StartPosT;
        ContinueRect.anchoredPosition = StartPosC;

        SetAlpha(GameOverImage, 0);
        SetAlpha(TitleImage, 0);
        SetAlpha(ContinueImage, 0);
    }

    private void Update()
    {
        if (PausManeger.ToGameOver)
        {
            FadeImage.GetComponent<Image>().color = new Color(red, green, blue, alfa);
            if (alfa < 1.0f) alfa += speed;
            if (alfa > 1.0f) StartCoroutine(GameOverAnimation());
        }
    }

    private IEnumerator GameOverAnimation()
    {
        yield return StartCoroutine(MoveAndFade(GameOverRect, GameOverImage, GameOverPos,MoveSpeed));

        yield return new WaitForSeconds(WaitTime);

        StartCoroutine(MoveAndFade(TitleRect, TitleImage, TitlePos,MoveSpeed2));
        yield return StartCoroutine(MoveAndFade(ContinueRect, ContinueImage, ContinuePos,MoveSpeed2));

        yield return new WaitForSeconds(0.1f);

        IsSelect = true;
        Time.timeScale = 0;
    }


    private IEnumerator MoveAndFade(RectTransform rc,Image img,Vector2 targetPos,float speed)
    {
        while ((Vector2)rc.anchoredPosition != targetPos || img.color.a < 1f) 
        {
            rc.anchoredPosition = Vector2.MoveTowards(rc.anchoredPosition, targetPos, speed * Time.deltaTime);

            Color tempColor = img.color;
            tempColor.a = Mathf.MoveTowards(tempColor.a,1f,FadeSpeed*Time.deltaTime);
            img.color = tempColor;

            yield return null;
        }
    }

    private void SetAlpha(Image img,float alpha)
    {
        Color C = img.color;
        C.a = alpha;
        img.color = C;
    }
}
