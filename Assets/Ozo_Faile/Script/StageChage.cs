using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

///<summary>
/// ステージ移動の管理するクラス
/// (現在は仮でPキーを入力するとワープする)
/// </summary>
public class StageChage : MonoBehaviour
{
    [Header("ステージチェンジ")]
    public bool Chage = false;//ステージチェンジ中かどうか

    [Header("ステージナンバー（0～3）")]
    public int StageNumber = 0;//現在のステージナンバー

    [Header("スポーンポイント(B1～B3)")]
    [SerializeField] GameObject SpawnPoint_B1;

    [SerializeField] GameObject SpawnPoint_B2;

    [SerializeField] GameObject SpawnPoint_B3;

    public static bool IsChage = false;//FadeController共有用変数

    private void Start()
    {
        if (StageNumber < 0 || StageNumber > 3)
        { 
            StageNumber = 0;
            transform.position = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        if (Chage)
        {
            //IsChage = true;
            Invoke(nameof(Warp), 1f);
        }
    }

    /// <summary>
    /// ステージチェンジ時のワープ関数
    /// </summary>
    private void Warp()
    {
        //一旦キャラクターコントロールを無効化する

        if (StageNumber == 1 /*&& !EndWarp*/)
            transform.position = SpawnPoint_B1.transform.position;
        else if (StageNumber == 2 /*&& !EndWarp*/)
            transform.position = SpawnPoint_B2.transform.position;
        else if (StageNumber == 3 /*&& !EndWarp*/)
            transform.position = SpawnPoint_B3.transform.position;
        else if (StageNumber == 0 /*&& !EndWarp*/)
            transform.position = new Vector3(0, 0, 0);

        //EndWarp = true;

        Chage = false;

        Invoke(nameof(NotIsChage), 1f);
    }

    /// <summary>
    /// ステージチェンジ時のワープのデバッグ用
    /// </summary>
    /// <param name="var"></param>
    private void OnDv_Warp(InputValue var)
    {
        if (!PausManeger.ToPause && !PausManeger.ToGameOver)
        {
            Chage = true;
            StageNumber++;
            if (StageNumber > 3) StageNumber = 0;
            IsChage = true;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Warps")
        {
            Chage = true;
            StageNumber++;
            if (StageNumber > 3) StageNumber = 0;
            IsChage = true;
            //GoWarp();
        }
    }

    /// <summary>
    /// IsChageをfalseにする関数
    /// </summary>
    private void NotIsChage()
    {
        IsChage = false;
        //_CharacterController.enabled = false;
        //EndWarp = false;
    }

    private void OnDestroy()
    {
        // Destroy時に登録したInvokeをすべてキャンセルさせる。
        CancelInvoke();
    }
}
