using TeamSuneat.Data;
using TeamSuneat.Setting;

namespace TeamSuneat
{
    public partial class DamageCalculator
    {
        private bool TryCheatDamage(HitmarkAssetData damageAsset, ref DamageResult damageResult)
        {
            if (Attacker == null)
            {
                return TryReceiveDamageOnlyOne(damageAsset, ref damageResult);
            }
            else if (damageAsset.DamageType.IsDamage() || damageAsset.DamageType == DamageTypes.Grab)
            {
                return TryReceiveDamageOnlyOne(damageAsset, ref damageResult);
            }

            return false;
        }

        private bool DetermineEvasion(DamageResult damageResult)
        {
            if (damageResult.Asset.IgnoreEvasion)
            {
                return false;
            }

            if (damageResult.TargetCharacter == null)
            {
                return false;
            }

            float targetEvasionChance = damageResult.TargetCharacter.Stat.FindValueOrDefault(StatNames.EvasionChance);
            if (RandomEx.GetFloatValue() < targetEvasionChance)
            {
                SpawnFloatyText("Evasion");
                _ = GlobalEvent<DamageResult>.Send(GlobalEventType.PLAYER_CHARACTER_DODGE, damageResult);

                return true;
            }

            return false;
        }

        private bool DetermineImmuneCC(bool isCrowdControl)
        {
            if (!isCrowdControl)
            {
                return false;
            }
            else if (TargetCharacter == null)
            {
                return false;
            }
            else if (TargetCharacter.IsPlayer && GameSetting.Instance.Cheat.NotCrowdControl)
            {
                SpawnFloatyText("Immune");
                return true;
            }
            else if (TargetCharacter.IgnoreCrowdControl)
            {
                SpawnFloatyText("Immune");
                return true;
            }

            return false;
        }

        private bool DetermineCritical(HitmarkAssetData hitmarkAssetData)
        {
            if (Attacker == null || TargetCharacter == null)
            {
                return false;
            }

            if (Attacker != TargetCharacter && Attacker.IsPlayer)
            {
                if (GameSetting.Instance.Cheat.CriticalType == GameCheat.CriticalTypes.Critical)
                {
                    LogInfoCriticalAppliedByCheat();
                    return true;
                }
                else if (GameSetting.Instance.Cheat.CriticalType == GameCheat.CriticalTypes.NoCritical)
                {
                    LogInfoNoCriticalAppliedByCheat();
                    return false;
                }
            }

            float criticalChance = 0f;
            float targetCriticalChance = 0f;

            TryAddAttackerCalculateStatValue(StatNames.CriticalChance, ref criticalChance);

            GameDefineAssetData defineAssetData = ScriptableDataManager.Instance.GetGameDefine().Data;
            float resultCriticalChance = criticalChance + targetCriticalChance;       

            // 치명타 확률이 0% 이하이면 치명타 발생하지 않음
            if (resultCriticalChance <= 0f)
            {
                LogCriticalHitNotApplied(resultCriticalChance, criticalChance, targetCriticalChance);
                return false;
            }

            float randomValue = RandomEx.GetFloatValue();
            if (randomValue < resultCriticalChance)
            {
                LogCriticalHitApplied(resultCriticalChance, criticalChance, targetCriticalChance);
                return true;
            }

            LogCriticalHitNotApplied(resultCriticalChance, criticalChance, targetCriticalChance);
            return false;
        }

        private bool DetermineExecute(DamageResult damageResult)
        {
            if (damageResult.TargetVital != null)
            {
                if (damageResult.TargetVital.LifeRate <= damageResult.Asset.ExecutionConditionalTargetLifeRate)
                {
                    LogExecutionApplied(damageResult.TargetVital.LifeRate, damageResult.Asset.ExecutionConditionalTargetLifeRate);
                    return true;
                }
            }

            return false;
        }
    }
}