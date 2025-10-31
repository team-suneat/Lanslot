using Sirenix.OdinInspector;
using System;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    [Serializable]
    public class LevelExpAssetData : ScriptableData<int>
    {
        [SuffixLabel("경험치 증가 패턴")]
        [SerializeField]
        private ExpGrowthPattern _growthPattern = ExpGrowthPattern.Linear;

        [SuffixLabel("증가 배율 (지수적 증가시 사용)")]
        [SerializeField, ShowIf("_growthPattern", ExpGrowthPattern.Exponential)]
        private float _growthMultiplier = 1.5f;

        [SuffixLabel("증가량 (선형 증가시 사용)")]
        [SerializeField, ShowIf("_growthPattern", ExpGrowthPattern.Linear)]
        private int _growthAmount = 8;

        [SuffixLabel("기본 경험치 (2레벨 기준)")]
        [SerializeField]
        private int _baseExp = 7;

        [SuffixLabel("최대 레벨")]
        [SerializeField]
        private int _maxLevel = 100;

        public ExpGrowthPattern GrowthPattern => _growthPattern;
        public float GrowthMultiplier => _growthMultiplier;
        public int GrowthAmount => _growthAmount;
        public int BaseExp => _baseExp;
        public int MaxLevel => _maxLevel;

        public void Validate()
        {
            if (_baseExp < 0)
            {
                _baseExp = 7;
                Log.Warning("기본 경험치는 0 이상이어야 합니다. 7로 설정합니다.");
            }

            if (_maxLevel < 1)
            {
                _maxLevel = 100;
                Log.Warning("최대 레벨은 1 이상이어야 합니다. 100으로 설정합니다.");
            }

            if (_growthMultiplier <= 1.0f && _growthPattern == ExpGrowthPattern.Exponential)
            {
                _growthMultiplier = 1.5f;
                Log.Warning("지수적 증가시 배율은 1.0보다 커야 합니다. 1.5로 설정합니다.");
            }

            if (_growthAmount <= 0 && _growthPattern == ExpGrowthPattern.Linear)
            {
                _growthAmount = 8;
                Log.Warning("선형 증가시 증가량은 0보다 커야 합니다. 8로 설정합니다.");
            }
        }

        public override void Refresh()
        {
            base.Refresh();
        }

        /// <summary>
        /// 특정 레벨까지의 총 경험치를 계산합니다.
        /// </summary>
        public int CalculateTotalExpToLevel(int targetLevel)
        {
            int totalExp = 0;

            for (int level = 2; level <= targetLevel; level++)
            {
                int expForLevel = GetRequiredExpForLevel(level);
                totalExp += expForLevel;
            }

            return totalExp;
        }

        /// <summary>
        /// 특정 레벨에 도달하기 위해 필요한 경험치를 계산합니다.
        /// </summary>
        public int GetRequiredExpForLevel(int level)
        {
            if (level <= 1) return 0;

            switch (_growthPattern)
            {
                case ExpGrowthPattern.Linear:
                    return _baseExp + (level - 2) * _growthAmount;

                case ExpGrowthPattern.Exponential:
                    return Mathf.RoundToInt(_baseExp * Mathf.Pow(_growthMultiplier, level - 2));

                case ExpGrowthPattern.Quadratic:
                    return Mathf.RoundToInt(_baseExp + (level - 1) * (level - 1) * _growthAmount);

                default:
                    return _baseExp;
            }
        }

        /// <summary>
        /// 특정 레벨에서 다음 레벨까지 필요한 경험치를 가져옵니다.
        /// </summary>
        public int GetRequiredExpForNextLevel(int currentLevel)
        {
            if (currentLevel < 1) return 0;
            return GetRequiredExpForLevel(currentLevel + 1);
        }

        /// <summary>
        /// 특정 레벨까지 도달하는데 필요한 총 경험치를 가져옵니다.
        /// </summary>
        public int GetTotalExpToReachLevel(int targetLevel)
        {
            return CalculateTotalExpToLevel(targetLevel);
        }

        /// <summary>
        /// 현재 경험치로 도달 가능한 최대 레벨을 계산합니다.
        /// </summary>
        public int GetMaxReachableLevel(int currentExp)
        {
            int level = 1;
            int totalExp = 0;

            while (totalExp <= currentExp && level < _maxLevel)
            {
                level++;
                int expForNextLevel = GetRequiredExpForLevel(level);
                if (totalExp + expForNextLevel > currentExp)
                    break;

                totalExp += expForNextLevel;
            }

            return level - 1;
        }

        /// <summary>
        /// 특정 레벨의 경험치 정보가 유효한지 확인합니다.
        /// </summary>
        public bool IsValidLevel(int level)
        {
            return level >= 1 && level <= _maxLevel;
        }
    }

    /// <summary>
    /// 경험치 증가 패턴 열거형
    /// </summary>
    public enum ExpGrowthPattern
    {
        [LabelText("선형 증가")]
        Linear,

        [LabelText("지수적 증가")]
        Exponential,

        [LabelText("제곱 증가")]
        Quadratic
    }
}