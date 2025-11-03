using System;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VWeapon
    {
        [NonSerialized] public WeaponNames Weapon;
        public string WeaponName;
        public int Level;

        public VWeapon()
        {
            WeaponName = string.Empty;
            Level = 0;
        }

        public VWeapon(WeaponNames weapon, int level)
        {
            Weapon = weapon;
            WeaponName = weapon.ToString();
            Level = level;
        }

        public void OnLoadData()
        {
            _ = EnumEx.ConvertTo(ref Weapon, WeaponName);
        }
    }
}