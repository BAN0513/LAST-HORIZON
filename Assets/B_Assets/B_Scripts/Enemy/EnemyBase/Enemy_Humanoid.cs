using UnityEngine;

public class Enemy_Humanoid : Enemy
{
    protected EnemyAttackRollController _weaponController;

    //被弾アニメーション中かどうか
    protected bool isHit = false;

    protected override void Start()
    {
        base.Start();

        _weaponController = GetComponentInChildren<EnemyAttackRollController>();
    }

    protected override void Update()
    {
        base.Update();

        //移動アニメーションの変更処理
        MoveAnimControl();
    }


    private void MoveAnimControl()
    {
        if (IsActionAnimation || enemyBaseState != EnemyBaseState.Contact) 
        {
            enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Dash, false);
            enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Walk, false);
            return;
        }

        if (Agent.velocity.magnitude < 0.1f || Distance <= Agent.stoppingDistance)
        {
            enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Walk, false);
            //Agent.speed = enemySO.walkMoveSpeed * DebufDEX;
        }
        else
        {
            enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Walk, true);
            Agent.speed = enemySO.walkMoveSpeed * DebufDEX;

            //if (Distance >= enemySO.engageDis || Mathf.Abs(Target.position.y - transform.position.y) >= 0.5f)
            //{
            //    enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Dash, true);
            //    enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Walk, false);
            //    SetDashSpeed(); // ダッシュ速度に設定
            //}
            //else
            //{
            //    enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Walk, true);
            //    enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Dash, false);
            //    SetWalkSpeed(); // 歩き速度に設定
            //}
        }
    }

    public override void TakeDamage(int damage, SoundManager sound = null, int seNumber = -1)
    {
        base.TakeDamage(damage, sound, seNumber);

        if (!isHit && HP > 0)
        {
            enemyAnimatorController.SetTriggerAnim(EnemyAnimatorController.AnimationBase.Hit);
            SetLookPlayerAndEnemyStop(false, true);
            isHit = true;
        }
    }

    public override void Init()
    {
        base.Init();
        isHit = false;
    }

    protected override void Death()
    {
        base.Death();

        if (_weaponController != null)
        {
            _weaponController.SetColliderActive(false);
        }
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
