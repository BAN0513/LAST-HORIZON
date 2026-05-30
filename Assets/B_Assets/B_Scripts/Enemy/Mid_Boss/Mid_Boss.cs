using System.Collections;
using TMPro;
using UnityEngine;

public class Mid_Boss : Enemy_Humanoid
{

    [Header("二段目の攻撃に派生する確率")]
    [Range(0, 100)][SerializeField] private float melee2Probability = 50;

    private EnemyShieldController _shieldController;

    private Mid_BossAnimatorController mid_BossAnimatorController;

    protected override void Start()
    {
        base.Start();

        _shieldController = GetComponentInChildren<EnemyShieldController>();
        _shieldController.Enemy = this;

        mid_BossAnimatorController = GetComponent<Mid_BossAnimatorController>();
    }

    protected override void Update()
    {
        base.Update();
        CheckPlayerAttack();
    }

    private void CheckPlayerAttack()
    {
        if (isAnimation || mid_BossAnimatorController.CheckCurrentAnim("Hit")) { return; }
        float blockDistance = 3;  //この値より距離が遠いとブロックしない
        float blockDot = 0.5f;    //この値よりDotが低いとブロックしない

        if (isAnimation)
        {
            mid_BossAnimatorController.SetBoolAnim(Mid_BossAnimatorController.Mid_BossAnimation.Block, false);
        }
        else if (playerAnimationController.IsAttackMove && distance <= blockDistance && dot > blockDot)
        {

            mid_BossAnimatorController.SetBoolAnim(Mid_BossAnimatorController.Mid_BossAnimation.Block, true);
            agent.isStopped = true;
            isLookPlayer = false;
            _shieldController.SetColliderActive(true);

        }
    }
    protected override void AttackAction()
    {
        AttackMove();
    }

    private void AttackMove()
    {
        //もし後退中なら後退を止める
        if (backMoveCor != null)
        {
            mid_BossAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.BackMove, false);
            StopCoroutine(backMoveCor);
        }

        //一定距離近づくと止まるのでstoppingDistanceを0にする
        agent.stoppingDistance = 0;

        //一定距離近づくと攻撃する
        if (distance <= attackDis)
        {
            LookPlayerChange(false);
            rand = 0;

            float attackDot = 0.4f;

            if (dot >= attackDot && !mid_BossAnimatorController.CheckCurrentAnim("RotationAttack"))
            {
                mid_BossAnimatorController.SetTriggerAnim(Mid_BossAnimatorController.Mid_BossAnimation.Melee1);
            }
            else if (dot < attackDot && !mid_BossAnimatorController.CheckCurrentAnim("Melee1"))
            {
                mid_BossAnimatorController.SetTriggerAnim(Mid_BossAnimatorController.Mid_BossAnimation.RotationAttack);
            }
        }
    }

    //ここから下はAnimator関連の関数
    public void Melee2()
    {
        int rand = Random.Range(1, 101);
        float secondAttackDis = 3;     //この値より距離が遠いと二段目の攻撃に派生しない
        float secondAttackDot = 0.4f;  //この値よりDotが高いと前方攻撃、低いと360度攻撃

        //一定確率で二段目の攻撃に派生する
        if (rand <= melee2Probability && distance <= secondAttackDis)
        {
            if (dot >= secondAttackDot)
            {
                mid_BossAnimatorController.SetTriggerAnim(Mid_BossAnimatorController.Mid_BossAnimation.Melee2);
            }
            else
            {
                mid_BossAnimatorController.SetTriggerAnim(Mid_BossAnimatorController.Mid_BossAnimation.RotationAttack);
            }
        }
        else
        {
            InitAll();
        }
    }

    //攻撃のアニメーションが終わったら全部初期化する
    public override void Init()
    {
        base.Init();
    }

    public override void InitAnim()
    {
        base.InitAnim();
    }

    public override void InitAll()
    {
        base.InitAll();
        mid_BossAnimatorController.ResetTriggerAnim(Mid_BossAnimatorController.Mid_BossAnimation.Melee1);
        mid_BossAnimatorController.ResetTriggerAnim(Mid_BossAnimatorController.Mid_BossAnimation.Melee2);
        mid_BossAnimatorController.ResetTriggerAnim(Mid_BossAnimatorController.Mid_BossAnimation.RotationAttack);
        mid_BossAnimatorController.ResetTriggerAnim(Mid_BossAnimatorController.Mid_BossAnimation.Block);
    }

    public void BlockEnd()
    {
        agent.isStopped = false;
        mid_BossAnimatorController.SetBoolAnim(Mid_BossAnimatorController.Mid_BossAnimation.Block, false);
        isLookPlayer = true;
        _shieldController.SetColliderActive(false);
    }
}
