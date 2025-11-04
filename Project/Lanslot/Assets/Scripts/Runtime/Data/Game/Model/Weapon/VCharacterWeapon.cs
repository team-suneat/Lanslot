using System.Collections.Generic;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VCharacterWeapon
    {
        public Dictionary<string, VWeapon> Weapons = new();
        public List<string> UnlockedWeapons = new();

        public void OnLoadGameData()
        {
            foreach (KeyValuePair<string, VWeapon> kvp in Weapons)
            {
                VWeapon weapon = kvp.Value;
                weapon.OnLoadGameData();
            }
        }

        public void ClearIngameData()
        {
            Log.Info(LogTags.GameData_Weapon, "인게임 무기를 초기화합니다. 인게임 무기의 수: {0}개", Weapons.Count);
            Weapons.Clear();
        }

        //

        public bool CheckUnlocked(ItemNames weaponName)
        {
            return UnlockedWeapons.Contains(weaponName.ToString());
        }

        public void Unlock(ItemNames weaponName)
        {
            string key = weaponName.ToString();
            if (!UnlockedWeapons.Contains(key))
            {
                UnlockedWeapons.Add(key);
                Log.Info(LogTags.GameData_Weapon, "무기를 해금합니다: {0}", weaponName);
            }
        }

        //

        public bool HasWeapon(ItemNames weaponName)
        {
            return Weapons.ContainsKey(weaponName.ToString());
        }

        public void AddWeapon(ItemNames weaponName)
        {
            string key = weaponName.ToString();
            if (!Weapons.ContainsKey(key))
            {
                VWeapon newWeapon = new(weaponName);
                Weapons.Add(key, newWeapon);

                Log.Info(LogTags.GameData_Weapon, "인게임 무기를 등록합니다: {0}", weaponName.ToLogString());
            }
        }

        public void AddWeapon(ItemNames weaponName, GradeNames gradeName, StatNames statName)
        {
            AddWeapon(weaponName);

            if (gradeName != GradeNames.None && statName != StatNames.None)
            {
                string key = weaponName.ToString();
                Weapons[key].AddGrade(gradeName, statName);
            }
            else
            {
                Log.Error("인게임 무기 추가에 필요한 올바른 등급 또는 능력치 이름이 아닙니다: {0}, {1}", gradeName.ToLogString(), statName.ToLogString());
            }
        }

        public void RemoveWeapon(ItemNames weaponName)
        {
            string key = weaponName.ToString();
            if (Weapons.ContainsKey(key))
            {
                Weapons.Remove(key);
                Log.Info(LogTags.GameData_Weapon, "인게임 무기를 등록해제합니다: {0}", weaponName.ToLogString());
            }
        }

        //

        public static VCharacterWeapon CreateDefault()
        {
            VCharacterWeapon defaultWeapons = new();

            defaultWeapons.Unlock(ItemNames.WarriorSword);
            defaultWeapons.Unlock(ItemNames.ExecutionerDagger);
            defaultWeapons.Unlock(ItemNames.CrimsonAxe);
            defaultWeapons.Unlock(ItemNames.PaladinShield);
            defaultWeapons.Unlock(ItemNames.CoinPurse);
            defaultWeapons.Unlock(ItemNames.GemPurse);
            defaultWeapons.Unlock(ItemNames.Stone);
            defaultWeapons.Unlock(ItemNames.Shovel);

            return defaultWeapons;
        }
    }
}