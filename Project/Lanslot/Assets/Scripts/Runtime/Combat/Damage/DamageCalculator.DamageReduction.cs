using TeamSuneat.Data;

namespace TeamSuneat
{
    public partial class DamageCalculator
    {
        private float CalculatePhysicalDamageReduction(DamageResult damageResult)
        {
#if UNITY_EDITOR
            int index = _stringBuilder.Length;
#endif
            GameDefineAsset defineAsset = ScriptableDataManager.Instance.GetGameDefine();
            GameDefineAssetData defineAssetData = defineAsset.Data;
            float damageReduction = 1f;

            // 공통 피해 감소율
            float commonRate = CalculateDamageReduction(damageResult); // 피해 감소율: 0.3이면 30% 감소
            float minReductionFactor = defineAssetData.MAX_DAMAGE_REDUCTION_RATE;

            if (commonRate > minReductionFactor)
            {
                AddLogMaxPhysicalDamageReduction(commonRate, minReductionFactor);
                commonRate = minReductionFactor;
            }

            // 피해 감소율
            float combinedMultiplier = 1 - commonRate; // 예: 1 - 0.3 = 0.7(70%)
            damageReduction *= combinedMultiplier;
            AddLogPhysicalDamageReduction(commonRate);

            // 피해 증폭 (쇠퇴율) 적용
            if (!DecrescenceRate.IsZero())
            {
                float decrescenceMultiplier = 1 + DecrescenceRate;
                damageReduction *= decrescenceMultiplier;
                AddLogDamageDecrescenceRate(decrescenceMultiplier);
            }

            // 최종 피해 계수 로그
#if UNITY_EDITOR
            if (!damageReduction.Compare(1))
            {
                AddLogTotalPhysicalDamageReduction(index, damageReduction);
            }
#endif

            return damageReduction;
        }

        private float CalculateMagicDamageReduction(DamageResult damageResult)
        {
#if UNITY_EDITOR
            int index = _stringBuilder.Length;
#endif
            GameDefineAssetData defineAssetData = ScriptableDataManager.Instance.GetGameDefine().Data;

            float damageReduction = 1f;

            // 1. 공통 + 마법 피해 감소율 합산 처리
            float commonRate = 1f - CalculateDamageReduction(damageResult); // 예: 0.3 ▶ 30% 감소
            float magicRate = 0f;
            float baseReductionRate = commonRate + magicRate;

            float maxReduction = defineAssetData.MAX_DAMAGE_REDUCTION_RATE;
            if (baseReductionRate > maxReduction)
            {
                AddLogMaxMagicDamageReduction(baseReductionRate, maxReduction);
                baseReductionRate = maxReduction;
            }

            float baseMultiplier = 1f - baseReductionRate;
            damageReduction *= baseMultiplier;

            AddLogMagicDamageReduction(baseReductionRate);

            // 피해 증폭 (쇠퇴율) 적용
            if (!DecrescenceRate.IsZero())
            {
                float decrescenceMultiplier = 1 + DecrescenceRate;
                damageReduction *= decrescenceMultiplier;
                AddLogDamageDecrescenceRate(decrescenceMultiplier);
            }

#if UNITY_EDITOR
            // 최종 피해 계수 로그
            if (!damageReduction.Compare(1))
            {
                LogTotalMagicDamageReduction(index, damageReduction);
            }
#endif

            return damageReduction;
        }

        private float CalculateDamageReduction(DamageResult damageResult)
        {
            float damageReduction = 0f;
            if (TryAddTargetStatValue(StatNames.DamageReduction, ref damageReduction))
            {
                LogCommonDamageReductionFromStat(damageReduction);
            }

            return damageReduction;
        }

        //

        private float CalculateDamageReductionFromResistance(DamageAssetData damageAsset)
        {
            float magicalResistanceRate = 0f;
            float elementResistanceRate = 0f;
            GameDefineAssetData defineAssetData = ScriptableDataManager.Instance.GetGameDefine().Data;
            float result = (magicalResistanceRate + elementResistanceRate) * 0.5f;
            if (result > defineAssetData.MAX_DAMAGE_RESISTANCE_REDUCTION_RATE)
            {
                result = defineAssetData.MAX_DAMAGE_RESISTANCE_REDUCTION_RATE;
            }

            return result;
        }

        #region Log

        private void AddLogMaxPhysicalDamageReduction(float reduction, float maxReduction)
        {
            if (Log.LevelInfo)
            {
#if UNITY_EDITOR
                string content = string.Format("{0} ▶ {1}(능력치에 의한 물리 피해 감소 최대값 적용)",
                    ValueStringEx.GetPercentString(reduction, 0),
                    ValueStringEx.GetPercentString(maxReduction, 0));

                _stringBuilder.Append(content);

#endif
            }
        }

        private void AddLogPhysicalDamageReduction(float reduction)
        {
            if (Log.LevelInfo)
            {
#if UNITY_EDITOR
                _stringBuilder.Append(string.Format("{0}(능력치에 의한 물리 피해 감소)", ValueStringEx.GetPercentString(reduction, 0)));

#endif
            }
        }

        private void AddLogPhysicalDamageReductionByArmor(float reduction)
        {
            if (Log.LevelInfo)
            {
#if UNITY_EDITOR
                _stringBuilder.Append(string.Format("* {0}(방어력을 통한 물리 피해 감소)", ValueStringEx.GetPercentString(reduction, 0)));

#endif
            }
        }

        private void AddLogTotalPhysicalDamageReduction(int index, float result)
        {
            if (Log.LevelInfo)
            {
#if UNITY_EDITOR
                _stringBuilder.Insert(index, "최종 물리 피해 감소량을 계산합니다. [");
                _stringBuilder.AppendLine($"] => {ValueStringEx.GetPercentString(result, 0)}");

#endif
            }
        }

        //

        private void AddLogMaxMagicDamageReduction(float reduction, float maxReduction)
        {
            if (Log.LevelInfo)
            {
#if UNITY_EDITOR
                string content = string.Format("{0} ▶ {1}(능력치에 의한 마법 피해 감소 최대값 적용)",
                    ValueStringEx.GetPercentString(reduction, 0),
                    ValueStringEx.GetPercentString(maxReduction, 0));

                _stringBuilder.Append(content);

#endif
            }
        }

        private void AddLogMagicDamageReduction(float reduction)
        {
            if (Log.LevelInfo)
            {
#if UNITY_EDITOR
                _stringBuilder.Append(string.Format("{0}(능력치에 의한 마법 피해 감소)", ValueStringEx.GetPercentString(reduction, 0)));
#endif
            }
        }

        private void AddLogMagicDamageReductionByResistance(float reduction)
        {
            if (Log.LevelInfo)
            {
#if UNITY_EDITOR
                _stringBuilder.Append(string.Format(" * {0}(저항력을 통한 마법 피해 감소)", ValueStringEx.GetPercentString(reduction, 0)));

#endif
            }
        }

        private void LogTotalMagicDamageReduction(int index, float result)
        {
#if UNITY_EDITOR
            _stringBuilder.Insert(index, "최종 마법 피해 감소량을 계산합니다. [");
            _stringBuilder.AppendLine($"] => {ValueStringEx.GetPercentString(-1 + result, 0)}");
#endif
        }

        //

        private void AddLogDamageDecrescenceRate(float decrescenceRate)
        {
            if (Log.LevelInfo)
            {
#if UNITY_EDITOR
                _stringBuilder.Append(string.Format(" * {0}(히트마크의 피해량 점감)", ValueStringEx.GetPercentString(decrescenceRate, 0)));

#endif
            }
        }

        private void LogCommonDamageReductionFromStat(float reduction)
        {
            if (Log.LevelInfo)
            {
#if UNITY_EDITOR
                LogInfo("공통 피해 감소량을 계산합니다. {0}(능력치에 의한 공통 피해 감소)", ValueStringEx.GetPercentString(reduction, 0));

#endif
            }
        }

        private void LogCommonDamageReductionFromDamageType(float reduction, DamageTypes damageType)
        {
            if (Log.LevelInfo)
            {
#if UNITY_EDITOR
                LogInfo("공통 피해 감소량을 계산합니다. {0}(피해 종류에 의한 공통 피해 감소: {1})", ValueStringEx.GetPercentString(reduction, 0), damageType);

#endif
            }
        }

        private void LogCommonDamageReductionFromMonster(float reduction)
        {
            if (Log.LevelInfo)
            {
#if UNITY_EDITOR
                LogInfo("공통 피해 감소량을 계산합니다. {0}(몬스터 등급에 의한 공통 피해 감소)", ValueStringEx.GetPercentString(reduction, 0));

#endif
            }
        }

        private void LogCommonDamageReductionFromDistance(float reduction)
        {
            if (Log.LevelInfo)
            {
#if UNITY_EDITOR
                LogInfo("공통 피해 감소량을 계산합니다. {0}(거리에 의한 공통 피해 감소)", ValueStringEx.GetPercentString(reduction, 0));

#endif
            }
        }

        #endregion Log
    }
}