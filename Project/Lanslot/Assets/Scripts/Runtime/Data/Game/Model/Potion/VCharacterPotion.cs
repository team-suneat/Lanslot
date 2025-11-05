using System.Collections.Generic;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VCharacterPotion
    {
        public Dictionary<string, VPotion> Potions = new();
        public List<string> UnlockedPotions = new();

        public List<ItemNames> GetPotionNames()
        {
            List<ItemNames> itemNames = new();
            ItemNames itemName = ItemNames.None;
            foreach (KeyValuePair<string, VPotion> kvp in Potions)
            {
                if (EnumEx.ConvertTo(ref itemName, kvp.Key))
                {
                    itemNames.Add(itemName);
                }
            }
            return itemNames;
        }

        public void OnLoadGameData()
        {
            if (Potions.IsValid())
            {
                foreach (KeyValuePair<string, VPotion> potion in Potions)
                {
                    potion.Value.OnLoadGameData();
                }
            }
        }

        public void ClearIngameData()
        {
            Potions.Clear();
        }

        //

        public bool CheckUnlocked(ItemNames potionName)
        {
            return UnlockedPotions.Contains(potionName.ToString());
        }

        public void Unlock(ItemNames potionName)
        {
            string key = potionName.ToString();
            if (!UnlockedPotions.Contains(key))
            {
                UnlockedPotions.Add(key);
            }
        }

        //

        public bool HasPotion(ItemNames potionName)
        {
            return Potions.ContainsKey(potionName.ToString());
        }

        public void AddPotion(ItemNames potionName)
        {
            string key = potionName.ToString();
            if (!Potions.ContainsKey(key))
            {
                Potions.Add(key, new VPotion(potionName));
            }
        }

        public void RemovePotion(ItemNames potionName)
        {
            string key = potionName.ToString();
            if (Potions.ContainsKey(key))
            {
                _ = Potions.Remove(key);
            }
        }

        public void LevelUpPotion(ItemNames potionName)
        {
            string key = potionName.ToString();
            if (Potions.ContainsKey(key))
            {
                Potions[key].LevelUp();
            }
        }

        public static VCharacterPotion CreateDefault()
        {
            return new VCharacterPotion();
        }
    }
}