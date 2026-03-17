using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "Scriptable Objects/EnemySO")]
public class EnemySO : ScriptableObject
{
    [Header("敵全体が持つ値の初期設定。\n" +
        "敵がそれぞれ持つ値は敵のinspectorで設定")]

    [Header("敵のHP")]
    public int maxHP;

    [Header("敵の攻撃力")]
    public float damage;

    [Header("敵の走るスピード")]
    public float dashMoveSpeed;

    [Header("敵の歩くスピード")]
    public float walkMoveSpeed;

    [Header("敵の防御力")]
    public int def;

    [Header("敵がこの値以上は近づかない")]
    public float stoopingDis; 

    [Header("敵の振り向きの速度")]
    public float lookRotationSpeed;

    [Header("敵の探知範囲")]
    public float searchDistance;

    [Header("敵の追跡範囲")]
    public float contactDistance;

    [Header("接敵距離（この値以下になると攻撃の抽選を開始する）")]
    public float engageDis;


    [Header("プレイヤーとの距離が値以下になると下がる行動をする")]
    public float backActionDis;

    [Header("下がる行動の時に下がる距離")]
    public float backMoveDis;

    [Header("攻撃の確率")]
    public float attackInitProbability;

    [Header("抽選で攻撃以外になった時に攻撃確率を上げるための値")]
    public float attackUpProbability;

    [Header("攻撃後、この値の分の秒数は攻撃の抽選は行わない")]
    public float attackCoolDown;
}
