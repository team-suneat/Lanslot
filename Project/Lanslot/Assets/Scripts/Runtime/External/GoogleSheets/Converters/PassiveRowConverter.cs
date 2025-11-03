using System.Collections.Generic;
using TeamSuneat.Data;

namespace TeamSuneat
{
    public sealed class PassiveRowConverter : IGoogleSheetRowConverter<PassiveData>
    {
        public bool TryConvert(Dictionary<string, string> row, out PassiveData model)
        {
            model = null;

            if (!row.TryGetValue("Name", out string nameStr) || !GoogleSheetValueParsers.TryParseEnum(nameStr, out PassiveNames name))
            {
                Log.Warning($"필수 컬럼 Name 누락 또는 enum 파싱 실패: {nameStr}");
                return false;
            }

            _ = row.TryGetValue("DisplayName", out string displayName);
            _ = row.TryGetValue("DisplayDesc", out string displayDesc);

            // 모델 생성 및 기본 필드 설정
            PassiveData m = new()
            {
                Name = (ItemNames)name,
                DisplayName = displayName,
                DisplayDesc = displayDesc,
            };

            model = m;
            return true;
        }
    }
}