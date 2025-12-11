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

        public float GetStatValue(GradeNames gradeName)
        {
            switch (gradeName)
            {
                case GradeNames.Common:
                    return CommonStatValue;
                case GradeNames.Uncommon:
                    return UncommonStatValue;
                case GradeNames.Rare:
                    return RareStatValue;
                case GradeNames.Epic:
                    return EpicStatValue;
                case GradeNames.Legendary:
                    return LegendaryStatValue;    
                default:
                    return BaseStatValue;
            }
        }
    }
}