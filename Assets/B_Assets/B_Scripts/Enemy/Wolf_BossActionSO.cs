using UnityEngine;

[CreateAssetMenu(fileName = "EnemyActionSO", menuName = "ActionSO/Wolf_BossActionSO")]
public abstract class Wolf_BossActionSO : ScriptableObject
{
    [Header("攻撃時の最適距離と最適角度")]
    public float bestDis = 0;
    [Range(-1,1)] public float bestDot = 0;

    [Header("最大距離と最大角度")]
    public float maxDis = 0;
    [Range(-1, 1)] public float maxDot = 0;

    [Header("最小距離と最小角度")]
    public float minDis = 0;
    [Range(-1, 1)] public float minDot = 0;

    [Header("距離と角度の重要性")]
    [Range(0, 1)] public float baseDis = 0.5f;
    [Range(0, 1)] public float baseDot = 0.5f;

    //スコアの計算
    //今後アクションによって条件を付けることがあるかもしれないから、継承しておく
    public virtual float ScoreCalculation(float dis, float dot)
    {
        if (dis > maxDis || dot > maxDot || dis < minDis || dot < minDot) { return 0; }

        float disDiff =  Mathf.Abs(dis - bestDis);
        float dotDiff = Mathf.Abs(dot - bestDot);
        float disScore = 1 - Mathf.Clamp01(disDiff / maxDis);
        float dotScore = 1 - Mathf.Clamp01(dotDiff / maxDot);

        return disScore * baseDis + dotScore * baseDot;
    }

    //アニメーションの実行
    public abstract void Execute(Wolf_BossAnimatorController wolf_Anim);
}
