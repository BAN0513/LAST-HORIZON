using UnityEngine;

public class NightCheck : MonoBehaviour
{
    [Header("監視するライト")]
    [SerializeField] private Light mainLight;

    [Header("夜になったと判定する明るさの値")]
    [SerializeField] private float NightHold = 0.2f;

    [Header("スポーンさせるエネミー")]
    [SerializeField] private GameObject EnemyPre;
    [Header("スポーン先のオブジェクト")]
    [SerializeField] private GameObject SpawnPosition;

    [Header("スポーン時間の設定")]
    [SerializeField] private float SpawnTime = 10.0f;

    [Header("スポーン確率の数値")]
    [SerializeField] private float Probability_Value = 30;

    private float time = 0.0f;//カウント用の時間


    private bool isNight = false;

    private void Start()
    {
        isNight = false;
    }

    private void Update()
    {
        if (mainLight == null)
        {
            Debug.LogError("メインのライトが設定されてません！");
            return;
        }

        bool currentlyNight = !mainLight.enabled || mainLight.intensity <= NightHold;

        if (currentlyNight && !isNight)
        {
            isNight = true;
        }
        else if (!currentlyNight && isNight)
        {
            isNight = false;
        }


        if (isNight) OnNightSpawn();
        else if(!isNight) OnDayExit();
    }

    private void OnNightSpawn()
    {
        Debug.Log("スポーン処理開始");
        time += Time.deltaTime;
        if (time > SpawnTime)
        {
            if (Probability(Probability_Value)) Instantiate(EnemyPre, SpawnPosition.transform.position, transform.rotation);

            time = 0.0f;
        }
    }
    private void OnDayExit()
    {
        Debug.Log("スポーン処理停止");
        time = 0.0f;
    }

    private bool Probability(float value)
    {
        float probabilityRate = UnityEngine.Random.value * 100.0f;

        if (value == 100.0f && probabilityRate == value)
        {
            Debug.Log("スポーン成功");
            return true;
        }
        else if (probabilityRate < value)
        {
            Debug.Log("スポーン成功");
            return true;
        }
        else
        {
            Debug.Log("スポーン失敗");
            return false;
        }
    }
}
