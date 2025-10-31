using System.Linq;
using TeamSuneat.Data;

namespace TeamSuneat
{
    /// <summary>
    /// 하이브리드 GamePrefs 시스템
    /// - PC/모바일: PlayerPrefs 사용 (기존 방식, 최적화됨)
    /// - 콘솔: VPrefs 사용 (GameDataManager를 통한 안전한 저장)
    /// </summary>
    public static class GamePrefs
    {
        /// <summary>
        /// 게임 이름을 반환합니다.
        /// </summary>
        /// <returns>게임 이름</returns>
        public static string GetGameName()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || _DEVELOPMENT_BUILD
            return "DEV_DRAGON_IS_DEAD_";
#else
            return "DRAGON_IS_DEAD_";
#endif
        }

        /// <summary>
        /// GamePrefTypes를 키로 변환합니다.
        /// </summary>
        /// <param name="type">GamePrefTypes</param>
        /// <returns>변환된 키</returns>
        private static string GetKey(GamePrefTypes type)
        {
            return $"{GetGameName()}{type.ToUpperString()}";
        }

        /// <summary>
        /// string 타입을 키로 변환합니다.
        /// </summary>
        /// <param name="type">string 타입</param>
        /// <returns>변환된 키</returns>
        private static string GetKey(string type)
        {
            return $"{GetGameName()}{type.ToUpperString()}";
        }

        /// <summary>
        /// 플랫폼별 최적화된 저장 방식 선택
        /// </summary>
        private static bool UsePlayerPrefs()
        {
#if UNITY_PS4 || UNITY_PS5 || UNITY_XBOXONE || UNITY_SWITCH
            return false; // 콘솔: VPrefs 사용
#else
            // PC/모바일: PlayerPrefs 사용
            return true;
#endif
        }

        /// <summary>
        /// VPrefs 인스턴스를 반환합니다.
        /// </summary>
        private static VPrefs GetVPrefs()
        {
            if (GameApp.Instance?.dataManager?.Data?.Prefs != null)
            {
                return GameApp.Instance.dataManager.Data.Prefs;
            }

            Log.Warning(LogTags.GamePref, "GameApp이 초기화되지 않았습니다. 저장 또는 탐색에 실패합니다.");
            return null;
        }

        public static bool HasKey(string type)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                if (UsePlayerPrefs())
                {
                    return PlayerPrefsEx.HasKey(key);
                }
                else
                {
                    VPrefs prefsInfo = GetVPrefs();
                    if (prefsInfo != null)
                    {
                        return prefsInfo.HasKey(key);
                    }
                }
            }

            return false;
        }

        public static bool HasKey(GamePrefTypes type)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                if (UsePlayerPrefs())
                {
                    return PlayerPrefsEx.HasKey(key);
                }
                else
                {
                    VPrefs prefsInfo = GetVPrefs();
                    if (prefsInfo != null)
                    {
                        return prefsInfo.HasKey(type);
                    }
                }
            }

            return false;
        }

        public static bool GetBool(GamePrefTypes type)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                if (UsePlayerPrefs())
                {
                    return PlayerPrefsEx.GetBool(key);
                }
                else
                {
                    VPrefs prefsInfo = GetVPrefs();
                    if (prefsInfo != null)
                    {
                        return prefsInfo.GetBool(type);
                    }
                }
            }

            return false;
        }

        public static bool GetBoolOrDefault(GamePrefTypes type, bool defaultValue)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                if (UsePlayerPrefs())
                {
                    return PlayerPrefsEx.GetBoolOrDefault(key, defaultValue);
                }
                else
                {
                    VPrefs prefsInfo = GetVPrefs();
                    if (prefsInfo != null)
                    {
                        return prefsInfo.GetBoolOrDefault(type, defaultValue);
                    }
                }
            }

            return defaultValue;
        }

        public static int GetInt(GamePrefTypes type, int defaultValue = 0)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                if (UsePlayerPrefs())
                {
                    return PlayerPrefsEx.GetInt(key, defaultValue);
                }
                else
                {
                    VPrefs prefsInfo = GetVPrefs();
                    if (prefsInfo != null)
                    {
                        return prefsInfo.GetInt(type, defaultValue);
                    }
                }
            }

            return defaultValue;
        }

        public static float GetFloat(GamePrefTypes type)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                if (UsePlayerPrefs())
                {
                    return PlayerPrefsEx.GetFloat(key);
                }
                else
                {
                    VPrefs prefsInfo = GetVPrefs();
                    if (prefsInfo != null)
                    {
                        return prefsInfo.GetFloat(type);
                    }
                }
            }

            return 0;
        }

        public static string GetString(GamePrefTypes type)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                if (UsePlayerPrefs())
                {
                    return PlayerPrefsEx.GetString(key);
                }
                else
                {
                    return GetVPrefs()?.GetString(type);
                }
            }

            return string.Empty;
        }

        public static string GetString(string type)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                if (UsePlayerPrefs())
                {
                    return PlayerPrefsEx.GetString(key);
                }
                else
                {
                    return GetVPrefs()?.GetString(key);
                }
            }

            return string.Empty;
        }

        public static void SetString(string type, string value)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                if (UsePlayerPrefs())
                {
                    PlayerPrefsEx.SetString(key, value);
                }
                else
                {
                    GetVPrefs()?.SetString(key, value);
                }
            }
        }

        public static void SetString(GamePrefTypes type, string value)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                if (UsePlayerPrefs())
                {
                    PlayerPrefsEx.SetString(key, value);
                }
                else
                {
                    GetVPrefs()?.SetString(type, value);
                }
            }
        }

        public static void SetBool(GamePrefTypes type, bool value)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                if (UsePlayerPrefs())
                {
                    PlayerPrefsEx.SetBool(key, value);
                }
                else
                {
                    GetVPrefs()?.SetBool(type, value);
                }
            }
        }

        public static void SetInt(GamePrefTypes type, int value)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                if (UsePlayerPrefs())
                {
                    PlayerPrefsEx.SetInt(key, value);
                }
                else
                {
                    GetVPrefs()?.SetInt(type, value);
                }
            }
        }

        public static void SetFloat(GamePrefTypes type, float value)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                if (UsePlayerPrefs())
                {
                    PlayerPrefsEx.SetFloat(key, value);
                }
                else
                {
                    GetVPrefs()?.SetFloat(type, value);
                }
            }
        }

        public static void ClearOnEntryPoint()
        {
            Delete(GamePrefTypes.EARLY_ACCESS_ENTER_NOTICE);
        }

        public static void Clear()
        {
            if (UsePlayerPrefs())
            {
                PlayerPrefsEx.Clear();
            }
            else
            {
                GetVPrefs()?.Clear();
            }
        }

        public static void Delete(GamePrefTypes type)
        {
            string key = GetKey(type);

            if (false == string.IsNullOrEmpty(key))
            {
                if (UsePlayerPrefs())
                {
                    PlayerPrefsEx.Delete(key);
                }
                else
                {
                    GetVPrefs()?.Delete(type);
                }
            }
        }

        public static void Delete(string type)
        {
            string key = GetKey(type);

            if (false == string.IsNullOrEmpty(key))
            {
                if (UsePlayerPrefs())
                {
                    PlayerPrefsEx.Delete(key);
                }
                else
                {
                    GetVPrefs()?.Delete(key);
                }
            }
        }

        public static void DeleteAllJoystickInput()
        {
            GamePrefTypes[] types = EnumEx.GetValues<GamePrefTypes>().Where(x => x.ToString().Contains("JOYSTICK")).ToArray();

            for (int i = 0; i < types.Length; i++)
            {
                Delete(types[i]);
            }
        }

        public static void DeleteAllInput()
        {
            Delete(GamePrefTypes.KEYBOARD_MOVEUP);
            Delete(GamePrefTypes.KEYBOARD_MOVEDOWN);
            Delete(GamePrefTypes.KEYBOARD_MOVELEFT);
            Delete(GamePrefTypes.KEYBOARD_MOVERIGHT);
            Delete(GamePrefTypes.KEYBOARD_JUMP);
            Delete(GamePrefTypes.KEYBOARD_ATTACK);
            Delete(GamePrefTypes.KEYBOARD_SUBATTACK);
            Delete(GamePrefTypes.KEYBOARD_CAST1);
            Delete(GamePrefTypes.KEYBOARD_CAST2);
            Delete(GamePrefTypes.KEYBOARD_CAST3);
            Delete(GamePrefTypes.KEYBOARD_CAST4);
            Delete(GamePrefTypes.KEYBOARD_POTION1);
            Delete(GamePrefTypes.KEYBOARD_INTERACT);
            Delete(GamePrefTypes.KEYBOARD_ORDERINTERACT);
            Delete(GamePrefTypes.KEYBOARD_POPUPSKILL);
            Delete(GamePrefTypes.KEYBOARD_POPUPINVENTORY);
            Delete(GamePrefTypes.KEYBOARD_POPUPITEM);
            Delete(GamePrefTypes.KEYBOARD_COMPARE);
            Delete(GamePrefTypes.KEYBOARD_SYNERGY);
            Delete(GamePrefTypes.KEYBOARD_KEYBINDING);
            Delete(GamePrefTypes.KEYBOARD_WORLDDIFFICULTY);
        }
    }
}