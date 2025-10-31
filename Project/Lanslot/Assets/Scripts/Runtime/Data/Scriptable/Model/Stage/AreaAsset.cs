using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "AreaAsset", menuName = "TeamSuneat/Scriptable/Area")]
    public class AreaAsset : XScriptableObject
    {
        public bool IsChangingAsset;

        [EnableIf("IsChangingAsset")]
        public AreaNames Name;

        [EnableIf("IsChangingAsset")]
        public AreaAssetData Data;

        public int TID => BitConvert.Enum32ToInt(Name);

        public override void OnLoadData()
        {
            base.OnLoadData();
        }

#if UNITY_EDITOR

        public override void Validate()
        {
            if (!IsChangingAsset)
            {
                EnumEx.ConvertTo(ref Name, NameString);

                base.Validate();
            }
        }

        public override void Rename()
        {
            Rename("Area");
        }

        public override void Refresh()
        {
            if (Name != 0)
            {
                NameString = Name.ToString();
            }

            IsChangingAsset = false;
            base.Refresh();
        }

        protected override void CreateAll()
        {
            base.CreateAll();

            AreaNames[] areaNames = EnumEx.GetValues<AreaNames>();
            for (int i = 1; i < areaNames.Length; i++)
            {
                if (areaNames[i] == AreaNames.None)
                {
                    continue;
                }

                AreaAsset asset = ScriptableDataManager.Instance.FindArea(areaNames[i]);
                if (asset == null)
                {
                    asset = CreateAsset<AreaAsset>("World", "Area", areaNames[i].ToString(), true);
                    if (asset != null)
                    {
                        asset.Data = new AreaAssetData();
                        asset.Name = areaNames[i];
                        asset.NameString = areaNames[i].ToString();
                    }
                }
            }

            PathManager.UpdatePathMetaData();
        }

#endif
    }
}