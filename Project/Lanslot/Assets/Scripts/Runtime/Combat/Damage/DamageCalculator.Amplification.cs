using System.Linq;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public partial class DamageCalculator
    {
        private float CalculateDamageAmplification(HitmarkAssetData damageAssetData, DamageResult damageResult)
        {
            float damageAmplification = 1f;
            float criticalDamageMultiplier = GetCriticalDamageMultiplier(damageResult);

            if (!criticalDamageMultiplier.IsEqual(1))
            {
                damageAmplification *= criticalDamageMultiplier;
            }

            return Mathf.Max(0.01f, damageAmplification); // 최소값 보정
        }

        private float GetCriticalDamageMultiplier(DamageResult damageResult)
        {
            if (damageResult.DamageType.IsInstantDamage())
            {
                damageResult.IsCritical = DetermineCritical(HitmarkAssetData);
                if (damageResult.IsCritical)
                {
                    return CalculateCriticalDamageMultiplier(damageResult);
                }
            }

            return 1f;
        }

        private float GetDamageAmplificationByStat(DamageResult damageResult, StatNames statName)
        {
            float result = 1f;
            if (TryAddAttackerStatValueIfNotEqual(statName, ref result, 1f,
                (value) => LogDamageAmplificationByStat(statName, value)))
            {
                return result;
            }
            return 1f;
        }

        private bool TryMultiplierState()
        {
            StateEffects[] states = { StateEffects.Weak, StateEffects.Dazed, StateEffects.Vulnerable, StateEffects.EnhancedVulnerable };
            return states.Any(state => ContainsTargetCharacterStateEffect(state));
        }

        private void AddLogDamageAmplication(float damageAmplification,
            float damageAmplificationByDamageType,
            float damageAmplificationByTick,
            float dotDamageAmplification,
            float criticalDamageMultiplier,
            float damageAmplificationFromTarget)
        {
            if (Log.LevelInfo)
            {
#if UNITY_EDITOR
                _stringBuilder.AppendLine();
                _stringBuilder.AppendFormat("피해 증폭 배율[({0}) = 피해종류:{1} * 틱마다:{2} * 지속피해:{3} * 치명타:{4} * 피격자:{5}",
                        ValueStringEx.GetPercentString(damageAmplification, 1),
                        ValueStringEx.GetPercentString(damageAmplificationByDamageType, 1),
                        ValueStringEx.GetPercentString(damageAmplificationByTick, 1),
                        ValueStringEx.GetPercentString(dotDamageAmplification, 1),
                        ValueStringEx.GetPercentString(criticalDamageMultiplier, 1),
                        ValueStringEx.GetPercentString(damageAmplificationFromTarget, 1));
                _stringBuilder.AppendLine();
#endif
            }
        }

        private void LogDamageAmplificationByStat(StatNames statName, float statValue)
        {
            if (Log.LevelProgress)
            {
                LogProgress("능력치({0})에 의해 피해량 증폭이 적용됩니다. {1}", statName.ToLogString(), ValueStringEx.GetPercentString(statValue, 0));
            }
        }
    }
}