using System.Collections;
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
        private List<BattlefieldTile> _flashTiles = new();

        public override void Initialization()
        {
            base.Initialization();
            EnsureReferences();
        }

        public void Apply(ItemNames itemName, System.Action onCompleted = null)
        {
            if (itemName == ItemNames.None)
            {
                onCompleted?.Invoke();
                return;
            }

            EnsureReferences();

            if (Owner == null)
            {
                Log.Warning(LogTags.UI_SlotMachine, "플레이어 캐릭터를 찾을 수 없습니다.");
                onCompleted?.Invoke();
                return;
            }

            WeaponData weaponData = JsonDataManager.FindWeaponDataClone(itemName);
            if (weaponData == null || weaponData.Name == ItemNames.None)
            {
                Log.Warning(LogTags.UI_SlotMachine, "무기 데이터를 찾을 수 없습니다: {0}", itemName);
                onCompleted?.Invoke();
                return;
            }

            if (weaponData.IsBlock)
            {
                Log.Warning(LogTags.UI_SlotMachine, "현재 빌드에서 차단된 무기입니다: {0}", weaponData.Name);
                onCompleted?.Invoke();
                return;
            }

            if (weaponData.RewardCurrency != CurrencyNames.None)
            {
                ApplyRewardCurrency(weaponData);
                onCompleted?.Invoke();
                return;
            }

            if (weaponData.AttackRange <= 0)
            {
                ApplyWeaponEffectToCharacter(Owner, weaponData);
                onCompleted?.Invoke();
                return;
            }

            if (_tileGroup == null)
            {
                Log.Warning(LogTags.UI_SlotMachine, "스테이지 정보를 찾을 수 없습니다.");
                onCompleted?.Invoke();
                return;
            }

            if (!_tileGroup.TryFindPrimaryMonsterInRange(weaponData.AttackRange, out MonsterCharacter mainTarget, out int targetRow, out int targetColumn))
            {
                Log.Warning(LogTags.UI_SlotMachine, "사거리 내 몬스터가 없습니다. AttackRange={0}", weaponData.AttackRange);
                onCompleted?.Invoke();
                return;
            }

            // 타일 깜박임 효과 시작
            StartCoroutine(ApplyWeaponEffectWithFlash(targetRow, targetColumn, weaponData, onCompleted));
        }

        private IEnumerator ApplyWeaponEffectWithFlash(int targetRow, int targetColumn, WeaponData weaponData, System.Action onCompleted)
        {
            // 공격 영역의 타일들을 깜박임
            FlashAttackAreaTiles(targetRow, targetColumn, weaponData.AttackRow, weaponData.AttackColumn);

            // 깜박임 효과를 잠시 보여줌
            yield return new WaitForSeconds(0.3f);

            // 몬스터 수집 및 공격 적용
            _tileGroup.CollectMonstersInBox(targetRow, targetColumn, weaponData.AttackRow, weaponData.AttackColumn, _targetBuffer);
            if (!_targetBuffer.IsValid())
            {
                BattlefieldTile targetTile = _tileGroup.GetTile(targetRow, targetColumn);
                if (targetTile != null && targetTile.CurrentMonster != null)
                {
                    _targetBuffer.Add(targetTile.CurrentMonster);
                }
            }

            for (int i = 0; i < _targetBuffer.Count; i++)
            {
                if (_targetBuffer[i] != null)
                {
                    ApplyWeaponEffectToCharacter(_targetBuffer[i], weaponData);
                }
            }

            // 공격이 들어간 후 깜박임 해제
            StopFlashTiles();

            onCompleted?.Invoke();
        }

        private void FlashAttackAreaTiles(int centerRow, int centerColumn, int attackRow, int attackColumn)
        {
            _flashTiles.Clear();

            if (_tileGroup == null)
            {
                return;
            }

            int minRow = Mathf.Max(0, centerRow - attackRow);
            int maxRow = Mathf.Min(_tileGroup.Height - 1, centerRow + attackRow);
            int minColumn = Mathf.Max(0, centerColumn - attackColumn);
            int maxColumn = Mathf.Min(_tileGroup.Width - 1, centerColumn + attackColumn);

            for (int row = minRow; row <= maxRow; row++)
            {
                for (int column = minColumn; column <= maxColumn; column++)
                {
                    BattlefieldTile tile = _tileGroup.GetTile(row, column);
                    if (tile != null)
                    {
                        tile.Flash(0.3f);
                        _flashTiles.Add(tile);
                    }
                }
            }
        }

        private void StopFlashTiles()
        {
            // 타일 깜박임은 자동으로 종료되지만, 명시적으로 정리
            _flashTiles.Clear();
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