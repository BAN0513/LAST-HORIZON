using System.Collections;
using UnityEngine;

public class Enemy_1 : Enemy_Humanoid
{
    [Header("二段目の攻撃に派生する確率")]
    [Range(0, 100)][SerializeField] private float melee2Probability = 50;

    private Enemy_1AnimatorController enemy_1AnimatorController;

    protected override void Start()
    {
        base.Start();

        enemy_1AnimatorController = GetComponent<Enemy_1AnimatorController>();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void AttackAction()
    {
        Enemy_1ActionSO action = (Enemy_1ActionSO)CalcAction(enemySO.action);

        if (action != null)
        {
            AnimStart();
            action.Execute(enemy_1AnimatorController);
            AttackProbabilityReset();
        }
        else
        {
            DoNotAttackAction();
        }
    }

    protected override void DoNotAttackAction()
    {
        Enemy_1ActionSO action = (Enemy_1ActionSO)CalcAction(enemySO.doNotAttack_Action);

        if (action != null)
        {
            AnimStart();
            action.Execute(enemy_1AnimatorController);
            AttackProbabilityUP();
        }
        else
        {
            Init();
        }
    }

    //ここから下はAnimator関連の関数
    public void Melee2()
    {
        enemy_1AnimatorController.ResetTriggerAnim(Enemy_1AnimatorController.Enemy_1Animation.Melee1);
        int rand = Random.Range(1, 101);

        //一定確率で二段目の攻撃に派生する
        if (rand <= melee2Probability && distance <= 3 && dot > 0.3f)
        {
            enemy_1AnimatorController.SetTriggerAnim(Enemy_1AnimatorController.Enemy_1Animation.Melee2);
        }
        else
        {
            Init();
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

        enemy_1AnimatorController.ResetTriggerAnim(Enemy_1AnimatorController.Enemy_1Animation.Melee1);
        enemy_1AnimatorController.ResetTriggerAnim(Enemy_1AnimatorController.Enemy_1Animation.Melee2);
    }
}
