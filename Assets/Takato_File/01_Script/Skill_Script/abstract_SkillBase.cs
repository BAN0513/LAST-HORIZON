using UnityEngine;

namespace Takato
{
    public abstract class SkillBase : ScriptableObject
    {
        [Header("(スキルの共通ステータス)")]
        [Space(10)]
        [Header("スキル名")]
        public string skillName;
        [Header("スキルの説明")]
        public float cooldown;

        // スキル発動時の共通インターフェース
        public abstract void Activate(PlayerController player);
    }
}
