using System.Collections;
using System.Collections.Generic;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat
{
    public class PlayerWeaponAbility : CharacterAbility
    {
        private const float FLASH_DURATION = 0.3f;        
        private const float AREA_ATTACK_DELAY = 0.3f;
        private const float CENTER_TILE_BLEND = 0.5f;
        private const float NORMAL_TILE_BLEND = 0.1f;
        private const int MIN_HIT_COUNT = 1;
        private const int MIN_ATTACK_RANGE = 0;

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

            if (weaponData.AttackRange <= MIN_ATTACK_RANGE)
            {
                StartCoroutine(ApplyWeaponEffectToCharacterCoroutine(Owner, weaponData, onCompleted));
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
            bool isAreaApplied = false;
            yield return StartCoroutine(TryApplyAreaWeaponEffect(targetRow, targetColumn, weaponData, (result) => isAreaApplied = result));
            if (!isAreaApplied)
            {
                yield return StartCoroutine(ApplyWeaponEffectFallback(targetRow, targetColumn, weaponData));
            }

            // 공격이 들어간 후 깜박임 해제
            StopFlashTiles();

            onCompleted?.Invoke();
        }

        private void FlashAttackAreaTiles(int centerRow, int centerColumn, int attackRow, int attackColumn, AttackAreaShape shape)
        {
            _flashTiles.Clear();

            if (_tileGroup == null)
            {
                return;
            }

            switch (shape)
            {
                case AttackAreaShape.Rectangle:
                    FlashRectangleTiles(centerRow, centerColumn, attackRow, attackColumn);
                    break;

                case AttackAreaShape.Cross:
                    FlashCrossTiles(centerRow, centerColumn, attackRow, attackColumn);
                    break;

                case AttackAreaShape.X:
                    FlashXTiles(centerRow, centerColumn, attackRow, attackColumn);
                    break;

                default:
                    FlashRectangleTiles(centerRow, centerColumn, attackRow, attackColumn);
                    break;
            }
        }

        private void FlashRectangleTiles(int centerRow, int centerColumn, int attackRow, int attackColumn)
        {
            _tileGroup.GetRectangleBounds(centerRow, centerColumn, attackRow, attackColumn, out int minRow, out int maxRow, out int minColumn, out int maxColumn);

            for (int row = minRow; row <= maxRow; row++)
            {
                for (int column = minColumn; column <= maxColumn; column++)
                {
                    BattlefieldTile tile = _tileGroup.GetTile(row, column);
                    if (tile != null)
                    {
                        float blend = row == centerRow && column == centerColumn ? CENTER_TILE_BLEND : NORMAL_TILE_BLEND;
                        tile.Flash(FLASH_DURATION, Color.red, blend);
                        _flashTiles.Add(tile);
                    }
                }
            }
        }

        private void FlashCrossTiles(int centerRow, int centerColumn, int attackRow, int attackColumn)
        {
            // 십자가 형태: 중심에서 가로선과 세로선
            // 가로선 (같은 row, column 범위) - AttackRow는 실제로 가로(column) 방향
            _tileGroup.GetCrossHorizontalBounds(centerRow, centerColumn, attackRow, out int minColumn, out int maxColumn);
            for (int column = minColumn; column <= maxColumn; column++)
            {
                BattlefieldTile tile = _tileGroup.GetTile(centerRow, column);
                if (tile != null)
                {
                    float blend = column == centerColumn ? CENTER_TILE_BLEND : NORMAL_TILE_BLEND;
                    tile.Flash(FLASH_DURATION, Color.red, blend);
                    _flashTiles.Add(tile);
                }
            }

            // 세로선 (같은 column, row 범위) - AttackColumn은 실제로 세로(row) 방향
            _tileGroup.GetCrossVerticalBounds(centerRow, centerColumn, attackColumn, out int minRow, out int maxRow);
            for (int row = minRow; row <= maxRow; row++)
            {
                BattlefieldTile tile = _tileGroup.GetTile(row, centerColumn);
                if (tile != null)
                {
                    float blend = row == centerRow ? CENTER_TILE_BLEND : NORMAL_TILE_BLEND;
                    tile.Flash(FLASH_DURATION, Color.red, blend);
                    _flashTiles.Add(tile);
                }
            }
        }

        private void FlashXTiles(int centerRow, int centerColumn, int attackRow, int attackColumn)
        {
            // 엑스자 형태: 대각선 2개
            // 대각선 1: 왼쪽 위에서 오른쪽 아래 (row 증가, column 증가)
            int maxDistance = Mathf.Max(attackRow, attackColumn);
            for (int i = -maxDistance; i <= maxDistance; i++)
            {
                int row = centerRow + i;
                int column = centerColumn + i;
                if (_tileGroup.IsValidTile(row, column))
                {
                BattlefieldTile tile = _tileGroup.GetTile(row, column);
                if (tile != null)
                {
                    float blend = row == centerRow && column == centerColumn ? CENTER_TILE_BLEND : NORMAL_TILE_BLEND;
                    tile.Flash(FLASH_DURATION, Color.red, blend);
                    _flashTiles.Add(tile);
                }
                }
            }

            // 대각선 2: 오른쪽 위에서 왼쪽 아래 (row 증가, column 감소)
            for (int i = -maxDistance; i <= maxDistance; i++)
            {
                int row = centerRow + i;
                int column = centerColumn - i;
                if (_tileGroup.IsValidTile(row, column))
                {
                BattlefieldTile tile = _tileGroup.GetTile(row, column);
                if (tile != null)
                {
                    float blend = row == centerRow && column == centerColumn ? CENTER_TILE_BLEND : NORMAL_TILE_BLEND;
                    tile.Flash(FLASH_DURATION, Color.red, blend);
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

        private IEnumerator ApplyWeaponEffectToCharacter(Character targetCharacter, WeaponData weaponData)
        {
            if (targetCharacter == null || targetCharacter.MyVital == null)
            {
                yield break;
            }

            int hitCount = Mathf.Max(MIN_HIT_COUNT, weaponData.MultiHitCount);

            Log.Info(LogTags.Weapon, "무기 효과 적용: 대상={0}, HitCount={1}", targetCharacter.Name.ToLogString(), hitCount.ToSelectString());

            Owner.SetTarget(targetCharacter);
            AttackEntity attackEntity = Owner.Attack.FindEntity(weaponData.Hitmark);
            if (attackEntity != null)
            {
                attackEntity.SetTarget(targetCharacter.MyVital);
            }
            float? weaponDamageOverride = weaponData.Damage;
            for (int i = 0; i < hitCount; i++)
            {
                Owner.Attack.Activate(weaponData.Hitmark, weaponDamageOverride);
                // 각 공격 사이에 작은 지연 추가
                if (i < hitCount - 1)
                {
                    yield return null;
                }
            }
        }

        private IEnumerator ApplyWeaponEffectToCharacterCoroutine(Character targetCharacter, WeaponData weaponData, System.Action onCompleted)
        {
            yield return StartCoroutine(ApplyWeaponEffectToCharacter(targetCharacter, weaponData));
            onCompleted?.Invoke();
        }

        private IEnumerator TryApplyAreaWeaponEffect(int targetRow, int targetColumn, WeaponData weaponData, System.Action<bool> onResult)
        {
            AttackAreaEntity areaEntity = FindOrSpawnAttackAreaEntity(weaponData.Hitmark);
            if (areaEntity == null)
            {
                onResult?.Invoke(false);
                yield break;
            }

            int hitCount = Mathf.Max(MIN_HIT_COUNT, weaponData.MultiHitCount);
            float? weaponDamageOverride = weaponData.Damage;

            for (int i = 0; i < hitCount; i++)
            {
                // 각 공격 전에 타일을 깜박임
                FlashAttackAreaTiles(targetRow, targetColumn, weaponData.AttackRow, weaponData.AttackColumn, weaponData.AttackAreaShape);

                // 깜박임 효과를 잠시 보여줌
                yield return new WaitForSeconds(FLASH_DURATION);

                areaEntity.SetWeaponDamageOverride(weaponDamageOverride);
                areaEntity.SetAreaByTile(targetRow, targetColumn, weaponData.AttackRow, weaponData.AttackColumn, weaponData.AttackAreaShape, _tileGroup);
                areaEntity.Activate();

                // 각 공격 사이에 지연 추가
                if (i < hitCount - 1)
                {
                    yield return new WaitForSeconds(AREA_ATTACK_DELAY);
                }
            }

            onResult?.Invoke(true);
        }

        private AttackAreaEntity FindOrSpawnAttackAreaEntity(HitmarkNames hitmark)
        {
            if (Owner == null || Owner.Attack == null)
            {
                return null;
            }

            AttackEntity attackEntity = Owner.Attack.FindEntity(hitmark);
            if (attackEntity == null)
            {
                attackEntity = Owner.Attack.SpawnAndRegisterEntity(hitmark);
            }

            AttackAreaEntity areaEntity = attackEntity as AttackAreaEntity;
            if (areaEntity == null)
            {
                Log.Warning(LogTags.Weapon, "영역 공격용 AttackAreaEntity를 찾을 수 없습니다. Hitmark: {0}", hitmark.ToLogString());
                return null;
            }

            areaEntity.SetOwner(Owner);
            return areaEntity;
        }

        private IEnumerator ApplyWeaponEffectFallback(int targetRow, int targetColumn, WeaponData weaponData)
        {
            _targetBuffer.Clear();
            _tileGroup.CollectMonstersInArea(targetRow, targetColumn, weaponData.AttackRow, weaponData.AttackColumn, weaponData.AttackAreaShape, _targetBuffer);
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
                    yield return StartCoroutine(ApplyWeaponEffectToCharacter(_targetBuffer[i], weaponData));
                }
            }
        }
    }
}