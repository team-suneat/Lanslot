using System;
using UnityEngine;

namespace TeamSuneat.Data
{
    /// <summary>
    /// 캐릭터의 기본 스탯 정보를 저장하는 데이터 클래스
    /// </summary>
    [Serializable]
    public class CharacterBaseStat
    {
        [Header("스탯 정보")]
        [SerializeField] private StatNames _statName;
        [SerializeField] private string _statNameString;
        [SerializeField] private float _baseValue;
        [SerializeField] private float _growthValue;
        [SerializeField] private bool _isPercentage;

        public StatNames StatName => _statName;
        public float BaseValue => _baseValue;
        public float GrowthValue => _growthValue;
        public bool IsPercentage => _isPercentage;

        public CharacterBaseStat()
        {
            _statName = StatNames.None;
            _baseValue = 0f;
            _growthValue = 0f;
            _isPercentage = false;
        }

        public CharacterBaseStat(StatNames statName, float baseValue, float growthValue = 0f, bool isPercentage = false)
        {
            _statName = statName;
            _baseValue = baseValue;
            _growthValue = growthValue;
            _isPercentage = isPercentage;
        }

        /// <summary>
        /// 특정 레벨에서의 스탯 값을 계산합니다.
        /// </summary>
        /// <param name="level">캐릭터 레벨</param>
        /// <returns>계산된 스탯 값</returns>
        public float GetStatValueAtLevel(int level)
        {
            if (level <= 0) return _baseValue;
            return _baseValue + (_growthValue * (level - 1));
        }

        /// <summary>
        /// 스탯이 유효한지 확인합니다.
        /// </summary>
        public bool IsValid()
        {
            return _statName != StatNames.None && (_baseValue != 0f || _growthValue != 0f);
        }

        /// <summary>
        /// Inspector에서 표시할 스탯 표시명을 가져옵니다.
        /// </summary>
        public string GetStatDisplayName()
        {
            if (!IsValid())
            {
                return "Empty";
            }

            string valueText = string.Empty;
            if (_isPercentage)
            {
                valueText = $"{_baseValue:P1}";
            }
            else
            {
                valueText = _baseValue.ToString("F1");
            }

            string growthText = string.Empty;
            if (_growthValue != 0f)
            {
                if (_isPercentage)
                {
                    growthText = $"+{_growthValue:P1}/Lv";
                }
                else
                {
                    growthText = $"+{_growthValue:F1}/Lv";
                }
            }

            return $"{_statName}: {valueText}{growthText}";
        }

        public void Refresh()
        {
            _statNameString = _statName.ToString();
        }

        public void Validate()
        {
            EnumEx.ConvertTo(ref _statName, _statNameString);
        }
    }
}