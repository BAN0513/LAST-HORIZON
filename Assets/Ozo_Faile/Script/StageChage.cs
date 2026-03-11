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
        if(Chage)
        {
            StageNumber++;
            if (StageNumber > 3) StageNumber = 0;
            IsChage = true;
            Invoke(nameof(Warp), 1f);
            Chage = false;
        }
    }

    /// <summary>
    /// ステージチェンジ時のワープ関数
    /// </summary>
    private void Warp()
    {
        
        if (StageNumber == 1)
            transform.position = new Vector3(0, 1, 15);
        if (StageNumber == 2)
            transform.position = new Vector3(-15, 1, 15);
        if (StageNumber == 3)
            transform.position = new Vector3(20, 1, 15);
        if (StageNumber == 0)
            transform.position = new Vector3(0, 1, 0);

        Invoke(nameof(NotIsChage), 1f);
    }

    /// <summary>
    /// ステージチェンジ時のワープのデバッグ用
    /// </summary>
    /// <param name="var"></param>
    private void OnDv_Warp(InputValue var)
    {
        Chage = true;
    }

    /// <summary>
    /// IsChageをfalseにする関数
    /// </summary>
    private void NotIsChage()
    {
        IsChage = false; 
    }

    private void OnDestroy()
    {
        // Destroy時に登録したInvokeをすべてキャンセルさせる。
        CancelInvoke();
    }
}
