using System;
using System.Collections.Generic;

namespace TeamSuneat.Data
{
    [System.Flags]
    public enum BattleStageSlotFlags
    {
        None = 0,
        First = 1 << 0, // 1
        Second = 1 << 1, // 2
        Third = 1 << 2, // 4
        Fourth = 1 << 3, // 8
    }

    public static class BattleStageSlotFlagsExtensions
    {
        public static List<int> GetSlotNumbers(this BattleStageSlotFlags flags)
        {
            List<int> result = new();

            foreach (BattleStageSlotFlags value in Enum.GetValues(typeof(BattleStageSlotFlags)))
            {
                if (value == BattleStageSlotFlags.None)
                    continue;

                if (flags.HasFlag(value))
                {
                    int slotNumber = (int)Math.Log((int)value, 2) + 1;
                    result.Add(slotNumber);
                }
            }

            return result;
        }

        public static bool IncludesSlot(this BattleStageSlotFlags flags, int slotNumber)
        {
            if (slotNumber <= 0 || slotNumber > 31)
                return false;

            BattleStageSlotFlags slotFlag = (BattleStageSlotFlags)(1 << (slotNumber - 1));
            return flags.HasFlag(slotFlag);
        }
    }
}