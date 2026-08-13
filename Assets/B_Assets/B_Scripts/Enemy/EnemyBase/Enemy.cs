using System.Collections.Generic;
using Takato;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    [Header("敵のScriptable Object")]
    [SerializeField] protected EnemySO enemySO;

    [Header("HPのスライダー")]
    [SerializeField] protected Slider hpSliider;

    public Transform Target {  get { return target; } }
    protected Transform target;

    public NavMeshAgent Agent { get { return agent; } }
    protected NavMeshAgent agent;

    protected PlayerController playerController;
    protected CharacterController playerCharacterController;
    protected PlayerAnimationController playerAnimationController;
    protected EnemyAnimatorController enemyAnimatorController;
    protected EventProgress eventProgress;

    protected float lotteryTime;
    protected float lotteryMinTime = 0.5f;
    protected float lotteryMaxTime = 3.0f;

    protected float dot;

    //プレイヤーと自身の距離
    public float Distance { get { return distance; } set { distance = value; } }
    protected float distance;

    //攻撃確率の保存用変数
    protected float attackProbability = 0;

    //敵のHP
    protected int hp;

    //敵のスピードにかかるデバフ
    public float DebufDEX { get { return debufDEX; } set { debufDEX = value; } }
    private float debufDEX = 1.0f;

    //敵の防御力にかかるデバフ
    public int DebufDEF { get { return debufDEF; } set { debufDEF = value; } }
    private int debufDEF;

    //何かしらアクションが抽選されているかどうか
    public bool IsAction { get { return isAction; } set { isAction = value; } }
    protected bool isAction;

    //アニメーションが再生されているかどうか
    public bool IsActionAnimation { get { return isActionAnimation; } set { isActionAnimation = value; } }
    protected bool isActionAnimation;

    //プレイヤーを見続けるかどうか
    protected bool isLookPlayer = true;

    //歩いているか
    protected bool isWalk = true;

    //敵が攻撃された後に連続で攻撃が当たらないようにするための変数
    private float invincibilityTime  = 0.5f;
    protected float invincibilityTimer = 0;

    //振り向きのスピード
    public float LookRotationSpeed { get { return lookRotationSpeed; } set { lookRotationSpeed = value; } }
    protected float lookRotationSpeed = 0;

    //最後に使用したアクションの保存用
    private EnemyActionSO lastAction_1;
    private EnemyActionSO lastAction_2;

    //敵が死亡したかどうか
    protected bool isDead = false;

    //敵がプレイヤーを発見しているかどうか
    protected bool isContact = false;
    private float contactDis;
    private float contactDot;
    private float searchDis;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("navMeshが見つからない");
        }
        enemyAnimatorController = GetComponent<EnemyAnimatorController>();
        eventProgress = GetComponentInParent<EventProgress>();

        // 開始時にすでにシーンにプレイヤーがいる場合のみ実行
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            UpdateTarget(playerObj);
        }
    }

    protected virtual void Start()
    {
        isAction = false;

        if (hpSliider != null)
        {
            hpSliider.maxValue = enemySO.maxHP;
            hpSliider.minValue = 0;
            hpSliider.value = enemySO.maxHP;
        }

        distance = Vector3.Distance(transform.position, target.position);

        agent.updateRotation = false;

        hp = enemySO.maxHP;

        agent.stoppingDistance = enemySO.stoopingDis;

        lookRotationSpeed = enemySO.lookRotationSpeed;

        isActionAnimation = false;

        isDead = false;

        isContact = false;
        contactDis = enemySO.contactDis;
        contactDot = enemySO.contactDot;
        searchDis = enemySO.searchDis;

        //画面の明るさ変更（後で別のとこに書く）
        //RenderSettings.ambientIntensity = SystemManager.instance.valueLight;
    }

    protected virtual void Update()
    {
        if (isDead) { return; }

        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }

        //playerタグのついたオブジェクトを見つける(Awakeで取得失敗した際の保険です。)
        if(target == null)
        {
            target = GameObject.FindWithTag("Player").transform;
            playerController = target.GetComponent<PlayerController>();
            playerCharacterController = target.GetComponent<CharacterController>();
            playerAnimationController = target.GetComponent<PlayerAnimationController>();
        }

        //プレイヤーと自身の距離計算
        distance = Vector3.Distance(transform.position, target.position);

        //自身からプレイヤーのDotを取る
        DotPlayer();

        //追跡するかしないかを調整する関数
        AgentContact();

        if (!isContact) { return; }

        //常にプレイヤーの方向を見るようにする
        LookPlayer();

        //交戦時の処理
        EngageMoveControl();
    }

    private void DotPlayer()
    {
        //自身からプレイヤーの方向を取る
        Vector3 toTarget = (target.position - transform.position).normalized;
        toTarget.y = 0;
        //自身の前方方向を取る
        Vector3 forwardDir = (transform.position + transform.forward - transform.position).normalized;
        forwardDir.y = 0;

        //内積で方向の一致度を取る
        dot = Vector3.Dot(toTarget, forwardDir);
    }

    private void LookPlayer()
    {
        if (isLookPlayer)
        {
            Vector3 dir;
            dir.x = 0;
            dir.z = 0;

            if (agent.speed == enemySO.walkMoveSpeed)
            {
                dir = target.position - transform.position;

            }
            else
            {
                dir = (transform.position + agent.velocity) - transform.position;
            }
            dir.y = 0;
            SetRotation(dir);
        }
    }
    private void SetRotation(Vector3 dir)
    {
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * lookRotationSpeed
                );
        }
    }

    private void AgentContact()
    {
        if (agent.isOnNavMesh)
        {
            if (distance <= contactDis && dot >= contactDot && !isContact)
            {
                agent.SetDestination(target.position);
                agent.isStopped = false;
                isContact = true;
                ContactAnimation();
            }
            else if (distance >= searchDis && isContact)
            {
                isContact = false;
                InitAll();
                agent.isStopped = true;
                enemyAnimatorController.ForcedQuitAnimation();
            }
        }

        if (isActionAnimation) { return; }

        //agent.stoppingDistanceの値の付近を行ったり来たりするとアニメーションがガタガタするのでそれ対策
        if (distance <= agent.stoppingDistance && isWalk)
        {
            isWalk = false;
            agent.stoppingDistance++;
        }
        else if (distance >= agent.stoppingDistance && !isWalk)
        {
            isWalk = true;
            agent.stoppingDistance--;
        }
    }

    protected virtual void ContactAnimation() { }

    private void EngageMoveControl()
    {
        if (distance <= enemySO.engageDis && Mathf.Abs(target.position.y - transform.position.y) < 0.5f)
        {

            //敵のスピードを少しだけ遅くする
            agent.speed = enemySO.walkMoveSpeed * DebufDEX;

            if (!isAction)
            {
                lotteryTime -= Time.deltaTime;      

                if (lotteryTime <= 0)
                {
                    isAction = true;

                    //次の抽選に必要な時間をランダムで決める
                    lotteryTime = Random.Range(lotteryMinTime, lotteryMaxTime);
                }
            }
        }
        else
        {
            agent.speed = enemySO.dashMoveSpeed - DebufDEX;
            //isAction = false;
        }
    }

    protected EnemyActionSO CalcAction(EnemyActionSO[] action)
    {
        List<float> scores = new List<float>();
        EnemyActionSO firstActionSO = null;
        EnemyActionSO secondActionSO = null;
        foreach (var a in action)
        {
            scores.Add(a.ScoreCalculation(distance, dot, this));
        }

        float firstScore = 0;
        float secondScore = 0;
        int firstIndex = -1;
        int secondIndex = -1;

        for (int i = 0; i < scores.Count; i++)
        {
            float score = scores[i];

            if (score > firstScore)
            {
                secondScore = firstScore;
                secondIndex = firstIndex;

                firstScore = score;
                firstIndex = i;
            }
            else if (score > secondScore)
            {
                secondScore = score;
                secondIndex = i;
            }
        }

        if (firstScore != 0)  { firstActionSO = action[firstIndex]; }
        if (secondScore != 0) { secondActionSO = action[secondIndex]; }

        if (firstActionSO != null)
        {
            return firstActionSO;
            //if (lastAction_1 == firstActionSO || lastAction_2 == firstActionSO)
            //{
            //    if (secondActionSO != null)
            //    {
            //        lastAction_2 = lastAction_1;
            //        lastAction_1 = secondActionSO;
            //        return secondActionSO;
            //    }
            //    else
            //    {
            //        lastAction_2 = null;
            //        Init();
            //    }
            //}
            //else
            //{
            //    lastAction_2 = lastAction_1;
            //    lastAction_1 = firstActionSO;
            //    return firstActionSO;
            //}
        }

        lastAction_1 = null;
        return null;
    }

    public virtual void TakeDamage(int damage, SoundManager sound = null, int seNumber = -1)
    {
        if (enemyAnimatorController.CheckCurrentAnim("Death")) { return; }
        if (invincibilityTimer > 0) { return; }

        if (sound != null)
        {
            sound.PlaySE(seNumber);
        }

        damage -= (enemySO.def - debufDEF);
        if (damage <= 0) { return; }

        hp -= damage;

        hpSliider.value = hp;

        Debug.Log("Enemy_HP" + hp);

        if (hp <= 0)
        {
            Death();
        }

        invincibilityTimer = invincibilityTime;
    }

    //移動速度の倍率
    protected float moveSpeedMultiplier = 1.0f;
    public float MoveSpeedMultiplier
    {
        get => moveSpeedMultiplier;
        set
        {
            moveSpeedMultiplier = value;

            // 必ず現在の速度を再設定
            if (enemyAnimatorController.CheckCurrentAnim("Running"))
                SetDashSpeed();
            else if (enemyAnimatorController.CheckCurrentAnim("Walking") || enemyAnimatorController.CheckCurrentAnim("Walk Back"))
                SetWalkSpeed();
            else
                SetWalkSpeed(); // 停止時もwalk速度
        }
    }

    // NavMeshAgentの速度を更新するメソッド
    protected void SetDashSpeed()
    {
        if (agent != null && enemySO != null)
            debufDEX = moveSpeedMultiplier;
    }

    // NavMeshAgentの速度を更新するメソッド
    protected void SetWalkSpeed()
    {
        if (agent != null && enemySO != null)
            debufDEX = moveSpeedMultiplier;
    }

    protected virtual void Death()
    {
        isDead = true;
        InitAll();
        hpSliider.gameObject.SetActive(false);
        agent.isStopped = true;
        enemyAnimatorController.SetTriggerAnim(EnemyAnimatorController.AnimationBase.Death);
    }

    /// <summary>
    /// 第一引数はtrueの場合プレイヤーを見る。 第二引数はtrueの場合移動を止める
    /// </summary>
    /// <param name="isLook"></param>
    /// <param name="isStop"></param>
    public void SetLookPlayerAndEnemyStop(bool isLook, bool isStop)
    {
        isLookPlayer = isLook;
        agent.isStopped = isStop;
    }


    public virtual void Init()
    {
        agent.stoppingDistance = enemySO.stoopingDis;
        lookRotationSpeed = enemySO.lookRotationSpeed;
        isLookPlayer = true;
        agent.isStopped = false;
        isAction = false;
        isActionAnimation = false;
    }

    public virtual void InitAnim()
    {
        //全アニメーションのリセット
        enemyAnimatorController.ResetTriggerAnim(EnemyAnimatorController.AnimationBase.Hit);
        enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Walk, false);
        enemyAnimatorController.SetBoolAnim(EnemyAnimatorController.AnimationBase.Dash, false);
        enemyAnimatorController.ForcedQuitAnimation();
    }

    public virtual void InitAll()
    {
        Init();
        InitAnim();
    }

    //攻撃判定の出現
    public void AttackJudgmentActive(EnemyAttackRollController _weaponController)
    {
        _weaponController.SetColliderActive(true);
    }

    //攻撃判定の終了
    public void AttackJudgmentEnd(EnemyAttackRollController _weaponController)
    {
        _weaponController.SetColliderActive(false);
    }

    public void DeathAnimEnd()
    {
        if (eventProgress != null)
        {
            eventProgress.DestroyEnemy(this);
        }
        else
        {
            Debug.Log("EventProgressが見つかりません。");
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// キャラクター切り替え時やスポーン時に、新しいプレイヤーの参照を設定し直す
    /// </summary>
    public void UpdateTarget(GameObject newPlayer)
    {
        if (newPlayer == null) return;

        target = newPlayer.transform;

        //ここで各コンポーネントを安全に取得
        playerController = target.GetComponent<Takato.PlayerController>();
        playerCharacterController = target.GetComponent<CharacterController>();
        playerAnimationController = target.GetComponent<Takato.PlayerAnimationController>();

        // デバッグ用ログ
        if (playerController == null)
            Debug.LogWarning($"[{gameObject.name}] PlayerController の取得に失敗しました。Playerプレハブにスクリプトが付いているか確認してください。");
    }
}
