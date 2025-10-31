using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    [Serializable]
    public class CharacterStatAssetData : ScriptableData<int>
    {
        public bool IsChangingAsset;

        [EnableIf("IsChangingAsset")]
        public CharacterNames Name;

        [LabelText("능력치(기본 + 성장/레벨)")]
        [EnableIf("IsChangingAsset")]
        [SerializeField, ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "GetStatDisplayName")]
        private List<CharacterBaseStat> _baseStats = new();

        public IReadOnlyList<CharacterBaseStat> BaseStats => _baseStats;

        public void Validate()
        {
            if (_baseStats.IsValid())
            {
                for (int i = 0; i < _baseStats.Count; i++)
                {
                    CharacterBaseStat stat = _baseStats[i];
                    stat.Validate();
                }
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            if (_baseStats.IsValid())
            {
                for (int i = 0; i < _baseStats.Count; i++)
                {
                    CharacterBaseStat stat = _baseStats[i];
                    stat.Refresh();
                }
            }

            IsChangingAsset = false;
        }
        public void CleanupEmptyStats()
        {
            int removedCount = _baseStats.RemoveAll(s => !s.IsValid());
            if (removedCount > 0)
            {
                Log.Info("빈 스탯 {0}개를 제거했습니다.", removedCount);
            }
            else
            {
                Log.Info("제거할 빈 스탯이 없습니다.");
            }
        }
    }
}