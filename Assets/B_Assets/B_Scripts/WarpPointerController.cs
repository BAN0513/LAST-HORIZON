using Takato;
using UnityEngine;

public class WarpPointerController : MonoBehaviour
{
    private PlayerController player;
    public GameObject warpObj;
    [SerializeField] private GameObject testWarp;

    private void Start()
    {
        player = GetComponentInParent<PlayerController>();

        SaveManager saveManager = SaveManager.Instance;

        GameObject saveWarpPoint = GameObject.Find(saveManager.save.warpPointName);
        warpObj = saveWarpPoint;

        if (warpObj == null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (warpObj == null) return;

        Vector3 dir = warpObj.transform.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    public void SetWarpPoint(GameObject warpPoint)
    {
        gameObject.SetActive(true);
        warpObj = warpPoint;
    }

    public void DestroyWarpPoint()
    {
        gameObject.SetActive(false);
        warpObj = null;
    }

    [ContextMenu("ワープポイントセット")]
    public void Set()
    {
        SetWarpPoint(testWarp);
    }
}
