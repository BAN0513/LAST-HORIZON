using UnityEngine;

public class SystemTextSprit : MonoBehaviour
{
    [Header("システム")]
    public Sprite SystemimageON;
    public Sprite SystemimageOFF;

    public bool on = false;

    private void Update()
    {
        if (on && SelectManeger.IsDecision)
        {
            Debug.Log("システム画面");
        }
        if (PausManeger.ToPause)
        {
            if (SelectManeger.SelectNo == 4)
            {
                on = true;
                GetComponent<SpriteRenderer>().sprite = SystemimageON;
            }
            else
            {
                on = false;
                GetComponent<SpriteRenderer>().sprite = SystemimageOFF;
            }
        }
        else on = false;
    }
}
