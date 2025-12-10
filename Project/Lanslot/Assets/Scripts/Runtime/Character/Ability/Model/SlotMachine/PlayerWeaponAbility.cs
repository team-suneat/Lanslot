using System.Collections.Generic;
using Sirenix.OdinInspector;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 슬롯머신 무기 효과 적용 어빌리티
    /// </summary>
    public class PlayerWeaponAbility : CharacterAbility
    {
        [FoldoutGroup("#Component")][SerializeField] private PlayerCharacter _playerCharacter;
        [FoldoutGroup("#Component")][SerializeField] private StageSystem _stageSystem;
        [FoldoutGroup("#Component")][SerializeField] private BattlefieldTileGroup _tileGroup;

        private readonly List<MonsterCharacter> _targetBuffer = new();

        public override void Initialization()
        {
            base.Initialization();
            EnsureReferences();
        }

        /// <summary>
        /// 슬롯 결과 무기 아이템을 적용합니다.
        /// </summary>
        public void Apply(ItemNames itemName)
        {
            if (itemName == ItemNames.None)
            {
                return;
            }

            EnsureReferences();

            if (_playerCharacter == null)
            {
                Log.Warning(LogTags.UI_SlotMachine, "플레이어 캐릭터를 찾을 수 없습니다.");
                return;
            }

            WeaponData weaponData = JsonDataManager.FindWeaponDataClone(itemName);
            if (weaponData == null || weaponData.Name == ItemNames.None)
            {
                Log.Warning(LogTags.UI_SlotMachine, "무기 데이터를 찾을 수 없습니다: {0}", itemName);
                return;
            }

            if (weaponData.IsBlock)
            {
                Log.Warning(LogTags.UI_SlotMachine, "현재 빌드에서 차단된 무기입니다: {0}", weaponData.Name);
                return;
            }

            if (weaponData.AttackRange <= 0)
            {
                ApplyWeaponEffectToCharacter(_playerCharacter, weaponData);
                return;
            }

            if (_tileGroup == null)
            {
                Log.Warning(LogTags.UI_SlotMachine, "스테이지 정보를 찾을 수 없습니다.");
                return;
            }

            if (!_tileGroup.TryFindPrimaryMonsterInRange(weaponData.AttackRange, out MonsterCharacter mainTarget, out int targetRow, out int targetColumn))
            {
                Log.Warning(LogTags.UI_SlotMachine, "사거리 내 몬스터가 없습니다. AttackRange={0}", weaponData.AttackRange);
                return;
            }

            _tileGroup.CollectMonstersInBox(targetRow, targetColumn, weaponData.AttackRow, weaponData.AttackColumn, _targetBuffer);
            if (!_targetBuffer.IsValid())
            {
                _targetBuffer.Add(mainTarget);
            }

            for (int i = 0; i < _targetBuffer.Count; i++)
            {
                ApplyWeaponEffectToCharacter(_targetBuffer[i], weaponData);
            }
        }

        private void EnsureReferences()
        {
            _playerCharacter ??= Owner as PlayerCharacter;
            _stageSystem ??= GameApp.Instance != null && GameApp.Instance.gameManager != null ? GameApp.Instance.gameManager.CurrentStageSystem : null;
            _tileGroup ??= _stageSystem != null ? _stageSystem.BattlefieldTileGroup : null;
        }

        private void ApplyWeaponEffectToCharacter(Character targetCharacter, WeaponData weaponData)
        {
            if (targetCharacter == null || targetCharacter.MyVital == null)
            {
                return;
            }

            float baseDamage = _playerCharacter.Stat != null ? _playerCharacter.Stat.FindValueOrDefault(StatNames.Damage) : 0f;
            if (baseDamage <= 0f)
            {
                baseDamage = 1f;
            }

            int hitCount = Mathf.Max(1, weaponData.MultiHitCount);
            for (int i = 0; i < hitCount; i++)
            {
                DamageResult damageResult = new()
                {
                    DamageValue = baseDamage,
                    Attacker = _playerCharacter,
                    TargetVital = targetCharacter.MyVital,
                    HitmarkLevel = 1
                };

                _ = targetCharacter.MyVital.TakeDamage(damageResult);
            }

            Log.Info(LogTags.UI_SlotMachine, "무기 효과 적용: 대상={0}, HitCount={1}, Damage={2}", targetCharacter.name, hitCount, baseDamage);
        }
    }
}

