using System;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Wizard : Enemy_Humanoid
{
    private Enemy_WizardAnimatorController enemy_WizardAnimator;

    [SerializeField] private GameObject fireEffect;
    [SerializeField] private GameObject impactEffect;

    [SerializeField] private Transform[] warpPos;

    protected override void Start()
    {
        base.Start();

        enemy_WizardAnimator = GetComponent<Enemy_WizardAnimatorController>();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void AttackAction()
    {
        Enemy_WizardActionSO action = (Enemy_WizardActionSO)CalcAction(enemySO.action);

        if (action != null)
        {
            AnimStart();
            action.Execute(enemy_WizardAnimator);
            AttackProbabilityReset();
        }
        else
        {
            DoNotAttackAction();
        }
    }

    protected override void DoNotAttackAction()
    {
        if (distance <= 3)
        {
            int randomWarp = UnityEngine.Random.Range(0, warpPos.Length);
            transform.position = warpPos[randomWarp].position;
        }
        else
        {
            Init();
        }
    }

    //ここから下はAnimator関連の関数

    public void FireSpawn()
    {
        Vector3 toTarget = Target.position - transform.position + transform.forward;
        Vector3 nor = (toTarget).normalized;
        Quaternion quaternion = Quaternion.LookRotation(toTarget);

        GameObject fire = Instantiate(fireEffect, transform.position + transform.forward + transform.up, quaternion);
        FireController fireController = fire.GetComponent<FireController>();
        fireController.Damage = enemySO.damage;
        fireController.Player = playerController;

        Rigidbody rb = fire.GetComponent<Rigidbody>();

        rb.linearVelocity = nor * 5;
    }

    public void Impact()
    {
        GameObject impact = Instantiate(impactEffect, transform.position, Quaternion.identity);
        ImpactController impactController = impact.GetComponentInChildren<ImpactController>();
        impactController.damage = enemySO.damage;
        Destroy(impact, 1);
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
}
