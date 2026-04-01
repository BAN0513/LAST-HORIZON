using UnityEngine;
using System.Collections;

namespace Takato
{
    /// <summary>
    /// 攻撃力を一定時間上げるスキルのクラス
    /// </summary>
    [CreateAssetMenu(menuName = "Takato/Skill/AttackBuffSkill")]
    public class AttackBuffSkill : SkillBase
    {
        [Header("(攻撃力バフスキルのステータス)")]
        [Space(10)]

        [Header("攻撃力バフの基礎倍率")]
        public float baseBuffMultiplier;
        [Header("レベルごとの倍率加算値")]
        public float buffMultiplierPerLevel;
        [Header("攻撃力バフの基礎持続時間")]
        public float baseDuration = 5f;
        [Header("レベルごとの効果時間加算値")]
        public float durationPerLevel = 1f;
        [Header("攻撃力バフのエフェクトプレハブ")]
        public ParticleSystem effectPrefab;
        [Header("スキルレベル")]
        public int skillLevel = 1;
        [Header("最大スキルレベル")]
        public int maxSkillLevel = 5;

        /// <summary>
        /// 攻撃力を一定時間上げるスキルの発動処理
        /// </summary>
        public override void Activate(PlayerController player)
        {
            ParticleSystem effect = null;
            // エフェクトをプレイヤーの位置に生成
            if (effectPrefab != null)
            {
                effect = Instantiate(effectPrefab, player.transform.position, Quaternion.identity);
                effect.transform.SetParent(player.transform); // プレイヤーに追従させる
                effect.Play();
            }

            // レベルに応じた倍率・効果時間を計算
            int level = Mathf.Clamp(skillLevel, 1, maxSkillLevel);
            float buffMultiplier = baseBuffMultiplier + buffMultiplierPerLevel * (level - 1);
            float duration = baseDuration + durationPerLevel * (level - 1);

            // 武器の攻撃力を上げる
            var weapon = player.GetComponentInChildren<Weapon>();
            if (weapon != null)
            {
                player.StartCoroutine(ApplyAttackBuff(weapon, effect, buffMultiplier, duration));
            }

            Debug.Log($"{skillName} 発動: Lv{level} 攻撃力{buffMultiplier}倍, {duration}秒");
        }

        // 攻撃力バフのコルーチン
        private IEnumerator ApplyAttackBuff(Weapon weapon, ParticleSystem effect, float buffMultiplier, float duration)
        {
            float originalBaseAttack = GetBaseAttackDamage(weapon);

            // baseAttackDamageにバフをかける
            weapon.SetAttackDamage(originalBaseAttack * buffMultiplier);

            yield return new WaitForSeconds(duration);

            // 元の攻撃力に戻す
            weapon.SetAttackDamage(originalBaseAttack);

            // エフェクトを停止・破棄
            if (effect != null)
            {
                effect.Stop();
                Destroy(effect.gameObject, effect.main.duration);
            }
        }

        // WeaponのprivateなbaseAttackDamageをリフレクションで取得
        private float GetBaseAttackDamage(Weapon weapon)
        {
            var field = typeof(Weapon).GetField("baseAttackDamage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                return (float)field.GetValue(weapon);
            }
            return weapon.AttackDamage;
        }

        // スキルレベルを上げるメソッド
        public void LevelUp()
        {
            if (skillLevel < maxSkillLevel)
            {
                skillLevel++;
            }
        }
    }
}
