using System;

namespace TeamSuneat
{
    public partial class DamageCalculator
    {
        // 자주 사용되는 능력치 이름들을 상수로 정의

        // 물리 피해량 배율 (Physical Damage Multiplier)
        private float CalculatePhysicalDamageMultiplier(DamageResult damageResult, int hitmarkLevel, int stack)
        {
#if UNITY_EDITOR
            int index = _stringBuilder.Length;
#endif

            float damageMultiplier = 0f;
            float damageMultiplierFromDamageType = CalculateDamageMultiplierByDamageType(damageResult.Asset.DamageType);

            damageMultiplier += damageMultiplierFromDamageType;
            if (stack > 1)
            {
                damageMultiplier += stack - 1;
                AddStackMultiplier(stack - 1);
            }

#if UNITY_EDITOR
            string content = GenerateContent(index, damageMultiplierFromDamageType, "물리 피해량 배율을 계산합니다. [능력치 물리 피해량 배율: ", "Calculates the Physical Damage Multiplier. [Stat Physical Damage Multiplier: ");
            _stringBuilder.Insert(index, content);
            _stringBuilder.AppendFormat("] => {0}", ValueStringEx.GetPercentString(damageMultiplier, 1));
            _stringBuilder.AppendLine();
#endif

            return damageMultiplier;
        }

        // 가시 피해량 배율 (Physical Damage Multiplier)
        private float CalculateThornsDamageMultiplier(DamageResult damageResult, int hitmarkLevel, int stack)
        {
#if UNITY_EDITOR
            int index = _stringBuilder.Length;
#endif

            float damageMultiplier = 0f;
            float damageMultiplierFromDamageType = CalculateDamageMultiplierByDamageType(damageResult.Asset.DamageType);

            damageMultiplier += damageMultiplierFromDamageType;

            if (stack > 1)
            {
                damageMultiplier += stack - 1;
                AddStackMultiplier(stack - 1);
            }

#if UNITY_EDITOR
            string content = GenerateContent(index, damageMultiplierFromDamageType, "물리 피해량 배율을 계산합니다. [능력치 물리 피해량 배율: ", "Calculates the Physical Damage Multiplier. [Stat Physical Damage Multiplier: ");
            _stringBuilder.Insert(index, content);
            _stringBuilder.AppendFormat("] => {0}", ValueStringEx.GetPercentString(damageMultiplier, 1));
            _stringBuilder.AppendLine();
#endif

            return damageMultiplier;
        }

        // 피해량 배율 계산 (Damage Multiplier)

        /// <summary> 피해 종류에 따른 피해량 배율을 계산합니다. </summary>
        private float CalculateDamageMultiplierByDamageType(DamageTypes damageType)
        {
            float damageRate = 1f;
            float elementDamageRate = 0;
            float magicDamageRateByStates = 0;
            float damageOverTimeRate = 0;

            // TODO: 특정 조건 피해량 적용

            float result = damageRate + damageOverTimeRate + elementDamageRate + magicDamageRateByStates;
            LogProgressDamageTypeMultiplier(damageRate, elementDamageRate, magicDamageRateByStates, damageOverTimeRate, result);

            return result;
        }

        private float AddMultiplier(float currentMultiplier, float additionalMultiplier, string koreanText, string englishText)
        {
            if (!additionalMultiplier.IsZero())
            {
                currentMultiplier += additionalMultiplier;
#if UNITY_EDITOR
                _stringBuilder.Append(string.Format(" + {0}({1})", ValueStringEx.GetPercentString(additionalMultiplier, 1), koreanText));
#endif
            }
            return currentMultiplier;
        }

        private void AddStackMultiplier(int stackMultiplier)
        {
#if UNITY_EDITOR
            _stringBuilder.Append(string.Format(" + {0}(스택에 따른 피해량 배율)", ValueStringEx.GetPercentString(stackMultiplier, 1)));
#endif
        }

        private string GenerateContent(int index, float damageMultiplierFromDamageType, string koreanText, string englishText)
        {
            string content = string.Format(koreanText + "{0}", ValueStringEx.GetPercentString(damageMultiplierFromDamageType, 1));
            return content;
        }

        /// <summary> 치명타 피해량 배율을 계산합니다. </summary>
        private float CalculateCriticalDamageMultiplier(DamageResult damageResult)
        {
            float criticalDamageMultiplier = 0f;
            _ = TryAddAttackerStatValue(StatNames.CriticalDamageMulti, ref criticalDamageMultiplier, LogCriticalDamageRate);
            return criticalDamageMultiplier;
        }

        private void TryAddStatValueIfCondition(object isCurrentMaxLife, StatNames damageWhileHealthy, ref float result, Action<float> logDamageWhileHealthy)
        {
        }

        #region Log

        private void LogDamageOfCoreSkillFromBasicSkill(float statValue)
        {
            if (Log.LevelProgress)
            {
                LogProgress("기본 기술의 피해량만큼 핵심 기술의 피해량 배율({0})을 적용합니다.", ValueStringEx.GetPercentString(statValue, true));
            }
        }

        private void LogCoreSkillToChilledEnemies(float statValue)
        {
            if (Log.LevelProgress)
            {
                LogProgress("피격 캐릭터가 오한 상태일 때 핵심 기술의 피해량 배율({0})을 적용합니다.", ValueStringEx.GetPercentString(statValue, true));
            }
        }

        private void LogCoreSkillOnCurrentResource(float statValue, int resource)
        {
            if (Log.LevelProgress)
            {
                LogProgress("캐릭터의 현재 자원({0})에 비례하여 핵심 기술의 피해량 배율({1})을 적용합니다: {2}",
                    ValueStringEx.GetValueString(resource, true),
                    ValueStringEx.GetPercentString(statValue, true),
                    ValueStringEx.GetPercentString(statValue * resource, true));
            }
        }

        private void LogPowerSkillOnCurrentResource(float statValue, float resource)
        {
            if (Log.LevelProgress)
            {
                LogProgress("캐릭터의 현재 자원({0})에 비례하여 숙련 기술의 피해량 배율({1})을 적용합니다: {2}",
                    ValueStringEx.GetValueString(resource, true),
                    ValueStringEx.GetPercentString(statValue, true),
                    ValueStringEx.GetPercentString(statValue * resource, true));
            }
        }

        private void LogPowerSkillToBurningEnemies(float statValue)
        {
            if (Log.LevelProgress)
            {
                LogProgress("피격 캐릭터가 연소 상태일 때 강한 기술의 피해량 배율({0})을 적용합니다.", ValueStringEx.GetPercentString(statValue, true));
            }
        }

        private void LogDamageMultiplierBySkillType(string skillType, float statValue)
        {
            if (Log.LevelProgress)
            {
                LogProgress("기술 종류({0})에 따른 피해량 배율({1})을 계산합니다.", skillType, ValueStringEx.GetPercentString(statValue, true));
            }
        }

        //

        private void LogDamageWhileHealthy(float damageWhileHealthy)
        {
            if (Log.LevelProgress)
            {
                LogProgress("최대 생명력일 때 추가 피해 배율({0})을 계산합니다.", ValueStringEx.GetPercentString(damageWhileHealthy, true));
            }
        }

        private void LogDamageToEnemies(float damageToEnemies, float damageByStrength, float damageByDexterity, float damageByIntelligence)
        {
            if (Log.LevelProgress)
            {
                LogProgress("적에게 주는 피해 배율({0})을 계산합니다. 힘 비례:{1}, 민첩 비례:{2}, 지능 비례:{3}",
                    ValueStringEx.GetPercentString(damageToEnemies, true),
                    ValueStringEx.GetPercentString(damageByStrength, 0),
                    ValueStringEx.GetPercentString(damageByDexterity, 0),
                    ValueStringEx.GetPercentString(damageByIntelligence, 0));
            }
        }

        private void LogDamageToBossEnemies(float damageToBossEnemies)
        {
            if (Log.LevelProgress)
            {
                LogProgress("보스에게 주는 피해 배율({0})을 계산합니다.", ValueStringEx.GetPercentString(damageToBossEnemies, true));
            }
        }

        private void LogDamageToIncapacitatedEnemies(float damageToIncapacitatedEnemies)
        {
            if (Log.LevelProgress)
            {
                LogProgress("행동불가 상태 피해 배율({0})을 계산합니다.", ValueStringEx.GetPercentString(damageToIncapacitatedEnemies, true));
            }
        }

        private void LogDamageToStateEffectEnemies(StateEffects stateEffect, float damageToBurningEnemies)
        {
            if (Log.LevelProgress)
            {
                LogProgress("{0} 상태 피해 배율({1})을 계산합니다.", stateEffect.GetLocalizedString(), ValueStringEx.GetPercentString(damageToBurningEnemies, true));
            }
        }

        private void LogDamageToEnemiesWithStateEffect(float statValue)
        {
            if (Log.LevelProgress)
            {
                LogProgress("둘 이상의 상태 이상 피해 배율({0})을 계산합니다.", ValueStringEx.GetPercentString(statValue, 0));
            }
        }

        private void LogDamageToEnemiesWithMore2StatusEffect(float statValue)
        {
            if (Log.LevelProgress)
            {
                LogProgress("둘 이상의 상태 이상 피해 배율({0})을 계산합니다.", ValueStringEx.GetPercentString(statValue, 0));
            }
        }

        private void LogDamageToEnemiesWithMore3StatusEffect(float statValue)
        {
            if (Log.LevelProgress)
            {
                LogProgress("셋 이상의 상태 이상 피해 배율({0})을 계산합니다.", ValueStringEx.GetPercentString(statValue, 0));
            }
        }

        private void LogCriticalDamageRate(float criticalDamageRate)
        {
            if (Log.LevelProgress)
            {
                LogProgress("공격자의 치명타 피해 배율({0})을 적용합니다.", ValueStringEx.GetPercentString(criticalDamageRate, 1));
            }
        }

        private void LogCruelStrikeCriticalDamageRate(float attackSpeedRate, float skillCriticalDamageRate)
        {
            if (Log.LevelProgress)
            {
                LogProgress("광전사의 [무자비한 잔혹한 타격] 효과인 추가 공격 속도({0})에 비례한 치명타 피해 배율({1})을 적용합니다.",
                    ValueStringEx.GetPercentString(attackSpeedRate, 0),
                    ValueStringEx.GetPercentString(skillCriticalDamageRate, 1));
            }
        }

        private void LogTargetCriticalDamageRate(float targetCriticalDamageRate)
        {
            if (Log.LevelProgress)
            {
                LogProgress("피격자의 치명타 피해 배율({0})을 적용합니다.", ValueStringEx.GetPercentString(targetCriticalDamageRate, 0));
            }
        }

        private void LogSpinRendCriticalDamageRate(float spinRendCriticalDamageRate)
        {
            if (Log.LevelProgress)
            {
                LogProgress("취약 상태의 적에게 광전사의 [회전 분쇄] 치명타 피해 배율({0})을 적용합니다.", ValueStringEx.GetPercentString(spinRendCriticalDamageRate, 0));
            }
        }

        //

        private void LogProgressDamageMultiplier(string koreanText, string englishText, float damageRate)
        {
            if (Log.LevelProgress)
            {
                LogProgress(koreanText + " {0}", ValueStringEx.GetPercentString(damageRate, 1));
            }
        }

        private void LogProgressDamageTypeMultiplier(float damageRate, float elementDamageRate, float magicDamageRateByStates, float damageOverTimeRate, float result)
        {
            if (Log.LevelProgress)
            {
                LogProgress(string.Format("물리 또는 마법 피해량 배율({0}) + 속성 피해량 배율({1}) + 상태이상에 따른 피해량 배율({2}) + 지속 피해량 배율({3}) = {4}",

                            ValueStringEx.GetPercentString(damageRate, 1),
                            ValueStringEx.GetPercentString(elementDamageRate, 0),
                            ValueStringEx.GetPercentString(magicDamageRateByStates, 0),
                            ValueStringEx.GetPercentString(damageOverTimeRate, 0),
                            ValueStringEx.GetPercentString(result, 1)));
            }
        }

        private void LogAdditionalDamageMultiplierByStat(StatNames statName, float statValue, float magnification, float result)
        {
            if (Log.LevelProgress)
            {
                LogProgress("능력치 비례 피해량 배율을 계산합니다. " +
                        $" 능력치 이름({statName.ToLogString()})" +
                        $", 능력치 값({ValueStringEx.GetPercentString(statValue, 0)})" +
                        $"* 능력치 비율({ValueStringEx.GetPercentString(magnification, 1)}) " +
                        $"= 최종 능력치 비례 피해 배율:({ValueStringEx.GetPercentString(result, 1)})");
            }
        }

        #endregion Log
    }
}