using TeamSuneat;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public class VBuff
    {
        public BuffNames Name;
        public string NameString;

        public int Level;        
        public float Duration;        
        public int Stack;

        public VBuff(BuffNames name, int level, float duration, int stack)
        {
            Name = name;
            NameString = name.ToString();
            
            Level = level;
            Duration = duration;
            Stack = stack;
        }

        public void OnLoadGameData()
        {
            EnumEx.ConvertTo(ref Name, NameString);
        }

    }
}