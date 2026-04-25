using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class TitleUIManager : MonoBehaviour
{
    public static TitleUIManager Instance { get; private set; }

    private SaveData saveData;
    private FadeManager fadeManager;
    private SystemManager system;

    [Header("タイトル画面のキャンバスグループ")]
    public CanvasGroup groupStart;
    public CanvasGroup groupLoad;
    public CanvasGroup groupSystem;
    public CanvasGroup groupCharacterSelect;
    public CanvasGroup groupCharacterSelectCheck;

    [Header("コントローラーで操作するときに一番初めに選択されるオブジェクト")]
    public GameObject screenStart;
    public GameObject screenLoad;
    public GameObject screenSystem;
    public GameObject screenCharaSelect;
    public GameObject screenCharaSelectCheck;

    [Header("スライダーの調整で使う者たち")]
    public Slider sliderSE;
    public TMP_InputField textSE;

    public Slider sliderBGM;
    public TMP_InputField textBGM;

    public Slider sliderLight;
    public TMP_InputField textLight;

    [Header("ロード画面でプレイ時間を書くためのもの")]
    [SerializeField] private Text[] playTimeText;

    [Header("CharacterSelect画面で使う確認用のイメージ画像")]
    [SerializeField] private Sprite[] charaCheckSprites;

    [Header("CharacterSelect画面の確認用画像を表示するイメージ")]
    [SerializeField] private Image charaCheckImage;

    //コントローラーが接続されているかどうか
    private bool isGamePadConnection = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        system = SystemManager.instance;
        fadeManager = FadeManager.instance;

        //マウスは自由に動かせるようにする
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        CanvasGroupSetting();

        //画面の明るさの設定
        float lightValue = system.valueLight;
        RenderSettings.ambientIntensity = lightValue;

        SystemUISetting(sliderSE, textSE, system.volueSE);
        SystemUISetting(sliderBGM, textBGM, system.volueBGM);
        SystemUISetting(sliderLight, textLight, lightValue);

        SetPlayTimeText();

        CheckDevice();
    }

    //CanvasGroupの設定
    private void CanvasGroupSetting()
    {
        groupStart.transform.SetAsLastSibling();
        groupStart.alpha = 1.0f;
        ChangeInteractable(groupLoad, false);
        groupLoad.alpha = 0;
        ChangeInteractable(groupSystem, false);
        groupSystem.alpha = 0;
        ChangeInteractable(groupCharacterSelect, false);
        groupCharacterSelect.alpha = 0;
        ChangeInteractable(groupCharacterSelectCheck, false);
        groupCharacterSelectCheck.alpha = 0;
    }

    //Load画面のプレイ時間の設定
    private void SetPlayTimeText()
    {
        for (int i = 0; i < playTimeText.Length; i++)
        {
            //詳しくはSaveManagerみる。
            string path = Application.persistentDataPath + $"/save_{i + 1}.json";
            string json = File.ReadAllText(path);
            SaveData save = JsonUtility.FromJson<SaveData>(json);

            //時間の計算
            float time = save.playTime / 3600;
            float notVery = save.playTime % 3600;
            float minute = notVery / 60;
            float second = notVery % 60;

            //Textに時間設定
            playTimeText[i].text = $"{Mathf.FloorToInt(time)} : {Mathf.FloorToInt(minute)} : {Mathf.FloorToInt(second)}";
        }
    }

    //仕様デバイスのチェック
    private void CheckDevice()
    {
        // デバイス一覧を取得
        foreach (var device in InputSystem.devices)
        {
            // デバイス名をログ出力
            Debug.Log(device.name);

            //GamePadに後々変更する。今はデバッグ用でKeybordを使用する
            if (device.name == "Keyboar")
            {
                //キーボードじゃなかったらカーソルをロックして見えなくする
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                isGamePadConnection = true;

                //スティックで動かせるようにオブジェクトを選択状態にする
                EventSystem.current.firstSelectedGameObject = screenStart;
                EventSystem.current.SetSelectedGameObject(screenStart);
            }
        }
    }

    //System画面のスライダーとテキストの調整
    private void SystemUISetting(Slider slider, TMP_InputField text, float value)
    {
        slider.minValue = 0;
        slider.maxValue = 10;
        slider.value = value * 10 - 10;
        text.text = slider.value.ToString();
    }

    //Load画面のスロットが押されたときの処理
    public void Slot(int slot)
    {
        SaveManager.Instance.LoadGame(slot);
    }

    //CharacterSelect画面でCharacterのボタンが押されたときの処理
    public void CharacterSelect(ButtonControl.ButtonType type)
    {
        //確認画面を出す
        StartCoroutine(FadeInOutControl(groupCharacterSelectCheck, null, screenCharaSelectCheck, null, groupCharacterSelect));

        //確認画面の文字を設定
        switch (type)
        {
            case ButtonControl.ButtonType.Characetr_Sword:
                charaCheckImage.sprite = charaCheckSprites[0];
                break;
            case ButtonControl.ButtonType.Character_GreatSword:
                charaCheckImage.sprite = charaCheckSprites[1];
                break;
            case ButtonControl.ButtonType.Character_Wizard:
                charaCheckImage.sprite = charaCheckSprites[2];
                break;
        }
    }

    //確認画面でYesが押されたときの処理
    public void CharacterSelectCheck_YES()
    {
        //剣士
        if (charaCheckImage.sprite == charaCheckSprites[0])
        {
            SaveManager.Instance.NewGame(SaveData.Character.Sword);
        }
        //大剣使い
        else if (charaCheckImage.sprite == charaCheckSprites[1])
        {
            SaveManager.Instance.NewGame(SaveData.Character.GreateSword);
        }
        //魔法使い
        else
        {
            SaveManager.Instance.NewGame(SaveData.Character.Wizard);
        }
    }

    //Sliderが動かされた時呼ばれるの処理
    public void SystemChage(Slider slider, TMP_InputField text, Action<float> action)
    {
        float value = slider.value / 10 + 1;
        text.text = slider.value.ToString("F0");
        action(value);
    }

    public void SEChange(float value) { system.volueSE = value; }

    public void BGMChange(float value) { system.volueBGM = value; }

    public void LightChange(float value) 
    {
        system.valueLight = value;
        RenderSettings.ambientIntensity = value;
    }

    //Textが書き換えられたときにSliderを動かす処理
    public void SliderMove(TMP_InputField text, Slider slider)
    {
        if (text.text == "")
        {
            text.text = slider.value.ToString("F0");
        }
        else
        {
            float num = float.Parse(text.text);
            if (num < 0 || num > 10)
            {
                RectTransform rect = text.GetComponent<RectTransform>();
                rect.right = new Vector3(0, 0, 0);
                text.text = slider.value.ToString("F0");
            }
            else
            {
                slider.value = num;
            }
        }

    }

    //notFadeGroupはFadeはしないけど判定は消したりつけたりしたいときに使う
    public IEnumerator FadeInOutControl(CanvasGroup inGroup, CanvasGroup outGroup, GameObject setSelectObj, CanvasGroup notFadeInGroup = null, CanvasGroup notFadeOutGroup = null)
    {
        //FadeInするグループがあれば一番下に持っていく
        if (inGroup != null) { inGroup.transform.SetAsLastSibling(); }

        //FadeOutしないけど判定は消す
        if (notFadeOutGroup != null) { ChangeInteractable(notFadeOutGroup, false); }
        //FadeOutするグループの判定を消す
        ChangeInteractable(outGroup, false);

        //FadeIn,FadeOutする
        yield return StartCoroutine(fadeManager.Fade(1, 0, outGroup));
        yield return StartCoroutine(fadeManager.Fade(0, 1, inGroup));

        //GamePadが接続されているならFadeInの後オブジェクトを選択された状態にする
        if (isGamePadConnection)
        {
            EventSystem.current.SetSelectedGameObject(setSelectObj);
        }

        //FadeInしたグループの判定を出す
        ChangeInteractable(inGroup, true);

        //FadeInしないけど判定出したり、一番下に持って行ったりする
        if (notFadeInGroup != null) { ChangeInteractable(notFadeInGroup, true); }
        if (notFadeInGroup != null) { notFadeInGroup.transform.SetAsLastSibling(); }
    }

    //判定の変更
    private void ChangeInteractable(CanvasGroup canvasGroup, bool active)
    {
        if (canvasGroup == null) { return; }
        canvasGroup.interactable = active;
    }

    //ESCキーが押されたときの処理
    private void OnEsc(InputValue value)
    {
        //Load->Start
        if (groupLoad.alpha == 1)
        {
            StartCoroutine(FadeInOutControl(groupStart, groupLoad, screenStart));
        }
        //System->Start
        else if (groupSystem.alpha == 1)
        {
            StartCoroutine(FadeInOutControl(groupStart, groupSystem, screenStart));
        }
        //CharacterSelectCheckを消す
        else if (groupCharacterSelectCheck.alpha == 1)
        {
            StartCoroutine(FadeInOutControl(null, groupCharacterSelectCheck, screenCharaSelect, groupCharacterSelect, null));
        }
        //CharacterSelect->Start
        else if (groupCharacterSelect.alpha == 1)
        {
            StartCoroutine(FadeInOutControl(groupStart, groupCharacterSelect, screenStart));
        }
    }

    //これがないとESCでスタート画面に戻るとスタートボタンが選択された状態になってしまう
    private void OnClick(InputValue value)
    {
        if (!isGamePadConnection) EventSystem.current.SetSelectedGameObject(null);
    }
}
