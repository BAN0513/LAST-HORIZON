using UnityEngine;

public class Enemy_Weak : Enemy_Humanoid
{
    public Enemy_WeakAnimatorController enemy_WeakAnimator { get; set; }

    public bool isBlocking { get; set; }

    protected override void Start()
    {
        base.Start();

        enemy_WeakAnimator = GetComponent<Enemy_WeakAnimatorController>();

        isBlocking = false;
    }

    protected override void Update()
    {
        base.Update();

        if (isActionAnimation || isDead) { return; }
        Enemy_WeakActionSO action = (Enemy_WeakActionSO)CalcAction(enemySO.action);

        if (action != null)
        {
            action.Execute(enemy_WeakAnimator);
        }
    }

    public override void TakeDamage(int damage, SoundManager sound = null, int seNumber = -1)
    {
        if (enemyAnimatorController.CheckCurrentAnim("Block"))
        {
            isBlocking = true;
            damage /= 2;
        }

        base.TakeDamage(damage, sound, seNumber);
    }

    public override void Init()
    {
        base.Init();
    }

    public override void InitAnim()
    {
        base.InitAnim();
        enemy_WeakAnimator.SetBoolAnim(Enemy_WeakAnimatorController.Enemy_WeakAnimation.Melee, false);
        enemy_WeakAnimator.SetBoolAnim(Enemy_WeakAnimatorController.Enemy_WeakAnimation.Block, false);
        enemy_WeakAnimator.ResetTriggerAnim(Enemy_WeakAnimatorController.Enemy_WeakAnimation.ChargeAttack);
        enemy_WeakAnimator.ResetTriggerAnim(Enemy_WeakAnimatorController.Enemy_WeakAnimation.BlockReaction);
    }
}
