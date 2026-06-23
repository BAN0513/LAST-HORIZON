using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Takato
{
    /// <summary>
    /// 攻撃バフ系スキル
    /// </summary>
    [CreateAssetMenu(menuName = "Takato/Skill/AttackBuffSkill")]
    public class AttackBuffSkill : SkillBase
    {
        [Header("(攻撃の共通ステータス)")]
        [Space(10)]

        [Header("攻撃の倍率（アクティブ用）")]
        public float baseBuffMultiplier;
        [Header("成長による倍率増分（アクティブ用）")]
        public float buffMultiplierPerLevel;
        [Header("バフの基本継続時間（アクティブ用）")]
        public float baseDuration = 5f;
        [Header("成長による継続時間増分（アクティブ用）")]
        public float durationPerLevel;
        [Header("発生エフェクト")]
        public ParticleSystem effectPrefab;
        [Header("スキルレベル")]
        public int skillLevel = 1;
        [Header("最大スキルレベル")]
        public int maxSkillLevel = 5;

        [Space(8)]
        [Header("パッシブ（装備中）設定")]
        [Header("装備中の攻撃倍率（パッシブ、レベル依存なし）")]
        public float passiveAttackMultiplier;
        [Header("装備中の移動速度バフ（パッシブ、レベル依存なし）")]
        public float passiveMoveSpeedBuff;

        // パッシブ状態をプレイヤー単位で管理
        private class PassiveState
        {
            public Weapon weapon;
            public float originalBaseAttack;
            public ParticleSystem effect;
            public float originalMoveSpeed;
        }

        private Dictionary<int, PassiveState> passiveStates = new Dictionary<int, PassiveState>();

        /// <summary>
        /// アクティブ時の発動（従来の一時バフ）
        /// </summary>
        public override void Activate(PlayerController player)
        {
            // コストチェック
            if (player.GetCurrentCost() < cost)
            {
                Debug.Log($"{skillName}：コスト不足（必要{cost}、所持{player.GetCurrentCost()}）");
                return;
            }
            player.ConsumeCost(cost);//コスト消費

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

            // 移動速度バフ（短時間）
            if (moveSpeedBuff != 0f)
            {
                player.StartCoroutine(ApplyMoveSpeedBuff(player, moveSpeedBuff, duration));
            }

            Debug.Log($"{skillName} 発動: Lv{level} 攻撃倍率{buffMultiplier}, 継続{duration}s");
        }

        /// <summary>
        /// 装備時（パッシブ）：装備中は常時バフを適用する
        /// </summary>
        public override void OnEquip(PlayerController player)
        {
            if (player == null) return;
            int id = player.GetInstanceID();
            if (passiveStates.ContainsKey(id)) return; // 既に適用済み

            // パッシブはレベル依存しない単一の倍率・移動速度を使用
            float buffMultiplier = passiveAttackMultiplier;

            var weapon = player.GetComponentInChildren<Weapon>();
            float originalBaseAttack = 0f;
            if (weapon != null)
            {
                originalBaseAttack = GetBaseAttackDamage(weapon);
                weapon.SetAttackDamage(originalBaseAttack * buffMultiplier);
            }

            float originalMoveSpeed = player.GetMoveSpeed();
            if (passiveMoveSpeedBuff != 0f)
            {
                player.SetMoveSpeed(originalMoveSpeed + passiveMoveSpeedBuff);
            }

            passiveStates[id] = new PassiveState
            {
                weapon = weapon,
                originalBaseAttack = originalBaseAttack,
                originalMoveSpeed = originalMoveSpeed
            };

            Debug.Log($"{skillName} を装備（パッシブ適用）: 攻撃倍率{buffMultiplier} 移動速度+{passiveMoveSpeedBuff}");
        }

        /// <summary>
        /// 装備解除時（パッシブ解除）
        /// </summary>
        public override void OnUnequip(PlayerController player)
        {
            if (player == null) return;
            int id = player.GetInstanceID();
            if (!passiveStates.TryGetValue(id, out var state)) return;

            if (state.weapon != null)
            {
                state.weapon.SetAttackDamage(state.originalBaseAttack);
            }

            if (state.effect != null)
            {
                state.effect.Stop();
                Destroy(state.effect.gameObject, state.effect.main.duration);
            }

            // 移動速度を元に戻す
            player.SetMoveSpeed(state.originalMoveSpeed);

            passiveStates.Remove(id);

            Debug.Log($"{skillName} のパッシブを解除");
        }

        /// <summary>
        /// 移動速度バフ（アクティブ用のコルーチン）
        /// </summary>
        private IEnumerator ApplyMoveSpeedBuff(PlayerController player, float speedBuff, float duration)
        {
            float originalSpeed = player.GetMoveSpeed();
            player.SetMoveSpeed(originalSpeed + speedBuff);

            yield return new WaitForSeconds(duration);

            player.SetMoveSpeed(originalSpeed);
        }

        /// <summary>
        /// 攻撃バフ（アクティブ用のコルーチン）
        /// </summary>
        private IEnumerator ApplyAttackBuff(Weapon weapon, ParticleSystem effect, float buffMultiplier, float duration)
        {
            float originalBaseAttack = GetBaseAttackDamage(weapon);

            // baseAttackDamage を変更
            weapon.SetAttackDamage(originalBaseAttack * buffMultiplier);

            yield return new WaitForSeconds(duration);

            // 元に戻す
            weapon.SetAttackDamage(originalBaseAttack);

            // エフェクト停止
            if (effect != null)
            {
                effect.Stop();
                Destroy(effect.gameObject, effect.main.duration);
            }
        }

        /// <summary>
        /// Weapon の private な baseAttackDamage を取得する補助
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

        // レベルアップ（アクティブ用の成長のみ）
        public void LevelUp()
        {
            if (skillLevel < maxSkillLevel)
            {
                skillLevel++;
            }
        }
    }
}
