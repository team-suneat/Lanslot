using System;
using Sirenix.OdinInspector;

namespace TeamSuneat.Data
{
    [Serializable]
    public class StatAssetData
    {
        public StatNames Name;

        [ReadOnly]
        public string NameString;

        public float Value;
        public float ValueByLevel;

        public float MinValue;
        public float MaxValue;

        public float GrowthValue;

        public int TID => BitConvert.Enum32ToInt(Name);

        public bool Validate()
        {
            return EnumEx.ConvertTo(ref Name, NameString);
        }

        public void Refresh()
        {
            if (Name != 0)
            {
                NameString = Name.ToString();
            }
        }

        private float GetValue()
        {
            if (!Value.IsZero())
            {
                return Value;
            }

            if (!MinValue.IsZero() && !MaxValue.IsZero())
            {
                if (MinValue == MaxValue)
                {
                    return MinValue;
                }
                else
                {
                    return RandomEx.Range(MinValue, MaxValue);
                }
            }

            return 0f;
        }

        public float GetValueWithLevel(int level)
        {
            float resultValue = GetValue();

            if (level > 1)
            {
                resultValue += ValueByLevel * (level - 1);
            }

            return resultValue;
        }
    }
}