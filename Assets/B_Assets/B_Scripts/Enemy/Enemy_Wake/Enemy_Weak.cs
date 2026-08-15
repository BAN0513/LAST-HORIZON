using UnityEngine;

public class Enemy_Weak : Enemy_Humanoid
{
    public Enemy_WeakAnimatorController enemy_WeakAnimator { get; set; }

    private Enemy_WeakActionSO currentAction;

    public bool IsBlocking { get; set; }
    public bool IsBlockingReaction { get; set; }
    private bool isDown = false;

    private int downDamage = 21;

    protected override void Start()
    {
        base.Start();

        enemy_WeakAnimator = GetComponent<Enemy_WeakAnimatorController>();

        IsBlocking = false;
    }

    protected override void Update()
    {
        base.Update();

        if (isActionAnimation || isDead || isDown || isHit) { return; }
        Enemy_WeakActionSO action = (Enemy_WeakActionSO)CalcAction(enemySO.action);

        if (action != null)
        {
            action.Execute(enemy_WeakAnimator);

            if (currentAction != null && currentAction != action)
            {
                CurrentActionEnd();
            }

            currentAction = action;
        }
        else if (currentAction  != null)
        {
            CurrentActionEnd();
        }
    }

    void CurrentActionEnd()
    {
        currentAction.ActionEnd(enemy_WeakAnimator);
        currentAction = null;
    }

    protected override void ContactAnimation()
    {
        base.ContactAnimation();
        SetLookPlayerAndEnemyStop(false, true);
        enemy_WeakAnimator.SetTriggerAnim(Enemy_WeakAnimatorController.Enemy_WeakAnimation.Contact);
        isActionAnimation = true;
    }

    public override void TakeDamage(int damage, SoundManager sound = null, int seNumber = -1)
    {
        if (isDead) { return; }

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
            enemy_WeakAnimator.SetBoolAnim(Enemy_WeakAnimatorController.Enemy_WeakAnimation.Down, true);
            isDown = true;
            isHit = true;
        }

        base.TakeDamage(damage, sound, seNumber);

        if (currentAction != null)
        {
            CurrentActionEnd();
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
        enemy_WeakAnimator.SetBoolAnim(Enemy_WeakAnimatorController.Enemy_WeakAnimation.Melee, false);
        enemy_WeakAnimator.SetBoolAnim(Enemy_WeakAnimatorController.Enemy_WeakAnimation.Block, false);
        enemy_WeakAnimator.ResetTriggerAnim(Enemy_WeakAnimatorController.Enemy_WeakAnimation.ChargeAttack);
        enemy_WeakAnimator.ResetTriggerAnim(Enemy_WeakAnimatorController.Enemy_WeakAnimation.BlockReaction);
        enemy_WeakAnimator.SetBoolAnim(Enemy_WeakAnimatorController.Enemy_WeakAnimation.Down, false);
    }

    [ContextMenu("DownDamage")]
    public void Down()
    {
        TakeDamage(downDamage);
    }
}
