using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class MonsterAttackAbility : CharacterAbility
    {
        [SerializeField] private HitmarkNames _hitmarkOverride = HitmarkNames.None;

        private StageSystem _stageSystem;
        private BattlefieldTileGroup _tileGroup;
        private CharacterManager _characterManager;
        private MonsterCharacter _monster;
        private PlayerCharacter _player;

        public override void Initialization()
        {
            base.Initialization();
            EnsureReferences();
        }

        public bool TryAttackPlayerOnBottomRow()
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

            if (!IsPlayerValid(_player))
            {
                Log.Progress(LogTags.Monster, "플레이어를 찾을 수 없거나 공격할 수 없습니다.");
                return false;
            }

            int attackRange = _monster.Stat.FindValueOrDefaultToInt(StatNames.AttackRange);
            if (attackRange <= 0)
            {
                Log.Progress(LogTags.Monster, "몬스터 사거리가 0 이하입니다. AttackRange={0}", attackRange);
                return false;
            }

            int bottomRow = _tileGroup.GetBottomRowIndex();
            int maxAttackableRow = Mathf.Min(_tileGroup.Height - 1, bottomRow + (attackRange - 1));
            if (currentRow > maxAttackableRow)
            {
                Log.Progress(LogTags.Monster, "사거리 밖의 줄에서 공격할 수 없습니다. 현재줄={0}, 가능최대줄={1}, AttackRange={2}", currentRow, maxAttackableRow, attackRange);
                return false;
            }

            HitmarkNames hitmark = ResolveHitmark();
            if (hitmark == HitmarkNames.None)
            {
                Log.Warning(LogTags.Monster, "몬스터 기본 공격 히트마크가 설정되지 않았습니다.");
                return false;
            }

            return ExecuteAttack(hitmark);
        }

        private bool ExecuteAttack(HitmarkNames hitmark)
        {
            if (_player == null || _player.MyVital == null)
            {
                return false;
            }

            _monster.SetTarget(_player);
            AttackEntity attackEntity = _monster.Attack.FindEntity(hitmark);
            if (attackEntity != null)
            {
                attackEntity.SetTarget(_player.MyVital);
            }

            _monster.Attack.Activate(hitmark);

            Log.Info(LogTags.Monster, "최하단 몬스터가 플레이어를 공격했습니다. Monster={0}", _monster.Name.ToLogString());
            return true;
        }

        private HitmarkNames ResolveHitmark()
        {
            if (_hitmarkOverride != HitmarkNames.None)
            {
                return _hitmarkOverride;
            }

            if (_monster != null && _monster.Attack != null && _monster.Attack.BasicAttackHitmark != HitmarkNames.None)
            {
                return _monster.Attack.BasicAttackHitmark;
            }

            return HitmarkNames.None;
        }

        private bool EnsureReferences()
        {
            _monster ??= Owner as MonsterCharacter ?? GetComponent<MonsterCharacter>();
            _characterManager ??= CharacterManager.Instance;
            _stageSystem ??= GameApp.Instance != null && GameApp.Instance.gameManager != null ? GameApp.Instance.gameManager.CurrentStageSystem : null;
            _tileGroup ??= _stageSystem != null ? _stageSystem.BattlefieldTileGroup : null;
            _player ??= _characterManager != null ? _characterManager.Player : null;

            return _monster != null && _tileGroup != null && _characterManager != null;
        }

        private bool IsPlayerValid(PlayerCharacter player)
        {
            if (player == null)
            {
                return false;
            }

            if (!player.IsAlive)
            {
                return false;
            }

            if (player.MyVital == null)
            {
                return false;
            }

            return true;
        }
    }
}