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

    private AudioSource audioSource;  //オーディオソースのコンポーネント

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("SoundManager に AudioSource がアタッチされていません。");
            enabled = false;
            return;
        }

        // UI/SE 向けに 2D サウンドに固定
        audioSource.spatialBlend = 0f;
    }

    private void Start()
    {
        audioSource.volume = Mathf.Clamp01(BGMVolume);
    }

    private void Update()
    {
        // BGMの自動再生
        if (audioSource != null && !audioSource.isPlaying && BGM != null && BGM.Length > 0 && BGM[0] != null)
        {
            audioSource.clip = BGM[0];
            audioSource.Play();
        }
    }

    /// <summary>
    /// SEの再生関数
    /// </summary>
    public void PlaySE(int SENumber)
    {
        if (audioSource == null)
        {
            Debug.LogWarning("PlaySE 呼び出しだが AudioSource がありません。");
            return;
        }

        if (SE == null || SENumber < 0 || SENumber >= SE.Length || SE[SENumber] == null)
        {
            Debug.LogError("SE番号が範囲外、または SE が設定されていません。番号: " + SENumber);
            return;
        }

        Debug.Log($"PlaySE: 再生 SE#{SENumber} (音量={SEVolume})");
        audioSource.PlayOneShot(SE[SENumber], Mathf.Clamp01(SEVolume));
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

    private void OnEnable()
    {
        // 再生テスト：カメラ位置で強制再生
        var soundManager = Object.FindFirstObjectByType<SoundManager>();
        if (soundManager != null && soundManager.GetComponent<AudioSource>().clip != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(soundManager.GetComponent<AudioSource>().clip, Camera.main.transform.position, 1f);
        }
    }
}
