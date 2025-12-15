using System.Collections.Generic;
using TeamSuneat.Data;

namespace TeamSuneat
{
    public sealed class MonsterCharacterRowConverter : IGoogleSheetRowConverter<MonsterCharacterData>
    {
        public bool TryConvert(Dictionary<string, string> row, out MonsterCharacterData model)
        {
            model = null;

            if (!row.TryGetValue("Name", out string nameStr) || !GoogleSheetValueParsers.TryParseEnum<CharacterNames>(nameStr, out CharacterNames name))
            {
                Log.Warning($"필수 컬럼 Name 누락 또는 enum 파싱 실패: {nameStr}");
                return false;
            }

            // 선택: 표시 이름
            row.TryGetValue("DisplayName", out string displayName);

            if (!row.TryGetValue("Health", out string maxHPString) || !GoogleSheetValueParsers.TryParseInt(maxHPString, out int maxHP))
            {
                Log.Warning($"Name {name}: Health 파싱 실패: {maxHPString}");
                return false;
            }

            if (!row.TryGetValue("Damage", out string damageString) || !GoogleSheetValueParsers.TryParseInt(damageString, out int damage))
            {
                Log.Warning($"Name {name}: Damage 파싱 실패: {damageString}");
                return false;
            }

            if (!row.TryGetValue("AttackRange", out string attackRangeString) || !GoogleSheetValueParsers.TryParseInt(attackRangeString, out int attackRange))
            {
                Log.Warning($"Name {name}: AttackRange 파싱 실패: {attackRangeString}");
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
            MonsterCharacterData m = new()
            {
                Name = name,
                DisplayName = displayName,
                Health = maxHP,
                Damage = damage,
                AttackRange = attackRange,
                SupportedBuildTypes = builds,
            };

            model = m;
            return true;
        }
    }
}