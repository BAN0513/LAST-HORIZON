using UnityEngine;
using UnityEngine.UI;

public class DestinationUI : MonoBehaviour
{
    [SerializeField] private Text text_Destination;
    public Text Text_Destination
    {
        get { return text_Destination; }
        set { text_Destination = value; }
    }

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

    private void Start()
    {
        text_Destination.text = SaveManager.Instance.save.destinationText;
    }

    public void SetDestinationText(string text)
    {
        text_Destination.text = text;
    }
}
