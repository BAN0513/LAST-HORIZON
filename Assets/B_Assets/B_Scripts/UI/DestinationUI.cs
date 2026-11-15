using UnityEngine;
using UnityEngine.UI;

public class DestinationUI : MonoBehaviour
{
    public static DestinationUI Instance { get; private set; }

    [SerializeField] private Text text_Destination;
    public Text Text_Destination
    {
        get { return text_Destination; }
        set { text_Destination = value; }
    }

    private WarpPointerController warpPointerController;

    [Header("テスト用")]
    [SerializeField] private GameObject testWarp;

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

        warpPointerController = GetComponentInChildren<WarpPointerController>();
    }

    private void Start()
    {
        text_Destination.text = SaveManager.Instance.save.destinationText;
    }

    public void SetDestinationText(string text)
    {
        text_Destination.text = text;
    }

    public void SetWarpPoint(GameObject warpPoint)
    {
        warpPointerController.SetWarpPoint(warpPoint);
    }

    public void DestroyWarpPoint()
    {
        warpPointerController.DestroyWarpPoint();
    }


    [ContextMenu("ワープポイントセット")]
    public void Set()
    {
        SetWarpPoint(testWarp);
    }

    [ContextMenu("ワープポイント削除")]
    public void Destroy()
    {
        DestroyWarpPoint();
    }
}
