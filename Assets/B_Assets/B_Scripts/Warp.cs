using Takato;
using UnityEngine;

public class Warp : MonoBehaviour
{
    [SerializeField] private WarpManager warpManager;

    [Header("ワープ後どのステージのワープポイントに向かうか")]
    [SerializeField] private WarpManager.WarpPoint warpPoint;

    [Header("ここがメインステージかどうか")]
    [SerializeField] private bool isMainStage = false;

    private void Start()
    {
        if (isMainStage)
        {
            MainStageWarpPosSet();
        }
    }

    public void MainStageWarpPosSet()
    {
        int stageNum = SaveManager.Instance.save.stage;

        warpPoint = warpManager.GetStage(stageNum);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            if (player.CompareTag("Player"))
            {
                StartCoroutine(FadeManager.instance.FadeInOut(warpManager, warpPoint));
                warpManager.CheckStageNumber(warpPoint);
            }
        }
        else
        {
            Debug.Log("PlayerControllerが見つかりません");
        }

    }
}
