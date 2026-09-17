using UnityEngine;

public class Enemy_Wizard : Enemy_Humanoid
{
    [SerializeField] private GameObject meraObj;
    [SerializeField] private GameObject meraZomaObj;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private GameObject meraStormObj;
    [SerializeField] private GameObject enemy_WeakObj;
    [SerializeField] private GameObject shieldObj;
    [SerializeField] private Transform[] summonPos;
    [SerializeField] private Transform[] warpPos;

    private Enemy_WizardAnimatorController enemy_WizardAnimator;
    private int weakMagicCnt = 0;
    private int shortManaChargePlayCnt = 2;

    public bool IsHitAction { get; set; } = false;

    public bool IsShield { private get; set; }

    public int SummonEnemyCount { get; set; }

    public bool ShortManaCharge { get; private set; } = false;

    public bool LongManaCharge { get; private set; } = false;

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
        if (IsShield) { return; }  //シールド中は念のためダメージが入らないようにする

        if (IsHitAction) { isHit = true; }

        base.TakeDamage(damage, sound, seNumber);

        IsHitAction = true;
    }

    //ここから下はAnimator関連の関数

    public void Mera()
    {
        CheckShortManaCharge();
        MeraObjSpawn(meraObj);
    }

    public void MeraZoma()
    {
        LongManaCharge = true;
        MeraObjSpawn(meraZomaObj);
    }

    private void MeraObjSpawn(GameObject spawnMeraObj)
    {
        Vector3 toTarget = Target.position - transform.position;
        Vector3 dir = (toTarget).normalized;
        Quaternion quaternion = Quaternion.LookRotation(toTarget);
        GameObject fire = Instantiate(spawnMeraObj, transform.position + transform.forward + transform.up, quaternion);
        FireController fireController = fire.GetComponent<FireController>();
        fireController.Damage = currentAction.damage;
        fireController.Player = playerController;
        fireController.Direction = dir;
    }

    public void MeraStorm()
    {
        LongManaCharge = true;
        GameObject storm = Instantiate(meraStormObj, transform.position, Quaternion.identity);
        TornadoController tornadoController = storm.GetComponent<TornadoController>();
        tornadoController.Damage = currentAction.damage;
    }

    public void Impact()
    {
        GameObject impact = Instantiate(impactEffect, transform.position, Quaternion.identity);
        ImpactController impactController = impact.GetComponentInChildren<ImpactController>();
        impactController.damage = currentAction.damage;
        Destroy(impact, 1);
    }

    public void Summon()
    {
        CheckShortManaCharge();
        Vector3 dir = (Target.position - transform.position).normalized;
        foreach (var pos in summonPos)
        {
            Enemy_Weak weak = Instantiate(enemy_WeakObj, pos.position, Quaternion.LookRotation(dir)).GetComponent<Enemy_Weak>();
            SummonEnemyCount++;
            weak.Wizard = this;
        }
    }

    public void Teleport()
    {
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
        transform.rotation = Quaternion.LookRotation((Target.position - transform.position).normalized);
        return;
    }

    public void ShieldSpawn()
    {
        IsShield = true;
        IsHitAction = false;

        GameObject shield = Instantiate(shieldObj, transform.position, Quaternion.identity);
        
        if (shield.TryGetComponent<ShieldController>(out var shieldController))
        {
            shieldController.ParentEnemyWizard = this;
        }
    }

    private void CheckShortManaCharge()
    {
        weakMagicCnt++;

        if (weakMagicCnt >= shortManaChargePlayCnt)
        {
            ShortManaCharge = true;
        }
    }

    public void ManaChargeReset()
    {
        weakMagicCnt    = 0;
        ShortManaCharge = false;
        LongManaCharge  = false;
    }

    //攻撃のアニメーションが終わったら全部初期化する
    public override void Init()
    {
        base.Init();

        if (IsHitAction || ShortManaCharge || LongManaCharge)
        {
            IsAction = true;
        }
    }

    public override void InitAnim()
    {
        base.InitAnim();
    }
}
