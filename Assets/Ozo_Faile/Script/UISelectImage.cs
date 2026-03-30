using UnityEngine;

public class UISelectImage : MonoBehaviour
{

    [Header("セーブ")]
    public Sprite SaveimageON;
    public Sprite SaveimageOFF;

    [Header("ロード")]
    public Sprite LoadimageON;
    public Sprite LoadimageOFF;

    [Header("マニュアル")]
    public Sprite ManualimageON;
    public Sprite ManualimageOFF;

    [Header("システム")]
    public Sprite SystemimageON;
    public Sprite SystemimageOFF;

    [Header("タイトル")]
    public Sprite TitleimageON;
    public Sprite TitleimageOFF;

    public bool on = false;

    private void Update()
    {
        if (on && SelectManeger.IsDecision)
        {
            Debug.Log("セーブ画面");
        }
        if (PausManeger.ToPause)
        {
            on = true;
            switch (SelectManeger.SelectNo)
            {
                case 1:
                    GetComponent<SpriteRenderer>().sprite = SaveimageON;
                    GetComponent<SpriteRenderer>().sprite = LoadimageOFF;
                    GetComponent<SpriteRenderer>().sprite = ManualimageOFF;
                    GetComponent<SpriteRenderer>().sprite = SystemimageOFF;
                    GetComponent<SpriteRenderer>().sprite = TitleimageOFF;
                    break;
                case 2:
                    GetComponent<SpriteRenderer>().sprite = SaveimageON;
                    GetComponent<SpriteRenderer>().sprite = LoadimageOFF;
                    GetComponent<SpriteRenderer>().sprite = ManualimageOFF;
                    GetComponent<SpriteRenderer>().sprite = SystemimageOFF;
                    GetComponent<SpriteRenderer>().sprite = TitleimageOFF;
                    break;
                case 3:
                    GetComponent<SpriteRenderer>().sprite = SaveimageON;
                    GetComponent<SpriteRenderer>().sprite = LoadimageOFF;
                    GetComponent<SpriteRenderer>().sprite = ManualimageOFF;
                    GetComponent<SpriteRenderer>().sprite = SystemimageOFF;
                    GetComponent<SpriteRenderer>().sprite = TitleimageOFF;
                    break;
                case 4:
                    GetComponent<SpriteRenderer>().sprite = SaveimageON;
                    GetComponent<SpriteRenderer>().sprite = LoadimageOFF;
                    GetComponent<SpriteRenderer>().sprite = ManualimageOFF;
                    GetComponent<SpriteRenderer>().sprite = SystemimageOFF;
                    GetComponent<SpriteRenderer>().sprite = TitleimageOFF;
                    break;
                case 5:
                    GetComponent<SpriteRenderer>().sprite = SaveimageON;
                    GetComponent<SpriteRenderer>().sprite = LoadimageOFF;
                    GetComponent<SpriteRenderer>().sprite = ManualimageOFF;
                    GetComponent<SpriteRenderer>().sprite = SystemimageOFF;
                    GetComponent<SpriteRenderer>().sprite = TitleimageOFF;
                    break;
            }

        }
        else
            on = false;
    }
}
