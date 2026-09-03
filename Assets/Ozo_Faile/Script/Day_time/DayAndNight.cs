using UnityEngine;
using UnityEngine.UI;

public class DayAndNight : MonoBehaviour
{
    //昼夜管理用のコード

    [Header("現在時刻")]
    [SerializeField] private float nowTime = 0.0f;//確認しやすくするためにここで時間を表示中

    [Header("処理開始時間　初期値 : 1200秒(20分)")]
    [SerializeField] private float ChageDayAndNight = 1200.0f;//処理開始の時間をこちらで設定

    [Header("昼間")]
    [SerializeField] private bool day = false;//昼

    [Header("夜間")]
    [SerializeField] private bool night = false;//夜

    [Header("")]
    [Header("デバッグ用（ テキスト ）")]
    [SerializeField] private Text timeText;
    private int resultTime = 0; //デバッグ時表示される数値を切り下げて表示するためのもの

    void Start()
    {
        //一度初期化
        nowTime = 0.0f;
        day = true;
        night = true;
    }
    void Update()
    {
        nowTime += Time.deltaTime;

        if (timeText != null)
        {
            resultTime = Mathf.FloorToInt(nowTime);
            timeText.text = resultTime.ToString();
        }
        else
        {
            Debug.LogError("デバッグ用のテキストが設定されていません!");
        }

        if (nowTime <= ChageDayAndNight) return;

        if (day)
        {
            day = false;
            night = true;
        }
        else if (night)
        {
            day = true;
            night = false;
        }
        nowTime = 0.0f;
    }
}
