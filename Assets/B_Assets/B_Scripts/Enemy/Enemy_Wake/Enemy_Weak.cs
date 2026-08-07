using UnityEngine;

public class Enemy_Weak : Enemy_Humanoid
{
    public Enemy_WeakAnimatorController enemy_WeakAnimator { get; set; }

    protected override void Start()
    {
        base.Start();

        enemy_WeakAnimator = GetComponent<Enemy_WeakAnimatorController>();

    }

    protected override void Update()
    {
        base.Update();

        if (isNotLoopAnimation) { return; }
        Enemy_WeakActionSO action = (Enemy_WeakActionSO)CalcAction(enemySO.action);

        if (action != null)
        {
            action.Execute(enemy_WeakAnimator);
        }
    }

    public override void Init()
    {
        base.Init();
    }
}
