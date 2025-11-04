namespace TeamSuneat.Data
{
    [System.Serializable]
    public class WeaponLevelData : IData<int>
    {
        public ItemNames Name;
        public string DisplayName;

        public StatNames StatName;
        public float BaseStatValue;
        public float CommonStatValue;
        public float UncommonStatValue;
        public float RareStatValue;
        public float EpicStatValue;
        public float LegendaryStatValue;

        public int GetKey()
        {
            return Name.ToInt();
        }

        public void Refresh()
        {
        }

        public void OnLoadData()
        {
        }
    }
}