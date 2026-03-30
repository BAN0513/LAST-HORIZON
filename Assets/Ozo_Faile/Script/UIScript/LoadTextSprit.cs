using UnityEngine;

public class LoadTextSprit : MonoBehaviour
{
    [Header("ロード")]
    public Sprite LoadimageON;
    public Sprite LoadimageOFF;

    public bool on = false;

    private void Update()
    {
        if (on && SelectManeger.IsDecision)
        {
            Debug.Log("ロード画面");
        }
        if (PausManeger.ToPause)
        {
            if (SelectManeger.SelectNo == 2)
            {
                on = true;
                GetComponent<SpriteRenderer>().sprite = LoadimageON;
            }
            else
            {
                on = false;
                GetComponent<SpriteRenderer>().sprite = LoadimageOFF;
            }
        }
        else on = false;
    }
}
