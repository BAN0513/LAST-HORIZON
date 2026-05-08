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

    [Header("AudioSource（オプション: Inspectorで割当可）")]
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource seSource;

    private void Awake()
    {
        // BGM用 AudioSource を確保
        if (bgmSource == null)
        {
            bgmSource = GetComponentInChildren<AudioSource>();
            if (bgmSource == null)
            {
                var go = new GameObject("BGM_AudioSource");
                go.transform.SetParent(transform, false);
                bgmSource = go.AddComponent<AudioSource>();
            }
        }

        // SE用 AudioSource を確保（BGM用と区別）
        if (seSource == null)
        {
            // 既に拾った bgmSource と同じにならないように確認
            seSource = GetComponentInChildren<AudioSource>();
            if (seSource == null || seSource == bgmSource)
            {
                var go = new GameObject("SE_AudioSource");
                go.transform.SetParent(transform, false);
                seSource = go.AddComponent<AudioSource>();
            }
        }

        // UI/SE 向けに 2D サウンドに固定
        bgmSource.spatialBlend = 0f;
        seSource.spatialBlend = 0f;

        // SEは PlayOneShot 利用のため playOnAwake を切っておく
        seSource.playOnAwake = false;
        bgmSource.playOnAwake = false;
    }

    private void Start()
    {
        bgmSource.volume = Mathf.Clamp01(BGMVolume);
        seSource.volume = Mathf.Clamp01(SEVolume);

        // 初期BGM自動再生
        if (BGM != null && BGM.Length > 0 && BGM[0] != null && !bgmSource.isPlaying)
        {
            bgmSource.clip = BGM[0];
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    /// <summary>
    /// SEの再生関数
    /// </summary>
    public void PlaySE(int SENumber)
    {
        if (seSource == null)
        {
            Debug.LogWarning("PlaySE 呼び出しだが seSource がありません。");
            return;
        }

        if (SE == null || SENumber < 0 || SENumber >= SE.Length || SE[SENumber] == null)
        {
            Debug.LogError("SE番号が範囲外、または SE が設定されていません。番号: " + SENumber);
            return;
        }

        Debug.Log($"PlaySE: 再生 SE#{SENumber} (音量={SEVolume})");
        seSource.PlayOneShot(SE[SENumber], Mathf.Clamp01(SEVolume));
    }

    /// <summary>
    /// ワールド位置で鳴らしたい場合
    /// </summary>
    public void PlaySEAtPoint(int SENumber, Vector3 position)
    {
        if (SE == null || SENumber < 0 || SENumber >= SE.Length || SE[SENumber] == null)
        {
            Debug.LogError("PlaySEAtPoint: SE番号が範囲外、または SE が設定されていません。番号: " + SENumber);
            return;
        }

        AudioSource.PlayClipAtPoint(SE[SENumber], position, Mathf.Clamp01(SEVolume));
    }

    /// <summary>
    /// 指定BGMを再生（ループ可）
    /// </summary>
    public void PlayBGM(int index, bool loop = true)
    {
        if (bgmSource == null)
        {
            Debug.LogWarning("PlayBGM 呼び出しだが bgmSource がありません。");
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
    }

    /// <summary>
    /// BGMを停止
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }

    /// <summary>
    /// BGMの音量を設定（0.0～1.0）
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        BGMVolume = Mathf.Clamp01(volume);
        if (bgmSource != null) bgmSource.volume = BGMVolume;
    }

    /// <summary>
    /// SEの音量を設定（0.0～1.0）
    /// </summary>
    public void SetSEVolume(float volume)
    {
        SEVolume = Mathf.Clamp01(volume);
        if (seSource != null) seSource.volume = SEVolume;
    }

    /// <summary>
    /// 有効化してすぐにBGMを再生
    /// </summary>
    private void OnEnable()
    {
        // デバッグ用の簡易再生（既存挙動を維持しつつ安全に）
        if (bgmSource != null && bgmSource.clip != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(bgmSource.clip, Camera.main.transform.position, bgmSource.volume);
        }
    }
}
