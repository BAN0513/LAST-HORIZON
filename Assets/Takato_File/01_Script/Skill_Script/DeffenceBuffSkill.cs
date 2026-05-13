using Takato;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

        [Space(8)]
        [Header("パッシブ(装備中)")]
        [Header("装備中のダメージカット率（パッシブ）")]
        public float passiveCutRate;
        [Header("装備中の移動速度バフ（パッシブ）")]
        public float passiveMoveSpeedBuff;

        // パッシブ状態をプレイヤー単位で管理
        private class PassiveState
        {
            public float originalCutRate;
            public ParticleSystem effect;
            public float originalMoveSpeed;
        }

        private Dictionary<int, PassiveState> passiveStates = new Dictionary<int, PassiveState>();

        /// <summary>
        /// 防御力を一定時間上げるスキルの発動処理（アクティブ）
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
            float cutRate = baseCutRate + cutRatePerLevel * (level - 1);
            float duration = baseDuration + durationPerLevel * (level - 1);

            player.StartCoroutine(ApplyDefenceBuff(player, effect, cutRate, duration));

            // 移動速度バフを適用（アクティブ）
            if (moveSpeedBuff > 0)
            {
                player.StartCoroutine(ApplyMoveSpeedBuff(player, moveSpeedBuff, duration));
            }

            Debug.Log($"{skillName} 発動: Lv{level} ダメージカット率{cutRate:P0}, {duration}秒");
        }

        /// <summary>
        /// 装備時（パッシブ）：装備中は常時バフを適用する（レベル依存なし）
        /// </summary>
        public override void OnEquip(PlayerController player)
        {
            if (player == null) return;
            int id = player.GetInstanceID();
            if (passiveStates.ContainsKey(id)) return; // 既に適用済み

            float originalCutRate = player.GetDamageCutRate();
            player.SetDamageCutRate(originalCutRate + passiveCutRate);

            float originalMoveSpeed = player.GetMoveSpeed();
            if (passiveMoveSpeedBuff != 0f)
            {
                player.SetMoveSpeed(originalMoveSpeed + passiveMoveSpeedBuff);
            }

            passiveStates[id] = new PassiveState
            {
                originalCutRate = originalCutRate,
                originalMoveSpeed = originalMoveSpeed
            };

            Debug.Log($"{skillName} を装備（パッシブ適用）: ダメージカット率+{passiveCutRate} 移動速度+{passiveMoveSpeedBuff}");
        }

        /// <summary>
        /// 装備解除時（パッシブ解除）
        /// </summary>
        public override void OnUnequip(PlayerController player)
        {
            if (player == null) return;
            int id = player.GetInstanceID();
            if (!passiveStates.TryGetValue(id, out var state)) return;

            player.SetDamageCutRate(state.originalCutRate);

            if (state.effect != null)
            {
                state.effect.Stop();
                Destroy(state.effect.gameObject, state.effect.main.duration);
            }

            player.SetMoveSpeed(state.originalMoveSpeed);

            passiveStates.Remove(id);

            Debug.Log($"{skillName} のパッシブを解除");
        }

        /// <summary>
        /// 移動速度バフを適用するコルーチン（アクティブ用）
        /// </summary>
        private IEnumerator ApplyMoveSpeedBuff(PlayerController player, float speedBuff, float duration)
        {
            float originalSpeed = player.GetMoveSpeed();
            player.SetMoveSpeed(originalSpeed + speedBuff);

            yield return new WaitForSeconds(duration);

            player.SetMoveSpeed(originalSpeed);
        }



        /// <summary>
        /// 防御力バフを適用するコルーチン（アクティブ用）
        /// </summary>
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

        // スキルレベルを上げるメソッド（アクティブ用の成長のみ）
        public void LevelUp()
        {
            if (skillLevel < maxSkillLevel)
            {
                skillLevel++;
            }
        }
    }
}