using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Humanoid : Enemy
{
    [Header("HPのスライダー")]
    [SerializeField] protected Slider hpSliider;

    [Header("武器のスクリプト")]
    [SerializeField] protected EnemyWeaponController _weaponController;

    protected override void Start()
    {
        base.Start();

        if (hpSliider != null)
        {
            hpSliider.maxValue = enemySO.maxHP;
            hpSliider.minValue = 0;
            hpSliider.value = enemySO.maxHP;
        }

        if (_weaponController != null)
        {
            _weaponController.Damage = enemySO.damage;
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
        //距離がbackActionDisより小さかったり、攻撃をしていない場合に下がる動作をする
        if (distance <= enemySO.backActionDis && !isAction)
        {
            if (isBackMove) { return; }
            backMoveCor = StartCoroutine(BackMove());
        }
    }

    IEnumerator BackMove()
    {
        agent.stoppingDistance = 0;

        while (distance <= enemySO.backMoveDis)
        {
            //敵の方向を取る
            Vector3 toTarget = (target.position - transform.position).normalized;
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
        Vector3 toTarget = (target.position - transform.position).normalized;
        Vector3 moveDir = agent.velocity.normalized;
        float moveDot = Vector3.Dot(toTarget, moveDir);

        if (agent.velocity.magnitude < 0.1f)
        {
            isWalking = false;
            isBackMove = false;
        }
        else if (moveDot > 0)
        {
            if (distance >= enemySO.engageDis)
            {
                isDash = true;
                isWalking = false;
                SetDashSpeed(); // ダッシュ速度に設定
            }
            else
            {
                isWalking = true;
                isDash = false;
                SetWalkSpeed(); // 歩き速度に設定
            }
            isBackMove = false;
        }
        else
        {
            isWalking = false;
            isBackMove = true;
            SetWalkSpeed(); // 後退も歩き速度で
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        hpSliider.value = hp;

        if (hp <= 0)
        {
            hpSliider.gameObject.SetActive(false);

        }
    }

    protected override void InitAnim()
    {
        base.InitAnim();
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
