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

        public bool CheckUnlocked(WeaponNames weaponName)
        {
            return UnlockedWeapons.Contains(weaponName.ToString());
        }

        public void Unlock(WeaponNames weaponName)
        {
            string key = weaponName.ToString();
            if (!UnlockedWeapons.Contains(key))
            {
                UnlockedWeapons.Add(key);
                Log.Info(LogTags.GameData_Weapon, "무기를 해금합니다: {0}", weaponName);
            }
        }

        //

        public bool HasWeapon(WeaponNames weaponName)
        {
            return Weapons.ContainsKey(weaponName.ToString());
        }

        public void AddWeapon(WeaponNames weaponName)
        {
            string key = weaponName.ToString();
            if (!Weapons.ContainsKey(key))
            {
                VWeapon newWeapon = new(weaponName);
                Weapons.Add(key, newWeapon);

                Log.Info(LogTags.GameData_Weapon, "인게임 무기를 등록합니다: {0}", weaponName.ToLogString());
            }
        }

        public void AddWeapon(WeaponNames weaponName, GradeNames gradeName, StatNames statName)
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

        public void RemoveWeapon(WeaponNames weaponName)
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

            defaultWeapons.Unlock(WeaponNames.WarriorSword);
            defaultWeapons.Unlock(WeaponNames.ExecutionerDagger);
            defaultWeapons.Unlock(WeaponNames.CrimsonAxe);
            defaultWeapons.Unlock(WeaponNames.PaladinShield);
            defaultWeapons.Unlock(WeaponNames.CoinPurse);
            defaultWeapons.Unlock(WeaponNames.GemPurse);
            defaultWeapons.Unlock(WeaponNames.Stone);
            defaultWeapons.Unlock(WeaponNames.Shovel);

            return defaultWeapons;
        }
    }
}