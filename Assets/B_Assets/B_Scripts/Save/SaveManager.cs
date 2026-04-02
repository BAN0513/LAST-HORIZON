using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance {  get; private set; }
    public SaveData save;

    //プレイ時間計測用
    private float playTimeCnt = 0;
    private bool isPlay = false;

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

        //save = new SaveData();
    }

    private void Update()
    {
        if (isPlay)
        {
            playTimeCnt += Time.deltaTime;
        }
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
        playTimeCnt = 0;

        //初期化
        if (isNewGame)
        {
            save = new SaveData();
            SceneManager.LoadScene("Ozo_Scene");
        }
        else
        {
            // セーブファイルが存在するか確認
            if (File.Exists(path))
            {
                // ファイルからJSON文字列を読み込む
                string json = File.ReadAllText(path);

                // JSON文字列からGameDataオブジェクトに復元
                save = JsonUtility.FromJson<SaveData>(json);

                SceneManager.LoadScene("Ozo_Scene");

                // --- ここで復元したデータをゲームに反映させる ---

                //levelやstageは各スクリプトのスタートで反映する
                //PlayerController player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
                //player.transform.position = save.playerPosition;

                playTimeCnt = save.playTime;

                Debug.Log("Load successful!");
            }
            else
            {
                //セーブデータが存在しない場合は新しく作る
                save = new SaveData();
                SceneManager.LoadScene("Ozo_Scene");
            }
        }
        isPlay = true;
    }

    public void SaveButton(int slot)
    {
        isPlay = false;

        // --- ここで現在のゲーム状態をgameDataオブジェクトに反映させる ---
        GameObject target = GameObject.FindWithTag("Player");
        StageChage stage = target.GetComponent<StageChage>();

        save.playTime = playTimeCnt;
        save.playerPosition = target.transform.position;
        save.stage = stage.StageNumber;
        // ----------------------------------------------------------

        SaveGame(save, slot);
        SceneManager.LoadScene("B_TitleScene");
    }

    public void LoadButton(int slot, bool isNewGame)
    {
        LoadGame(slot,isNewGame);
    }

    //デバッグ用
    [ContextMenu("ReturnTitle_save1")]
    public void ReturnTitle_save1()
    {
        SaveManager.Instance.SaveButton(1);
    }

    [ContextMenu("ReturnTitle_save2")]
    public void ReturnTitle_save2()
    {
        SaveManager.Instance.SaveButton(2);
    }

    [ContextMenu("ReturnTitle_3")]
    public void ReturnTitle_3()
    {
        SaveManager.Instance.SaveButton(3);
    }
}
