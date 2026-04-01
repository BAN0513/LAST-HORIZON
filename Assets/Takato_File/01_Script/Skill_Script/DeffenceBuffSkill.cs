using Takato;
using UnityEngine;
using System.Collections;

namespace Takato
{
    /// <summary>
    /// 防御力を一定時間上げるスキルのクラス
    /// </summary>
    [CreateAssetMenu(menuName = "Takato/Skill/DeffenceBuffSkill")]
    public class DeffenceBuffSkill : SkillBase
    {
        [Header("(防御力バフスキルのステータス)")]
        [Space(10)]

        [Header("基礎ダメージカット率（0～1）")]
        public float baseCutRate;
        [Header("レベルごとのカット率加算値")]
        public float cutRatePerLevel;
        [Header("基礎持続時間")]
        public float baseDuration;
        [Header("レベルごとの効果時間加算値")]
        public float durationPerLevel;
        [Header("防御バフのエフェクトプレハブ")]
        public ParticleSystem effectPrefab;
        [Header("スキルレベル")]
        public int skillLevel;
        [Header("最大スキルレベル")]
        public int maxSkillLevel;

        /// <summary>
        /// 防御力を一定時間上げるスキルの発動処理
        /// </summary>
        public override void Activate(PlayerController player)
        {
            ParticleSystem effect = null;
            if (effectPrefab != null)
            {
                effect = Instantiate(effectPrefab, player.transform.position, Quaternion.identity);
                effect.transform.SetParent(player.transform);
                effect.Play();
            }

            int level = Mathf.Clamp(skillLevel, 1, maxSkillLevel);
            float cutRate = baseCutRate + cutRatePerLevel * (level - 1);
            float duration = baseDuration + durationPerLevel * (level - 1);

            player.StartCoroutine(ApplyDefenceBuff(player, effect, cutRate, duration));

            Debug.Log($"{skillName} 発動: Lv{level} ダメージカット率{cutRate:P0}, {duration}秒");
        }

        private IEnumerator ApplyDefenceBuff(PlayerController player, ParticleSystem effect, float cutRate, float duration)
        {
            float originalCutRate = player.GetDamageCutRate();
            player.SetDamageCutRate(originalCutRate + cutRate);

            yield return new WaitForSeconds(duration);

            player.SetDamageCutRate(originalCutRate);

            if (effect != null)
            {
                effect.Stop();
                Destroy(effect.gameObject, effect.main.duration);
            }
        }

        // スキルレベルを上げるメソッド（必要に応じて呼び出し）
        public void LevelUp()
        {
            if (skillLevel < maxSkillLevel)
            {
                skillLevel++;
            }
        }
    }
}