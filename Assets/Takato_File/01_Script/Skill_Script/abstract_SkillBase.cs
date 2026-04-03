using UnityEngine;

namespace Takato
{
    public abstract class SkillBase : ScriptableObject
    {
        [Header("(スキルの共通ステータス)")]
        [Space(10)]
        [Header("スキル名")]
        public string skillName;
        [Header("スキルの発動コスト")]
        public int cost;
        [Header("スキルのクールタイム")]
        public float cooldown;
        [Header("スキル発動時の移動速度のバフ(共通ステータス)")]
        public float moveSpeedBuff;

        // スキル発動時の共通インターフェース
        public abstract void Activate(PlayerController player);
    }
}
