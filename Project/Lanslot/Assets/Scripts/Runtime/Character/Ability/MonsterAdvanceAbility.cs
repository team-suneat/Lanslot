using UnityEngine;
using System.Collections;

namespace TeamSuneat
{
    public class MonsterAdvanceAbility : CharacterAbility
    {
        [SerializeField] private float _advanceFlashDuration = 0.3f;

        private StageSystem _stageSystem;
        private BattlefieldTileGroup _tileGroup;
        private CharacterManager _characterManager;
        private MonsterCharacter _monster;

        public override void Initialization()
        {
            base.Initialization();
            EnsureReferences();
        }

        public IEnumerator AdvanceWithFlashCoroutine()
        {
            if (!EnsureReferences())
            {
                yield break;
            }

            if (!_tileGroup.TryFindMonster(_monster, out int currentRow, out int currentColumn))
            {
                Log.Warning(LogTags.Monster, "몬스터 위치를 찾을 수 없습니다.");
                yield break;
            }

            if (!_tileGroup.CanMoveDown(currentRow, currentColumn, out BattlefieldTile nextTile, out int nextRow, out int nextColumn))
            {
                Log.Progress(LogTags.Monster, "전진 불가: ({0}, {1})", currentRow, currentColumn);
                yield break;
            }

            // 이동 예정 타일을 깜박여 이동 경로 표시 (흰색)
            nextTile?.Flash(_advanceFlashDuration, Color.white, 0.1f);

            if (_advanceFlashDuration > 0f)
            {
                yield return new WaitForSeconds(_advanceFlashDuration);
            }

            PerformMove(nextTile, currentRow, currentColumn, nextRow, nextColumn);
        }

        public bool TryAdvanceImmediate()
        {
            if (!EnsureReferences())
            {
                return false;
            }

            if (!_tileGroup.TryFindMonster(_monster, out int currentRow, out int currentColumn))
            {
                Log.Warning(LogTags.Monster, "몬스터 위치를 찾을 수 없습니다.");
                return false;
            }

            if (!_tileGroup.CanMoveDown(currentRow, currentColumn, out BattlefieldTile nextTile, out int nextRow, out int nextColumn))
            {
                Log.Progress(LogTags.Monster, "전진 불가: ({0}, {1})", currentRow, currentColumn);
                return false;
            }

            nextTile?.Flash(_advanceFlashDuration, Color.white, 0.1f);
            return PerformMove(nextTile, currentRow, currentColumn, nextRow, nextColumn);
        }

        private bool PerformMove(BattlefieldTile targetTile, int fromRow, int fromColumn, int toRow, int toColumn)
        {
            if (targetTile == null || _tileGroup == null || _monster == null)
            {
                return false;
            }

            bool moved = _tileGroup.MoveMonster(_monster, fromRow, fromColumn, toRow, toColumn);
            if (!moved)
            {
                Log.Warning(LogTags.Monster, "전진 실패: ({0}, {1}) → ({2}, {3})", fromRow, fromColumn, toRow, toColumn);
                return false;
            }

            Log.Info(LogTags.Monster, "전진 완료: ({0}, {1}) → ({2}, {3})", fromRow, fromColumn, toRow, toColumn);
            return true;
        }

        private bool EnsureReferences()
        {
            _monster ??= Owner as MonsterCharacter ?? GetComponent<MonsterCharacter>();
            _characterManager ??= CharacterManager.Instance;
            _stageSystem ??= GameApp.Instance != null && GameApp.Instance.gameManager != null ? GameApp.Instance.gameManager.CurrentStageSystem : null;
            _tileGroup ??= _stageSystem != null ? _stageSystem.BattlefieldTileGroup : null;

            if (_monster == null || _tileGroup == null || _characterManager == null)
            {
                return false;
            }

            return true;
        }
    }
}