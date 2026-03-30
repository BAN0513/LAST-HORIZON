using UnityEngine;

public class SaveTextSprit : MonoBehaviour
{
    public GameObject UITextObj;

    [Header("セーブ")]
    [SerializeField] private Sprite[] Sprite_UIOn;

    public bool on = false;

    private void Update()
    {
        if (on && SelectManeger.IsDecision)
        {
            Debug.Log("セーブ画面");
        }
        if (PausManeger.ToPause)
        {
            if (SelectManeger.SelectNo == 1)
            {
                on = true;
            }
            else
            {
                on = false;
            }
        }
        else on = false;
    }
}
