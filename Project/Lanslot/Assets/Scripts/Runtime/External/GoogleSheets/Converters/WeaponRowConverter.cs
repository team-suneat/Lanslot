using System.Collections.Generic;
using TeamSuneat.Data;

namespace TeamSuneat
{
    public sealed class WeaponRowConverter : IGoogleSheetRowConverter<WeaponData>
    {
        public bool TryConvert(Dictionary<string, string> row, out WeaponData model, IList<string> warnings)
        {
            model = null;

            // 필수: Name
            if (!row.TryGetValue("Name", out string nameStr) || !GoogleSheetValueParsers.TryParseEnum(nameStr, out WeaponNames name))
            {
                warnings?.Add($"필수 컬럼 Name 누락 또는 enum 파싱 실패: {nameStr}");
                return false;
            }

            // 선택: 표시 이름
            row.TryGetValue("DisplayName", out string displayName);

            // 선택: AttackRange / Passive / Hitmark / Reward
            AttackRangeTypes[] attackRanges;
            if (row.TryGetValue("AttackRange", out string attackRangeStr))
            {
                if (!GoogleSheetValueParsers.TryParseEnumArray(attackRangeStr, out attackRanges))
                {
                    attackRanges = System.Array.Empty<AttackRangeTypes>();
                    warnings?.Add($"Name {name}: AttackRange 파싱 실패 → 빈 배열 사용");
                }
            }
            else
            {
                attackRanges = System.Array.Empty<AttackRangeTypes>();
            }
            row.TryGetValue("Passive", out string passiveStr);
            row.TryGetValue("Hitmark", out string hitmarkStr);
            row.TryGetValue("Reward", out string rewardStr);

            // 선택: SupportedBuildTypes
            BuildTypes[] builds;
            if (row.TryGetValue("SupportedBuildTypes", out string buildsStr))
            {
                if (!GoogleSheetValueParsers.TryParseEnumArray(buildsStr, out builds))
                {
                    builds = System.Array.Empty<BuildTypes>();
                    warnings?.Add($"Name {name}: SupportedBuildTypes 파싱 실패 → 빈 배열 사용");
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
                AttackRanges = attackRanges,                
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
                    warnings?.Add($"Name {name}: Passive enum 파싱 실패('{passiveStr}')");
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
                    warnings?.Add($"Name {name}: Hitmark enum 파싱 실패('{hitmarkStr}')");
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
                    warnings?.Add($"Name {name}: Reward(Currency) enum 파싱 실패('{rewardStr}')");
                }
            }

            // WeaponLevel 형식 및 Base/LevelUp 수치 파싱은 별도 컨버터에서 처리합니다.

            model = m;
            return true;
        }        
    }
}