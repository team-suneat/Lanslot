using System.Collections.Generic;
using TeamSuneat.Data;

namespace TeamSuneat
{
    public sealed class WeaponRowConverter : IGoogleSheetRowConverter<WeaponData>
    {
        public bool TryConvert(Dictionary<string, string> row, out WeaponData model)
        {
            model = null;

            // 필수: Name
            if (!row.TryGetValue("Name", out string nameStr) || !GoogleSheetValueParsers.TryParseEnum(nameStr, out ItemNames name))
            {
                Log.Warning($"필수 컬럼 Name 누락 또는 enum 파싱 실패: {nameStr}");
                return false;
            }

            // 선택: 표시 이름
            row.TryGetValue("DisplayName", out string displayName);

            if (!row.TryGetValue("AttackRange", out string attackRangeStr) || !GoogleSheetValueParsers.TryParseInt(attackRangeStr, out int attackRange))
            {
                Log.Warning($"Name {name}: AttackRange 파싱 실패: {attackRangeStr}");
                return false;
            }
            if (!row.TryGetValue("AttackRow", out string attackRowStr) || !GoogleSheetValueParsers.TryParseInt(attackRowStr, out int attackRow))
            {
                Log.Warning($"Name {name}: AttackRow 파싱 실패: {attackRowStr}");
                return false;
            }
            if (!row.TryGetValue("AttackColumn", out string attackColumnStr) || !GoogleSheetValueParsers.TryParseInt(attackColumnStr, out int attackColumn))
            {
                Log.Warning($"Name {name}: AttackColumn 파싱 실패: {attackColumnStr}");
                return false;
            }
            if (!row.TryGetValue("MultiHitCount", out string multiHitCountStr) || !GoogleSheetValueParsers.TryParseInt(multiHitCountStr, out int multiHitCount))
            {
                Log.Warning($"Name {name}: MultiHitCount 파싱 실패: {multiHitCountStr}");
                return false;
            }
            if (!row.TryGetValue("Damage", out string damageStr) || !GoogleSheetValueParsers.TryParseInt(damageStr, out int damage))
            {
                Log.Warning($"Name {name}: Damage 파싱 실패: {damageStr}");
                return false;
            }

            row.TryGetValue("AttackAreaShape", out string attackAreaShapeStr);
            row.TryGetValue("Passive", out string passiveStr);
            row.TryGetValue("Hitmark", out string hitmarkStr);
            row.TryGetValue("RewardCurrency", out string rewardStr);

            // 선택: SupportedBuildTypes
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

            // 모델 생성
            WeaponData m = new()
            {
                Name = name,
                SupportedBuildTypes = builds,
                DisplayName = displayName,
                AttackRange = attackRange,
                AttackRow = attackRow,
                AttackColumn = attackColumn,
                MultiHitCount = multiHitCount,
                Damage = damage,
            };

            // enum 파싱 (실패 시 기본값 유지)
            if (!string.IsNullOrEmpty(passiveStr))
            {
                if (GoogleSheetValueParsers.TryParseEnum(passiveStr, out PassiveNames passiveEnum))
                {
                    m.Passive = passiveEnum;
                }
                else
                {
                    Log.Warning($"Name {name}: Passive enum 파싱 실패('{passiveStr}')");
                }
            }
            if (!string.IsNullOrEmpty(hitmarkStr))
            {
                if (GoogleSheetValueParsers.TryParseEnum(hitmarkStr, out HitmarkNames hitmarkEnum))
                {
                    m.Hitmark = hitmarkEnum;
                }
                else
                {
                    Log.Warning($"Name {name}: Hitmark enum 파싱 실패('{hitmarkStr}')");
                }
            }
            if (!string.IsNullOrEmpty(rewardStr))
            {
                if (GoogleSheetValueParsers.TryParseEnum(rewardStr, out CurrencyNames rewardEnum))
                {
                    m.RewardCurrency = rewardEnum;
                }
                else
                {
                    Log.Warning($"Name {name}: Reward(Currency) enum 파싱 실패('{rewardStr}')");
                }
            }
            if (!string.IsNullOrEmpty(attackAreaShapeStr))
            {
                if (GoogleSheetValueParsers.TryParseEnum(attackAreaShapeStr, out AttackAreaShape attackAreaShapeEnum))
                {
                    m.AttackAreaShape = attackAreaShapeEnum;
                }
                else
                {
                    Log.Warning($"Name {name}: AttackAreaShape enum 파싱 실패('{rewardStr}')");
                }
            }

            // WeaponLevel 형식 및 Base/LevelUp 수치 파싱은 별도 컨버터에서 처리합니다.

            model = m;
            return true;
        }
    }
}