using UnityEngine;
using UnityEngine.InputSystem;

public class PausManeger : MonoBehaviour
{
    [Header("ポーズ画面の各UI")]
    public GameObject BackImage;
    public GameObject BackBrownImage;
    public GameObject PauseTextImage;

    [Header("ゲームオーバー画面の各UI")]
    public GameObject GameOverBackImage;
    public GameObject GameOverTextImage;

    public bool IsPause = false;
    public static bool ToPause = false;

    public bool IsGameOver = false;
    public static bool ToGameOver = false;

    public void PauseGame()
    {
        BackImage.SetActive(true);
        BackBrownImage.SetActive(true);
        PauseTextImage.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        BackImage.SetActive(false);
        BackBrownImage.SetActive(false);
        PauseTextImage.SetActive(false);
        Time.timeScale = 1;
    }

    public void EndGame()
    {
        GameOverBackImage.SetActive(true);
        GameOverTextImage.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResetGame()
    {
        GameOverBackImage.SetActive(false);
        GameOverTextImage.SetActive(false);
        Time.timeScale = 1;
    }

    private void OnPause(InputValue var)
    {
        if(!IsPause && !IsGameOver)
        {
            PauseGame();
            IsPause = true;
            ToPause = true;
        }
        else if(IsPause && !IsGameOver)
        {
            ResumeGame();
            IsPause = false;
            ToPause = false;
        }
    }

    private void OnGameOver(InputValue var)
    {
        if (!IsGameOver && !IsPause)
        {
            EndGame();
            IsGameOver = true;
            ToGameOver = true;
        }
        else if (IsGameOver && !IsPause)
        {
            ResetGame();
            IsGameOver = false;
            ToGameOver = false;
        }
    }
}
