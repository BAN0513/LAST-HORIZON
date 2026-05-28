using System;
using UnityEngine;

/// <summary>
/// キャラクター切替のマネージャー（シーン上に置くシングルトン）
/// </summary>
public class CharacterChangeController : MonoBehaviour
{
    public static CharacterChangeController Instance { get; private set; }

    [Header("キャラクターを切り替えるスクリプトの詳細")]
    [Space(10)]

    [Header("切り替えるキャラクターのプレハブ")]
    [SerializeField] private GameObject[] characterPrefabs; // 切り替えるキャラクターのプレハブ

    [Header("このマネージャーをシーン跨ぎで保持するか")]
    [SerializeField] private bool persistAcrossScenes = false;

    private GameObject currentCharacter; // 現在のキャラクター
    private int currentCharacterIndex = 0;   // 現在のキャラクターのインデックス

    /// <summary>
    /// キャラクター変更時のイベント
    /// </summary>
    public event Action<int, GameObject> OnCharacterChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (persistAcrossScenes)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        SpawnCharacter(); // 最初のキャラクターをスポーン
    }

    /// <summary>
    /// キャラクターが切り替わるたびに呼び出されるスポーン処理
    /// </summary>
    public void SpawnCharacter()
    {
        if (characterPrefabs == null || characterPrefabs.Length == 0) return;

        var prefab = characterPrefabs[currentCharacterIndex];

        // デフォルトの位置/回転
        Vector3 pos = Vector3.zero;
        Quaternion rot = Quaternion.identity;

        // 既に currentCharacter がある場合は、その位置で新しいキャラを生成してから古いのを破棄
        if (currentCharacter != null)
        {
            pos = currentCharacter.transform.position;
            rot = currentCharacter.transform.rotation;

            var newChar = Instantiate(prefab, pos, rot);
            Destroy(currentCharacter);
            currentCharacter = newChar;

            OnCharacterChanged?.Invoke(currentCharacterIndex, currentCharacter);
            return;
        }

        // currentCharacter が null の場合、シーン内の "Player" タグや同名オブジェクトから位置を引き継ぐ
        GameObject tagged = null;
        try
        {
            tagged = GameObject.FindWithTag("Player");
        }
        catch
        {
            tagged = null;
        }

        if (tagged != null)
        {
            pos = tagged.transform.position;
            rot = tagged.transform.rotation;
            Destroy(tagged);
        }
        else
        {
            var prefabName = prefab.name;
            var byName = GameObject.Find(prefabName);
            if (byName != null)
            {
                pos = byName.transform.position;
                rot = byName.transform.rotation;
                Destroy(byName);
            }
        }

        currentCharacter = Instantiate(prefab, pos, rot); // 新しいキャラクターをスポーン
        OnCharacterChanged?.Invoke(currentCharacterIndex, currentCharacter); // キャラクター変更イベントを発火
    }

    /// <summary>
    /// 次のキャラクターに切り替えてスポーンする（ループ）
    /// </summary>
    public void NextCharacter()
    {
        if (characterPrefabs == null || characterPrefabs.Length == 0) return;

        currentCharacterIndex = (currentCharacterIndex + 1) % characterPrefabs.Length;
        SpawnCharacter();
    }

    /// <summary>
    /// 前のキャラクターに切り替えてスポーンする（ループ）
    /// </summary>
    public void PreviousCharacter()
    {
        if (characterPrefabs == null || characterPrefabs.Length == 0) return;

        currentCharacterIndex = (currentCharacterIndex - 1 + characterPrefabs.Length) % characterPrefabs.Length;
        SpawnCharacter();
    }

    /// <summary>
    /// 指定インデックスのキャラクターをスポーンする
    /// </summary>
    public void SpawnCharacterAt(int index)
    {
        if (characterPrefabs == null || characterPrefabs.Length == 0) return;
        if (index < 0 || index >= characterPrefabs.Length) return;

        currentCharacterIndex = index;
        SpawnCharacter();
    }

    public int GetCurrentIndex() => currentCharacterIndex;
    public GameObject GetCurrentCharacter() => currentCharacter;
}
