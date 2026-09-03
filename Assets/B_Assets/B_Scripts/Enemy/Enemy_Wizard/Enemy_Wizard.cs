using UnityEngine;

public class Enemy_Wizard : Enemy_Humanoid
{
    private Enemy_WizardAnimatorController enemy_WizardAnimator;


    [SerializeField] private GameObject meraObj;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private GameObject meraStormObj;
    [SerializeField] private GameObject enemy_WeakObj;
    [SerializeField] private GameObject shieldObj;
    [SerializeField] private Transform[] summonPos;
    [SerializeField] private Transform[] warpPos;

    private bool isTeleport = false;
    public bool IsTeleport 
    {
        get { return isTeleport; }
        set { isTeleport = value; }
    }

    private bool isShield = false;
    public bool IsShield { set { isShield = value; } }

    private int summonEnemyCount = 0;
    public int SummonEnemyCount
    {
        get { return summonEnemyCount; }
        set { summonEnemyCount = value; }
    }

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
        if (isShield) { return; }  //シールド中は念のためダメージが入らないようにする

        if (isTeleport) { isHit = true; }

        base.TakeDamage(damage, sound, seNumber);

        isTeleport = true;
    }

    //ここから下はAnimator関連の関数

    public void Mera()
    {
        Vector3 toTarget = target.position - transform.position;
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
            Enemy_Weak weak = Instantiate(enemy_WeakObj, pos.position, Quaternion.LookRotation(dir)).GetComponent<Enemy_Weak>();
            summonEnemyCount++;
            weak.Wizard = this;
        }
    }

    public void Teleport()
    {
        isTeleport = false;
        int warpLimit = 10;
        int randomWarp = 0;
        float notWarpLength = 3.0f;

        for (int i = 0; i < warpLimit; i++)
        {
            randomWarp = UnityEngine.Random.Range(0, warpPos.Length);
            if (Vector3.Distance(transform.position, warpPos[randomWarp].position) <= notWarpLength) { continue; }
            break;
        }
        transform.position = warpPos[randomWarp].position;
        transform.rotation = Quaternion.LookRotation((target.position - transform.position).normalized);
        return;
    }

    public void ShieldSpawn()
    {
        isShield = true;

        GameObject shield = Instantiate(shieldObj, transform.position, Quaternion.identity);
        
        if (shield.TryGetComponent<ShieldController>(out var shieldController))
        {
            shieldController.ParentEnemyWizard = this;
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
