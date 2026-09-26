using System.Collections;
using UnityEngine;

public class Enemy_WolfBoss : Enemy_FourLegs
{
    [Header("レーザーの出現位置")]
    [SerializeField] private Transform laserSpawnPosition;

    [Header("レーザーのプレハブ")]
    [SerializeField] private GameObject laserObj;

    public Enemy_WolfBossAnimatorController wolf_Anim;

    public bool IsGuard { get; set; }

    public enum WolfBoss_Form
    {
        None,
        One,
        Two
    }
    public WolfBoss_Form Form { get; set; } = WolfBoss_Form.One;


    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (wolf_Anim.CheckCurrentAnim("DownBefore") || wolf_Anim.CheckCurrentAnim("Down")) { return; }

        base.Update();
    }

    protected override void ContactAnimation()
    {
        base.ContactAnimation();
        SetLookPlayerAndEnemyStop(false, true);
        enemyAnimatorController.SetTriggerAnim(EnemyAnimatorController.AnimationBase.WolfBoss_Contact);
        IsActionAnimation = true;
    }

    public override void TakeDamage(int damage, SoundManager sound = null, int seNumber = -1)
    {
        if (!IsGuard)
        {
            IsGuard = true;
            IsAction = true;
        }
        else
        {
            damage /= 2;
        }

        base.TakeDamage(damage, sound, seNumber);
    }

    //ここから下はAnimator関連の関数

    //攻撃のアニメーションが終わったら全部初期化する
    public override void Init()
    {
        Agent.enabled = true;
        base.Init();
    }

    public override void InitAnim()
    {
        base.InitAnim();
    }

    public override void InitAll()
    {
        base.InitAll();

        wolf_Anim.ResetAllAnim();
    }

    public void DashAttak()
    {
        wolf_Anim.SetBoolAnim(EnemyAnimatorController.AnimationBase.WolfBoss_DashAttack, true);
        AttackJudgmentActive(BodyPart.AllBody);

        Agent.enabled = false;
    }

    public void Laser()
    {
        GameObject laser = Instantiate(laserObj, laserSpawnPosition.position, Quaternion.identity);
        LaserController laserController = laser.GetComponent<LaserController>();
        laserController.Damage = currentAction.damage;
        laserController.Target = Target.gameObject;
        Destroy(laser, 5.0f);
    }
}

