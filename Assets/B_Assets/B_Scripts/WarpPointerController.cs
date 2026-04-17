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

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (warpObj == null) return;

        Vector3 dir = warpObj.transform.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    public void SetWarpPoint(GameObject destinetion)
    {
        gameObject.SetActive(true);
        warpObj = destinetion;
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
