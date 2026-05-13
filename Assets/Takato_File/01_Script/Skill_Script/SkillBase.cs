using UnityEngine;

namespace Takato
{
    public abstract class SkillBase : ScriptableObject
    {
        [Header("(スキルの共通ステータス)")]
        [Space(10)]
        [Header("スキル名")]
        public string skillName;
        [Header("スキルの画像")]
        public Sprite skillIcon;
        [Header("スキルの発動コスト")]
        public int cost;
        [Header("スキルのクールタイム")]
        public float cooldown;
        [Header("スキル発動時の移動速度のバフ(共通ステータス)")]
        public float moveSpeedBuff;

        // スキル発動時の共通インターフェース
        public abstract void Activate(PlayerController player);

        //スキル装備(スロットに入っているとき)のパッシブ判定
        public virtual void OnEquip(PlayerController player) { }

        //スキル解除(スロットから外れた)時のパッシブ判定
        public virtual void OnUnequip(PlayerController player){ }
    }
}
