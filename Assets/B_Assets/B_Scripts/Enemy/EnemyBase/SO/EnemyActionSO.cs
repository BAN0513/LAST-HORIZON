using UnityEngine;

[CreateAssetMenu(fileName = "EnemyActionSO", menuName = "EnemyActionSO")]
public class EnemyActionSO : ScriptableObject
{
    [Header("名前")]
    public string actionName;

    [Header("最適距離")]
    public float bestDis = 0;

    [Header("最大距離")]
    public float maxDis = 0;

    [Header("最小距離")]
    public float minDis = 0;

    [Header("確率")]
    [Range(0, 100)] public float baseProbability = 1.0f;

    //スコアの計算
    //今後アクションによって条件を付けることがあるかもしれないから、overrideできるようにしておく
    public virtual float ScoreCalculation(float dis, float dot)
    {
        if (dis > maxDis || dis < minDis) { return 0; }

        float disDiff = Mathf.Abs(dis - bestDis);
        float disScore = 1 - Mathf.Clamp01(disDiff / maxDis);

        return disScore;
    }

    public virtual float ScoreCalculation(float dis, float dot, Enemy enemy)
    {
        return ScoreCalculation(dis, dot);
    }

    public virtual void Execute(EnemyAnimatorController animator) { animator.enemy.IsActionAnimation = true; }

    public virtual void ActionEnd(EnemyAnimatorController animator) { }
}
