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
    /// 指定のインデックスでキャラクターをスポーン
    /// </summary>
    public void SpawnCharacter()
    {
        if (characterPrefab == null || characterPrefab.Length == 0) return;

        // 現在のキャラクターがあれば削除
        if (currentCharacter != null)
        {
            Destroy(currentCharacter);
        }

        // 現在のインデックスでスポーン
        currentCharacter = Instantiate(characterPrefab[currentCharacterIndex], spawnPoint.position, spawnPoint.rotation);
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
