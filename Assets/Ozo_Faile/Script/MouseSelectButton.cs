using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MouseSelectButton : MonoBehaviour
{
    [SerializeField] private int ButtonSelectNO = 0;
    [SerializeField] private bool NowPaus = false;
    [SerializeField] private bool NowGameOver = false;
    [SerializeField] private bool MyTitle = false;
    [SerializeField] private string SeneName1;
    [SerializeField] private string SeneName2;

    public void OnPointerEnter()
    {
        if (NowPaus)
        {
            SelectManeger.SelectNo = ButtonSelectNO;
            SelectManeger.PausButtonNow = true;
        }
        else if (NowGameOver)
        {
            GameOverSelectImage.SelectNo = ButtonSelectNO;
            GameOverSelectImage.GameOverButtonNow = true;
        }
    }

    public void OnPointerExit()
    {
        if (NowPaus)
        {
            SelectManeger.PausButtonNow = false;
        }
        else if (NowGameOver)
        {
            GameOverSelectImage.GameOverButtonNow = false;
        }
    }

    public void OnButtonLoad()
    {
        Time.timeScale = 1;
        if (MyTitle) SceneManager.LoadScene(SeneName2);
        else if (!MyTitle) SceneManager.LoadScene(SeneName1);
    }
}
