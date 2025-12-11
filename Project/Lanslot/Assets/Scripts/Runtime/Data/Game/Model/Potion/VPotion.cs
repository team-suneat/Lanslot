using System;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VPotion
    {
        [NonSerialized]
        public ItemNames Name;
        public string NameString;

        public VPotion()
        { }

        public VPotion(ItemNames potionName)
        {
            Name = potionName;
            NameString = potionName.ToString();
        }

        public void OnLoadGameData()
        {
            EnumEx.ConvertTo(ref Name, NameString);
        }
    }
}