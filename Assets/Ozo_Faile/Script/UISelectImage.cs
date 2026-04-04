using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UISelectImage : MonoBehaviour
{

    [Header("セーブ")]
    [SerializeField] private UnityEngine.UI.Image Saveimage;
    [SerializeField] private Sprite[] Sprite_Save;            // UIの画像（配列）0:OFF 1:ON

    [Header("ロード")]
    [SerializeField] private UnityEngine.UI.Image Loadimage;
    [SerializeField] private Sprite[] Sprite_Load;            // UIの画像（配列）0:OFF 1:ON


    [Header("マニュアル")]
    [SerializeField] private UnityEngine.UI.Image Manualimage;
    [SerializeField] private Sprite[] Sprite_Manual;            // UIの画像（配列）0:OFF 1:ON


    [Header("システム")]
    [SerializeField] private UnityEngine.UI.Image Systemimage;
    [SerializeField] private Sprite[] Sprite_System;            // UIの画像（配列）0:OFF 1:ON


    [Header("タイトル")]
    [SerializeField] private UnityEngine.UI.Image Titleimage;
    [SerializeField] private Sprite[] Sprite_Title;            // UIの画像（配列）0:OFF 1:ON

    public string SeneName;

    public bool on = false;

    private void Update()
    {
        if (PausManeger.ToPause)
        {
            on = true;
            switch (SelectManeger.SelectNo)
            {
                case 1:
                    SelectSaveON();
                    SelectLoadOFF();
                    SelectManualOFF();
                    SelectSystemOFF();
                    SelectTitleOFF();
                    if (SelectManeger.IsDecision)
                    {
                        Debug.Log("セーブ画面");
                        SelectManeger.IsDecision = false;
                    }
                    break;
                case 2:
                    SelectSaveOFF();
                    SelectLoadON();
                    SelectManualOFF();
                    SelectSystemOFF();
                    SelectTitleOFF();
                    if (SelectManeger.IsDecision)
                    {
                        Debug.Log("ロード画面");
                        SelectManeger.IsDecision = false;
                    }
                    break;
                case 3:
                    SelectSaveOFF();
                    SelectLoadOFF();
                    SelectManualON();
                    SelectSystemOFF();
                    SelectTitleOFF();
                    if (SelectManeger.IsDecision)
                    {
                        Debug.Log("マニュアル画面");
                        SelectManeger.IsDecision = false;
                    }
                    break;
                case 4:
                    SelectSaveOFF();
                    SelectLoadOFF();
                    SelectManualOFF();
                    SelectSystemON();
                    SelectTitleOFF();
                    if (SelectManeger.IsDecision)
                    {
                        Debug.Log("システム画面");
                        SelectManeger.IsDecision = false;
                    }
                    break;
                case 5:
                    SelectSaveOFF();
                    SelectLoadOFF();
                    SelectManualOFF();
                    SelectSystemOFF();
                    SelectTitleON();
                    if (SelectManeger.IsDecision)
                    {
                        Debug.Log("タイトル画面");
                        SelectManeger.IsDecision = false;
                        Time.timeScale = 1;
                        Load();
                    }
                    break;
            }

        }
        else
            on = false;
    }

    private void Load()
    {
        SceneManager.LoadScene(SeneName);
    }

    private void SelectSaveON()
    {
        Saveimage.sprite = Sprite_Save[1];
    }
    private void SelectSaveOFF()
    {
        Saveimage.sprite = Sprite_Save[0];
    }
    private void SelectLoadON()
    {
        Loadimage.sprite = Sprite_Load[1];
    }
    private void SelectLoadOFF()
    {
        Loadimage.sprite = Sprite_Load[0];
    }
    private void SelectManualON()
    {
        Manualimage.sprite = Sprite_Manual[1];
    }
    private void SelectManualOFF()
    {
        Manualimage.sprite = Sprite_Manual[0];
    }
    private void SelectSystemON()
    {
        Systemimage.sprite = Sprite_System[1];
    }
    private void SelectSystemOFF()
    {
        Systemimage.sprite = Sprite_System[0];
    }
    private void SelectTitleON()
    {
        Titleimage.sprite = Sprite_Title[1];
    }
    private void SelectTitleOFF()
    {
        Titleimage.sprite = Sprite_Title[0];
    }
}
