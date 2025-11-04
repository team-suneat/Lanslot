using System;
using System.Collections.Generic;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VCharacterPotion
    {
        [NonSerialized]
        public List<PotionNames> Potions = new();
        public List<string> PotionStrings = new();

        public void OnLoadGameData()
        {
            EnumEx.ConvertTo(ref Potions, PotionStrings);
        }

        public void ClearIngameData()
        {
            Potions.Clear();
            PotionStrings.Clear();
        }

        public void AddPotion(PotionNames potionName)
        {
            if (!Potions.Contains(potionName))
            {
                Potions.Add(potionName);
                string key = potionName.ToString();
                if (!PotionStrings.Contains(key))
                {
                    PotionStrings.Add(key);
                }
            }
        }

        public static VCharacterPotion CreateDefault()
        {
            return new VCharacterPotion();
        }
    }
}