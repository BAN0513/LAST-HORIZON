using UnityEngine;
using UnityEngine.UI;

public class DestinationUI : MonoBehaviour
{
    [SerializeField] private Text text_Destination;

    public static DestinationUI Instance { get; private set; }

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

    public void SetDestinationText(string text)
    {
        text_Destination.text = text;
    }
}
