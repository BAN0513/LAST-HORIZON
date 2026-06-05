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

        //後ろに下がる行動の処理
        BackMoveControl();

        //移動アニメーションの変更処理
        MoveAnimControl();
    }

    private void BackMoveControl()
    {
        if (isAnimation) { return; }
        //距離がbackActionDisより小さかったり、攻撃をしていない場合に下がる動作をする
        if (distance <= enemySO.backActionDis && !isAnimation)
        {
            if (enemyAnimatorController.CheckCurrentAnim("Walk Back")) { return; }
            backMoveCor = StartCoroutine(BackMove());
        }
    }

    IEnumerator BackMove()
    {
        agent.stoppingDistance = 0;

        while (distance <= enemySO.backMoveDis)
        {
            //敵の方向を取る
            Vector3 toTarget = (Target.position - transform.position).normalized;
            toTarget.y = 0;

            //敵の方向と反対方向を取る
            Vector3 pos = transform.position + -toTarget * enemySO.backMoveDis;

            //キャラクターの後ろを目的地として設定する
            agent.SetDestination(pos);
            yield return null;
        }
        agent.stoppingDistance = enemySO.stoopingDis;
        backMoveCor = null;
    }

    private void MoveAnimControl()
    {
        Vector3 toTarget = (Target.position - transform.position).normalized;
        Vector3 moveDir = agent.velocity.normalized;
        float moveDot = Vector3.Dot(toTarget, moveDir);

        if (agent.velocity.magnitude < 0.1f)
        {
            enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Walk, false);
            enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.BackMove, false);
        }
        else if (moveDot > 0)
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
            enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.BackMove, false);
        }
        else
        {
            enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Walk, false);
            enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.BackMove, true);
            SetWalkSpeed(); // 後退も歩き速度で
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (!enemyAnimatorController.CheckCurrentAnim("Hit") && hp > 0)
        {
            enemyAnimatorController.SetTriggerAnim(EnemyAnimatorController.AnimationBase.Hit);

            agent.isStopped = true;
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

    protected override void LookPlayerChange(bool isLook)
    {
        StopBackMoveCor();

        base.LookPlayerChange(isLook);
    }

    protected void StopBackMoveCor()
    {
        //もし後退中なら後退を止める
        if (backMoveCor != null)
        {
            enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.BackMove, false);
            StopCoroutine(backMoveCor);
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
