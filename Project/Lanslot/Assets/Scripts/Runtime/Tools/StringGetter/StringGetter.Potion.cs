using System.Text;
using TeamSuneat.Data;
using TeamSuneat.Setting;

namespace TeamSuneat
{
    public static partial class StringGetter
    {
        public static string GetLocalizedString(this PotionNames key)
        {
            return GetLocalizedString(key, GameSetting.Instance.Language.Name);
        }

        public static string GetLocalizedString(this PotionNames key, LanguageNames languageName)
        {
            StringBuilder stringBuilder = new();
            _ = stringBuilder.Append("Item_Name_");
            _ = stringBuilder.Append(key.ToString());
            return JsonDataManager.FindStringClone(stringBuilder.ToString(), languageName);
        }
    }
}