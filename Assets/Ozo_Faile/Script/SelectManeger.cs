using UnityEngine;
using UnityEngine.InputSystem;

public class SelectManeger : MonoBehaviour
{
    [SerializeField] private GameObject SelectObj;
    [SerializeField] private GameObject SaveObj;
    [SerializeField] private GameObject LoadObj;
    [SerializeField] private GameObject ManualObj;
    [SerializeField] private GameObject SystemObj;
    [SerializeField] private GameObject TitleObj;

    public static bool IsDecision = false;
    public static int SelectNo = 1;

    private void Update()
    {
        if(PausManeger.ToPause)
        {
            switch(SelectNo)
            {
                case 1:
                    SelectObj.transform.position = SaveObj.transform.position;
                    break;
                case 2:
                    SelectObj.transform.position = LoadObj.transform.position;
                    break;
                case 3:
                    SelectObj.transform.position = ManualObj.transform.position;
                    break;
                case 4:
                    SelectObj.transform.position = SystemObj.transform.position;
                    break;
                case 5:
                    SelectObj.transform.position = TitleObj.transform.position;
                    break;
            }

        }
    }

    private void OnSelect(InputValue var)
    {
        if (PausManeger.ToPause)
        {
            Vector2 InputValue = var.Get<Vector2>();

            if (InputValue.y < 0)
            {
                SelectNo++;
            }
            else if (InputValue.y > 0)
            {
                SelectNo--;
            }

            if (SelectNo > 5) SelectNo = 1;
            if (SelectNo < 1) SelectNo = 5;
        }
    }
}
