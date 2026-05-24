using UnityEngine;

/// <summary>
/// キャラクターを切り替えるためのクラス
/// </summary>
public class CharacterChangeController : MonoBehaviour
{
    [Header("きゃらくたーを切り替えるスクリプトの詳細")]
    [Space(10)]

    [Header("切り替えるキャラクターのプレハブ")]
    [SerializeField] private GameObject[] characterPrefab; // 切り替えるキャラクターのプレハブ
    [Header("切り替えるキャラクターのスポーン位置")]
    [SerializeField] private Transform spawnPoint; // 切り替えるキャラクターのスポーン位置

    private GameObject currentCharacter; // 現在のキャラクター
    private int currentCharacterIndex = 0;   // 現在のキャラクターのインデックス

    // Start() を削除：起動時に自動スポーンしないようにする

    /// <summary>
    /// シーンにキャラクターをスポーンするメソッド
    /// </summary>
    public void SpawnCharacter()
    {
        if (characterPrefab == null || characterPrefab.Length == 0) return;

        // 既に管理しているキャラクターがあれば破棄
        if (currentCharacter != null)
        {
            Destroy(currentCharacter);
            currentCharacter = null;
        }
        else
        {
            // "Player" タグを持つオブジェクトを探して破棄
            GameObject tagged = null;
            try
            {
                tagged = GameObject.FindWithTag("Player");
            }
            catch
            {
                // "Player" タグが存在しない場合の例外は無視
                tagged = null;
            }

            if (tagged != null)
            {
                Destroy(tagged);
            }
            else
            {
                // プレハブ名と一致するルートオブジェクトを探す
                var prefabName = characterPrefab[currentCharacterIndex].name;
                var byName = GameObject.Find(prefabName);
                if (byName != null)
                {
                    Destroy(byName);
                }
            }
        }

        Vector3 pos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion rot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        // 現在のインデックスでスポーン
        currentCharacter = Instantiate(characterPrefab[currentCharacterIndex], pos, rot);
    }

    /// <summary>
    /// 次のキャラクターに切り替えてスポーンする（ループ）
    /// </summary>
    public void NextCharacter()
    {
        if (characterPrefab == null || characterPrefab.Length == 0) return;

        currentCharacterIndex = (currentCharacterIndex + 1) % characterPrefab.Length;
        SpawnCharacter();
    }
}
