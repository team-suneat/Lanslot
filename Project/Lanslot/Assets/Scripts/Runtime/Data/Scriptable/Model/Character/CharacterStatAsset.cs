using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TeamSuneat.Data
{
    /// <summary>
    /// 캐릭터별 기본 능력치를 저장하는 스크립터블 오브젝트
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterStat", menuName = "TeamSuneat/Scriptable/CharacterStat")]
    public class CharacterStatAsset : XScriptableObject
    {
        [SerializeField] private CharacterStatAssetData _data;

        [Title("설명")]
        [SerializeField, Multiline(3)]
        private string _description;

        #region Properties

        public int TID => BitConvert.Enum32ToInt(_data.Name);
        public CharacterNames Name => _data.Name;
        public IReadOnlyList<CharacterBaseStat> BaseStats => _data.BaseStats;

        #endregion Properties

        public override void Validate()
        {
            base.Validate();

            if (!_data.IsChangingAsset)
            {
                _ = EnumEx.ConvertTo(ref _data.Name, NameString);
            }
            _data.Validate();
        }

        public override void Refresh()
        {
            NameString = Name.ToString();
            _data.Refresh();
            base.Refresh();
        }

        public override void Rename()
        {
            Rename("CharacterStat");
        }

        #region Public Methods

        /// <summary>
        /// 특정 스탯의 기본값을 가져옵니다.
        /// </summary>
        public float GetBaseStatValue(StatNames statName)
        {
            CharacterBaseStat stat = BaseStats.FirstOrDefault(s => s.StatName == statName);
            return stat?.BaseValue ?? 0f;
        }

        /// <summary>
        /// 특정 스탯의 성장값을 가져옵니다.
        /// </summary>
        public float GetGrowthStatValue(StatNames statName)
        {
            CharacterBaseStat stat = BaseStats.FirstOrDefault(s => s.StatName == statName);
            return stat?.GrowthValue ?? 0f;
        }

        /// <summary>
        /// 특정 레벨에서의 스탯 값을 계산합니다.
        /// </summary>
        public float GetStatValueAtLevel(StatNames statName, int level)
        {
            CharacterBaseStat stat = BaseStats.FirstOrDefault(s => s.StatName == statName);
            return stat?.GetStatValueAtLevel(level) ?? 0f;
        }

        /// <summary>
        /// 모든 스탯의 레벨별 값을 가져옵니다.
        /// </summary>
        public Dictionary<StatNames, float> GetAllStatsAtLevel(int level)
        {
            Dictionary<StatNames, float> result = new();
            foreach (CharacterBaseStat stat in BaseStats.Where(s => s.IsValid()))
            {
                result[stat.StatName] = stat.GetStatValueAtLevel(level);
            }
            return result;
        }

        /// <summary>
        /// 스탯이 존재하는지 확인합니다.
        /// </summary>
        public bool HasStat(StatNames statName)
        {
            return BaseStats.Any(s => s.StatName == statName && s.IsValid());
        }

        #endregion Public Methods

        #region Editor Methods

#if UNITY_EDITOR

        /// <summary>
        /// 스탯 미리보기를 생성합니다.
        /// </summary>
        [FoldoutGroup("#Custom Button", 6)]
        [Button("레벨별 스탯 미리보기", ButtonSizes.Large)]
        private void ShowStatPreview()
        {
            Log.Info("─── {0} 스탯 미리보기 ───", Name);

            for (int level = 1; level <= 10; level++)
            {
                Log.Info("레벨 {0}:", level);
                foreach (CharacterBaseStat stat in BaseStats.Where(s => s.IsValid()))
                {
                    float value = stat.GetStatValueAtLevel(level);
                    string valueText = stat.IsPercentage ? $"{value:P1}" : value.ToString("F1");
                    Log.Info("  {0}: {1}", stat.StatName, valueText);
                }
            }
        }

        /// <summary>
        /// 빈 스탯을 정리합니다.
        /// </summary>
        [FoldoutGroup("#Custom Button", 6)]
        [Button("빈 스탯 정리", ButtonSizes.Large)]
        private void CleanupEmptyStats()
        {
            _data.CleanupEmptyStats();
        }

#endif

        #endregion Editor Methods
    }
}