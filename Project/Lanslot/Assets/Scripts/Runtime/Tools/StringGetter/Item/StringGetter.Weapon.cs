using System.Text;
using TeamSuneat.Data;
using TeamSuneat.Setting;

namespace TeamSuneat
{
    public static partial class StringGetter
    {
        public static string GetLocalizedString(this WeaponNames key)
        {
            return GetLocalizedString(key, GameSetting.Instance.Language.Name);
        }

        public static string GetLocalizedString(this WeaponNames key, LanguageNames languageName)
        {
            if (key == WeaponNames.None)
            {
                return string.Empty;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Weapon_Name_");
            stringBuilder.Append(key.ToString());

            string result = JsonDataManager.FindStringClone(stringBuilder.ToString(), languageName);

            if (string.IsNullOrEmpty(result))
            {
                Log.Warning($"무기 이름({key})을 찾을 수 없습니다.");
                return key.ToString();
            }

            return result;
        }

        //

        public static string GetDescString(this WeaponNames key)
        {
            return GetDescString(key, GameSetting.Instance.Language.Name);
        }

        public static string GetDescString(this WeaponNames key, LanguageNames languageName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Weapon_Desc_");
            stringBuilder.Append(key.ToString());

            string content = JsonDataManager.FindStringClone(stringBuilder.ToString(), languageName);
            if (!string.IsNullOrEmpty(content))
            {
                content = ReplaceStatName(content);
            }
            return content;
        }

        public static string GetDescString(this WeaponNames key, string[] values)
        {
            return GetDescString(key, values, GameSetting.Instance.Language.Name);
        }

        public static string GetDescString(this WeaponNames key, string[] values, LanguageNames languageName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Desc_");
            stringBuilder.Append(key.ToString());

            StringData stringData = JsonDataManager.FindStringData(stringBuilder.ToString());
            if (stringData.IsValid())
            {
                string content = stringData.GetString(languageName);
                if (values.IsValid())
                {
                    return Format(content, values, stringData.Arguments);
                }
                else
                {
                    return content;
                }
            }

            Log.Warning(LogTags.String, "스트링 데이터를 찾을 수 없습니다. {0}", stringBuilder.ToString());

            return string.Empty;
        }
    }
}