using UnityEngine;

public class TitleTextSprit : MonoBehaviour
{
    [Header("タイトル")]
    public Sprite TitleimageON;
    public Sprite TitleimageOFF;

    public bool on = false;

    private void Update()
    {
        if (on && SelectManeger.IsDecision)
        {
            Debug.Log("タイトル画面");
        }
        if (PausManeger.ToPause)
        {
            if (SelectManeger.SelectNo == 5)
            {
                on = true;
                GetComponent<SpriteRenderer>().sprite = TitleimageON;
            }
            else
            {
                on = false;
                GetComponent<SpriteRenderer>().sprite = TitleimageOFF;
            }
        }
        else on = false;
    }
}
