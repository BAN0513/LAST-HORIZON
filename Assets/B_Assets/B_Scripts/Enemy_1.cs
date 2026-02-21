using UnityEngine;

public class Enemy_1 : Enemy
{
    private float meleeCoolDown = 3;
    private float timeCnt = 0;
    public bool isMelee1;
    public bool isMelee2;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        timeCnt += Time.deltaTime;

        if (timeCnt < meleeCoolDown)
        {
            return;
        }
        if ( isAnimation)
        {
            return;
        }
        base.Update();

        if (distance <= 2)
        {
            agent.isStopped = true;
            isAnimation = true;
            isMelee1 = true;
        }
        else
        {
            isMelee1 = false;
        }
    }

    public void Melee2()
    {
        int rand = Random.Range(1, 101);

        if (rand <= 50)
        {
            isMelee2 = true;
        }
        else
        {
            AnimEnd();
        }
    }

    public void AnimEnd()
    {
        agent.isStopped = false;
        isAnimation = false;
        isMelee1 = false;
        isMelee2 = false;
        timeCnt = 0;
    }
}
