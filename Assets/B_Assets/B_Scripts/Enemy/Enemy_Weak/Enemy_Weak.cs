using Unity.AI.Navigation;
using UnityEngine;

public class Enemy_Weak : Enemy_Humanoid
{
    private bool isDown = false;
    private int downDamage = 21;


    public Enemy_WeakAnimatorController Enemy_WeakAnimator { get; private set; }
    public bool IsBlocking { get; set; }
    public bool IsBlockingReaction { get; set; }

    //中ボスの能力で召喚されたとき用の変数
    public Enemy_Wizard Wizard { private get; set; }

    protected override void Start()
    {
        base.Start();

        Enemy_WeakAnimator = GetComponent<Enemy_WeakAnimatorController>();

        IsBlocking = false;
    }

    protected override void Update()
    {
        //Debug.Log("dis" + distance);
        if (isDown || isHit) { return; }

        base.Update();
    }

    protected override void ContactAnimation()
    {
        base.ContactAnimation();
        SetLookPlayerAndEnemyStop(false, true);
        Enemy_WeakAnimator.SetTriggerAnim(EnemyAnimatorController.AnimationBase.Weak_Contact);
        IsActionAnimation = true;
    }

    public override void TakeDamage(int damage, SoundManager sound = null, int seNumber = -1)
    {
        if (enemyBaseState == EnemyBaseState.Dead) { return; }

        if (IsBlocking || IsBlockingReaction)
        {
            if (damage / 2 < downDamage)
            {
                IsBlockingReaction = true;
                isHit = true;
                damage /= 2;
            }
        }

        if (!IsBlocking && !isDown && damage >= downDamage)
        {
            InitAnim();
            Enemy_WeakAnimator.SetBoolAnim(EnemyAnimatorController.AnimationBase.Weak_Down, true);
            isDown = true;
            isHit = true;
        }

        base.TakeDamage(damage, sound, seNumber);

        if (!IsBlocking)
        {
            IsBlocking = true;
        }
    }    

    public override void Init()
    {
        base.Init();
        isDown = false;
        IsBlockingReaction = false;
    }

    public override void InitAnim()
    {
        base.InitAnim();
        Enemy_WeakAnimator.SetBoolAnim(EnemyAnimatorController.AnimationBase.Weak_Melee, false);
        Enemy_WeakAnimator.SetBoolAnim(EnemyAnimatorController.AnimationBase.Weak_Block, false);
        Enemy_WeakAnimator.ResetTriggerAnim(EnemyAnimatorController.AnimationBase.Weak_ChargeAttack);
        Enemy_WeakAnimator.ResetTriggerAnim(EnemyAnimatorController.AnimationBase.Weak_BlockReaction);
        Enemy_WeakAnimator.SetBoolAnim(EnemyAnimatorController.AnimationBase.Weak_Down, false);
    }

    protected override void Death()
    {
        base.Death();
        
        if (Wizard == null) { return; }

        Wizard.SummonEnemyCount--;
    }

    [ContextMenu("DownDamage")]
    public void Down()
    {
        TakeDamage(downDamage);
    }
}
