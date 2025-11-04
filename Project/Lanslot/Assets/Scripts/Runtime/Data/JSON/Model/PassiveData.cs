using System;
using System.Linq;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public class PassiveData : IData<int>
    {
        public ItemNames Name;
        public string DisplayName;
        public string DisplayDesc;

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
            // 필요 시 데이터 갱신 로직 작성
        }

        public void OnLoadData()
        {
            // 데이터 로딩시 호출되는 초기화/후처리 로직 작성
        }
    }
}