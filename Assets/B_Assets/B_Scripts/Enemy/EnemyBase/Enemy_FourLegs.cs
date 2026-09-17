using System.Collections.Generic;
using UnityEngine;

public class Enemy_FourLegs : Enemy
{
    [Header("ìGÇÃçUåÇÇÃìñÇΩÇËîªíË")]
    [SerializeField] private EnemyAttackRollController[] _weaponController_LeftLeg;
    [SerializeField] private EnemyAttackRollController[] _weaponController_RightLeg;
    [SerializeField] private EnemyAttackRollController[] _weaponController_LeftArm;
    [SerializeField] private EnemyAttackRollController[] _weaponController_RightArm;
    [SerializeField] private EnemyAttackRollController[] _weaponController_Tail;
    [SerializeField] private EnemyAttackRollController[] _weaponController_AllBody;

    public enum BodyPart
    {
        LeftLeg,
        RightLeg,
        LeftArm,
        RightArm,
        Tail,
        AllBody
    };

    private Dictionary<BodyPart, EnemyAttackRollController[]> _weaponControllers;

    private enum WalkType
    {
        Walk,
        Run
    };
    private WalkType walkType = WalkType.Run;

    float runTimer = 0.0f;
    float runTime = 1.0f;

    protected override void Start()
    {
        base.Start();
        isLookPlayer = false;

        _weaponControllers = new Dictionary<BodyPart, EnemyAttackRollController[]>()
        {
           { BodyPart.LeftLeg, _weaponController_LeftLeg },
           { BodyPart.RightLeg, _weaponController_RightLeg },
           { BodyPart.LeftArm, _weaponController_LeftArm },
           { BodyPart.RightArm, _weaponController_RightArm },
           { BodyPart.Tail, _weaponController_Tail },
           { BodyPart.AllBody, _weaponController_AllBody },
        };

        foreach (var w in _weaponControllers)
        {
            for (int i = 0; i < w.Value.Length; i++)
            {
                w.Value[i].Player = playerController;
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        MoveAnimControl();
    }

    private void MoveAnimControl()
    {
        if (runTimer > 0.0f)
        {
            runTimer -= Time.deltaTime;
        }

        if (IsActionAnimation || enemyBaseState != EnemyBaseState.Contact)
        {
            enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Dash, false);
            enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Walk, false);
            return;
        }
        else if (Agent.velocity.magnitude > 0)
        {
            if (Distance >= 20.0f && walkType == WalkType.Walk)
            {
                enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Dash, true);
                enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Walk, false);
                Agent.speed = enemySO.dashMoveSpeed * DebufDEX;
                walkType = WalkType.Run;
                runTimer = runTime;
            }
            else if (Distance < 20.0f && walkType == WalkType.Run && runTimer <= 0.0f)
            {
                enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Walk, true);
                enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Dash, false);
                Agent.speed = enemySO.walkMoveSpeed * DebufDEX;
                walkType = WalkType.Walk;
            }
            isLookPlayer = true;
        }
        else
        {
            enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Walk, false);
            isLookPlayer = false;
        }
    }

    public override void Init()
    {
        base.Init();
        isLookPlayer = false;
    }

    public override void InitAnim()
    {
        base.InitAnim();
    }

    public override void InitAll()
    {
        base.InitAll();
    }

    protected override void Death()
    {
        base.Death();

        foreach (var w in _weaponControllers)
        {
            for (int i = 0; i < w.Value.Length; i++)
            {
                AttackJudgmentEnd(w.Value[i]);
            }
        }
    }

    public void AttackJudgmentActive(BodyPart part)
    {
        for (int i = 0; i < _weaponControllers[part].Length; i++)
        {
            base.AttackJudgmentActive(_weaponControllers[part][i]);
        }
    }

    public void AttackJudgmentEnd(BodyPart part)
    {
        for (int i = 0; i < _weaponControllers[part].Length; i++)
        {
            base.AttackJudgmentEnd(_weaponControllers[part][i]);
        }
    }

}
