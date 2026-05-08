using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class Enemy_FourLegs : Enemy
{
    [SerializeField] private EnemyWeaponController _weaponController_LeftLeg;
    [SerializeField] private EnemyWeaponController _weaponController_RightLeg;
    [SerializeField] private EnemyWeaponController _weaponController_LeftArm;
    [SerializeField] private EnemyWeaponController _weaponController_RightArm;
    [SerializeField] private EnemyWeaponController _weaponController_Tail;
    [SerializeField] private EnemyWeaponController _weaponController_AllBody;

    public enum BodyPart
    {
        LeftLeg,
        RightLeg,
        LeftArm,
        RightArm,
        Tail,
        AllBody
    };

    private Dictionary<BodyPart, EnemyWeaponController> _weaponControllers;


    protected override void Start()
    {
        base.Start();

        _weaponControllers = new Dictionary<BodyPart, EnemyWeaponController>()
        {
           { BodyPart.LeftLeg, _weaponController_LeftLeg },
           { BodyPart.RightLeg, _weaponController_RightLeg },
           { BodyPart.LeftArm, _weaponController_LeftArm },
           { BodyPart.RightArm, _weaponController_RightArm },
           { BodyPart.Tail, _weaponController_Tail },
           { BodyPart.AllBody, _weaponController_AllBody },
        };
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void InitAnim()
    {
        base.InitAnim();
    }

    protected override void Death()
    {
        base.Death();

        foreach(var w in _weaponControllers)
        {
            AttackJudgmentEnd(w.Value);
        }
    }

    public void AttackJudgmentActive(BodyPart part) 
    {
        base.AttackJudgmentActive(_weaponControllers[part]);
    }

    public void AttackJudgmentEnd(BodyPart part)
    {
        base.AttackJudgmentEnd(_weaponControllers[part]);
    }

}
