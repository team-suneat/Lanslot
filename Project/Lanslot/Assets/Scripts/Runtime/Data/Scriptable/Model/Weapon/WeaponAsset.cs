using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "TeamSuneat/Scriptable/Weapon")]
    public class WeaponAsset : XScriptableObject
    {
        public int TID => BitConvert.Enum32ToInt(Data.Name);

        public ItemNames Name => Data.Name;

        public WeaponAssetData Data;

        public override void OnLoadData()
        {
            base.OnLoadData();

            LogError();

            Data.OnLoadData();
        }

        private void LogError()
        {
#if UNITY_EDITOR

            if (Data.IsChangingAsset)
            {
                Log.Error("Asset의 IsChangingAsset 변수가 활성화되어있습니다. {0}", name);
            }
            if (Data.Name == ItemNames.None)
            {
                Log.Warning(LogTags.ScriptableData, "[Weapon] Weapon Asset의 WeaponType 변수가 설정되지 않았습니다. {0}", name);
            }
            if (!Data.Hitmarks.IsValidArray())
            {
                Log.Warning(LogTags.ScriptableData, "[Weapon] Weapon Asset의 Hitmark 변수가 설정되지 않았습니다. {0}", name);
            }
            if (Data.AttackSpeed <= 0f)
            {
                Log.Warning(LogTags.ScriptableData, "[Weapon] Weapon Asset의 AttackSpeed 변수가 설정되지 않았습니다. {0}", name);
            }
            if (Data.AttackCount <= 0)
            {
                Log.Warning(LogTags.ScriptableData, "[Weapon] Weapon Asset의 AttackCount 변수가 설정되지 않았습니다. {0}", name);
            }

            if (Data.Hitmarks != null)
            {
                for (int i = 0; i < Data.Hitmarks.Length; i++)
                {
                    HitmarkNames hitmarkName = Data.Hitmarks[i];
                    if (hitmarkName == HitmarkNames.None) { continue; }

                    string hitmarkPath = PathManager.FindAssetPath($"Hitmark_{hitmarkName}");
                    if (string.IsNullOrEmpty(hitmarkPath))
                    {
                        Log.Error("무기의 히트마크 에셋 경로를 추적할 수 없습니다: {0}", hitmarkName.ToLogString());
                    }
                }
            }
#endif
        }

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            if (!Data.IsChangingAsset)
            {
                _ = EnumEx.ConvertTo(ref Data.Name, NameString);
            }

            Data.Validate();
        }

        public override void Refresh()
        {
            NameString = Data.Name.ToString();
            Data.Refresh();

            base.Refresh();
        }

        public override void Rename()
        {
            Rename("Weapon");
        }

        protected override void RefreshAll()
        {
#if UNITY_EDITOR
            if (Selection.objects.Length > 1)
            {
                Debug.LogWarning("여러 개의 스크립터블 오브젝트가 선택되었습니다. 하나만 선택한 상태에서 실행하세요.");
                return;
            }
#endif
            ItemNames[] weaponName = EnumEx.GetValues<ItemNames>();
            int weaponCount = 0;

            Log.Info("모든 무기 에셋의 갱신을 시작합니다: {0}", weaponName.Length);

            base.RefreshAll();

            for (int i = 1; i < weaponName.Length; i++)
            {
                if (weaponName[i] != ItemNames.None)
                {
                    WeaponAsset asset = ScriptableDataManager.Instance.FindWeapon(weaponName[i]);
                    if (asset.IsValid())
                    {
                        if (asset.RefreshWithoutSave())
                        {
                            weaponCount += 1;
                        }
                    }
                }

                float progressRate = (i + 1).SafeDivide(weaponName.Length);
                EditorUtility.DisplayProgressBar("모든 무기 에셋의 갱신", weaponName[i].ToString(), progressRate);
            }

            EditorUtility.ClearProgressBar();
            OnRefreshAll();

            Log.Info("모든 무기 에셋의 갱신을 종료합니다: {0}/{1}", weaponCount.ToSelectString(weaponName.Length), weaponName.Length);
        }

        protected override void CreateAll()
        {
            base.CreateAll();

            ItemNames[] itemNames = EnumEx.GetValues<ItemNames>();
            for (int i = 1; i < itemNames.Length; i++)
            {
                if (itemNames[i] is ItemNames.None)
                {
                    continue;
                }

                WeaponAsset asset = ScriptableDataManager.Instance.FindWeapon(itemNames[i]);
                if (asset == null)
                {
                    asset = CreateAsset<WeaponAsset>("Weapon", itemNames[i].ToString(), true);
                    if (asset != null)
                    {
                        asset.Data = new WeaponAssetData
                        {
                            Name = itemNames[i]
                        };
                        asset.NameString = itemNames[i].ToString();
                    }
                }
            }

            PathManager.UpdatePathMetaData();
        }

#endif

        public WeaponAssetData Clone()
        {
            return Data.Clone();
        }
    }
}