using System;
using System.Collections.Generic;
using System.Text;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat
{
    [System.Serializable]
    public partial class DamageCalculator
    {
#if UNITY_EDITOR
        private readonly StringBuilder _stringBuilder = new();
#endif
        private Vital _targetVital;
        private Collider2D _targetVitalCollider;

        // StringBuilder 헬퍼 메서드들

        private void AppendToLog(string value)
        {
#if UNITY_EDITOR
            _ = _stringBuilder.Append(value);
#endif
        }

        private void AppendLineToLog(string value = "")
        {
#if UNITY_EDITOR
            _ = _stringBuilder.AppendLine(value);
#endif
        }

        private void ClearLogBuilder()
        {
#if UNITY_EDITOR
            _ = _stringBuilder.Clear();
#endif
        }

        private void InsertToLog(int index, string value)
        {
#if UNITY_EDITOR
            _ = _stringBuilder.Insert(index, value);
#endif
        }

        private string GetLogString()
        {
#if UNITY_EDITOR
            return _stringBuilder.ToString();
#endif
            return string.Empty;
        }

        private VProfile ProfileInfo => GameApp.GetSelectedProfile();
        public Vital TargetVital => _targetVital;
        public Collider2D TargetVitalCollider => _targetVitalCollider;
        public Character TargetCharacter => _targetVital?.Owner;

        public int TargetVitalColliderIndex
        {
            get
            {
                if (TargetVital != null)
                {
                    return TargetVital.GetColliderIndex(TargetVitalCollider);
                }

                return -1;
            }
        }

        public HitmarkAssetData HitmarkAssetData { get; set; }
        public Character Attacker { get; private set; }
        public AttackEntity AttackEntity { get; set; }
        public List<DamageResult> DamageResults { get; private set; }
        public float ReferenceValue { get; set; }
        public float DamageReferenceValue { get; private set; }
        public float ManaCostReferenceValue { get; private set; }
        public float CooldownReferenceValue { get; private set; }
        public float DecrescenceRate { get; private set; }
        public int Level { get; private set; } = 1;
        public int Stack { get; set; }
        public int Tick { get; set; }

        //─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        private bool IsComputeValid()
        {
            if (!HitmarkAssetData.IsValid())
            {
                LogErrorHitmarkNotSet();
                return false;
            }
            if (!HitmarkAssetData.Damage.IsValid())
            {
                LogErrorDamageAssetDataNotSet();
                return false;
            }

            return true;
        }

        public void Execute()
        {
            if (!IsComputeValid())
            {
                return;
            }

            ResetDamageResults();
            _ = DetermineImmuneCC(HitmarkAssetData.IsCrowdControl);

            DamageAssetData damageAsset = HitmarkAssetData.Damage;
            DamageResult damageResult = CreateDamageResult(damageAsset);

            RefreshReferenceValue(damageAsset);
            ComputeByType(damageAsset, ref damageResult);

            if (damageResult.IsCritical)
            {
                _ = GlobalEvent<DamageResult>.Send(GlobalEventType.PLAYER_CHARACTER_ATTACK_MONSTER_CRITICAL, damageResult);
            }

            DamageResults.Add(damageResult);
        }

        public void ExecuteWithoutCritical()
        {
            if (!IsComputeValid())
            {
                return;
            }

            ResetDamageResults();

            DamageAssetData damageAsset = HitmarkAssetData.Damage;
            DamageResult damageResult = CreateDamageResult(damageAsset);
            RefreshReferenceValue(damageAsset);
            ComputeByType(damageAsset, ref damageResult);
            DamageResults.Add(damageResult);
        }

        private DamageResult CreateDamageResult(DamageAssetData damageAsset)
        {
            return new DamageResult
            {
                IsEvasion = false,
                Asset = damageAsset,
                HitmarkLevel = Level,
                Attacker = Attacker,
                AttackEntity = AttackEntity,
                TargetVital = TargetVital,
                TargetVitalCollider = TargetVitalCollider,
                TargetVitalColliderIndex = TargetVitalColliderIndex
            };
        }

        private bool TryReceiveDamageOnlyOne(DamageAssetData damageAsset, ref DamageResult damageResult)
        {
            return false;
        }

        private void ResetDamageResults()
        {
            if (DamageResults == null)
            {
                DamageResults = new();
            }
            else
            {
                DamageResults.Clear();
            }

            LogProgressResetDamageResults();
        }

        public void SetAttacker(Character attacker)
        {
            Attacker = attacker;
            if (attacker != null)
            {
                LogProgressAttacker(attacker.GetHierarchyName());
            }
        }

        public void SetTargetVital(Vital targetVital)
        {
            if (targetVital != null)
            {
                LogWarningBuffOnHit(targetVital);
                _targetVital = targetVital;
                LogProgressTargetVital(_targetVital.GetHierarchyPath());

                _targetVitalCollider = targetVital.GetNotGuardCollider();
                LogProgressTargetVitalCollider(_targetVitalCollider.GetHierarchyPath());
            }
            else
            {
                _targetVital = null;
                LogWarningTargetVital();
            }
        }

        private void LogWarningBuffOnHit(Vital targetVital)
        {
            if (!Log.LevelWarning)
            {
                return;
            }

            if (Attacker == null)
            {
                return;
            }

            if (targetVital.Owner != Attacker)
            {
                return;
            }

            if (!HitmarkAssetData.IsValid())
            {
                return;
            }

            if (!HitmarkAssetData.Damage.IsValid())
            {
                return;
            }

            if (HitmarkAssetData.Damage.BuffOnHit != BuffNames.None)
            {
                Log.Warning("{0} 공격의 목표와 공격자와 동일합니다. 공격 적중시 버프({1})가 잘못 적용될 수 있습니다.", HitmarkAssetData.Name, HitmarkAssetData.Damage.BuffOnHit);
            }
        }

        public void SetResourceCostRefrenceValue(int manaCost)
        {
            ManaCostReferenceValue = manaCost;
            LogProgressManaCostReferenceValue(ManaCostReferenceValue.ToSelectString(0));
        }

        public void SetDamageRefrenceValue(int damageValue)
        {
            DamageReferenceValue = damageValue;
            LogProgressDamageReferenceValue(DamageReferenceValue.ToSelectString(0));
        }

        public void SetCooldownRefrenceValue(float cooldownTime)
        {
            CooldownReferenceValue = cooldownTime;
            LogProgressCooldownReferenceValue(CooldownReferenceValue.ToSelectString(0));
        }

        #region Stack & Level

        public void SetStack(int stack)
        {
            Stack = stack;
        }

        public void SetLevel(int level)
        {
            Level = level;
        }

        #endregion Stack & Level

        // Floaty Text

        private void SpawnFloatyText(string content)
        {
            if (TargetVital != null)
            {
                ResourcesManager.SpawnFloatyText(content, true, TargetVital.transform);
            }
        }

        #region Log

        private void LogHealingPowerBySkillCategory(string skillCategory, float statValue)
        {
            if (Log.LevelInfo)
            {
#if UNITY_EDITOR
                string statValueContent = ValueStringEx.GetPercentString(statValue, true);
                string content = string.Format("기술 범주에 따른 회복량 배율을 계산합니다. {0}(기술 범주), {1}", skillCategory, statValueContent);
                AppendToLog(content);
#endif
            }
        }

        private void LogHealingOrResourceRestoration(DamageAssetData damageAsset, float fixedValue, float referenceValue, float magnification, float result)
        {
            if (Log.LevelInfo)
            {
                string format;
                if (damageAsset.DamageType.IsHeal())
                {
                    format = "생명력 회복량을 계산합니다. 고정 회복량({0}) + [참조 회복량({1}) * 참조 회복 계수({2})] = {3}";
                }
                else
                {
                    format = "{4} 회복량을 계산합니다. 고정 회복량({0}) + [참조 회복량({1}) * 참조 회복 계수({2})] = {3}";
                }

                LogInfo(format,
                    ValueStringEx.GetValueString(fixedValue),
                    ValueStringEx.GetValueString(referenceValue),
                    ValueStringEx.GetPercentString(magnification, 0f),
                    ValueStringEx.GetValueString(result), damageAsset.DamageType);
            }
        }

        private void LogCooldownReduction(DamageTypes damageType, float fixedValue, float referenceValue, float valueRatio, float magnification, float result)
        {
            if (Log.LevelInfo)
            {
                string format = "{0} 재사용 대기 시간 감소량을 계산합니다. {5}. 계산식: 고정 감소량({1}) + [{2}(참조 재사용 대기시간) * {3}(참조 재사용 대기시간 계수)* {4}(참조 재사용 대기시간 배율)]";
                LogInfo(format,
                    damageType, ValueStringEx.GetValueString(fixedValue),
                    ValueStringEx.GetValueString(referenceValue),
                    ValueStringEx.GetPercentString(valueRatio),
                    ValueStringEx.GetPercentString(magnification),
                    ValueStringEx.GetValueString(result));
            }
        }

        private void LogErrorHitmarkNotSet()
        {
            if (Log.LevelError)
            {
                LogError("설정된 히트마크가 없습니다.");
            }
        }

        private void LogErrorDamageAssetDataNotSet()
        {
            if (Log.LevelError)
            {
                LogError("설정된 히트마크의 DamageAssetData가 없습니다.");
            }
        }

        private void LogProgressResetDamageResults()
        {
            if (Log.LevelProgress)
            {
                LogProgress("이전 계산된 피해값을 초기화합니다.");
            }
        }

        private void LogProgressSkillLevel(string skillName, string level)
        {
            if (Log.LevelProgress)
            {
                LogProgress("기술 레벨을 설정합니다. {0}, 레벨: {1}", skillName, level);
            }
        }

        private void LogProgressAttacker(string path)
        {
            if (Log.LevelProgress)
            {
                LogProgress("공격자를 설정합니다. {0}", path);
            }
        }

        private void LogProgressTargetVital(string path)
        {
            if (Log.LevelProgress)
            {
                LogProgress("목표 바이탈을 설정합니다. {0}", path);
            }
        }

        private void LogWarningTargetVital()
        {
            if (Log.LevelWarning)
            {
                LogWarning("목표 바이탈을 설정할 수 없습니다.");
            }
        }

        private void LogProgressTargetVitalCollider(string path)
        {
            if (Log.LevelProgress)
            {
                LogProgress("목표 바이탈 충돌체를 설정합니다. {0}", path);
            }
        }

        private void LogProgressManaCostReferenceValue(string value)
        {
            if (Log.LevelProgress)
            {
                LogProgress("마나 소모 참조값을 설정합니다. Value: {0}", value);
            }
        }

        private void LogProgressDamageReferenceValue(string value)
        {
            if (Log.LevelProgress)
            {
                LogProgress("피해 참조값을 설정합니다. Value: {0}", value);
            }
        }

        private void LogProgressCooldownReferenceValue(string value)
        {
            if (Log.LevelProgress)
            {
                LogProgress("재사용 대기시간 참조값을 설정합니다. Value: {0}", value);
            }
        }

        private void LogProgressDecrescenceRate(float decrescenceRate, int hitCountAtTime)
        {
            if (Log.LevelProgress)
            {
                LogProgress("피해 점감 배율을 설정합니다. {0}, 횟수: {1}",
                    ValueStringEx.GetPercentString(decrescenceRate, true),
                    hitCountAtTime.ToSelectString(hitCountAtTime > 1));
            }
        }

        private void LogProgressResourceRecoveryOnHitRestricted()
        {
            if (Log.LevelProgress)
            {
                LogProgress("적중 시 자원 회복이 제한되어 있습니다.");
            }
        }

        private void LogInfoCriticalAppliedByCheat()
        {
            if (Log.LevelInfo)
            {
                LogInfo("치트를 통해 치명타를 적용합니다.");
            }
        }

        private void LogInfoNoCriticalAppliedByCheat()
        {
            if (Log.LevelInfo)
            {
                LogInfo("치트를 통해 치명타를 적용하지 않습니다.");
            }
        }

        private void LogMaxCriticalChanceExceeded(float resultCriticalChance, float maxCriticalChance)
        {
            if (Log.LevelInfo)
            {
                LogInfo("최대 치명타 확률을 초과했습니다. 결과: {0}, 최대: {1}",
                    ValueStringEx.GetPercentString(resultCriticalChance, 0),
                    ValueStringEx.GetPercentString(maxCriticalChance, GameColors.Value));
            }
        }

        private void LogCriticalHitApplied(float resultCriticalChance, float criticalChance, float targetCriticalChance)
        {
            if (Log.LevelInfo)
            {
                LogInfo("치명타가 적용되었습니다. 결과: {0}, 시전자: {1}, 피격자: {2}",
                    ValueStringEx.GetPercentString(resultCriticalChance, 0),
                    ValueStringEx.GetPercentString(criticalChance, 0),
                    ValueStringEx.GetPercentString(targetCriticalChance, 0));
            }
        }

        private void LogCriticalHitNotApplied(float resultCriticalChance, float criticalChance, float targetCriticalChance)
        {
            if (Log.LevelInfo)
            {
                LogInfo("치명타가 적용되지 않았습니다. 결과: {0}, 시전자: {1}, 피격자: {2}",
                    ValueStringEx.GetPercentString(resultCriticalChance, 0),
                    ValueStringEx.GetPercentString(criticalChance, 0),
                    ValueStringEx.GetPercentString(targetCriticalChance, 0));
            }
        }

        private void LogExecutionApplied(float targetLifeRate, float executionConditionalTargetLifeRate)
        {
            if (Log.LevelInfo)
            {
                LogInfo("처형이 적용되었습니다. 목표 생명력 비율: {0}, 처형 조건: {1}",
                    ValueStringEx.GetPercentString(targetLifeRate, 0),
                    ValueStringEx.GetPercentString(executionConditionalTargetLifeRate, 0));
            }
        }

        private void LogProgressFrostSlash(float additionalChance)
        {
            if (Log.LevelProgress)
            {
                LogProgress("서리 베기 추가 확률을 적용합니다. {0}", ValueStringEx.GetPercentString(additionalChance, 0));
            }
        }

        private void LogProgressFranticWirlwind(float additionalChance)
        {
            if (Log.LevelProgress)
            {
                LogProgress("광분 회오리 추가 확률을 적용합니다. {0}", ValueStringEx.GetPercentString(additionalChance, 0));
            }
        }

        private void LogProgressCruelStrike(float chanceOnCurrentResource)
        {
            if (Log.LevelProgress)
            {
                LogProgress("잔인한 일격 추가 확률을 적용합니다. {0}", ValueStringEx.GetPercentString(chanceOnCurrentResource, 0));
            }
        }

        private void LogProgressSpinRend(float statValue)
        {
            if (Log.LevelProgress)
            {
                LogProgress("회전 베기 추가 확률을 적용합니다. {0}", ValueStringEx.GetPercentString(statValue, 0));
            }
        }

        private void LogProgressCoreSkill(float additionalChance)
        {
            if (Log.LevelProgress)
            {
                LogProgress("핵심 기술 추가 확률을 적용합니다. {0}", ValueStringEx.GetPercentString(additionalChance, 0));
            }
        }

        private void LogProgressCoreSkillToBleedingEnemies(float additionalChance)
        {
            if (Log.LevelProgress)
            {
                LogProgress("핵심 기술(출혈 적) 추가 확률을 적용합니다. {0}", ValueStringEx.GetPercentString(additionalChance, 0));
            }
        }

        private void LogProgressPowerSkill(float additionalChance)
        {
            if (Log.LevelProgress)
            {
                LogProgress("파워 기술 추가 확률을 적용합니다. {0}", ValueStringEx.GetPercentString(additionalChance, 0));
            }
        }

        private void LogProgressMissingLife(float chanceOnMissingLife)
        {
            if (Log.LevelProgress)
            {
                LogProgress("생명력 부족 시 추가 확률을 적용합니다. {0}", ValueStringEx.GetPercentString(chanceOnMissingLife, 0));
            }
        }

        private void LogProgressMissingResource(float chanceOnMissingResource)
        {
            if (Log.LevelProgress)
            {
                LogProgress("자원 부족 시 추가 확률을 적용합니다. {0}", ValueStringEx.GetPercentString(chanceOnMissingResource, 0));
            }
        }

        #endregion Log
    }
}