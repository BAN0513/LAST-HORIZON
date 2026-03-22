using UnityEngine;
using UnityEngine.InputSystem;

public class PausManeger : MonoBehaviour
{
    [Header("ポーズ画面の各UI")]
    public GameObject BackImage;

    public bool IsPause = false;

    public void PauseGame()
    {
        BackImage.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        BackImage.SetActive(false);
        Time.timeScale = 1;
    }

    private void OnPause(InputValue var)
    {
        if(!IsPause)
        {
            PauseGame();
            IsPause = true;
        }
        else if(IsPause)
        {
            ResumeGame();
            IsPause = false;
        }
    }
}
