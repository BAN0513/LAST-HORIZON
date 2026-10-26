using Takato;
using UnityEngine;

public class WarpPointerController : MonoBehaviour
{
    private Player_Script_New player;
    public  GameObject warpObj;
    private RectTransform rect;
    private Camera mainCamera;

    [Header("テスト用")]
    [SerializeField] private GameObject testWarp;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player_Script_New>();
        rect = GetComponent<RectTransform>();
        mainCamera = Camera.main;

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
        //ワープ地点がないなら処理しない
        if (warpObj == null) return;

        //プレイヤーの位置からワープオブジェクトの方向を求める
        Vector3 dir = warpObj.transform.position - player.transform.position;
        dir.y = 0;
        dir = dir.normalized;

        //カメラの前方向を取得
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0;

        //ワープオブジェクトの方向とカメラの前方向から角度を求める
        float angle = Vector3.SignedAngle(dir, cameraForward, Vector3.up);

        //求めた角度の分矢印のを動かす
        rect.transform.rotation = Quaternion.Euler(0, 0, angle + 90);
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
