using System.Collections.Generic;
using TeamSuneat.Data;

namespace TeamSuneat
{
    public sealed class StageRowConverter : IGoogleSheetRowConverter<StageData>
    {
        public bool TryConvert(Dictionary<string, string> row, out StageData model)
        {
            model = null;

            if (!row.TryGetValue("Name", out string nameStr) || !GoogleSheetValueParsers.TryParseEnum<StageNames>(nameStr, out StageNames name))
            {
                Log.Warning($"필수 컬럼 Name 누락 또는 enum 파싱 실패: {nameStr}");
                return false;
            }

            row.TryGetValue("DisplayName", out string displayName);

            if (!row.TryGetValue("Width", out string widthStr) || !GoogleSheetValueParsers.TryParseInt(widthStr, out int width))
            {
                Log.Warning($"Name {name}: Width 파싱 실패: {widthStr}");
                return false;
            }

            BuildTypes[] builds;
            if (row.TryGetValue("SupportedBuildTypes", out string buildsStr))
            {
                if (!GoogleSheetValueParsers.TryParseEnumArray<BuildTypes>(buildsStr, out builds))
                {
                    builds = System.Array.Empty<BuildTypes>();
                    Log.Warning($"Name {name}: SupportedBuildTypes 파싱 실패 → 빈 배열 사용");
                }
            }
            else
            {
                builds = System.Array.Empty<BuildTypes>();
            }

            StageData m = new()
            {
                Name = name,
                DisplayName = displayName,
                Width = width,
                SupportedBuildTypes = builds,
            };

            model = m;
            return true;
        }
    }
}

