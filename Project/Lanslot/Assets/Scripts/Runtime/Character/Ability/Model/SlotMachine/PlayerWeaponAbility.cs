using System.Collections.Generic;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat
{
    public class PlayerWeaponAbility : CharacterAbility
    {
        private StageSystem _stageSystem;
        private BattlefieldTileGroup _tileGroup;
        private List<MonsterCharacter> _targetBuffer = new();

        public override void Initialization()
        {
            base.Initialization();
            EnsureReferences();
        }

        public void Apply(ItemNames itemName)
        {
            if (itemName == ItemNames.None)
            {
                return;
            }

            EnsureReferences();

            if (Owner == null)
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

            if (weaponData.RewardCurrency != CurrencyNames.None)
            {
                ApplyRewardCurrency(weaponData);
                return;
            }

            if (weaponData.AttackRange <= 0)
            {
                ApplyWeaponEffectToCharacter(Owner, weaponData);
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
            _stageSystem ??= GameApp.Instance != null && GameApp.Instance.gameManager != null ? GameApp.Instance.gameManager.CurrentStageSystem : null;
            _tileGroup ??= _stageSystem != null ? _stageSystem.BattlefieldTileGroup : null;
        }

        private void ApplyRewardCurrency(WeaponData weaponData)
        {
            if (weaponData.RewardCurrency == CurrencyNames.None)
            {
                return;
            }

            VWeapon weaponInfo = ProfileInfo.Weapon.FindWeapon(weaponData.Name);
            if (weaponInfo == null)
            {
                Log.Warning(LogTags.Weapon, "무기를 찾을 수 없습니다: {0}", weaponData.Name);
                return;
            }

            int amount = weaponData.Damage;
            int additionalAmount = 0;

            switch (weaponData.RewardCurrency)
            {
                case CurrencyNames.Gold:
                    additionalAmount = Owner.Stat.FindValueOrDefaultToInt(StatNames.InstantGold);
                    break;

                case CurrencyNames.Gem:
                    additionalAmount = Owner.Stat.FindValueOrDefaultToInt(StatNames.InstantGem);
                    break;
            }

            ProfileInfo.Currency.Add(weaponData.RewardCurrency, amount + additionalAmount);
        }

        private void ApplyWeaponEffectToCharacter(Character targetCharacter, WeaponData weaponData)
        {
            if (targetCharacter == null || targetCharacter.MyVital == null)
            {
                return;
            }

            int hitCount = Mathf.Max(1, weaponData.MultiHitCount);

            Log.Info(LogTags.Weapon, "무기 효과 적용: 대상={0}, HitCount={1}", targetCharacter.Name.ToLogString(), hitCount.ToSelectString());

            Owner.SetTarget(targetCharacter);
            float? weaponDamageOverride = weaponData.Damage;
            for (int i = 0; i < hitCount; i++)
            {
                Owner.Attack.Activate(weaponData.Hitmark, weaponDamageOverride);
            }
        }
    }
}