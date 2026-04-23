using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameOverSelectImage : MonoBehaviour
{
    [Header("タイトル")]
    [SerializeField] private UnityEngine.UI.Image Titleimage;
    [SerializeField] private Sprite[] Sprite_Title;            // UIの画像（配列）0:OFF 1:ON

    [Header("コンティニュー")]
    [SerializeField] private UnityEngine.UI.Image Continueimage;
    [SerializeField] private Sprite[] Sprite_Continue;            // UIの画像（配列）0:OFF 1:ON

    [Header("タイトル遷移用シーン")]
    public string SeneName;
    [Header("コンティニュー用シーン")]
    public string ContinueeneName;

    public bool GameOverIsDecision = false;

    public static int SelectNo = 1;

    public static bool GameOverButtonNow;

    private void Start()
    {
        GameOverIsDecision = false;
        SelectNo = 1;
    }

    void Update()
    {
        if (PausManeger.ToGameOver && GameOverAnim.IsSelect)
        {
            switch (SelectNo)
            {
                case 1:
                    SelectTitleON();
                    SelectContinueOFF();
                    if (GameOverIsDecision)
                    {
                        Debug.Log("タイトル画面");
                        GameOverIsDecision = false;
                        Time.timeScale = 1;
                        Load();
                    }
                    break;
                case 2:
                    SelectTitleOFF();
                    SelectContinueON();
                    if (GameOverIsDecision)
                    {
                        Debug.Log("コンティニュー");
                        GameOverIsDecision = false;
                        Time.timeScale = 1;
                        Continue();
                    }
                    break;
            }

        }
    }

    private void OnDecision(InputValue var)
    {
        if (PausManeger.ToGameOver && !GameOverIsDecision)
            GameOverIsDecision = true;
    }

    private void OnSelect(InputValue var)
    {
        if (PausManeger.ToGameOver && !GameOverButtonNow)
        {
            Vector2 InputValue = var.Get<Vector2>();

            if (InputValue.y < 0)
            {
                SelectNo++;
            }
            else if (InputValue.y > 0)
            {
                SelectNo--;
            }

            if (SelectNo > 2) SelectNo = 1;
            if (SelectNo < 1) SelectNo = 2;
        }
    }

    private void Load()
    {
        SceneManager.LoadScene(SeneName);
    }

    private void Continue()
    {
        SceneManager.LoadScene(ContinueeneName);
    }

    private void SelectTitleON()
    {
        Titleimage.sprite = Sprite_Title[1];
    }
    private void SelectTitleOFF()
    {
        Titleimage.sprite = Sprite_Title[0];
    }
    private void SelectContinueON()
    {
        Continueimage.sprite = Sprite_Continue[1];
    }
    private void SelectContinueOFF()
    {
        Continueimage.sprite = Sprite_Continue[0];
    }
}
