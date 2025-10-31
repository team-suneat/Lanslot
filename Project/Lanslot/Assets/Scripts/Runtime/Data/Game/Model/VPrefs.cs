using System.Collections.Generic;

namespace TeamSuneat.Data
{
    /// <summary>
    /// 게임 설정 데이터를 저장하는 클래스
    /// GameDataManager를 통해 자동으로 저장/로드됩니다.
    /// </summary>
    [System.Serializable]
    public class VPrefs
    {
        // 각 타입별로 분리된 Dictionary (bool은 int로 통합)
        public Dictionary<string, int> PrefsIntData = new();

        public Dictionary<string, float> PrefsFloatData = new();
        public Dictionary<string, string> PrefsStringData = new();

        public bool HasKey(string key)
        {
            return PrefsIntData.ContainsKey(key) || PrefsFloatData.ContainsKey(key) || PrefsStringData.ContainsKey(key);
        }

        public bool HasKey(GamePrefTypes type)
        {
            string key = GetKey(type);
            return HasKey(key);
        }

        public bool GetBool(GamePrefTypes type)
        {
            string key = GetKey(type);
            if (PrefsIntData.ContainsKey(key))
            {
                return PrefsIntData[key] == 1;
            }

            return false;
        }

        public bool GetBoolOrDefault(GamePrefTypes type, bool defaultValue)
        {
            string key = GetKey(type);
            if (PrefsIntData.ContainsKey(key))
            {
                return PrefsIntData[key] == 1;
            }

            return defaultValue;
        }

        public int GetInt(GamePrefTypes type, int defaultValue = 0)
        {
            string key = GetKey(type);
            if (PrefsIntData.ContainsKey(key))
            {
                return PrefsIntData[key];
            }

            return defaultValue;
        }

        public float GetFloat(GamePrefTypes type)
        {
            string key = GetKey(type);
            if (PrefsFloatData.ContainsKey(key))
            {
                return PrefsFloatData[key];
            }

            return 0f;
        }

        public string GetString(GamePrefTypes type)
        {
            string key = GetKey(type);
            if (PrefsStringData.ContainsKey(key))
            {
                return PrefsStringData[key];
            }

            return string.Empty;
        }

        public string GetString(string key)
        {
            if (PrefsStringData.ContainsKey(key))
            {
                return PrefsStringData[key];
            }

            return string.Empty;
        }

        public void SetBool(GamePrefTypes type, bool value)
        {
            string key = GetKey(type);
            PrefsIntData[key] = value ? 1 : 0;
            Log.Info(LogTags.GamePref, $"VPrefs Set Bool. key:({key}), value:({value.ToBoolString()}).");
        }

        public void SetInt(GamePrefTypes type, int value)
        {
            string key = GetKey(type);
            PrefsIntData[key] = value;
            Log.Info(LogTags.GamePref, $"VPrefs Set Int. key:({key}), value:({value}).");
        }

        public void SetFloat(GamePrefTypes type, float value)
        {
            string key = GetKey(type);
            PrefsFloatData[key] = value;
            Log.Info(LogTags.GamePref, $"VPrefs Set Float. key:({key}), value:({value}).");
        }

        public void SetString(GamePrefTypes type, string value)
        {
            string key = GetKey(type);
            PrefsStringData[key] = value;
            Log.Info(LogTags.GamePref, $"VPrefs Set String. key:({key}), value:({value}).");
        }

        public void SetString(string key, string value)
        {
            PrefsStringData[key] = value;
            Log.Info(LogTags.GamePref, $"VPrefs Set String. key:({key}), value:({value}).");
        }

        public void Delete(GamePrefTypes type)
        {
            string key = GetKey(type);
            bool hasInt = PrefsIntData.Remove(key);
            bool hasFloat = PrefsFloatData.Remove(key);
            bool hasString = PrefsStringData.Remove(key);

            if (hasInt || hasFloat || hasString)
            {
                Log.Info(LogTags.GamePref, $"VPrefs Delete. key:({key}), removed: int={hasInt}, float={hasFloat}, string={hasString}.");
            }
        }

        public void Delete(string key)
        {
            bool hasInt = PrefsIntData.Remove(key);
            bool hasFloat = PrefsFloatData.Remove(key);
            bool hasString = PrefsStringData.Remove(key);

            if (hasInt || hasFloat || hasString)
            {
                Log.Info(LogTags.GamePref, $"VPrefs Delete. key:({key}), removed: int={hasInt}, float={hasFloat}, string={hasString}.");
            }
        }

        public void Clear()
        {
            int intCount = PrefsIntData.Count;
            int floatCount = PrefsFloatData.Count;
            int stringCount = PrefsStringData.Count;

            PrefsIntData.Clear();
            PrefsFloatData.Clear();
            PrefsStringData.Clear();

            Log.Info(LogTags.GamePref, $"VPrefs Clear. removed: int={intCount}, float={floatCount}, string={stringCount}.");
        }

        public void OnLoadGameData()
        {
        }

        private string GetKey(GamePrefTypes type)
        {
            return GamePrefs.GetGameName() + type.ToUpperString();
        }
    }
}