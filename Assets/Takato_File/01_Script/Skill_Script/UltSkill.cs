using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ウルトラスキルを管理するクラス
/// </summary>
namespace Takato
{
    [CreateAssetMenu(menuName = "Takato/Skill/UltSkill")]
    public class UltSkill : SkillBase
    {
        [Header("必殺技の追加効果")]
        [Space(10)]

        [Header("スキルレベル")]
        [SerializeField, Min(1)]
        public int level;

        [Header("攻撃力バフ倍率（レベル1基準）")]
        public float baseAttackBuff;
        [Header("バフの持続時間（レベル1基準）")]
        public float baseDuration;
        [Header("敵に与えるダメージ（レベル1基準）")]
        public int baseDamage;

        [Header("レベルごとの上昇量")]
        [Header("攻撃力バフ倍率の上昇量")]
        public float attackBuffPerLevel;
        [Header("バフの持続時間の上昇量")]
        public float durationPerLevel;
        [Header("敵に与えるダメージの上昇量")]
        public int damagePerLevel;
        [Header("レベル10以上で敵の移動速度を下げる倍率")]
        public float moveSpeedDebuffPerLevel;

        [Header("レベル10以上で敵の防御を下げる量")]
        public int debuffDefValue;
        [Header("防御デバフの持続時間（秒）")]
        public float debuffDuration;

        [Space(8)]
        [Header("パッシブ(装備中)設定")]
        [Header("装備中のパッシブ移動速度")]
        public float passiveMoveSpeedBuff;
        [Header("装備中のパッシブ防御力")]
        public int passiveDefBuff;

        //[Header("装備中のパッシブSP回復力")]← 後で回復の処理を書きます。
        //public int passiveSPRecoveryBuff;

        // パッシブ状態をプレイヤー単位で管理
        private class PassiveState
        {
            public float originalMoveSpeed;
            public float originalDef;
            //public int originalSPRecovery;← 後で回復の処理を書きます。
        }
        private Dictionary<int, PassiveState> passiveStates = new Dictionary<int, PassiveState>();

        ///<summary>
        ///装備中のパッシブ効果の処理
        ///</summary>
        public override void OnEquip(PlayerController player)
        {
            if (player == null) return;
            int id = player.GetInstanceID();
            if (passiveStates.ContainsKey(id)) return;

            //現在の移動速度と防御力を保存してパッシブ効果を適用
            float originalMoveSpeed = player.GetMoveSpeed();
            float originalDef = player.GetDamageCutRate();
            //int originalSPRecovery = player.GetSPRecoveryRate();← 後で回復の処理を書きます。

            // 移動速度と防御力をバフ
            if(passiveMoveSpeedBuff != 0)
            {
                player.SetMoveSpeed(originalMoveSpeed * passiveMoveSpeedBuff);
            }
            if(passiveDefBuff != 0)
            {
                player.SetDamageCutRate(originalDef + passiveDefBuff);
            }
            //if(passiveSPRecoveryBuff != 0){} ← 後で回復の処理を書きます。

            passiveStates[id] = new PassiveState
            {
                originalMoveSpeed = originalMoveSpeed,
                originalDef = originalDef,
                //originalSPRecovery = originalSPRecovery← 後で回復の処理を書きます。
            };
            Debug.Log($"[UltSkill] Player {id} equipped. MoveSpeed: {originalMoveSpeed} -> {player.GetMoveSpeed()}, Def: {originalDef} -> {player.GetDamageCutRate()}");
        }

        /// <summary>
        /// 装備解除時にパッシブ効果を元に戻す処理
        /// </summary>
        /// <param name="player"></param>
        public override void OnUnequip(PlayerController player)
        {
            if (player == null) return;
            int id = player.GetInstanceID();
            if (!passiveStates.ContainsKey(id)) return;

            //保存していた元の値を復元
            var state = passiveStates[id];
            player.SetMoveSpeed(state.originalMoveSpeed);
            player.SetDamageCutRate(state.originalDef);
            //player.SetSPRecoveryRate(state.originalSPRecovery);← 後で回復の処理を書きます。

            passiveStates.Remove(id);
            Debug.Log($"[UltSkill] Player {id} unequipped. MoveSpeed restored to {state.originalMoveSpeed}, Def restored to {state.originalDef}");
        }

        /// <summary>
        /// スキル発動時の処理
        /// </summary>
        public override void Activate(PlayerController player)
        {
            player.ConsumeCost(cost);

            // レベルに応じた値を計算
            float attackBuff = baseAttackBuff + attackBuffPerLevel * (level - 1);
            float duration = baseDuration + durationPerLevel * (level - 1);
            int damage = baseDamage + damagePerLevel * (level - 1);

            // 一番近い敵を探す
            Enemy target = FindNearestEnemy(player.transform.position);
            if (target != null)
            {
                target.TakeDamage(damage);

                // レベル10以上なら防御デバフを付与
                if (level >= 10)
                {
                    player.StartCoroutine(ApplyEnemyDefDebuff(target));
                }

                // 敵の移動速度デバフを適用
                player.StartCoroutine(ApplyEnemyMoveSpeedDebuff(target, duration));
            }

            // プレイヤーの攻撃力バフのみ適用
            player.StartCoroutine(ApplyUltBuff(player, attackBuff, duration));
        }

        /// <summary>
        /// プレイヤーの攻撃力バフのみ適用（移動速度は変更しない）
        /// </summary>
        private IEnumerator ApplyUltBuff(PlayerController player, float attackBuff, float duration)
        {
            // 武器の攻撃力を取得・保存
            var weaponController = player.GetComponentInChildren<PlayerWeaponController>();
            if (weaponController == null || weaponController.EquippedWeapon == null)
                yield break;

            Weapon weapon = weaponController.EquippedWeapon;
            float originalBaseAttack = GetWeaponBaseAttack(weapon);

            // 武器の基本攻撃力をバフ
            weapon.SetAttackDamage(originalBaseAttack * attackBuff);

            yield return new WaitForSeconds(duration);

            // 元に戻す
            weapon.SetAttackDamage(originalBaseAttack);
        }

        // 敵の防御デバフ処理
        private IEnumerator ApplyEnemyDefDebuff(Enemy target)
        {
            int originalDef = target.DebufDEF;
            target.DebufDEF = originalDef - debuffDefValue;

            yield return new WaitForSeconds(debuffDuration);

            // 元に戻す
            target.DebufDEF = originalDef;
        }

        /// <summary>
        /// 敵の移動速度デバフを適用するコルーチン
        /// </summary>
        private IEnumerator ApplyEnemyMoveSpeedDebuff(Enemy target, float duration)
        {
            if (level < 10) yield break;

            // レベル10で0.8倍、レベル11で0.7倍..になっていきます.
            float moveSpeedDebuff = Mathf.Clamp01(1.0f - moveSpeedDebuffPerLevel * (level - 9));
            float originalMultiplier = target.MoveSpeedMultiplier;
            target.MoveSpeedMultiplier = moveSpeedDebuff;

            yield return new WaitForSeconds(duration);

            target.MoveSpeedMultiplier = originalMultiplier;
        }

        // 武器のbaseAttackDamageをリフレクションで取得
        private float GetWeaponBaseAttack(Weapon weapon)
        {
            var field = typeof(Weapon).GetField("baseAttackDamage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return field != null ? (float)field.GetValue(weapon) : 0f;
        }

        private Enemy FindNearestEnemy(Vector3 playerPosition)
        {
            Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            Enemy nearest = null;
            float minDist = float.MaxValue;
            foreach (var enemy in enemies)
            {
                float dist = Vector3.Distance(playerPosition, enemy.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = enemy;
                }
            }
            return nearest;
        }
    }
}
