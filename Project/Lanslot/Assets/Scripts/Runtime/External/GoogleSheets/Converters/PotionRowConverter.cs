using System.Collections.Generic;
using TeamSuneat.Data;

namespace TeamSuneat
{
    /// <summary>
    /// 구글 시트 Row를 PotionData로 변환하는 컨버터 클래스
    /// </summary>
    public sealed class PotionRowConverter : IGoogleSheetRowConverter<PotionData>
    {
        public bool TryConvert(Dictionary<string, string> row, out PotionData model)
        {
            model = null;

            
            if (!row.TryGetValue("Name", out string nameStr) || !GoogleSheetValueParsers.TryParseEnum(nameStr, out ItemNames name))
            {
                Log.Warning($"필수 컬럼 Name 누락 또는 enum 파싱 실패: {nameStr}");
                return false;
            }

            
            row.TryGetValue("DisplayName", out string displayName);

            
            if (!row.TryGetValue("StatName", out string statNameStr) || !GoogleSheetValueParsers.TryParseEnum(statNameStr, out StatNames statName))
            {
                Log.Warning($"Name {name}: StatName 누락 또는 enum 파싱 실패: {statNameStr}");
                return false;
            }

            
            if (!row.TryGetValue("StatValue", out string statValueStr) || !GoogleSheetValueParsers.TryParseFloat(statValueStr, out float statValue))
            {
                Log.Warning($"Name {name}: StatValue 파싱 실패: {statValueStr}");
                return false;
            }

            
            BuildTypes[] builds;
            if (row.TryGetValue("SupportedBuildTypes", out string buildsStr))
            {
                if (!GoogleSheetValueParsers.TryParseEnumArray(buildsStr, out builds))
                {
                    builds = System.Array.Empty<BuildTypes>();
                    Log.Warning($"Name {name}: SupportedBuildTypes 파싱 실패 → 빈 배열 사용");
                }
            }
            else
            {
                builds = System.Array.Empty<BuildTypes>();
            }

            // 모델 생성 및 필드 할당
            PotionData m = new()
            {
                Name = name,
                DisplayName = displayName,
                StatName = statName,
                StatValue = statValue,
                SupportedBuildTypes = builds,
            };

            model = m;
            return true;
        }
    }
}
