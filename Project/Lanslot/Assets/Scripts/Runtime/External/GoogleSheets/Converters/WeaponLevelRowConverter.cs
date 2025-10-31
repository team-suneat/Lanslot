using System.Collections.Generic;
using TeamSuneat.Data;

namespace TeamSuneat
{
    public sealed class WeaponLevelRowConverter : IGoogleSheetRowConverter<WeaponLevelData>
    {
        public bool TryConvert(Dictionary<string, string> row, out WeaponLevelData model, IList<string> warnings)
        {
            model = null;

            if (!row.TryGetValue("Name", out string nameStr) || !GoogleSheetValueParsers.TryParseEnum(nameStr, out WeaponNames name))
            {
                warnings?.Add($"필수 컬럼 Name 누락 또는 enum 파싱 실패: {nameStr}");
                return false;
            }

            // 선택: 표시 이름
            row.TryGetValue("DisplayName", out string displayName);

            if (!row.TryGetValue("StatName", out string statStr) || !GoogleSheetValueParsers.TryParseEnum<StatNames>(statStr, out StatNames stat))
            {
                warnings?.Add($"Name {name}: StatName 누락 또는 enum 파싱 실패: {statStr}");
                return false;
            }

            // 값 파싱
            TryParseFloat(row, "BaseStatValue", out float baseVal, warnings, name, stat);
            TryParseFloat(row, "CommonStatValue", out float commonVal, warnings, name, stat);
            TryParseFloat(row, "UncommonStatValue", out float uncommonVal, warnings, name, stat);
            TryParseFloat(row, "RareStatValue", out float rareVal, warnings, name, stat);
            TryParseFloat(row, "EpicStatValue", out float epicVal, warnings, name, stat);
            TryParseFloat(row, "LegendaryStatValue", out float legendaryVal, warnings, name, stat);

            // 단일 행 모델 생성
            WeaponLevelData m = new()
            {
                Name = name,
                DisplayName = displayName,
                StatName = stat,
                BaseStatValue = baseVal,
                CommonStatValue = commonVal,
                UncommonStatValue = uncommonVal,
                RareStatValue = rareVal,
                EpicStatValue = epicVal,
                LegendaryStatValue = legendaryVal,
            };

            model = m;
            return true;
        }

        private static bool TryParseFloat(Dictionary<string, string> row, string key, out float value, IList<string> warnings, WeaponNames name, StatNames stat)
        {
            value = 0f;
            if (!row.TryGetValue(key, out string s) || string.IsNullOrEmpty(s))
            {
                return false;
            }
            if (!GoogleSheetValueParsers.TryParseFloat(s, out value))
            {
                warnings?.Add($"Name {name}: {key} 실수 파싱 실패({s})");
                value = 0f;
                return false;
            }
            return true;
        }
    }
}