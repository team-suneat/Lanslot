using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "StageAsset", menuName = "TeamSuneat/Scriptable/Stage")]
    public class StageAsset : XScriptableObject
    {
        public StageAssetData Data;

        [FoldoutGroup("#String")]
        public string AreaString;

        public StageNames Name => Data.Name;

        public int TID => BitConvert.Enum32ToInt(Data.Name);

        public override void OnLoadData()
        {
            base.OnLoadData();

            Data.OnLoadData();
        }

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            if (!Data.IsChangingAsset)
            {
                EnumEx.ConvertTo(ref Data.Name, NameString);
                EnumEx.ConvertTo(ref Data.Name, NameString);
                EnumEx.ConvertTo(ref Data.Area, AreaString);

                Data.Refresh();
            }
        }

        public override void Rename()
        {
            Rename("Stage");
        }

        public override void Refresh()
        {
            if (Data.Name != 0)
            {
                NameString = Data.Name.ToString();
            }
            if (Data.Area != 0)
            {
                AreaString = Data.Area.ToString();
            }

            Data.Refresh();

            base.Refresh();
        }

        protected override void CreateAll()
        {
            base.CreateAll();

            StageNames[] stageNames = EnumEx.GetValues<StageNames>();
            for (int i = 1; i < stageNames.Length; i++)
            {
                if (stageNames[i] == StageNames.None)
                {
                    continue;
                }

                StageAsset asset = ScriptableDataManager.Instance.FindStage(stageNames[i]);
                if (asset == null)
                {
                    asset = CreateAsset<StageAsset>("World", "Stage", stageNames[i].ToString(), true);
                    if (asset != null)
                    {
                        asset.Data = new StageAssetData();
                        asset.Data.Name = stageNames[i];
                        asset.NameString = stageNames[i].ToString();
                    }
                }
            }

            PathManager.UpdatePathMetaData();
        }

#endif
    }
}