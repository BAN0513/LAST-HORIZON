using UnityEngine;

public class Enemy_Weak : Enemy_Humanoid
{
    public Enemy_WeakAnimatorController enemy_WeakAnimator { get; set; }

    public bool IsBlocking { get; set; }
    public bool IsBlockingReaction { get; set; }
    private bool isDown = false;

    private int downDamage = 21;

    //中ボスの能力で召喚されたとき用の変数
    public Enemy_Wizard Wizard { private get; set; }

    protected override void Start()
    {
        base.Start();

        enemy_WeakAnimator = GetComponent<Enemy_WeakAnimatorController>();

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
        enemy_WeakAnimator.SetTriggerAnim(EnemyAnimatorController.AnimationBase.Weak_Contact);
        isActionAnimation = true;
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
            enemy_WeakAnimator.SetBoolAnim(EnemyAnimatorController.AnimationBase.Weak_Down, true);
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
        enemy_WeakAnimator.SetBoolAnim(EnemyAnimatorController.AnimationBase.Weak_Melee, false);
        enemy_WeakAnimator.SetBoolAnim(EnemyAnimatorController.AnimationBase.Weak_Block, false);
        enemy_WeakAnimator.ResetTriggerAnim(EnemyAnimatorController.AnimationBase.Weak_ChargeAttack);
        enemy_WeakAnimator.ResetTriggerAnim(EnemyAnimatorController.AnimationBase.Weak_BlockReaction);
        enemy_WeakAnimator.SetBoolAnim(EnemyAnimatorController.AnimationBase.Weak_Down, false);
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
