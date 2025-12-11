using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public struct WeaponSlotData
    {
        public Sprite Icon;
        public ItemSlotViewState State;
        public int Level;

        public static WeaponSlotData Locked()
        {
            return new WeaponSlotData
            {
                State = ItemSlotViewState.Locked,
                Icon = null,
                Level = 0,
            };
        }

        public static WeaponSlotData Empty()
        {
            return new WeaponSlotData
            {
                State = ItemSlotViewState.Empty,
                Icon = null,
                Level = 0,
            };
        }

        public static WeaponSlotData Equipped(Sprite icon, int level)
        {
            return new WeaponSlotData
            {
                State = ItemSlotViewState.Equipped,
                Icon = icon,
                Level = level,
            };
        }
    }
}