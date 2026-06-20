using Takato;
using UnityEngine;

public class WarpPointerController : MonoBehaviour
{
    private PlayerController player;
    public GameObject warpObj;
    [SerializeField] private GameObject arrowObj;
    [SerializeField] private GameObject testWarp;

    private void Start()
    {
        player = GetComponentInParent<PlayerController>();

        SaveManager saveManager = SaveManager.Instance;

        GameObject saveWarpPoint = GameObject.Find(saveManager.save.warpPointName);
        warpObj = saveWarpPoint;

        if (warpObj == null)
        {
            arrowObj.SetActive(false);
        }
        else
        {
            arrowObj.SetActive(true);
        }
    }

    private void Update()
    {
        if (warpObj == null) return;

        Vector3 dir = warpObj.transform.position - player.transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    public void SetWarpPoint(GameObject warpPoint)
    {
        arrowObj.SetActive(true);
        warpObj = warpPoint;
    }

    public void DestroyWarpPoint()
    {
        arrowObj.SetActive(false);
        warpObj = null;
    }

    [ContextMenu("ワープポイントセット")]
    public void Set()
    {
        SetWarpPoint(testWarp);
    }
}
