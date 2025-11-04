using System.Collections.Generic;

namespace TeamSuneat.Data
{
    public partial class JsonDataManager
    {
        public static PlayerCharacterData FindPlayerCharacterDataClone(CharacterNames key)
        {
            return FindPlayerCharacterDataClone(BitConvert.Enum32ToInt(key));
        }

        public static PlayerCharacterData FindPlayerCharacterDataClone(int key)
        {
            if (_playerCharacterSheetData.ContainsKey(key))
            {
                return _playerCharacterSheetData[key];
            }

            return new PlayerCharacterData();
        }

        public static MonsterCharacterData FindMonsterCharacterDataClone(CharacterNames key)
        {
            return FindMonsterCharacterDataClone(BitConvert.Enum32ToInt(key));
        }

        public static MonsterCharacterData FindMonsterCharacterDataClone(int key)
        {
            if (_monsterCharacterSheetData.ContainsKey(key))
            {
                return _monsterCharacterSheetData[key];
            }

            return new MonsterCharacterData();
        }

        public static StageData FindStageDataClone(StageNames key)
        {
            return FindStageDataClone(BitConvert.Enum32ToInt(key));
        }

        public static StageData FindStageDataClone(int key)
        {
            if (_stageSheetData.ContainsKey(key))
            {
                return _stageSheetData[key];
            }

            return new StageData();
        }

        public static PotionData FindPotionDataClone(ItemNames key)
        {
            return FindPotionDataClone(BitConvert.Enum32ToInt(key));
        }

        public static PotionData FindPotionDataClone(int key)
        {
            if (_potionSheetData.ContainsKey(key))
            {
                return _potionSheetData[key];
            }

            return new PotionData();
        }

        public static StatData FindStatDataClone(StatNames key)
        {
            int tid = BitConvert.Enum32ToInt(key);

            if (!_statSheetData.IsValid())
            {
                Log.Error("스탯 데이터를 불러오지 못했습니다. Count: 0");
            }

            return FindStatDataClone(tid);
        }

        public static StatData FindStatDataClone(int key)
        {
            if (_statSheetData.ContainsKey(key))
            {
                return _statSheetData[key];
            }

            return new StatData();
        }

        public static string FindStringClone(string key)
        {
            if (_stringSheetData.ContainsKey(key))
            {
                return _stringSheetData[key].GetString();
            }

            return string.Empty;
        }

        public static string FindStringClone(string key, LanguageNames languageName)
        {
            if (_stringSheetData.ContainsKey(key))
            {
                return _stringSheetData[key].GetString(languageName);
            }

            Log.Warning(LogTags.JsonData, "스트링을 찾을 수 없습니다: {0}", key);
            return string.Empty;
        }

        public static StringData FindStringData(string key)
        {
            if (_stringSheetData.ContainsKey(key))
            {
                return _stringSheetData[key];
            }

            return new StringData();
        }

        public static WeaponData FindWeaponDataClone(ItemNames key)
        {
            return FindWeaponDataClone(BitConvert.Enum32ToInt(key));
        }

        public static WeaponData FindWeaponDataClone(int key)
        {
            if (_weaponSheetData.ContainsKey(key))
            {
                return _weaponSheetData[key];
            }

            return new WeaponData();
        }

        public static CharacterLevelExpData FindCharacterLevelExpDataClone(int level)
        {
            if (_characterLevelExpSheetData.ContainsKey(level))
            {
                return _characterLevelExpSheetData[level];
            }

            return new CharacterLevelExpData();
        }

        public static CharacterRankExpData FindCharacterRankExpDataClone(int rank)
        {
            if (_characterRankExpSheetData.ContainsKey(rank))
            {
                return _characterRankExpSheetData[rank];
            }

            return new CharacterRankExpData();
        }
    }
}