using System.Collections;
using UnityEngine;

public class Wolf_Boss : Enemy_FourLegs
{
    //アニメーションで使うやつ
    public bool isAttack_1 { get; private set; }
    public bool isAttack_2 { get; private set; }
    public bool isDashAttacKBefore { get; private set; }
    public bool isRotationAttack { get; private set; }
    public bool isTailAttack { get; private set; }

    protected override void Start()
    {
        base.Start();
        attackProbability = enemySO.attackInitProbability;
    }

    protected override void Update()
    {
        Debug.Log(agent.isStopped);
        base.Update();
    }

    protected override void ShortDistanceAction()
    {
        //確率で行動を決める
        switch (rand)
        {
            case int r when (r > 0 && r <= attackProbability):

                break;
            default:
                DoNotAttack();
                break;
        }
    }

    protected override void MediumDistanceAction()
    {
        switch (rand)
        {
            case int r when (r > 0 && r <= attackProbability):

                break;
            default:
                DoNotAttack();
                break;
        }
    }

    protected override void LongDistanceAction()
    {
        switch (rand)
        {
            //case int r when (r > 0 && r <= attackProbability):
            //    StartCoroutine(Tornado());
            //    break;
            case int r when (r > 0 && r <= attackProbability):

                break;
            default:
                DoNotAttack();
                break;
        }
    }

    private void DoNotAttack()
    {
        //攻撃じゃなかったら攻撃の確率を上げる
        isAction = false;
        attackProbability += enemySO.attackUpProbability;

    }

    private void LookPlayerChange(bool isLook)
    {
        StopBackMoveCor();

        isLookPlayer = isLook;
        agent.isStopped = true;
    }

    private void StopBackMoveCor()
    {
        //rand = 0;
        //もし後退中なら後退を止める
        if (backMoveCor != null)
        {
            isBackMove = false;
            StopCoroutine(backMoveCor);
        }
    }


    //ここから下はAnimator関連の関数

    //攻撃のアニメーションが終わったら全部初期化する
    protected override void InitAnim()
    {
        base.InitAnim();
        isAttack_1 = false;
        isAttack_2 = false;
        isDashAttacKBefore = false;
        isRotationAttack = false;
        isTailAttack = false;

    }
}

