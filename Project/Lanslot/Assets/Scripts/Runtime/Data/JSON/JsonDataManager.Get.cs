using System.Collections.Generic;
using System.Linq;

namespace TeamSuneat.Data
{
    public partial class JsonDataManager
    {
        public static List<PlayerCharacterData> GetPlayerCharacterDataClones()
        {
            List<PlayerCharacterData> result = new();

            CharacterNames[] allPlayerCharacterNames = EnumEx.GetValues<CharacterNames>();
            int characterTID;

            for (int i = 0; i < allPlayerCharacterNames.Length; i++)
            {
                characterTID = BitConvert.Enum32ToInt(allPlayerCharacterNames[i]);
                if (_playerCharacterSheetData.ContainsKey(characterTID))
                {
                    result.Add(_playerCharacterSheetData[characterTID]);
                }
            }

            return result;
        }

        public static List<WeaponData> GetWeaponDataClones()
        {
            List<WeaponData> result = new();

            ItemNames[] allWeaponNames = EnumEx.GetValues<ItemNames>();
            int weaponTID;

            for (int i = 1; i < allWeaponNames.Length; i++)
            {
                weaponTID = BitConvert.Enum32ToInt(allWeaponNames[i]);
                if (_weaponSheetData.ContainsKey(weaponTID))
                {
                    WeaponData weaponData = _weaponSheetData[weaponTID];
                    if (!weaponData.IsBlock)
                    {
                        result.Add(weaponData);
                    }
                }
            }

            return result;
        }

        public static List<WeaponLevelData> GetWeaponLevelDataClone(ItemNames weaponName)
        {
            int weaponTID = weaponName.ToInt();
            if (_weaponLevelSheetData.ContainsKey(weaponTID))
            {
                _ = new List<WeaponLevelData>();
                if (_weaponLevelSheetData.TryGetValue(weaponTID, out List<WeaponLevelData> result))
                {
                    return result;
                }
            }

            return null;
        }

        public static StringData[] GetLoadingStringData()
        {
            List<StringData> result = new();

            StringData[] dataArray = _stringSheetData.Values.ToArray();
            for (int i = 0; i < dataArray.Length; i++)
            {
                if (dataArray[i].GetKey().Contains("Loading"))
                {
                    result.Add(dataArray[i]);
                }
            }

            return result.ToArray();
        }

        public static List<WaveData> GetWaveDataClone(StageNames stageName)
        {
            int stageTID = stageName.ToInt();
            if (_waveSheetData.ContainsKey(stageTID))
            {
                if (_waveSheetData.TryGetValue(stageTID, out List<WaveData> result))
                {
                    return result;
                }
            }

            return null;
        }

        public static WaveData GetWaveDataByNumber(StageNames stageName, int waveNumber)
        {
            List<WaveData> waveList = GetWaveDataClone(stageName);
            if (waveList == null || waveList.Count == 0)
            {
                return null;
            }

            // 보스 웨이브는 별도로 처리 (10, 20, 30, ...)
            if (waveNumber % 10 == 0)
            {
                for (int i = 0; i < waveList.Count; i++)
                {
                    WaveData waveData = waveList[i];
                    if (waveData.WaveNumber == waveNumber && waveData.WaveType == WaveTypes.Boss)
                    {
                        return waveData;
                    }
                }
            }

            // 일반 웨이브: 템플릿 웨이브 번호 계산
            // 1-9 → 1, 11-19 → 11, 21-29 → 21, ...
            int templateWaveNumber = ((waveNumber - 1) / 10) * 10 + 1;

            for (int i = 0; i < waveList.Count; i++)
            {
                WaveData waveData = waveList[i];
                if (waveData.WaveNumber == templateWaveNumber && waveData.WaveType == WaveTypes.Normal)
                {
                    return waveData;
                }
            }

            return null;
        }
    }
}