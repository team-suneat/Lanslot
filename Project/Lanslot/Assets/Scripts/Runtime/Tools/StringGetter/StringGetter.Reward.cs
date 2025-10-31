using System.Text;
using TeamSuneat.Data;
using TeamSuneat.Setting;

namespace TeamSuneat
{
    public static partial class StringGetter
    {
        public static string GetLocalizedString(this RewardTypes key)
        {
            return GetLocalizedString(key, GameSetting.Instance.Language.Name);
        }

        public static string GetLocalizedString(this RewardTypes key, LanguageNames languageName)
        {
            string stringKey = GetStringKey(key);
            return JsonDataManager.FindStringClone(stringKey, languageName);
        }

        public static string GetStringKey(this RewardTypes key)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Reward_Type_");
            stringBuilder.Append(key.ToString());

            return stringBuilder.ToString();
        }
    }
}