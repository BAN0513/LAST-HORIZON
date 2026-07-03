using System.Collections.Generic;
using Takato;
using UnityEngine;

public class WarpManager : MonoBehaviour
{
    public enum WarpPoint
    {
        Stage_Main,
        Stage_1,
        Stage_2,
        Stage_3
    };

    [Header("ワープ時にキャラクターが出現する場所")]
    [SerializeField] private Transform[] warpPositions;

    [Header("各ステージ")]
    [SerializeField] private GameObject[] stages;

    private Dictionary<WarpPoint, Transform> warpControls;
    public Dictionary<WarpPoint, Transform> WarpControls
    {
        get { return warpControls; }
    }

    private Dictionary<WarpPoint, GameObject> stageControls;
    public Dictionary<WarpPoint, GameObject> StageControls
    {
        get { return stageControls; }
    }

    private Dictionary<WarpPoint, string> warpAfterText;
    public Dictionary<WarpPoint, string> WarpAfterText
    {
        get { return warpAfterText; }
    }

    private int stageNum = 0;
    public int StageNum
    {
        get { return stageNum; }
    }

    private void Start()
    {
        warpControls = new Dictionary<WarpPoint, Transform>
        {
            { WarpPoint.Stage_Main, warpPositions[0] },
            { WarpPoint.Stage_1,    warpPositions[1] },
            { WarpPoint.Stage_2,    warpPositions[2] },
            { WarpPoint.Stage_3,    warpPositions[3] },
        };

        stageControls = new Dictionary<WarpPoint, GameObject>
        {
            { WarpPoint.Stage_Main, stages[0] },
            { WarpPoint.Stage_1,    stages[1] },
            { WarpPoint.Stage_2,    stages[2] },
            { WarpPoint.Stage_3,    stages[3] },
        };

        warpAfterText = new Dictionary<WarpPoint, string>
        {
            { WarpPoint.Stage_Main, "遺跡に向かう"   },
            { WarpPoint.Stage_1,    "敵をすべて倒す" },
            { WarpPoint.Stage_2,    "敵を倒す"       },
            { WarpPoint.Stage_3,    "最後の敵を倒す" },
        };

        DestinationUI.Instance.SetDestinationText(WarpAfterText[WarpManager.WarpPoint.Stage_Main]);
        FadeManager.instance.SceneFadeOut();
    }

    //ワープ後のステージのナンバーをSaveManagerに渡しておく
    public void CheckStageNumber(WarpPoint point)
    {
        for (int i = 0; i < stageControls.Count; i++)
        {
            if (stages[i] == stageControls[point])
            {
                stageNum = i;
                break;
            }
        }
    }

    public WarpPoint GetStage(int stageNum)
    {
        GameObject stage = (GameObject)stages.GetValue(stageNum);

        if (stage == stages[0] || stage == stages[1])
        {
            return WarpPoint.Stage_1;
        }
        else if(stage == stages[2])
        { 
            return WarpPoint.Stage_2;
        }
        else if (stage == stages[3])
        {
            return WarpPoint.Stage_3;
        }

        return WarpPoint.Stage_1;
    }

    public void StageAllNotActive()
    {
        foreach(var s in stageControls)
        {
            s.Value.SetActive(false);
        }
    }
}
