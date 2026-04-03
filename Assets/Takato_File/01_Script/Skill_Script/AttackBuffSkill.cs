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
            // コストチェック
            if (player.GetCurrentCost() < cost)
            {
                Debug.Log($"{skillName}：コスト不足で発動できません。必要コスト：{cost}、現在：{player.GetCurrentCost()}");
                return;
            }
            player.ConsumeCost(cost);//コストを消費

            ParticleSystem effect = null;
            if (effectPrefab != null)
            {
                effect = Instantiate(effectPrefab, player.transform.position, Quaternion.identity);
                effect.transform.SetParent(player.transform);
                effect.Play();
            }

            int level = Mathf.Clamp(skillLevel, 1, maxSkillLevel);
            float buffMultiplier = baseBuffMultiplier + buffMultiplierPerLevel * (level - 1);
            float duration = baseDuration + durationPerLevel * (level - 1);

            var weapon = player.GetComponentInChildren<Weapon>();
            if (weapon != null)
            {
                player.StartCoroutine(ApplyAttackBuff(weapon, effect, buffMultiplier, duration));
            }

            // 移動速度バフを適用
            if (moveSpeedBuff > 0)
            {
                player.StartCoroutine(ApplyMoveSpeedBuff(player, moveSpeedBuff, duration));
            }

            Debug.Log($"{skillName} 発動: Lv{level} 攻撃力{buffMultiplier}倍, {duration}秒");
        }



        /// <summary>
        /// 移動速度バフのコルーチン
        /// </summary>
        private IEnumerator ApplyMoveSpeedBuff(PlayerController player, float speedBuff, float duration)
        {
            float originalSpeed = player.GetMoveSpeed();
            player.SetMoveSpeed(originalSpeed + speedBuff);

            yield return new WaitForSeconds(duration);

            player.SetMoveSpeed(originalSpeed);
        }

        /// <summary>
        /// 攻撃力バフのコルーチン
        /// </summary>
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

        /// <summary>
        /// WeaponクラスのbaseAttackDamageをリフレクションで取得するメソッド
        /// </summary>
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
