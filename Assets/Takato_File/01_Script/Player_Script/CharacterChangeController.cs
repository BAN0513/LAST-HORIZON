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

    [Header("切り替えるキャラクターの PlayerSO (Prefab + ステータスを持つ)")]
    [SerializeField] private PlayerSO[] playerSOs;

    [Header("このマネージャーをシーン跨ぎで保持するか")]
    [SerializeField] private bool persistAcrossScenes = false;

    private GameObject currentCharacter;     // 現在のキャラクター
    private int currentCharacterIndex = 0;   // 現在のキャラクターのインデックス

    // キャラクターが切り替わるたびに、インデックスと新しいキャラクターの GameObject を引数にしてイベントを起こす。
    public event Action<int, GameObject> OnCharacterChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 既にインスタンスが存在する場合はこのオブジェクトを破棄
            return;
        }

        Instance = this;

        if (persistAcrossScenes)
        {
            DontDestroyOnLoad(gameObject); // シーン跨ぎでこのオブジェクトを保持
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
        if (playerSOs == null || playerSOs.Length == 0) return;

        var playerSO = playerSOs[currentCharacterIndex];
        if (playerSO == null || playerSO.PlayerPrefab == null) return;

        var prefab = playerSO.PlayerPrefab; // PlayerSO からプレハブを取得

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

            // PlayerController に PlayerSO を割り当てる
            AssignPlayerSOToInstance(currentCharacter, playerSO);

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
            tagged = null; // タグが存在しない場合は null を返す
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

        // PlayerController に PlayerSO を割り当てる
        AssignPlayerSOToInstance(currentCharacter, playerSO);

        OnCharacterChanged?.Invoke(currentCharacterIndex, currentCharacter); // キャラクター変更イベントを発火
    }

    private void AssignPlayerSOToInstance(GameObject instance, PlayerSO playerSO)
    {
        if (instance == null || playerSO == null) return;

        // PlayerController を探して PlayerSO をセットする
        var playerCtrl = instance.GetComponent<Takato.PlayerController>() ?? instance.GetComponentInChildren<Takato.PlayerController>();
        if (playerCtrl != null)
        {
            playerCtrl.SetPlayerSO(playerSO, preserveHPPercent: true);
        }
    }

    /// <summary>
    /// 次のキャラクターに切り替えてスポーンする（ループ）
    /// </summary>
    public void NextCharacter()
    {
        if (playerSOs == null || playerSOs.Length == 0) return;

        currentCharacterIndex = (currentCharacterIndex + 1) % playerSOs.Length;
        SpawnCharacter();
    }

    /// <summary>
    /// 前のキャラクターに切り替えてスポーンする（ループ）
    /// </summary>
    public void PreviousCharacter()
    {
        if (playerSOs == null || playerSOs.Length == 0) return;

        currentCharacterIndex = (currentCharacterIndex - 1 + playerSOs.Length) % playerSOs.Length;
        SpawnCharacter();
    }

    /// <summary>
    /// 指定インデックスのキャラクターをスポーンする
    /// </summary>
    public void SpawnCharacterAt(int index)
    {
        if (playerSOs == null || playerSOs.Length == 0) return;
        if (index < 0 || index >= playerSOs.Length) return;

        currentCharacterIndex = index;
        SpawnCharacter();
    }

    // 現在のキャラクターのインデックスと GameObject を取得するためのメソッド
    public int GetCurrentIndex() => currentCharacterIndex;

    /// 現在のキャラクターの GameObject を取得するためのメソッド
    public GameObject GetCurrentCharacter() => currentCharacter;
}
