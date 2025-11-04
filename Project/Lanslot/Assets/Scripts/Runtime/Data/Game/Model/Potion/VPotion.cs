using System;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VPotion
    {
        [NonSerialized]
        public ItemNames Name;
        public string NameString;

        public int Level;

        public VPotion()
        { }

        public VPotion(ItemNames potionName)
        {
            Name = potionName;
            NameString = potionName.ToString();
            Level = 1;
        }

        public void OnLoadGameData()
        {
            _ = EnumEx.ConvertTo(ref Name, NameString);
        }

        public void LevelUp()
        {
            Level++;
        }
    }
}

