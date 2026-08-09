using UnityEngine;

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
        if (isActionAnimation) 
        {
            enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Dash, false);
            enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Walk, false);
            return;
        }

        if (agent.velocity.magnitude < 0.1f || distance <= agent.stoppingDistance)
        {
            enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Walk, false);
        }
        else
        {
            if (distance >= enemySO.engageDis || Mathf.Abs(target.position.y - transform.position.y) >= 0.5f)
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

    public override void TakeDamage(int damage, SoundManager sound = null, int seNumber = -1)
    {
        base.TakeDamage(damage, sound, seNumber);

        if (invincibilityTimer > 0) { return; }
        if (!enemyAnimatorController.CheckCurrentAnim("Hit") && hp > 0)
        {
            enemyAnimatorController.SetTriggerAnim(EnemyAnimatorController.AnimationBase.Hit);

            SetLookPlayer(false);
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
