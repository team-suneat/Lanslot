using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat.Data
{
    public partial class JsonDataManager
    {
        public static List<T> DeserializeObject<T>(string jsonData)
        {
            try
            {
                return JsonConvert.DeserializeObject<List<T>>(jsonData);
            }
            catch (Exception e)
            {
                Debug.LogError(typeof(T).ToString() + ", Json 파일의 역직렬화 작업 중 에러가 나타났습니다.\n" + e.ToString());

                return new List<T>();
            }
        }

        private static List<T> DeserializeJsonData<T>(string jsonData)
        {
            try
            {
                List<T> dataList = DeserializeObject<T>(jsonData);

                if (dataList == null || dataList.Count == 0)
                {
                    Debug.LogError(typeof(T).ToString() + ", Json 파일을 역직렬화할 수 없습니다.");
                }

                return dataList;
            }
            catch (Exception ex)
            {
                Debug.LogError(typeof(T).ToString() + ", Json 파일의 역직렬화 작업 중 에러가 나타났습니다.\n" + ex.ToString());
            }

            return null;
        }

        public static void ParseJsonData(_Sheet sheet, string jsonData)
        {
            switch (sheet)
            {
                //───────────────────────────────────────────────────────────────────────────────────────

                case _Sheet.PlayerCharacter:
                    {
                        ParsePlayerCharacterJsonData(sheet, jsonData);
                    }
                    break;

                case _Sheet.MonsterCharacter:
                    {
                        ParseMonsterCharacterJsonData(sheet, jsonData);
                    }
                    break;

                //───────────────────────────────────────────────────────────────────────────────────────

                case _Sheet.Weapon:
                    {
                        ParseWeaponJsonData(sheet, jsonData);
                    }
                    break;

                case _Sheet.WeaponLevel:
                    {
                        ParseWeaponLevelJsonData(sheet, jsonData);
                    }
                    break;

                case _Sheet.Potion:
                    {
                        ParsePotionDataJsonData(sheet, jsonData);
                    }
                    break;

                //───────────────────────────────────────────────────────────────────────────────────────
                case _Sheet.Stat:
                    {
                        ParseStatJsonData(sheet, jsonData);
                    }
                    break;

                //───────────────────────────────────────────────────────────────────────────────────────

                case _Sheet.String:
                    {
                        ParseStringJsonData(sheet, jsonData);
                    }
                    break;
            }
        }

        #region Parse-Details

        private static void ParsePlayerCharacterJsonData(_Sheet sheet, string jsonData)
        {
            List<PlayerCharacterData> dataList = DeserializeJsonData<PlayerCharacterData>(jsonData);

            for (int i = 0; i < dataList.Count; i++)
            {
                dataList[i].Refresh();

                if (!_playerCharacterSheetData.ContainsKey(dataList[i].GetKey()))
                {
                    _playerCharacterSheetData.Add(dataList[i].GetKey(), dataList[i]);
                }
                else
                {
                    LogErrorSameKeyAlreadyExists(dataList[i].Name.ToString(), sheet.ToString());
                }
            }

            Log.Progress(LogTags.JsonData, $"({sheet.ToSelectString()}) Json 데이터를 읽어옵니다. 불러온 데이터의 수: {dataList.Count.ToSelectString()})");
        }

        private static void ParseMonsterCharacterJsonData(_Sheet sheet, string jsonData)
        {
            List<MonsterCharacterData> dataList = DeserializeJsonData<MonsterCharacterData>(jsonData);

            for (int i = 0; i < dataList.Count; i++)
            {
                dataList[i].Refresh();

                if (!_monsterCharacterSheetData.ContainsKey(dataList[i].GetKey()))
                {
                    _monsterCharacterSheetData.Add(dataList[i].GetKey(), dataList[i]);
                }
                else
                {
                    LogErrorSameKeyAlreadyExists(dataList[i].Name.ToString(), sheet.ToString());
                }
            }

            Log.Progress(LogTags.JsonData, $"({sheet.ToSelectString()}) Json 데이터를 읽어옵니다. 불러온 데이터의 수: {dataList.Count.ToSelectString()})");
        }

        private static void ParseWeaponJsonData(_Sheet sheet, string jsonData)
        {
            List<WeaponData> dataList = DeserializeJsonData<WeaponData>(jsonData);

            for (int i = 0; i < dataList.Count; i++)
            {
                dataList[i].Refresh();

                if (!_weaponSheetData.ContainsKey(dataList[i].GetKey()))
                {
                    _weaponSheetData.Add(dataList[i].GetKey(), dataList[i]);
                }
                else
                {
                    LogErrorSameKeyAlreadyExists(dataList[i].Name.ToString(), sheet.ToString());
                }
            }

            Log.Progress(LogTags.JsonData, $"({sheet.ToSelectString()}) Json 데이터를 읽어옵니다. 불러온 데이터의 수: {dataList.Count.ToSelectString()})");
        }

        private static void ParseWeaponLevelJsonData(_Sheet sheet, string jsonData)
        {
            List<WeaponLevelData> dataList = DeserializeJsonData<WeaponLevelData>(jsonData);

            for (int i = 0; i < dataList.Count; i++)
            {
                dataList[i].Refresh();

                if (!_weaponLevelSheetData.ContainsKey(dataList[i].GetKey()))
                {
                    _weaponLevelSheetData.Add(dataList[i].GetKey(), dataList[i]);
                }
                else
                {
                    LogErrorSameKeyAlreadyExists(dataList[i].Name.ToString(), sheet.ToString());
                }
            }

            Log.Progress(LogTags.JsonData, $"({sheet.ToSelectString()}) Json 데이터를 읽어옵니다. 불러온 데이터의 수: {dataList.Count.ToSelectString()})");
        }

        private static void ParsePotionDataJsonData(_Sheet sheet, string jsonData)
        {
            List<PotionData> dataList = DeserializeJsonData<PotionData>(jsonData);

            for (int i = 0; i < dataList.Count; i++)
            {
                dataList[i].Refresh();

                if (!_potionSheetData.ContainsKey(dataList[i].GetKey()))
                {
                    _potionSheetData.Add(dataList[i].GetKey(), dataList[i]);
                }
                else
                {
                    LogErrorSameKeyAlreadyExists(dataList[i].GetKey().ToString(), sheet.ToString());
                }
            }

            Log.Progress(LogTags.JsonData, $"({sheet.ToSelectString()}) Json 데이터를 읽어옵니다. 불러온 데이터의 수: {dataList.Count.ToSelectString()})");
        }

        private static void ParseStatJsonData(_Sheet sheet, string jsonData)
        {
            List<StatData> dataList = DeserializeJsonData<StatData>(jsonData);

            for (int i = 0; i < dataList.Count; i++)
            {
                dataList[i].Refresh();

                if (!_statSheetData.ContainsKey(dataList[i].GetKey()))
                {
                    _statSheetData.Add(dataList[i].GetKey(), dataList[i]);
                }
                else
                {
                    LogErrorSameKeyAlreadyExists(dataList[i].Name.ToString(), sheet.ToString());
                }
            }

            Log.Progress(LogTags.JsonData, $"({sheet.ToSelectString()}) Json 데이터를 읽어옵니다. 불러온 데이터의 수: {dataList.Count.ToSelectString()})");
        }

        private static void ParseStringJsonData(_Sheet sheet, string jsonData)
        {
            List<StringData> dataList = DeserializeJsonData<StringData>(jsonData);

            for (int i = 0; i < dataList.Count; i++)
            {
                dataList[i].Refresh();

                if (!_stringSheetData.ContainsKey(dataList[i].GetKey()))
                {
                    _stringSheetData.Add(dataList[i].GetKey(), dataList[i]);
                }
                else
                {
                    LogErrorSameKeyAlreadyExists(dataList[i].GetKey().ToString(), sheet.ToString());
                }
            }

            Log.Progress(LogTags.JsonData, $"({sheet.ToSelectString()}) Json 데이터를 읽어옵니다. 불러온 데이터의 수: {dataList.Count.ToSelectString()})");
        }

        #endregion Parse-Details
    }
}