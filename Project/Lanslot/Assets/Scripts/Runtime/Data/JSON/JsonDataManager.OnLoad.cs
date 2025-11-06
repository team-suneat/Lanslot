using System.Collections.Generic;

namespace TeamSuneat.Data
{
    public partial class JsonDataManager
    {
        #region OnLoadData

        private static void OnLoadJsonDatas()
        {
            OnLoadJsonData(_playerCharacterSheetData.Values);
            OnLoadJsonData(_monsterCharacterSheetData.Values);
            OnLoadJsonData(_passiveSheetData.Values);
            OnLoadJsonData(_statSheetData.Values);
            OnLoadJsonData(_weaponSheetData.Values);
            OnLoadJsonData(_potionSheetData.Values);
            OnLoadJsonData(_stageSheetData.Values);
            OnLoadJsonData(_stringSheetData.Values);
            OnLoadJsonData(_weaponLevelSheetData);
            OnLoadJsonData(_waveSheetData);
            OnLoadJsonData(_characterLevelExpSheetData.Values);
            OnLoadJsonData(_characterRankExpSheetData.Values);
        }

        private static void OnLoadJsonData(IEnumerable<IData<int>> datas)
        {
            foreach (IData<int> data in datas)
            {
                data.OnLoadData();
            }
        }

        private static void OnLoadJsonData(IEnumerable<IData<string>> datas)
        {
            foreach (IData<string> data in datas)
            {
                data.OnLoadData();
            }
        }

        private static void OnLoadJsonData<T>(ListMultiMap<int, T> multiMap) where T : IData<int>
        {
            foreach (List<T> valueList in multiMap.Storage.Values)
            {
                foreach (T data in valueList)
                {
                    data.OnLoadData();
                }
            }
        }

        private static void OnLoadJsonData<T>(ListMultiMap<string, T> multiMap) where T : IData<string>
        {
            foreach (List<T> valueList in multiMap.Storage.Values)
            {
                foreach (T data in valueList)
                {
                    data.OnLoadData();
                }
            }
        }

        #endregion OnLoadData
    }
}