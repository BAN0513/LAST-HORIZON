using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TitleManager : MonoBehaviour
{
    private SaveData saveData;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void OnNewGameButton()
    {
        SaveManager.Instance.LoadButton(1,true);
    }

    public void OnContinue()
    {
        SaveManager.Instance.LoadButton(1,false);
    }
}
