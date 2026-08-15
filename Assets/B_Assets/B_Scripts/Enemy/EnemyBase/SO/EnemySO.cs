using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "EnemySO/EnemySO")]
public class EnemySO : ScriptableObject
{
    [Header("敵全体が持つ値の初期設定。\n" +
        "敵がそれぞれ持つ値は敵のinspectorで設定")]

    [Header("敵の攻撃時の行動")]
    public EnemyActionSO[] action;

    //[Header("敵の攻撃しなかったときの行動")]
    //public EnemyActionSO[] doNotAttack_Action;

    [Header("敵のHP")]
    public int maxHP;

    [Header("敵の攻撃力")]
    public int damage;

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

    [Header("接敵距離（この値以下になると攻撃の抽選を開始する）")]
    public float engageDis;

    [Header("探知範囲（この値以下の距離になると戦闘を開始する）")]
    public float contactDis;

    [Header("探知範囲（この値以上のDotになると戦闘を開始する）")]
    public float contactDot;

    [Header("追跡範囲（この値以上の距離になると戦闘を終了する）")]
    public float searchDis;
}
