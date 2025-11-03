using System.Collections.Generic;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VCharacterWeapon
    {
        private readonly Dictionary<string, VWeapon> _weapons = new();
        private readonly List<string> _currentWeapons = new();

        public void OnLoadData()
        {
            foreach (KeyValuePair<string, VWeapon> kvp in _weapons)
            {
                VWeapon weapon = kvp.Value;
                weapon.OnLoadData();
            }
        }

        public void AddWeapon(WeaponNames weaponName, int weaponLevel)
        {
            string key = weaponName.ToString();
            if (_weapons.ContainsKey(key))
            {
                VWeapon newWeapon = new(weaponName, weaponLevel);
                _weapons.Add(key, newWeapon);
            }
            else
            {
                _weapons[key].Level = weaponLevel;
            }

            
            Log.Info(LogTags.GameData_Weapon, "무기를 해금합니다: {0}(Lv.{1})", weaponName, weaponLevel);
        }

        public void SelectWeapon(WeaponNames weaponName)
        {
            string key = weaponName.ToString();
            if (!_currentWeapons.Contains(key))
            {
                _currentWeapons.Add(key);
                Log.Info(LogTags.GameData_Weapon, "인게임 무기를 등록합니다: {0}", weaponName);
            }
        }

        public void DeselectWeapon(WeaponNames weaponName)
        {
            string key = weaponName.ToString();
            if (_currentWeapons.Contains(key))
            {
                _ = _currentWeapons.Remove(key);
                Log.Info(LogTags.GameData_Weapon, "인게임 무기를 등록해제합니다: {0}", weaponName);
            }
        }

        public void ClearIngameData()
        {
            Log.Info(LogTags.GameData_Weapon, "인게임 무기를 초기화합니다: {0}", _currentWeapons.JoinToString());
            _currentWeapons.Clear();
        }

        public bool HasWeapon(WeaponNames weapon)
        {
            return _currentWeapons.Contains(weapon.ToString());
        }
    }
}