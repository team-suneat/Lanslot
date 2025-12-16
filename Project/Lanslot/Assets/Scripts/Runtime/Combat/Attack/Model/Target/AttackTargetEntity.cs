namespace TeamSuneat
{
    public class AttackTargetEntity : AttackEntity
    {
        public override void AutoNaming()
        {
            SetGameObjectName(Name.ToString());
        }

        public override void SetTarget(Vital targetVital)
        {
            base.SetTarget(targetVital);

            if (targetVital != null)
            {
                _damageInfo.SetTargetVital(targetVital);
                UpdatePositionToTargetTile();
            }
        }

        public override void Activate()
        {
            // Activate에서 피드백이 호출되기 전에 위치를 설정해야 함
            UpdatePositionToTargetTile();

            base.Activate();

            Execute();
        }

        public override void Execute()
        {
            base.Execute();

            if (!CanExecuteAttack())
            {
                return;
            }

            RefreshTarget();
            UpdatePositionToTargetTile();
            ApplyAttack();
        }

        private bool CanExecuteAttack()
        {
            if (!AssetData.IsValid())
            {
                LogWarning("공격 에셋 데이터가 유효하지 않습니다. Hitmark: {0}, Entity: {1}", Name.ToLogString(), this.GetHierarchyPath());
                return false;
            }

            return true;
        }

        private void RefreshTarget()
        {
            if (_damageInfo.TargetVital != null)
            {
                return;
            }

            Vital targetVital = GetTargetVital();
            if (CheckDamageableVital(targetVital))
            {
                _damageInfo.SetTargetVital(targetVital);
            }
        }

        private void UpdatePositionToTargetTile()
        {
            if (_damageInfo.TargetVital == null)
            {
                return;
            }

            Character targetCharacter = _damageInfo.TargetVital.Owner;
            if (targetCharacter == null)
            {
                return;
            }

            // BattlefieldTileGroup 가져오기
            BattlefieldTileGroup tileGroup = GetBattlefieldTileGroup();
            if (tileGroup == null)
            {
                // 타일 그룹을 찾을 수 없으면 타겟의 position 사용
                transform.position = _damageInfo.TargetVital.position;
                return;
            }

            // MonsterCharacter인 경우 타일 위치 찾기
            if (targetCharacter is MonsterCharacter monsterCharacter)
            {
                if (tileGroup.TryFindMonster(monsterCharacter, out int row, out int column))
                {
                    transform.position = tileGroup.GetTileWorldPosition(row, column);
                    return;
                }
            }

            // 타일 위치를 찾을 수 없으면 타겟의 position 사용
            transform.position = _damageInfo.TargetVital.position;
        }

        private BattlefieldTileGroup GetBattlefieldTileGroup()
        {
            if (GameApp.Instance?.gameManager?.CurrentStageSystem != null)
            {
                return GameApp.Instance.gameManager.CurrentStageSystem.BattlefieldTileGroup;
            }

            return null;
        }

        public override Vital GetTargetVital()
        {
            switch (AssetData.AttackTargetType)
            {
                case AttackTargetTypes.Owner:
                    return GetOwnerVital();

                case AttackTargetTypes.TargetOfOwner:
                    return GetOwnerTargetVital();

                default:
                    return null;
            }
        }

        private Vital GetOwnerVital()
        {
            return Owner?.MyVital;
        }

        private Vital GetOwnerTargetVital()
        {
            return Owner?.TargetCharacter?.MyVital;
        }

        private void ApplyAttack()
        {
            bool isAttackSucceeded = AttackToTarget();
            OnExecute(isAttackSucceeded);
        }

        private bool AttackToTarget()
        {
            if (!ValidateAttackConditions())
            {
                return false;
            }

            ExecuteDamageCalculation();
            return ProcessDamageResults();
        }

        private void ExecuteDamageCalculation()
        {
            _damageInfo?.Execute();
        }
    }
}