using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    public Image FadeImage;
    public float FadeSpeed = 0.01f;
    public bool Fade = false;
    public float GoalTime = 2;
    
    private float a;
    private float CountTime = 0;

    void Start()
    {
        a = FadeImage.GetComponent<Image>().color.a;
    }

    private void Update()
    {


        if(Fade)
        {
            StartCoroutine(FadeInPanel());
        }

        if(!Fade)
        {
            StartCoroutine(FadeOutPanel());
        }
    }

    public void IsFade()
    {
        Fade = true;
        while (CountTime < GoalTime)
        {
            CountTime += Time.deltaTime;
        }
        Fade = false;
        CountTime = 0;
    }

    public IEnumerator FadeInPanel()
    {
        while (a < 1) 
        {
            FadeImage.GetComponent<Image>().color += new Color(0, 0, 0, FadeSpeed);
            a += FadeSpeed;
            yield return null;
        }
    }
    public IEnumerator FadeOutPanel()
    {
        while (a > 0) 
        {
            FadeImage.GetComponent<Image>().color -= new Color(0, 0, 0, FadeSpeed);
            a -= FadeSpeed;
            yield return null;
        }
    }

    private void OnFadeIn(InputValue var)
    {
        Fade = true;
    }

    private void OnFadeOut(InputValue var)
    {
        Fade = false;
    }
}
