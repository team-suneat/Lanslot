using System.Collections.Generic;
using TeamSuneat;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public class VStat
    {
        [System.NonSerialized]
        public StatNames Name;
        public string NameString;
        public List<StatModifier> Modifiers = new();

        public void OnLoadGameData()
        {
            EnumEx.ConvertTo(ref Name, NameString);
        }

        public void AddModifier(StatModifier modifier)
        {
            Modifiers.Add(modifier);
        }

        public void AddStatToCharacter(Character character)
        {
            if (character != null)
            {
                StatData statData;
                for (int i = 0; i < Modifiers.Count; i++)
                {
                    statData = JsonDataManager.FindStatDataClone(Name);
                    character.Stat.AddWithModifier(statData, Modifiers[i]);
                }
            }
        }

        public void RemoveStatToCharacter(Character character)
        {
            if (character != null)
            {
                for (int i = 0; i < Modifiers.Count; i++)
                {
                    character.Stat.RemoveByModifier(Name, Modifiers[i]);
                }
            }
        }

        public string GetValueString(bool useColor)
        {
            float value = 0;

            if (Modifiers != null && Modifiers.Count > 0)
            {
                for (int i = 0; i < Modifiers.Count; i++)
                {
                    value += Modifiers[i].Value;
                }
            }

            return Name.GetStatValueString(value, useColor);
        }
    }
}