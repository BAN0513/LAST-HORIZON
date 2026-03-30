using UnityEngine;

public class ManualTextSprit : MonoBehaviour
{
    [Header("マニュアル")]
    public Sprite ManualimageON;
    public Sprite ManualimageOFF;

    public bool on = false;

    private void Update()
    {
        if (on && SelectManeger.IsDecision)
        {
            Debug.Log("マニュアル画面");
        }
        if (PausManeger.ToPause)
        {
            if (SelectManeger.SelectNo == 3)
            {
                on = true;
                GetComponent<SpriteRenderer>().sprite = ManualimageON;
            }
            else
            {
                on = false;
                GetComponent<SpriteRenderer>().sprite = ManualimageOFF;
            }
        }
        else on = false;
    }
}
