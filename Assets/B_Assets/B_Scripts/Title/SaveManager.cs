using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance {  get; private set; }
    public SaveData save;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        save = new SaveData();
    }

    public void SaveGame(SaveData data, int slot)
    {
        string json = JsonUtility.ToJson(data, true);

        string path = Application.persistentDataPath + $"/save_{slot}.json";


        // JSON文字列をファイルに書き込む
        File.WriteAllText(path, json);

        Debug.Log("Save successful! Path: " + path);
    }

    public void LoadGame(int slot, bool isNewGame)
    {
        string path = Application.persistentDataPath + $"/save_{slot}.json";

        //初期化
        if (isNewGame)
        {
            save = new SaveData();
            SceneManager.LoadScene("B_TestScene");
        }
        else
        {
            // セーブファイルが存在するか確認
            if (File.Exists(path))
            {
                Debug.Log(path);

                // ファイルからJSON文字列を読み込む
                string json = File.ReadAllText(path);

                // JSON文字列からGameDataオブジェクトに復元
                save = JsonUtility.FromJson<SaveData>(json);

                SceneManager.LoadScene("B_TestScene");

                // --- ここで復元したデータをゲームに反映させる ---
                // 例：
                // FindObjectOfType<GameManager>().currentLevel = gameData.level;
                // --------------------------------------------------

                Debug.Log("Load successful!");
            }
            else
            {
                //セーブデータが存在しない場合は新しく作る
                save = new SaveData();
                SceneManager.LoadScene("B_TestScene");
            }
        }
    }

    public void SaveButton(int slot)
    {
        // --- ここで現在のゲーム状態をgameDataオブジェクトに反映させる ---
        // 例：
        // gameData.level = FindObjectOfType<GameManager>().currentLevel;
        GameObject target = GameObject.FindWithTag("Enemy");
        save.playerPosition = target.transform.position;
        // ----------------------------------------------------------

        SaveGame(save, slot);
        SceneManager.LoadScene("B_TitleScene");
    }

    public void LoadButton(int slot, bool isNewGame)
    {
        LoadGame(slot,isNewGame);
    }
}
