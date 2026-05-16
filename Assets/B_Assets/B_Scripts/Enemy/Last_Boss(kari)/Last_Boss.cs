using System.Collections;
using UnityEngine;

public class Last_Boss : Enemy_Humanoid
{
    //    [SerializeField] private GameObject magicCircleEffect;
    //    [SerializeField] private GameObject tornadoEffect;
    //    [SerializeField] private GameObject fireEffect;
    //    [SerializeField] private GameObject impactEffect;
    //    private GameObject magicCircle;



    //    private bool bigTrickReady = false;
    //    private bool bigTrickUsed = false;

    //    //アニメーションで使うやつ
    //    public bool isChant { get; private set; }
    //    public bool isMagic { get; private set; }
    //    public bool isFire { get; private set; }
    //    public bool isSlash { get; private set; }
    //    public bool isRunJumpAttack {  get; private set; }

    //    protected override void Start()
    //    {
    //        base.Start();
    //    }

    //    protected override void Update()
    //    {
    //        //Debug.Log(distance);
    //        if (isDeath || isHit) 
    //        {
    //            Destroy(magicCircle);
    //            return; 
    //        }
    //        base.Update();
    //    }

    //    protected override void ShortDistanceAction()
    //    {
    //        //確率で行動を決める
    //        switch (rand)
    //        {
    //            case int r when (r > 0 && r <= attackProbability):
    //                StartCoroutine(Slash());
    //                break;
    //            default:
    //                DoNotAttack();
    //                break;
    //        }
    //    }

    //    protected override void MediumDistanceAction()
    //    {
    //        switch (rand)
    //        {
    //            case int r when (r > 0 && r <= attackProbability):
    //                StartCoroutine(Fire());
    //                break;
    //            default:
    //                DoNotAttack();
    //                break;
    //        }
    //    }

    //    protected override void LongDistanceAction()
    //    {
    //        switch (rand)
    //        {
    //            //case int r when (r > 0 && r <= attackProbability):
    //            //    StartCoroutine(Tornado());
    //            //    break;
    //            case int r when (r > 0 && r <= attackProbability):
    //                StartCoroutine(DashJumpAttack());
    //                break;
    //            default:
    //                DoNotAttack();
    //                break;
    //        }
    //    }

    //    private void DoNotAttack()
    //    {
    //        //攻撃じゃなかったら攻撃の確率を上げる
    //        isAnimation = false;
    //        attackProbability += enemySO.attackUpProbability;

    //    }

    //    IEnumerator Slash()
    //    {
    //        StopBackMoveCor();

    //        agent.stoppingDistance = 0;

    //        if (distance <= attackDis)
    //        {
    //            LookPlayerChange(true);
    //            isSlash = true;
    //        }

    //        yield return null;
    //    }

    //    IEnumerator DashJumpAttack()
    //    {
    //        LookPlayerChange(false);
    //        isRunJumpAttack = true;
    //        yield return null;
    //    }

    //    IEnumerator Fire()
    //    {
    //        LookPlayerChange(false);
    //        isFire = true;
    //        yield return null;
    //    }

    //    IEnumerator Tornado()
    //    {
    //        LookPlayerChange(false);
    //        isChant = true;

    //        yield return null;
    //    }



    //    //ここから下はAnimator関連の関数
    //    public void ChantStart()
    //    {
    //        magicCircle = Instantiate(magicCircleEffect, transform.position, Quaternion.identity);
    //        Destroy(magicCircle, 10);
    //    }

    //    public void ChantEnd()
    //    {
    //        isMagic = true;
    //    }

    //    public void Magic()
    //    {
    //        GameObject tornado = Instantiate(tornadoEffect, transform.position, Quaternion.identity);
    //        TornadoController tornadoController = tornado.GetComponent<TornadoController>();
    //        tornadoController.Damage = enemySO.damage;
    //        bigTrickUsed = true;
    //        bigTrickReady = false;
    //        Destroy(tornado, 15);
    //    }

    //    public void FireSpawn()
    //    {
    //        Vector3 toTarget = target.position - forward.position;
    //        Vector3 nor = (toTarget).normalized;
    //        Quaternion quaternion = Quaternion.LookRotation(toTarget);

    //        GameObject fire = Instantiate(fireEffect, forward.position, quaternion);
    //        FireController fireController = fire.GetComponent<FireController>();
    //        fireController.Damage = enemySO.damage;
    //        fireController.Player = playerController;

    //        Rigidbody rb = fire.GetComponent<Rigidbody>();

    //        rb.linearVelocity = nor * 5;
    //    }

    //    public void Impact()
    //    {
    //        GameObject impact = Instantiate(impactEffect, transform.position, Quaternion.identity);
    //        ImpactController impactController = impact.GetComponentInChildren<ImpactController>();
    //        impactController.damage = enemySO.damage;
    //        Destroy(impact, 1);
    //    }

    //    //攻撃のアニメーションが終わったら全部初期化する
    //    protected override void InitAnim()
    //    {
    //        base.InitAnim();
    //        isChant = false;
    //        isMagic = false;
    //        isFire = false;
    //        isSlash = false;
    //        isRunJumpAttack = false;

    //        if (hp <= enemySO.maxHP / 2 && !bigTrickUsed)
    //        {
    //            bigTrickReady = true;
    //        }
    //    }
}
