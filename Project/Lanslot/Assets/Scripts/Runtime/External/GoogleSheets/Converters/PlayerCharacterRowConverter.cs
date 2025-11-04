using System.Collections.Generic;
using TeamSuneat.Data;

namespace TeamSuneat
{
    public sealed class PlayerCharacterRowConverter : IGoogleSheetRowConverter<PlayerCharacterData>
    {
        public bool TryConvert(Dictionary<string, string> row, out PlayerCharacterData model)
        {
            model = null;

            if (!row.TryGetValue("Name", out string nameStr) || !GoogleSheetValueParsers.TryParseEnum(nameStr, out CharacterNames name))
            {
                Log.Warning($"필수 컬럼 Name 누락 또는 enum 파싱 실패: {nameStr}");
                return false;
            }

            row.TryGetValue("DisplayName", out string displayName);

            if (!row.TryGetValue("Weapon", out string weaponStr) || !GoogleSheetValueParsers.TryParseEnum(weaponStr, out ItemNames weapon))
            {
                Log.Warning($"Name {name}: Weapon enum 파싱 실패: {weaponStr}");
                return false;
            }

            if (!row.TryGetValue("Passive", out string passiveStr) || !GoogleSheetValueParsers.TryParseEnum(passiveStr, out PassiveNames passive))
            {
                Log.Warning($"Name {name}: Passive enum 파싱 실패: {passiveStr}");
                return false;
            }

            if (!row.TryGetValue("GrowStat1", out string growStat1Str) || !GoogleSheetValueParsers.TryParseEnum(growStat1Str, out StatNames growStat1))
            {
                Log.Warning($"Name {name}: GrowStat1 파싱 실패: {growStat1Str}");
                return false;
            }
            if (!row.TryGetValue("GrowStat2", out string growStat2Str) || !GoogleSheetValueParsers.TryParseEnum(growStat2Str, out StatNames growStat2))
            {
                Log.Warning($"Name {name}: GrowStat2 파싱 실패: {growStat2Str}");
                return false;
            }
            if (!row.TryGetValue("BaseStat1", out string baseStat1Str) || !GoogleSheetValueParsers.TryParseEnum(baseStat1Str, out StatNames baseStat1))
            {
                Log.Warning($"Name {name}: BaseStat1 파싱 실패: {baseStat1Str}");
                return false;
            }
            if (!row.TryGetValue("BaseStat2", out string baseStat2Str) || !GoogleSheetValueParsers.TryParseEnum(baseStat2Str, out StatNames baseStat2))
            {
                Log.Warning($"Name {name}: BaseStat2 파싱 실패: {baseStat2Str}");
                return false;
            }
            if (!row.TryGetValue("BaseStat3", out string baseStat3Str) || !GoogleSheetValueParsers.TryParseEnum(baseStat3Str, out StatNames baseStat3))
            {
                Log.Warning($"Name {name}: BaseStat3 파싱 실패: {baseStat3Str}");
                return false;
            }

            if (!row.TryGetValue("GrowStat1Value", out string growStat1ValueStr) || !GoogleSheetValueParsers.TryParseFloat(growStat1ValueStr, out float growStat1Value))
            {
                Log.Warning($"Name {name}: GrowStat1Value 파싱 실패: {growStat1ValueStr}");
                return false;
            }
            if (!row.TryGetValue("GrowStat2Value", out string growStat2ValueStr) || !GoogleSheetValueParsers.TryParseFloat(growStat2ValueStr, out float growStat2Value))
            {
                Log.Warning($"Name {name}: GrowStat2Value 파싱 실패: {growStat2ValueStr}");
                return false;
            }
            if (!row.TryGetValue("BaseStat1Value", out string baseStat1ValueStr) || !GoogleSheetValueParsers.TryParseFloat(baseStat1ValueStr, out float baseStat1Value))
            {
                Log.Warning($"Name {name}: BaseStat1Value 파싱 실패: {baseStat1ValueStr}");
                return false;
            }
            if (!row.TryGetValue("BaseStat2Value", out string baseStat2ValueStr) || !GoogleSheetValueParsers.TryParseFloat(baseStat2ValueStr, out float baseStat2Value))
            {
                Log.Warning($"Name {name}: BaseStat2Value 파싱 실패: {baseStat2ValueStr}");
                return false;
            }
            if (!row.TryGetValue("BaseStat3Value", out string baseStat3ValueStr) || !GoogleSheetValueParsers.TryParseFloat(baseStat3ValueStr, out float baseStat3Value))
            {
                Log.Warning($"Name {name}: BaseStat3Value 파싱 실패: {baseStat3ValueStr}");
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

            // 모델 생성 및 기본 필드 설정
            PlayerCharacterData m = new()
            {
                Name = name,
                DisplayName = displayName,
                Weapon = weapon,

                Passive = passive,
                GrowStats = new StatNames[] { growStat1, growStat2 },
                GrowStatValues = new float[] { growStat1Value, growStat2Value },
                BaseStats = new StatNames[] { baseStat1, baseStat2, baseStat3 },
                BaseStatValues = new float[] { baseStat1Value, baseStat2Value, baseStat3Value },

                SupportedBuildTypes = builds,
            };

            model = m;
            return true;
        }
    }
}