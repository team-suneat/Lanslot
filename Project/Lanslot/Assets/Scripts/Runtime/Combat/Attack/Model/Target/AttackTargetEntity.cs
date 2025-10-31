namespace TeamSuneat
{
    public class AttackTargetEntity : AttackEntity
    {
        public override void AutoNaming()
        {
            SetGameObjectName(Name.ToString());
        }

        public override void Execute()
        {
            base.Execute();

            if (!CanExecuteAttack())
            {
                return;
            }

            RefreshTarget();
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

        private bool CheckDamageableVital(Vital targetVital)
        {
            if (targetVital == null)
            {
                return false;
            }
            else if (!targetVital.IsAlive)
            {
                return false;
            }
            else if (targetVital.Life.CheckInvulnerable())
            {
                return false;
            }

            return true;
        }

        private void ApplyAttack()
        {
            bool isAttackSuccessed = AttackToTarget();
            OnExecute(isAttackSuccessed);
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

        private bool ValidateAttackConditions()
        {
            if (_damageInfo.TargetVital == null)
            {
                LogWarning("공격 독립체의 목표 바이탈이 설정되지 않았습니다. Hitmark: {0}, Entity: {1}", _damageInfo.HitmarkAssetData.Name.ToLogString(), this.GetHierarchyPath());
                return false;
            }

            if (!_damageInfo.HitmarkAssetData.IsValid())
            {
                LogError("피해량 정보의 히트마크 에셋이 올바르지 않습니다. Hitmark:{0}, Entity: {1}", Name.ToLogString(), this.GetHierarchyPath());
                return false;
            }

            return true;
        }

        private void ExecuteDamageCalculation()
        {
            _damageInfo.Execute();
        }

        private bool ProcessDamageResults()
        {
            if (!_damageInfo.DamageResults.IsValid())
            {
                LogWarning("공격 독립체의 피해 결과가 설정되지 않았습니다. Hitmark: {0}, Entity: {1}", _damageInfo.HitmarkAssetData.Name.ToLogString(), this.GetHierarchyPath());
                return false;
            }

            bool isAttackSuccessed = false;
            for (int i = 0; i < _damageInfo.DamageResults.Count; i++)
            {
                if (ProcessSingleDamageResult(_damageInfo.DamageResults[i]))
                {
                    isAttackSuccessed = true;
                }
            }

            return isAttackSuccessed;
        }

        private bool ProcessSingleDamageResult(DamageResult damageResult)
        {
            switch (damageResult.DamageType)
            {
                case DamageTypes.Heal:
                case DamageTypes.HealOverTime:
                    return ProcessHealDamage(damageResult);

                case DamageTypes.Charge:
                    return ProcessChargeDamage(damageResult);

                default:
                    return ProcessRegularDamage(damageResult);
            }
        }

        private bool ProcessHealDamage(DamageResult damageResult)
        {
            _damageInfo.TargetVital.Heal(damageResult.DamageValueToInt);
            _damageInfo.TargetVital.DamageBuffOnHit(damageResult);
            return true;
        }

        private bool ProcessChargeDamage(DamageResult damageResult)
        {
            _damageInfo.TargetVital.Charge(damageResult.DamageValueToInt);
            _damageInfo.TargetVital.DamageBuffOnHit(damageResult);
            return true;
        }

        private bool ProcessRegularDamage(DamageResult damageResult)
        {
            if (_damageInfo.TargetVital.CheckDamageImmunity(damageResult))
            {
                return false;
            }

            if (_damageInfo.TargetVital.TakeDamage(damageResult))
            {
                TriggerDamageFeedback();
                return true;
            }

            return false;
        }

        private void TriggerDamageFeedback()
        {
            if (_damageInfo.TargetVital.IsAlive)
            {
                TriggerAttackOnHitDamageableFeedback(_damageInfo.TargetVital.position);
            }
            else
            {
                TriggerAttackOnKillFeedback(_damageInfo.TargetVital.position);
            }
        }
    }
}