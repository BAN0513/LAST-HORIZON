using UnityEngine;

public class Enemy_Weak : Enemy_Humanoid
{
    public Enemy_WeakAnimatorController enemy_WeakAnimator { get; set; }

    private Enemy_WeakActionSO currentAction;

    public bool isBlocking { get; set; }
    private bool isDown = false;

    private int downDamage = 0;

    protected override void Start()
    {
        base.Start();

        enemy_WeakAnimator = GetComponent<Enemy_WeakAnimatorController>();

        isBlocking = false;
    }

    protected override void Update()
    {
        base.Update();

        if (isActionAnimation || isDead || isDown) { return; }
        Enemy_WeakActionSO action = (Enemy_WeakActionSO)CalcAction(enemySO.action);

        if (action != null)
        {
            action.Execute(enemy_WeakAnimator);
            currentAction = action;
        }
        else if (currentAction  != null)
        {
            currentAction.ActionEnd(enemy_WeakAnimator);
            currentAction = null;
        }
    }

    protected override void ContactAnimation()
    {
        SetLookPlayerAndEnemyStop(false, true);
        enemy_WeakAnimator.SetTriggerAnim(Enemy_WeakAnimatorController.Enemy_WeakAnimation.Contact);
    }

    public override void TakeDamage(int damage, SoundManager sound = null, int seNumber = -1)
    {
        if (isDead) { return; }

        if (enemyAnimatorController.CheckCurrentAnim("Block"))
        {
            if (damage / 2 < downDamage)
            {
                isBlocking = true;
                damage /= 2;
            }
        }

        if (!isBlocking && !isDown && damage >= downDamage)
        {
            InitAnim();
            enemy_WeakAnimator.SetBoolAnim(Enemy_WeakAnimatorController.Enemy_WeakAnimation.Down, true);
            isDown = true;
        }

        base.TakeDamage(damage, sound, seNumber);
    }

    public override void Init()
    {
        base.Init();
        isDown = false;
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
}
