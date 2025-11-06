using System.Collections.Generic;

namespace TeamSuneat
{
    /// <summary>
    /// 구글 스프레드시트 데이터셋 식별자.
    /// - 주의: 실제 GID 값은 정수 enum이 아닌 문자열입니다. 아래 레지스트리에서 관리합니다.
    /// </summary>
    public enum GoogleSheetDatasetId
    {
        None = 0,
        PlayerCharacter,
        MonsterCharacter,
        Passive,
        Stat,
        Weapon,
        WeaponLevel,
        Potion,
        Stage,
        Wave,
        String,
        CharacterLevelExp,
        CharacterRankExp,
    }

    /// <summary>
    /// 데이터셋 ↔ GID 매핑 레지스트리
    /// - 시트 탭의 gid 값을 문자열로 관리합니다.
    /// - 프로젝트 전역에서 동일한 참조를 사용하도록 고정 상수와 조회 API를 제공합니다.
    /// </summary>
    public static class GoogleSheetDatasetGids
    {
        // 고정 GID 상수
        public const string PlayerCharacter = "0";
        public const string MonsterCharacter = "1459762493";
        public const string Passive = "1242859680";
        public const string Stat = "135446725";
        public const string Weapon = "700832083";
        public const string WeaponLevel = "1159863095";
        public const string Potion = "1558675697";
        public const string Stage = "1085377210";
        public const string Wave = "500721965";
        public const string String = "595378682";
        public const string CharacterLevelExp = "913942234";
        public const string CharacterRankExp = "2053898515";

        private static readonly Dictionary<GoogleSheetDatasetId, string> DatasetIdToGid = new()
    {
        { GoogleSheetDatasetId.PlayerCharacter, PlayerCharacter },
        { GoogleSheetDatasetId.MonsterCharacter, MonsterCharacter },
        { GoogleSheetDatasetId.Passive, Passive},
        { GoogleSheetDatasetId.Stat, Stat },
        { GoogleSheetDatasetId.Weapon, Weapon },
        { GoogleSheetDatasetId.WeaponLevel, WeaponLevel },
        { GoogleSheetDatasetId.Potion, Potion },
        { GoogleSheetDatasetId.Stage, Stage },
        { GoogleSheetDatasetId.Wave, Wave },
        { GoogleSheetDatasetId.String, String },
        { GoogleSheetDatasetId.CharacterLevelExp, CharacterLevelExp },
        { GoogleSheetDatasetId.CharacterRankExp, CharacterRankExp },
    };

        public static string GetGid(GoogleSheetDatasetId datasetId)
        {
            return DatasetIdToGid.TryGetValue(datasetId, out string gid) ? gid : null;
        }

        public static IEnumerable<(GoogleSheetDatasetId datasetId, string gid)> EnumerateMappings()
        {
            foreach (KeyValuePair<GoogleSheetDatasetId, string> kv in DatasetIdToGid)
            {
                yield return (kv.Key, kv.Value);
            }
        }
    }
}