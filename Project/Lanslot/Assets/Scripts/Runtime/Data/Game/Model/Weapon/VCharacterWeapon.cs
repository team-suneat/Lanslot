using System.Collections.Generic;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VCharacterWeapon
    {
        private readonly ListMultiMap<string, VWeapon> _weapons = new();

        public void OnLoadData()
        {
            foreach (KeyValuePair<string, List<VWeapon>> kvp in _weapons.Storage)
            {
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    VWeapon weapon = kvp.Value[i];
                    weapon.OnLoadData();
                }
            }
        }

        public void AddWeapon(WeaponNames weapon, int itemLevel)
        {
            VWeapon newWeapon = new(weapon, itemLevel);
            _weapons.Add(weapon.ToString(), newWeapon);
        }

        public void RemoveWeapon(WeaponNames weapon)
        {
            _weapons.RemoveAll(weapon.ToString());
        }

        public void ClearIngameData()
        {
            _weapons.Clear();
        }

        public bool HasWeapon(WeaponNames weapon)
        {
            return _weapons.ContainsKey(weapon.ToString());
        }
    }
}