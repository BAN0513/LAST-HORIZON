using System.Collections;
using UnityEngine;

public class Enemy_1 : Enemy_Humanoid
{
    [Header("“ñ’i–Ú‚ÌUŒ‚‚É”h¶‚·‚éŠm—¦")]
    [Range(0, 100)][SerializeField] private float melee2Probability = 50;

    private Enemy_1AnimatorController enemy_1AnimatorController;

    protected override void Start()
    {
        base.Start();

        enemy_1AnimatorController = GetComponent<Enemy_1AnimatorController>();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void ShortDistanceAction()
    {
        Enemy_1ActionSO enemy_1Action = (Enemy_1ActionSO)enemySO.action[0];
        enemy_1Action.Execute(enemy_1AnimatorController);
        LookPlayerChange(false);
    }

    private void AttackMove()
    {
        //‚à‚µŒã‘Ş’†‚È‚çŒã‘Ş‚ğ~‚ß‚é
        if (backMoveCor != null)
        {
            enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.BackMove, false);
            StopCoroutine(backMoveCor);
        }

        //ˆê’è‹——£‹ß‚Ã‚­‚Æ~‚Ü‚é‚Ì‚ÅstoppingDistance‚ğ0‚É‚·‚é
        agent.stoppingDistance = 0;

        //ˆê’è‹——£‹ß‚Ã‚­‚ÆUŒ‚‚·‚é
        if (distance <= attackDis)
        {
            LookPlayerChange(false);

            if (!enemy_1AnimatorController.CheckCurrentAnim("Melee2"))
            {
                enemy_1AnimatorController.SetTriggerAnim(Enemy_1AnimatorController.Enemy_1Animation.Melee1);
            }
        }
    }

    //‚±‚±‚©‚ç‰º‚ÍAnimatorŠÖ˜A‚ÌŠÖ”
    public void Melee2()
    {
        enemy_1AnimatorController.ResetTriggerAnim(Enemy_1AnimatorController.Enemy_1Animation.Melee1);
        int rand = Random.Range(1, 101);

        //ˆê’èŠm—¦‚Å“ñ’i–Ú‚ÌUŒ‚‚É”h¶‚·‚é
        if (rand <= melee2Probability && distance <= 3 && dot > 0.3f)
        {
            enemy_1AnimatorController.SetTriggerAnim(Enemy_1AnimatorController.Enemy_1Animation.Melee2);
        }
        else
        {
            Init();
        }
    }

    //UŒ‚‚ÌƒAƒjƒ[ƒVƒ‡ƒ“‚ªI‚í‚Á‚½‚ç‘S•”‰Šú‰»‚·‚é
    public override void Init()
    {
        base.Init();
    }

    public override void InitAnim()
    {
        base.InitAnim();

        enemy_1AnimatorController.ResetTriggerAnim(Enemy_1AnimatorController.Enemy_1Animation.Melee1);
        enemy_1AnimatorController.ResetTriggerAnim(Enemy_1AnimatorController.Enemy_1Animation.Melee2);
    }
}
