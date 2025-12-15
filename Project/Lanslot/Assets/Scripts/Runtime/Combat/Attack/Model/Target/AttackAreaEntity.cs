using System.Collections.Generic;

namespace TeamSuneat
{
    public class AttackAreaEntity : AttackEntity
    {
        private BattlefieldTileGroup _tileGroup;
        private int _centerRow;
        private int _centerColumn;
        private int _attackRow;
        private int _attackColumn;
        private AttackAreaShape _attackAreaShape = AttackAreaShape.None;

        private readonly List<MonsterCharacter> _monsterBuffer = new();
        private readonly List<Vital> _targetVitals = new();

        public void SetAreaByTile(int centerRow, int centerColumn, int attackRow, int attackColumn, AttackAreaShape shape, BattlefieldTileGroup tileGroup)
        {
            _centerRow = centerRow;
            _centerColumn = centerColumn;
            _attackRow = attackRow;
            _attackColumn = attackColumn;
            _attackAreaShape = shape;
            _tileGroup = tileGroup;
            
            // 중앙 위치를 transform.position에 설정
            if (_tileGroup != null)
            {
                transform.position = _tileGroup.GetTileWorldPosition(_centerRow, _centerColumn);
            }
        }

        public override void Activate()
        {
            base.Activate();

            Execute();
        }

        public override void Execute()
        {
            base.Execute();

            if (!AssetData.IsValid())
            {
                LogWarning("공격 에셋 데이터가 유효하지 않습니다. Hitmark: {0}, Entity: {1}", Name.ToLogString(), this.GetHierarchyPath());
                return;
            }

            CollectTargets();
            if (_targetVitals.Count == 0)
            {
                LogWarning("영역 내 유효한 타겟이 없습니다. Hitmark: {0}, Entity: {1}", Name.ToLogString(), this.GetHierarchyPath());
                return;
            }

            bool isAttackSucceeded = false;
            for (int i = 0; i < _targetVitals.Count; i++)
            {
                if (ExecuteAttackForTarget(_targetVitals[i]))
                {
                    isAttackSucceeded = true;
                }
            }

            OnExecute(isAttackSucceeded);
        }

        private bool ExecuteAttackForTarget(Vital targetVital)
        {
            if (!CheckDamageableVital(targetVital))
            {
                return false;
            }

            _damageInfo.SetTargetVital(targetVital);

            if (!ValidateAttackConditions())
            {
                return false;
            }

            _damageInfo.Execute();
            return ProcessDamageResults();
        }

        private void CollectTargets()
        {
            _targetVitals.Clear();

            if (!ValidateAreaParameters())
            {
                return;
            }

            CollectTargetsFromTileGroup();
        }

        private bool ValidateAreaParameters()
        {
            if (_tileGroup == null)
            {
                LogWarning("타일 그룹이 설정되지 않아 영역 공격을 수행할 수 없습니다. Hitmark: {0}, Entity: {1}", Name.ToLogString(), this.GetHierarchyPath());
                return false;
            }

            if (_attackAreaShape == AttackAreaShape.None)
            {
                LogWarning("영역 공격의 Shape이 설정되지 않았습니다. Hitmark: {0}, Entity: {1}", Name.ToLogString(), this.GetHierarchyPath());
                return false;
            }

            return true;
        }

        private void CollectTargetsFromTileGroup()
        {
            _monsterBuffer.Clear();
            _tileGroup.CollectMonstersInArea(_centerRow, _centerColumn, _attackRow, _attackColumn, _attackAreaShape, _monsterBuffer);

            for (int i = 0; i < _monsterBuffer.Count; i++)
            {
                Vital targetVital = _monsterBuffer[i]?.MyVital;
                if (targetVital == null)
                {
                    continue;
                }

                if (_targetVitals.Contains(targetVital))
                {
                    continue;
                }

                if (CheckDamageableVital(targetVital))
                {
                    _targetVitals.Add(targetVital);
                }
            }
        }
    }
}

