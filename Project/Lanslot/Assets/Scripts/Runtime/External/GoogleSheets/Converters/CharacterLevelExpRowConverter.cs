using System.Collections.Generic;
using TeamSuneat.Data;

namespace TeamSuneat
{
    public sealed class CharacterLevelExpRowConverter : IGoogleSheetRowConverter<CharacterLevelExpData>
    {
        public bool TryConvert(Dictionary<string, string> row, out CharacterLevelExpData model)
        {
            model = null;

            if (!row.TryGetValue("Level", out string levelStr) || !GoogleSheetValueParsers.TryParseInt(levelStr, out int level))
            {
                Log.Warning($"필수 컬럼 Level 누락 또는 int 파싱 실패: {levelStr}");
                return false;
            }

            if (!row.TryGetValue("RequiredExperience", out string requiredExpStr) || !GoogleSheetValueParsers.TryParseInt(requiredExpStr, out int requiredExperience))
            {
                Log.Warning($"Level {level}: RequiredExperience 파싱 실패: {requiredExpStr}");
                return false;
            }

            CharacterLevelExpData m = new()
            {
                Level = level,
                RequiredExperience = requiredExperience,
            };

            model = m;
            return true;
        }
    }
}

