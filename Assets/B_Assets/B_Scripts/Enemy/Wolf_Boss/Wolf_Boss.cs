using System.Collections;
using UnityEngine;

public class Wolf_Boss : Enemy_FourLegs
{
    public Wolf_BossAnimatorController wolf_Anim;

    private bool isKnockBack = false;
    private float reflectionSpeed = 5;
    private Vector3 dir;

    protected override void Start()
    {
        base.Start();
        attackProbability = enemySO.attackInitProbability;
    }

    protected override void Update()
    {
        if (isKnockBack)
        {
            transform.position += Vector3.Normalize(-transform.forward) * 5 * Time.deltaTime;
        }

        if (wolf_Anim.CheckCurrentAnim("DownBefore") || wolf_Anim.CheckCurrentAnim("Down")) { return; }
        Debug.Log(agent.speed);
        base.Update();

        if (wolf_Anim.CheckCurrentAnim("DashAttack"))
        {
            transform.position += Vector3.Normalize(transform.forward) * 10 * Time.deltaTime;
        }
        else if (wolf_Anim.CheckCurrentAnim("RetreatAfter"))
        {
            transform.position += Vector3.Normalize(-transform.forward) * 10 * Time.deltaTime;
        }

        if (wolf_Anim.CheckCurrentAnim("Reflection"))
        {
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(dir),
                    Time.deltaTime * reflectionSpeed
                    );
            }
        }
    }

    protected override void ShortDistanceAction()
    {
        //確率で行動を決める
        switch (rand)
        {
            case int r when (r > 0 && r <= attackProbability):
                if (dot >= 0.8f)
                {
                    Retreat();
                }
                else
                {
                    RotationAttack();
                }
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
                if (r <= attackProbability / 2)
                {
                    isLookPlayer = true;

                    if (dot >= 0.9f)
                    {
                        Attack_2();
                    }
                }
                else
                {
                    agent.stoppingDistance = shortDis;
                }
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
            case int r when (r > 0 && r <= attackProbability):
                DashAttackBefore();
                break;
            default:
                DoNotAttack();
                break;
        }
    }


    private void RotationAttack()
    {
        wolf_Anim.SetTriggerAnim(Wolf_BossAnimatorController.WolfAnimation.RotationAttack);
        LookPlayerChange(false);
    }

    private void Attack_1()
    {
        wolf_Anim.SetTriggerAnim(Wolf_BossAnimatorController.WolfAnimation.Attack_1);
        LookPlayerChange(false);
    }

    private void Retreat()
    {
        wolf_Anim.SetTriggerAnim(Wolf_BossAnimatorController.WolfAnimation.Retreat);
        LookPlayerChange(false);
    }

    private void Attack_2()
    {
        agent.stoppingDistance = 0;

        if (distance <= 10 && dot >= 0.7f)
        {
            wolf_Anim.SetTriggerAnim(Wolf_BossAnimatorController.WolfAnimation.Attack_2);
            LookPlayerChange(false);
        }
    }

    private void DashAttackBefore()
    {
        wolf_Anim.SetTriggerAnim(Wolf_BossAnimatorController.WolfAnimation.DashAttackBefore);
        LookPlayerChange(false);
    }

    private void DoNotAttack()
    {
        //攻撃じゃなかったら攻撃の確率を上げる
        isAction = false;
        attackProbability += enemySO.attackUpProbability;

        if (dot <= 0)
        {
            wolf_Anim.SetTriggerAnim(Wolf_BossAnimatorController.WolfAnimation.Reflection);
            LookPlayerChange(false);
            dir = target.position - transform.position;
            dir.y = 0;
        }
    }


    //ここから下はAnimator関連の関数

    //攻撃のアニメーションが終わったら全部初期化する
    public override void Init()
    {
        agent.enabled = true;
        base.Init();
    }

    public override void InitAnim()
    {
        base.InitAnim();
    }

    public override void InitAll()
    {
        base.InitAll();
    }

    public void DashAttak()
    {
        wolf_Anim.SetBoolAnim(EnemyAnimatorController.AnimationBase.Dash, true);
        AttackJudgmentActive(BodyPart.AllBody);

        agent.enabled = false;
    }

    public void KnockBackStart()
    {
        isKnockBack = true;
    }

    public void KnockBackEnd()
    {
        isKnockBack = false;
    }
}

