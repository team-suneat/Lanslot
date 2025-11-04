using System.Linq;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public class PlayerCharacterData : IData<int>
    {
        public CharacterNames Name;
        public string DisplayName;
        public ItemNames Weapon;
        public PassiveNames Passive;
        public StatNames[] BaseStats;
        public float[] BaseStatValues;
        public StatNames[] GrowStats;
        public float[] GrowStatValues;
        public BuildTypes[] SupportedBuildTypes;

        public bool IsBlock
        {
            get
            {
                if (!SupportedBuildTypes.IsValidArray())
                {
                    return true;
                }

#if UNITY_EDITOR
                GameDefineAsset defineAsset = ScriptableDataManager.Instance.GetGameDefine();
                if (defineAsset != null)
                {
                    return !SupportedBuildTypes.Contains(defineAsset.Data.EDITOR_BUILD_TYPE);
                }
#endif
                if (GameDefine.IS_DEVELOPMENT_BUILD)
                {
                    return !SupportedBuildTypes.Contains(BuildTypes.Development);
                }

                return !SupportedBuildTypes.Contains(BuildTypes.Live);
            }
        }

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