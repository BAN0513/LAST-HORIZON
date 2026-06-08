using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Humanoid : Enemy
{

    protected EnemyAttackRollController _weaponController;

    protected override void Start()
    {
        base.Start();

        _weaponController = GetComponentInChildren<EnemyAttackRollController>();

        if (_weaponController != null)
        {
            _weaponController.Player = playerController;
        }
    }

    protected override void Update()
    {
        base.Update();

        //移動アニメーションの変更処理
        MoveAnimControl();
    }


    private void MoveAnimControl()
    {
        if (isAnimation) 
        {
            enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Dash, false);
            enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Walk, false);
            return;
        }

        Vector3 toTarget = (Target.position - transform.position).normalized;
        Vector3 moveDir = agent.velocity.normalized;

        if (agent.velocity.magnitude < 0.1f)
        {
            enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Walk, false);
        }
        else
        {
            if (distance >= enemySO.engageDis)
            {
                enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Dash, true);
                enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Walk, false);
                SetDashSpeed(); // ダッシュ速度に設定
            }
            else
            {
                enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Walk, true);
                enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Dash, false);
                SetWalkSpeed(); // 歩き速度に設定
            }
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (!enemyAnimatorController.CheckCurrentAnim("Hit") && hp > 0)
        {
            enemyAnimatorController.SetTriggerAnim(EnemyAnimatorController.AnimationBase.Hit);

            AnimStart();
        }
    }

    public override void Init()
    {
        base.Init();
        isLookPlayer = true;
    }

    protected override void Death()
    {
        base.Death();
        _weaponController.SetColliderActive(false);
    }

    public void AttackJudgmentActive()
    {
        base.AttackJudgmentActive(_weaponController);
    }

    public void AttackJudgmentEnd()
    {
        base.AttackJudgmentEnd(_weaponController);
    }

}
