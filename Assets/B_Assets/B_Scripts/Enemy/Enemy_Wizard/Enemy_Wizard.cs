using UnityEngine;

public class Enemy_Wizard : Enemy_Humanoid
{
    private Enemy_WizardAnimatorController enemy_WizardAnimator;

    public enum SummonEnemyWeakState 
    {
        NotSpawn,
        Spawn,
        FiftyPercentSpawn,
        SpawnEnd,
    }
    private SummonEnemyWeakState enemy_WeakSpawnState = SummonEnemyWeakState.NotSpawn;
    public SummonEnemyWeakState Enemy_WeakSpawnState
    {
        get {  return enemy_WeakSpawnState; }
        set {  enemy_WeakSpawnState = value; }
    }

    [SerializeField] private GameObject meraObj;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private GameObject meraStormObj;
    [SerializeField] private GameObject enemy_WeakObj;
    [SerializeField] private Transform[] summonPos;

    [SerializeField] private Transform[] warpPos;

    private bool isTeleport = false;

    protected override void Start()
    {
        base.Start();

        enemy_WizardAnimator = GetComponent<Enemy_WizardAnimatorController>();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void TakeDamage(int damage, SoundManager sound, int seNumber)
    {
        base.TakeDamage(damage, sound, seNumber);

        if (hp <= enemySO.maxHP / 2 && enemy_WeakSpawnState != SummonEnemyWeakState.SpawnEnd)
        {
            enemy_WeakSpawnState = SummonEnemyWeakState.FiftyPercentSpawn; 
        }

        isTeleport = true;
    }

    //ここから下はAnimator関連の関数

    public void Mera()
    {
        Vector3 toTarget = target.position - transform.position + transform.forward;
        Vector3 nor = (toTarget).normalized;
        Quaternion quaternion = Quaternion.LookRotation(toTarget);

        GameObject fire = Instantiate(meraObj, transform.position + transform.forward + transform.up, quaternion);
        FireController fireController = fire.GetComponent<FireController>();
        fireController.Damage = enemySO.damage;
        fireController.Player = playerController;

        Rigidbody rb = fire.GetComponent<Rigidbody>();

        rb.linearVelocity = nor * 5;
    }

    public void MeraStorm()
    {
        Instantiate(meraStormObj, transform.position, Quaternion.identity);
    }

    public void Impact()
    {
        GameObject impact = Instantiate(impactEffect, transform.position, Quaternion.identity);
        ImpactController impactController = impact.GetComponentInChildren<ImpactController>();
        impactController.damage = enemySO.damage;
        Destroy(impact, 1);
    }

    public void Summon()
    {
        Vector3 dir = (target.position - transform.position).normalized;
        foreach (var pos in summonPos)
        {
            Instantiate(enemy_WeakObj, pos.position, Quaternion.LookRotation(dir));
        }
    }

    //攻撃のアニメーションが終わったら全部初期化する
    public override void Init()
    {
        base.Init();
    }

    public override void InitAnim()
    {
        base.InitAnim();
    }
}
