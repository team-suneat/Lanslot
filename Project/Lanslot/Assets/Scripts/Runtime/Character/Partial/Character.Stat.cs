using System.Collections.Generic;
using System.Linq;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// Character 클래스의 스탯 관련 기능
    /// </summary>
    public partial class Character
    {
        /// <summary>
        /// 캐릭터 스탯을 적용합니다.
        /// </summary>

        public virtual void AddCharacterStats()
        {
            // 스크립터블 데이터에서 캐릭터 스탯 에셋 가져오기
            CharacterStatAsset statAsset = ScriptableDataManager.Instance.GetCharacterStatAsset(Name);
            if (statAsset != null)
            {
                // 기본 스탯 적용
                ApplyBaseStats(statAsset);

                // 레벨별 성장 스탯 적용
                ApplyGrowthStats(statAsset);

                LogInfo("캐릭터 스탯이 스크립터블 데이터에서 적용되었습니다. 캐릭터: {0}, 레벨: {1}", Name, Level);
            }
        }

        private void ApplyBaseStats(CharacterStatAsset statAsset)
        {
            for (int i = 0; i < statAsset.BaseStats.Count; i++)
            {
                CharacterBaseStat baseStat = statAsset.BaseStats[i];
                if (!baseStat.IsValid()) continue;

                float baseValue = baseStat.BaseValue;
                if (baseValue.IsZero()) continue;

                Stat.AddWithSourceInfo(baseStat.StatName, baseValue, this, NameString, "CharacterBase");
            }
        }

        protected void ApplyGrowthStats(CharacterStatAsset statAsset)
        {
            for (int i = 0; i < statAsset.BaseStats.Count; i++)
            {
                CharacterBaseStat baseStat = statAsset.BaseStats[i];
                if (!baseStat.IsValid()) continue;
                if (baseStat.GrowthValue.IsZero()) continue;

                float growthValue = baseStat.GetStatValueAtLevel(Level) - baseStat.BaseValue;
                if (growthValue.IsZero()) continue;

                Stat.AddWithSourceInfo(baseStat.StatName, growthValue, this, NameString, "CharacterGrowth");
            }
        }
    }
}