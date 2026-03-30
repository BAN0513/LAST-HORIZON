using UnityEngine;
using UnityEngine.InputSystem;

public class PausManeger : MonoBehaviour
{
    [Header("ポーズ画面の各UI")]
    public GameObject BackImage;
    public GameObject BackBrownImage;
    public GameObject PauseTextImage;

    public bool IsPause = false;
    public static bool ToPause = false;

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

    private void OnPause(InputValue var)
    {
        if(!IsPause)
        {
            PauseGame();
            IsPause = true;
            ToPause = true;
        }
        else if(IsPause)
        {
            ResumeGame();
            IsPause = false;
            ToPause = false;
        }
    }
}
