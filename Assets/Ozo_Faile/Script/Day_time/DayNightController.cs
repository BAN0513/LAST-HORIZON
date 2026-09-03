using UnityEngine;

public class DayNightController : MonoBehaviour
{
    [Header("対象のライト")]
    [SerializeField] private Light directionalLight;
    [Header("サイクルの時間")]
    [SerializeField] private float dayLength = 120f; // 時間の調整をここでします。
    public float time;

    //public Material daySkybox;
    //public Material nightSkybox;
    //public Color dayAmbientLight;
    //public Color nightAmbientLight;

    private void Start()
    {
        directionalLight.transform.localRotation = new Quaternion(50.0f, -30.0f, 0.0f, 1.0f);
    }

    void Update()
    {
        // 秒数カウント
        time += Time.deltaTime / dayLength;
        time %= 1;

        float sunAngle = time * 360f + 50f;
        directionalLight.transform.localRotation = Quaternion.Euler(sunAngle, 170f, 0f);

        if (time <= 0.23f || time >= 0.75f)
        {
            directionalLight.intensity = 0;
        }
        else if (time <= 0.25f)
        {
            directionalLight.intensity = Mathf.Lerp(0, 1, (time - 0.23f) * 50);
        }
        else if (time >= 0.73f)
        {
            directionalLight.intensity = Mathf.Lerp(1, 0, (time - 0.73f) * 50);
        }
        else
        {
            directionalLight.intensity = 1;
        }

        float sunHeight = directionalLight.transform.forward.y;

        float targetIntensity = Mathf.Clamp01(-sunHeight * 2.0f);

        directionalLight.intensity = targetIntensity;

        //if (time >= 0.25f && time < 0.75f)
        //{
        //    RenderSettings.skybox = daySkybox;
        //    RenderSettings.ambientLight = dayAmbientLight;
        //}
        //else
        //{
        //    RenderSettings.skybox = nightSkybox;
        //    RenderSettings.ambientLight = nightAmbientLight;
        //}

        //DynamicGI.UpdateEnvironment();
    }
}