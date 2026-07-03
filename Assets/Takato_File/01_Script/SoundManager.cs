using UnityEngine;

/// <summary>
/// SoundManagerクラス
/// </summary>
public class SoundManager : MonoBehaviour
{
    [Header("音関連の設定")]
    [Space(10)]

    [Header("BGM")]
    [SerializeField] AudioClip[] BGM;
    [Header("BGMの音量")]
    [SerializeField] float BGMVolume;
    [Header("SE")]
    [SerializeField] AudioClip[] SE;
    [Header("SEの音量")]
    [SerializeField] float SEVolume;

    [Header("AudioSource")]
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource seSource;
    public AudioSource SESource
    {
        get { return seSource; }
    }

    [Header("デバッグ")]
    [SerializeField] bool enableDebugLogs = true; // デバッグログの有効化

    private void Awake()
    {
        // Inspector で割当てられていればそれを尊重する
        // そうでなければ一度だけ子を含めて取得して振り分ける
        if (bgmSource == null || seSource == null)
        {
            var sources = GetComponentsInChildren<AudioSource>(true);

            // bgmSource が null なら、まずはこのオブジェクトの AudioSource を探す
            if (bgmSource == null)
            {
                bgmSource = GetComponent<AudioSource>() ?? System.Array.Find(sources, s => s != null);
            }

            if (seSource == null)
            {
                // bgmSource と重複しないものを選ぶ
                seSource = System.Array.Find(sources, s => s != null && s != bgmSource);
            }
        }

        if (bgmSource == null)
        {
            var go = new GameObject("BGM_AudioSource");
            go.transform.SetParent(transform, false);
            bgmSource = go.AddComponent<AudioSource>();
        }

        if (seSource == null)
        {
            var go = new GameObject("SE_AudioSource");
            go.transform.SetParent(transform, false);
            seSource = go.AddComponent<AudioSource>();
        }

        // 共通設定
        bgmSource.spatialBlend = 0f;
        seSource.spatialBlend = 0f;
        bgmSource.playOnAwake = false;
        seSource.playOnAwake = false;

        if (enableDebugLogs) LogBGMState("Awake 終了");
    }

    private void Start()
    {
        bgmSource.volume = Mathf.Clamp01(BGMVolume);
        seSource.volume = Mathf.Clamp01(SEVolume);

        // 初期BGM自動再生
        if (BGM != null && BGM.Length > 0 && BGM[0] != null)
        {
            bgmSource.clip = BGM[0];
            bgmSource.loop = true;
            bgmSource.Play();
            if (enableDebugLogs) Debug.Log($"PlayBGM: 再生 BGM#0 '{BGM[0].name}' via bgmSource");
        }

        if (enableDebugLogs) LogBGMState("Start 終了");
    }

    /// <summary>
    /// SEの再生関数
    /// </summary>
    public void PlaySE(int SENumber)
    {
        if (seSource == null)
        {
            if (enableDebugLogs) Debug.LogWarning("PlaySE: seSource がありません。");
            return;
        }

        if (SE == null || SENumber < 0 || SENumber >= SE.Length || SE[SENumber] == null)
        {
            Debug.LogError("SE番号が範囲外、または SE が設定されていません。番号: " + SENumber);
            return;
        }

        seSource.PlayOneShot(SE[SENumber], Mathf.Clamp01(SEVolume));
        if (enableDebugLogs) Debug.Log($"PlaySE: 再生 SE#{SENumber} '{SE[SENumber].name}'");
    }

    public void PlaySEAtPoint(int SENumber, Vector3 position)
    {
        if (SE == null || SENumber < 0 || SENumber >= SE.Length || SE[SENumber] == null)
        {
            Debug.LogError("PlaySEAtPoint: SE番号が範囲外、または SE が設定されていません。番号: " + SENumber);
            return;
        }

        AudioSource.PlayClipAtPoint(SE[SENumber], position, Mathf.Clamp01(SEVolume)); // 3D空間で再生
    }

    /// <summary>
    /// BGMの再生関数
    /// </summary>
    public void PlayBGM(int index, bool loop = true)
    {
        if (bgmSource == null)
        {
            Debug.LogWarning("PlayBGM: bgmSource がありません。");
            return;
        }

        if (BGM == null || index < 0 || index >= BGM.Length || BGM[index] == null)
        {
            Debug.LogError("PlayBGM: BGM番号が範囲外、または BGM が設定されていません。番号: " + index);
            return;
        }

        bgmSource.clip = BGM[index];
        bgmSource.loop = loop;
        bgmSource.volume = Mathf.Clamp01(BGMVolume);
        bgmSource.Play();
        if (enableDebugLogs) Debug.Log($"PlayBGM: 再生 BGM#{index} '{BGM[index].name}' via bgmSource");
    }

    /// <summary>
    /// BGMの停止関数
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Stop();
            if (enableDebugLogs) Debug.Log("StopBGM: 停止しました。");
        }
    }

    /// <summary>
    /// セットされたBGMの音量を変更する関数
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        BGMVolume = Mathf.Clamp01(volume);
        if (bgmSource != null) bgmSource.volume = BGMVolume;
    }

    /// <summary>
    /// セットされたSEの音量を変更する関数
    /// </summary>
    public void SetSEVolume(float volume)
    {
        SEVolume = Mathf.Clamp01(volume);
        if (seSource != null) seSource.volume = SEVolume;
    }

    /// <summary>
    /// コンポーネントが有効になったときに呼ばれる関数
    /// </summary>
    private void OnEnable()
    {
        // bgmSource にクリップがあれば再生
        if (bgmSource != null && bgmSource.clip != null && !bgmSource.isPlaying)
        {
            bgmSource.Play();
            if (enableDebugLogs) Debug.Log($"OnEnable: bgmSource 再生 '{bgmSource.clip.name}'");
        }
    }

    // デバッグ用の状態出力
    private void LogBGMState(string tag = "")
    {
        if (!enableDebugLogs) return; //デバッグログが無効な場合は何もしない

        if (bgmSource == null)
        {
            Debug.Log($"LogBGMState({tag}): bgmSource == null");
            return;
        }

        var clipName = bgmSource.clip != null ? bgmSource.clip.name : "null";
        var mixerName = bgmSource.outputAudioMixerGroup != null ? bgmSource.outputAudioMixerGroup.name : "None";
        Debug.Log($"LogBGMState({tag}): clip={clipName}, isPlaying={bgmSource.isPlaying}, volume={bgmSource.volume}, mute={bgmSource.mute}, outputMixerGroup={mixerName}");
    }
}
