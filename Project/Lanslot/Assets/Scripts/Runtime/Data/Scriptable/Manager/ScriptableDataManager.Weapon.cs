using System.Collections.Generic;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 무기 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region Weapon Get Methods

        /// <summary>
        /// 무기 에셋을 가져옵니다.
        /// </summary>
        public WeaponAsset GetWeaponAsset(ItemNames weaponName)
        {
            int key = BitConvert.Enum32ToInt(weaponName);
            return _weapons.TryGetValue(key, out var asset) ? asset : null;
        }

        /// <summary>
        /// 무기 에셋 데이터를 가져옵니다.
        /// </summary>
        public WeaponAssetData GetWeaponAssetData(ItemNames weaponName)
        {
            WeaponAsset weaponAsset = GetWeaponAsset(weaponName);
            return weaponAsset?.Data;
        }

        public WeaponAssetData FindWeaponClone(ItemNames weaponName)
        {
            if (weaponName != ItemNames.None)
            {
                WeaponAssetData assetData = FindWeaponClone(BitConvert.Enum32ToInt(weaponName));
                if (!assetData.IsValid())
                {
                    Log.Warning(LogTags.ScriptableData, "무기 데이터를 찾을 수 없습니다. {0}({1})", weaponName, weaponName.ToLogString());
                }
                return assetData;
            }

            return new WeaponAssetData();
        }

        public WeaponAssetData FindWeaponClone(int weaponTID)
        {
            if (_weapons.ContainsKey(weaponTID))
            {
                return _weapons[weaponTID].Clone();
            }

#if UNITY_EDITOR
            ItemNames weaponName = weaponTID.ToEnum<ItemNames>();
            Log.Warning(LogTags.ScriptableData, "무기 데이터를 찾을 수 없습니다. {0}({1})", weaponName, weaponName.ToLogString());
#endif

            return new WeaponAssetData();
        }

        #endregion Weapon Get Methods

        #region Weapon Find Methods

        /// <summary>
        /// 무기 에셋을 찾습니다.
        /// </summary>
        public WeaponAsset FindWeapon(ItemNames key)
        {
            return FindWeapon(BitConvert.Enum32ToInt(key));
        }

        private WeaponAsset FindWeapon(int tid)
        {
            if (_weapons.ContainsKey(tid))
            {
                return _weapons[tid];
            }

            return null;
        }

        /// <summary>
        /// 무기 에셋이 존재하는지 확인합니다.
        /// </summary>
        public bool HasWeapon(ItemNames weaponName)
        {
            return GetWeaponAsset(weaponName) != null;
        }

        /// <summary>
        /// 무기 에셋이 유효한지 확인합니다.
        /// </summary>
        public bool IsValidWeapon(ItemNames weaponName)
        {
            WeaponAsset weaponAsset = GetWeaponAsset(weaponName);
            return weaponAsset != null && weaponAsset.IsValid();
        }

        #endregion Weapon Find Methods

        #region Weapon Load Methods

        private bool LoadWeaponSync(string filePath)
        {
            if (!filePath.Contains("Weapon_"))
            {
                return false;
            }

            WeaponAsset asset = ResourcesManager.LoadResource<WeaponAsset>(filePath);
            if (asset != null)
            {
                if (asset.TID == 0)
                {
                    Log.Warning(LogTags.ScriptableData, "{0}, 무기 아이디가 설정되어있지 않습니다. {1}", asset.name, filePath);
                }
                else if (_weapons.ContainsKey(asset.TID))
                {
                    Log.Warning(LogTags.ScriptableData, "같은 TID로 중복 무기가 로드 되고 있습니다. TID: {0}, 기존: {1}, 새로운 이름: {2}",
                         asset.TID, _weapons[asset.TID].name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _weapons[asset.TID] = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        #endregion Weapon Load Methods

        #region Weapon Refresh Methods

        /// <summary>
        /// 모든 무기 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllWeapon()
        {
            foreach (KeyValuePair<int, WeaponAsset> item in _weapons)
            {
                Refresh(item.Value);
            }
        }

        private void Refresh(WeaponAsset weaponAsset)
        {
            weaponAsset?.Refresh();
        }

        #endregion Weapon Refresh Methods
    }
}