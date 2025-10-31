using UnityEngine;

namespace TeamSuneat
{
    public partial class DamageCalculator
    {
        // 공통 헬퍼 (DamageCalculator)
        private bool TryApplyReferenceDamage(DamageAssetData damageAssetData, ref DamageResult damageResult)
        {
            if (damageAssetData.LinkedDamageType == LinkedDamageTypes.Attack && ReferenceValue > 0f)
            {
                damageResult.DamageValue = ReferenceValue;
                damageResult.ReducedDamageValue = 0f;
                return true;
            }
            return false;
        }

        private int GetHitmarkLevel()
        {
            return Mathf.Max(Level, 1); // 히트마크 최소 레벨 설정
        }

        //

        private void ComputeByType(DamageAssetData damageAsset, ref DamageResult damageResult)
        {
            if (TryCheatDamage(damageAsset, ref damageResult))
            {
                return;
            }

            switch (damageAsset.DamageType)
            {
                case DamageTypes.Heal:
                case DamageTypes.HealOverTime: { ComputeHealValue(damageAsset, ReferenceValue, ref damageResult); } break;
                case DamageTypes.Charge: { ComputeChargeValue(damageAsset, ReferenceValue, ref damageResult); } break;
                case DamageTypes.Physical: { HandleComputePhysical(damageAsset, ref damageResult); } break;
                case DamageTypes.Grab: { HandleComputeGrab(damageAsset, ref damageResult); } break;
                case DamageTypes.Magical:
                case DamageTypes.Fire:
                case DamageTypes.Cold:
                case DamageTypes.Lightning:
                case DamageTypes.Poison:
                case DamageTypes.Darkness:
                case DamageTypes.Holy: { HandleComputeMagical(damageAsset, ref damageResult); } break;
                case DamageTypes.BleedOverTime: { HandleComputePhysicalOverTime(damageAsset, ref damageResult); } break;
                case DamageTypes.FireOverTime:
                case DamageTypes.ColdOverTime:
                case DamageTypes.LightningOverTime:
                case DamageTypes.PoisonOverTime:
                case DamageTypes.DarknessOverTime:
                case DamageTypes.HolyOverTime: { HandleComputeMagicalOverTime(damageAsset, ref damageResult); } break;
                case DamageTypes.Execution: { damageResult.DamageValue = float.MaxValue; } break;
                case DamageTypes.Thorns: { HandleComputeThorns(damageAsset, ref damageResult); } break;
                case DamageTypes.Overwhelm: { HandleComputeOverwhelm(damageAsset, ref damageResult); } break;
                default:
                    Log.Warning("데미지 에셋에서 피해 종류(DamageType) 설정이 올바르지 않을 가능성이 있습니다: {0}, {1}", damageAsset.Hitmark.ToLogString(), damageAsset.DamageType);
                    break;
            }
        }

        private void HandleComputePhysical(DamageAssetData damageAsset, ref DamageResult damageResult)
        {
            if (DetermineEvasion(damageResult)) { damageResult.IsEvasion = true; return; }
            if (DetermineExecute(damageResult)) { damageResult.DamageValue = float.MaxValue; return; }
            if (TryApplyReferenceDamage(damageAsset, ref damageResult))
            {
                return;
            }

            ComputePhysicalDamage(damageAsset, GetHitmarkLevel(), ref damageResult);
        }

        private void HandleComputeGrab(DamageAssetData damageAsset, ref DamageResult damageResult)
        {
            if (DetermineEvasion(damageResult)) { damageResult.IsEvasion = true; return; }
            if (TryApplyReferenceDamage(damageAsset, ref damageResult))
            {
                return;
            }

            ComputePhysicalDamage(damageAsset, GetHitmarkLevel(), ref damageResult);
        }

        private void HandleComputeMagical(DamageAssetData damageAsset, ref DamageResult damageResult)
        {
            if (DetermineEvasion(damageResult)) { damageResult.IsEvasion = true; return; }
            if (DetermineExecute(damageResult)) { damageResult.DamageValue = float.MaxValue; return; }
            if (TryApplyReferenceDamage(damageAsset, ref damageResult))
            {
                return;
            }

            ComputeMagicalDamage(damageAsset, GetHitmarkLevel(), ref damageResult);
        }

        private void HandleComputePhysicalOverTime(DamageAssetData damageAsset, ref DamageResult damageResult)
        {
            if (TryApplyReferenceDamage(damageAsset, ref damageResult))
            {
                return;
            }

            ComputePhysicalDamageOverTime(damageAsset, GetHitmarkLevel(), Stack, ref damageResult);
        }

        private void HandleComputeMagicalOverTime(DamageAssetData damageAsset, ref DamageResult damageResult)
        {
            if (TryApplyReferenceDamage(damageAsset, ref damageResult))
            {
                return;
            }

            ComputeMagicalDamageOverTime(damageAsset, GetHitmarkLevel(), Stack, ref damageResult);
        }

        private void HandleComputeThorns(DamageAssetData damageAsset, ref DamageResult damageResult)
        {
            if (TryApplyReferenceDamage(damageAsset, ref damageResult))
            {
                return;
            }

            ComputeThornsDamage(damageAsset, GetHitmarkLevel(), ref damageResult);
        }

        private void HandleComputeOverwhelm(DamageAssetData damageAsset, ref DamageResult damageResult)
        {
            if (TryApplyReferenceDamage(damageAsset, ref damageResult))
            {
                return;
            }
        }

        // Compute

        private void ComputePhysicalDamage(DamageAssetData damageAsset, int hitmarkLevel, ref DamageResult damageResult)
        {
            ClearLogBuilder();
            AppendLineToLog();

            // 피해량 계산
            float totalAttackPower = FindAttackerStatValue(StatNames.Damage);
            float fixedDamage = StatEx.GetValueByLevel(damageAsset.FixedDamage, damageAsset.FixedDamageByLevel, hitmarkLevel);
            float damageRatio = CalculatePhysicalDamageRatio(damageAsset, hitmarkLevel);
            float physicalDamage = (totalAttackPower + fixedDamage) * damageRatio;

            LogDamageCalculation("물리 피해량", physicalDamage, totalAttackPower, damageRatio);

            // 피해량 배율 계산
            float physicalDamageMultiplier = CalculatePhysicalDamageMultiplier(damageResult, hitmarkLevel, 0);

            // 피해 감소 계산
            float damageReduction = CalculatePhysicalDamageReduction(damageResult);

            // 최종 피해 계산 요소
            float damageAmplification = CalculateDamageAmplification(damageAsset, damageResult);

            /*
             * 총 물리 피해량 = (물리 피해량 × 배율) + 추가 피해
             * 최종 피해량 = 총 피해량 × 피해 감소 × 증폭 × 취약 배율
             * 감소된 피해량 = 총 피해량 × (1 - 피해 감소 배율)
             */

            float totalPhysicalDamage = (physicalDamage * physicalDamageMultiplier);
            float finalDamageValue = totalPhysicalDamage * damageReduction * damageAmplification;
            float reducedDamageValue = totalPhysicalDamage * (1f - damageReduction);

            LogDamageCalculationStart(
                Attacker,
                TargetVital,
                physicalDamage,
                physicalDamageMultiplier,
                damageReduction,
                damageAmplification,
                finalDamageValue);

            LogInfo(GetLogString());

            finalDamageValue = Mathf.Max(damageAsset.MinDamageValue, finalDamageValue);

            damageResult.DamageValue = finalDamageValue;
            damageResult.ReducedDamageValue = reducedDamageValue;
        }

        private void ComputePhysicalDamageOverTime(DamageAssetData damageAsset, int hitmarkLevel, int stack, ref DamageResult damageResult)
        {
            ClearLogBuilder();
            AppendLineToLog();

            // 피해량 계산
            float totalAttackPower = FindAttackerStatValue(StatNames.Damage);
            float damageRatio = CalculatePhysicalDamageRatio(damageAsset, hitmarkLevel);
            float physicalDamage = totalAttackPower * damageRatio;

            LogDamageCalculation("물리 지속 피해량", physicalDamage, totalAttackPower, damageRatio);

            // 피해량 배율 계산
            float physicalDamageMultiplier = CalculatePhysicalDamageMultiplier(damageResult, hitmarkLevel, stack);

            // 추가 피해량 계산
            float fixedDamage = StatEx.GetValueByLevel(damageAsset.FixedDamage, damageAsset.FixedDamageByLevel, hitmarkLevel);

            // 피해 감소 계산
            float damageReduction = CalculatePhysicalDamageReduction(damageResult);
            LogDamageReduction("물리 지속", damageReduction);

            // 최종 물리 피해 계산
            float damageAmplification = CalculateDamageAmplification(damageAsset, damageResult);

            /*
             * 최종 물리 지속 피해량=((물리 피해량×물리 피해 배율)+추가 물리 피해량)×피해 감소×피해 증폭
             * 감소된 물리 지속 피해량=((물리 피해량×물리 피해 배율)+추가 물리 피해량)×(1−피해 감소)
             */

            float totalPhysicalDamage = (physicalDamage * physicalDamageMultiplier);
            float finalDamageValue = totalPhysicalDamage * damageReduction * damageAmplification;
            float reducedDamageValue = totalPhysicalDamage * (1f - damageReduction);

            LogDamageCalculationStart(
                Attacker,
                TargetVital,
                physicalDamage,
                physicalDamageMultiplier,
                damageReduction,
                damageAmplification,
                finalDamageValue);

            LogInfo(GetLogString());

            finalDamageValue = Mathf.Max(damageAsset.MinDamageValue, finalDamageValue);

            damageResult.DamageValue = finalDamageValue;
            damageResult.ReducedDamageValue = reducedDamageValue;
        }

        private void ComputeMagicalDamage(DamageAssetData damageAsset, int hitmarkLevel, ref DamageResult damageResult)
        {
            ClearLogBuilder();
            AppendLineToLog();

            // 피해량 계산
            float totalAttackPower = FindAttackerStatValue(StatNames.Damage);
            float damageRatio = CalculateMagicalDamageRatio(damageAsset, hitmarkLevel);
            float magicalDamage = totalAttackPower * damageRatio;

            LogDamageCalculation("마법 피해량", magicalDamage, totalAttackPower, damageRatio);

            // 피해량 배율 계산
            float magicalDamageMultiplier = CalculateMagicDamageMultiplier(damageResult, hitmarkLevel, 0);

            // 추가 피해량 계산
            float fixedDamage = StatEx.GetValueByLevel(damageAsset.FixedDamage, damageAsset.FixedDamageByLevel, hitmarkLevel);

            // 피해 감소율 계산
            float damageReduction = CalculateMagicDamageReduction(damageResult);
            LogDamageReduction("마법", damageReduction);

            // 최종 마법 피해 계산
            float damageAmplification = CalculateDamageAmplification(damageAsset, damageResult);

            /*
             * 총 마법 피해량 = (마법 피해량 × 마법 피해 배율) + 추가 마법 피해량
             * 최종 마법 피해량 = 총 마법 피해량 × 피해 감소 × 피해 증폭 × 취약 피해 배율
             * 감소된 마법 피해량 = 총 마법 피해량 × (1 − 피해 감소 )
             */

            float totalMagicalDamage = (magicalDamage * magicalDamageMultiplier);
            float finalDamageValue = totalMagicalDamage * damageReduction * damageAmplification;
            float reducedDamageValue = totalMagicalDamage * (1f - damageReduction);

            LogDamageCalculationStart(
                Attacker,
                TargetVital,
                magicalDamage,
                magicalDamageMultiplier,
                damageReduction,
                damageAmplification,
                finalDamageValue);

            LogInfo(GetLogString());

            finalDamageValue = Mathf.Max(damageAsset.MinDamageValue, finalDamageValue);

            damageResult.DamageValue = finalDamageValue;
            damageResult.ReducedDamageValue = reducedDamageValue;
        }

        private void ComputeMagicalDamageOverTime(DamageAssetData damageAsset, int hitmarkLevel, int stack, ref DamageResult damageResult)
        {
            ClearLogBuilder();
            AppendLineToLog();

            // 피해량 계산
            float totalAttackPower = FindAttackerStatValue(StatNames.Damage);
            float damageRatio = CalculateMagicalDamageRatio(damageAsset, hitmarkLevel);
            float magicalDamage = totalAttackPower * damageRatio;

            LogDamageCalculation("마법 지속 피해량", magicalDamage, totalAttackPower, damageRatio);

            // 피해량 배율 계산
            float magicalDamageMultiplier = CalculateMagicDamageMultiplier(damageResult, hitmarkLevel, stack);

            // 추가 피해량 계산
            float fixedDamage = StatEx.GetValueByLevel(damageAsset.FixedDamage, damageAsset.FixedDamageByLevel, hitmarkLevel);

            // 피해 감소율 계산
            float damageReduction = CalculateMagicDamageReduction(damageResult);
            LogDamageReduction("마법 지속", damageReduction);

            // 최종 마법 지속 피해 계산
            float damageAmplification = CalculateDamageAmplification(damageAsset, damageResult);

            /*
             * 최종 마법 지속 피해량 = ((마법 피해량 × 마법 피해 배율) + 추가 마법 피해량) × 피해 감소 × 피해 증폭
             * 감소된 마법 지속 피해량 = 총 피해량 × (1 − 피해 감소 배율)
             */

            float totalMagicalDamage = (magicalDamage * magicalDamageMultiplier);
            float finalDamageValue = totalMagicalDamage * damageReduction * damageAmplification;
            float reducedDamageValue = totalMagicalDamage * (1f - damageReduction);

            LogDamageCalculationStart(
                Attacker,
                TargetVital,
                magicalDamage,
                magicalDamageMultiplier,
                damageReduction,
                damageAmplification,
                finalDamageValue);

            LogInfo(GetLogString());

            finalDamageValue = Mathf.Max(damageAsset.MinDamageValue, finalDamageValue);

            damageResult.DamageValue = finalDamageValue;
            damageResult.ReducedDamageValue = reducedDamageValue;
        }

        private void ComputeThornsDamage(DamageAssetData damageAsset, int hitmarkLevel, ref DamageResult damageResult)
        {
            ClearLogBuilder();
            AppendLineToLog();

            // 피해량 계산
            float totalAttackPower = FindAttackerStatValue(StatNames.Damage);
            float damageRatio = CalculatePhysicalDamageRatio(damageAsset, hitmarkLevel);
            float thornsDamage = totalAttackPower * damageRatio;

            LogDamageCalculation("가시 피해량", thornsDamage, totalAttackPower, damageRatio);

            // 피해량 배율 계산
            float thornsDamageMultiplier = CalculateThornsDamageMultiplier(damageResult, hitmarkLevel, 0);

            // 추가 피해량 계산
            float fixedDamage = StatEx.GetValueByLevel(damageAsset.FixedDamage, damageAsset.FixedDamageByLevel, hitmarkLevel);

            // 피해 감소 계산
            float damageReduction = CalculatePhysicalDamageReduction(damageResult);
            LogDamageReduction("가시", damageReduction);

            // 피해 증폭 계산
            float damageAmplification = CalculateDamageAmplification(damageAsset, damageResult);

            /*
             * 최종 가시 피해량 = ((기본 피해 × 배율) + 추가 피해) × 피해 감소 × 증폭
             * 감소된 가시 피해량 = 총 피해량 × (1 - 피해 감소 배율)
             */

            float totalThornsDamage = (thornsDamage * thornsDamageMultiplier);
            float finalDamageValue = totalThornsDamage * damageReduction * damageAmplification;
            float reducedDamageValue = totalThornsDamage * (1f - damageReduction);

            LogDamageCalculationStart(
                Attacker,
                TargetVital,
                thornsDamage,
                thornsDamageMultiplier,
                damageReduction,
                damageAmplification,
                finalDamageValue);

            LogInfo(GetLogString());

            finalDamageValue = Mathf.Max(damageAsset.MinDamageValue, finalDamageValue);

            damageResult.DamageValue = finalDamageValue;
            damageResult.ReducedDamageValue = reducedDamageValue;
        }

        private void ComputeHealValue(DamageAssetData damageAsset, float referenceValue, ref DamageResult damageResult)
        {
            float fixedValue;

            fixedValue = StatEx.GetValueByLevel(damageAsset.FixedDamage, damageAsset.FixedDamageByLevel, Level);

            float magnification;

            magnification = damageAsset.LinkedHitmarkMagnification;

            if (Level > 1)
            {
                magnification += (Level - 1) * damageAsset.LinkedValueMagnificationByLevel;
            }

            if (Stack > 1)
            {
                magnification += (Stack - 1) * damageAsset.LinkedValueMagnificationByStack;
            }

            float result = fixedValue + (referenceValue * magnification);
            result = Mathf.Max(result, damageAsset.MinDamageValue);

            damageResult.DamageValue = result;
            damageResult.ReducedDamageValue = 0;

            if (result.IsZero())
            {
                return;
            }

            LogHealingOrResourceRestoration(damageAsset, fixedValue, referenceValue, magnification, result);
        }

        private void ComputeChargeValue(DamageAssetData damageAsset, float referenceValue, ref DamageResult damageResult)
        {
            float fixedValue = StatEx.GetValueByLevel(damageAsset.FixedDamage, damageAsset.FixedDamageByLevel, Level);
            float magnification = damageAsset.LinkedHitmarkMagnification;

            if (Level > 1)
            {
                magnification += (Level - 1) * damageAsset.LinkedValueMagnificationByLevel;
            }

            if (Stack > 1)
            {
                magnification += (Stack - 1) * damageAsset.LinkedValueMagnificationByStack;
            }

            float result = fixedValue + (referenceValue * magnification);
            if (ContainsAttackerStatValue(StatNames.ShieldMulti))
            {
                float statValue = FindAttackerCalculateStatValue(StatNames.ShieldMulti);
                result *= statValue;
            }
            result = Mathf.Max(result, damageAsset.MinDamageValue);

            damageResult.DamageValue = result;
            damageResult.ReducedDamageValue = 0;

            if (result.IsZero())
            {
                return;
            }

            LogHealingOrResourceRestoration(damageAsset, fixedValue, referenceValue, magnification, result);
        }

        #region Log

        private void LogDamageCalculation(string damageType, float damageValue, float totalAttackPower, float damageRatio)
        {
            if (Log.LevelInfo)
            {
                string format = GetDamageCalculationFormat(damageType);
                LogProgress(format, damageValue.ToColorString(GameColors.Magical), totalAttackPower, ValueStringEx.GetPercentString(damageRatio, 1));
            }
        }

        private void LogAdditionalPhysicalDamage(float additionalDamage, float fixedDamage, float calculatedAdditionalDamage)
        {
            if (Log.LevelInfo)
            {
                string format = "추가 {0} 피해량을 계산합니다. {1} 계산식: {2}(고정 피해량) + {3}(추가 피해량) + {4}(능력치 비례 추가 피해량)";
                LogProgress(format,
                    "물리".ToColorString(GameColors.Physical),
                    ValueStringEx.GetValueString(additionalDamage, 0),
                    ValueStringEx.GetValueString(fixedDamage, 0),
                    ValueStringEx.GetValueString(calculatedAdditionalDamage, 0));
            }
        }

        private void LogAdditionalMagicalDamage(float additionalDamage, float fixedDamage, float calculatedAdditionalDamage)
        {
            if (Log.LevelInfo)
            {
                string format = "추가 {0} 피해량을 계산합니다. {1} 계산식: {2}(고정 피해량) + {3}(추가 피해량)";
                LogProgress(format,
                   "마법".ToColorString(GameColors.Magical),
                   ValueStringEx.GetValueString(additionalDamage, 0),
                   ValueStringEx.GetValueString(fixedDamage, 0),
                   ValueStringEx.GetValueString(calculatedAdditionalDamage, 0));
            }
        }

        private void LogDamageReduction(string damageType, float damageReduction)
        {
            if (Log.LevelProgress)
            {
                string format = GetDamageReductionFormat(damageType);
                LogProgress(format, ValueStringEx.GetPercentString(-1 + damageReduction, 0));
            }
        }

        private void LogDamageCalculationStart(Character attacker, Vital targetVital, float damageValue, float damageMultiplier,
            float damageReduction, float damageAmplification, float finalDamageValue)
        {
            if (Log.LevelInfo)
            {
                string format = "공격자({0}) ▶ 피격자({1}), 최종 피해량:{6}.\n계산식: <b>[</b>공격력({2}) * 공격력 배율({3})<b>]</b> * 피해 감소 적용 배율({4}) * 피해 증폭({5})\n";
                string targetString = string.Empty;
                string attackerName = attacker != null ? attacker.Name.ToLogString() : "None";

                if (targetVital != null)
                {
                    targetString = targetVital.Owner != null ? targetVital.Owner.GetHierarchyName() : targetVital.GetHierarchyName();
                }

                InsertToLog(0, string.Format(format,
                    attackerName,
                    targetString,
                    damageValue.ToColorString(GameColors.Physical),
                    ValueStringEx.GetPercentString(damageMultiplier, 1),
                    ValueStringEx.GetPercentString(damageReduction, 0),
                    ValueStringEx.GetPercentString(damageAmplification, 1),
                    finalDamageValue.ToColorString(GameColors.Physical)));
            }
        }

        private string GetDamageCalculationFormat(string damageType)
        {
            return $"{damageType} 피해량을 계산합니다. {{0}} 계산식: [{{1}}(총 공격력) * {{2}}(피해량 계수)]";
        }

        private string GetDamageReductionFormat(string damageType)
        {
            return $"{damageType} 피해 감소량을 계산합니다. {0}.";
        }

        #endregion Log
    }
}