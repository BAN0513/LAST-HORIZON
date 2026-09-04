using UnityEngine;

public class NightCheck : MonoBehaviour
{
    [Header("監視するライト")]
    [SerializeField] private Light mainLight;

    [Header("夜になったと判定する明るさの値")]
    [SerializeField] private float NightHold = 0.2f;

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
    }
    private void OnDayExit()
    {
        Debug.Log("スポーン処理停止");
    }
}
