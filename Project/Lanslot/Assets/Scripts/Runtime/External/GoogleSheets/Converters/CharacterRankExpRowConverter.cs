using System.Collections.Generic;
using TeamSuneat.Data;

namespace TeamSuneat
{
    public sealed class CharacterRankExpRowConverter : IGoogleSheetRowConverter<CharacterRankExpData>
    {
        public bool TryConvert(Dictionary<string, string> row, out CharacterRankExpData model)
        {
            model = null;

            if (!row.TryGetValue("Rank", out string rankStr) || !GoogleSheetValueParsers.TryParseInt(rankStr, out int rank))
            {
                Log.Warning($"필수 컬럼 Rank 누락 또는 int 파싱 실패: {rankStr}");
                return false;
            }

            if (!row.TryGetValue("RequiredExperience", out string requiredExpStr) || !GoogleSheetValueParsers.TryParseInt(requiredExpStr, out int requiredExperience))
            {
                Log.Warning($"Rank {rank}: RequiredExperience 파싱 실패: {requiredExpStr}");
                return false;
            }

            CharacterRankExpData m = new()
            {
                Rank = rank,
                RequiredExperience = requiredExperience,
            };

            model = m;
            return true;
        }
    }
}

