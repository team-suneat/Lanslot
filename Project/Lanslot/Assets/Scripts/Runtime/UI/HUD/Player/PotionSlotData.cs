using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public struct PotionSlotData
    {
        public Sprite Icon;
        public ItemSlotViewState State;

        public static PotionSlotData Locked()
        {
            return new PotionSlotData
            {
                State = ItemSlotViewState.Locked,
                Icon = null,
            };
        }

        public static PotionSlotData Empty()
        {
            return new PotionSlotData
            {
                State = ItemSlotViewState.Empty,
                Icon = null,
            };
        }

        public static PotionSlotData Equipped(Sprite icon)
        {
            return new PotionSlotData
            {
                State = ItemSlotViewState.Equipped,
                Icon = icon,
            };
        }
    }
}
