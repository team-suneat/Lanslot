using System.Linq;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public class WeaponData : IData<int>
    {
        public ItemNames Name;
        public string DisplayName;
        public int AttackRange;
        public int AttackRow;
        public int AttackColumn;
        public AttackAreaShape AttackAreaShape;
        public int MultiHitCount;

        public PassiveNames Passive;
        public HitmarkNames Hitmark;
        public int Damage;
        public CurrencyNames RewardCurrency;

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