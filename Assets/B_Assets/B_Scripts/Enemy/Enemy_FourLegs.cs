using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
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

        if (isAnimation)
        {
            enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Walk, false);
        }
        else
        {
            if (agent.velocity.magnitude > 0)
            {
                enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Walk, true);
                isLookPlayer = true;
            }
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
