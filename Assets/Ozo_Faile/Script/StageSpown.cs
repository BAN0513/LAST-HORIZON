using UnityEngine;

//カメラ操作で見上げると前のステージが映り込むタイミングがあったのでそれ回避用のプログラム
public class StageSpown : MonoBehaviour
{
    [Header("各ステージ(B1～B3)")]
    [SerializeField] GameObject Stage_Ground;

    [SerializeField] GameObject Stage_B1;

    [SerializeField] GameObject Stage_B2;

    [SerializeField] GameObject Stage_B3;

    private void Start()
    {
        Stage_B1.SetActive(false);
        Stage_B2.SetActive(false);
        Stage_B3.SetActive(false);
        Stage_Ground.SetActive(true);
    }

    void Update()
    {
        if(StageChage.IsChage)
        {
            switch(StageChage.IsStage)
            {
                case 1:
                    Stage_B1.SetActive(true);
                    Stage_B2.SetActive(false);
                    Stage_B3.SetActive(false);
                    Stage_Ground.SetActive(false);
                    break;
                case 2:
                    Stage_B1.SetActive(false);
                    Stage_B2.SetActive(true);
                    Stage_B3.SetActive(false);
                    Stage_Ground.SetActive(false);
                    break;
                case 3:
                    Stage_B1.SetActive(false);
                    Stage_B2.SetActive(false);
                    Stage_B3.SetActive(true);
                    Stage_Ground.SetActive(false);
                    break;
                default:
                    Stage_B1.SetActive(false);
                    Stage_B2.SetActive(false);
                    Stage_B3.SetActive(false);
                    Stage_Ground.SetActive(true);
                    break;

            }
        }
    }
}
