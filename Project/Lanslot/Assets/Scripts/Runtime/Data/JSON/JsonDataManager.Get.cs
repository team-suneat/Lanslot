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
                List<WeaponLevelData> result = new();
                if (_weaponLevelSheetData.TryGetValue(weaponTID, out result))
                {
                    return result;
                }
            }

            return null;
        }

        public static StringData[] GetLoadingStringData()
        {
            List<StringData> result = new List<StringData>();

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
    }
}