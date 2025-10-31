using System;
using System.Text;
using System.Text.RegularExpressions;
using TeamSuneat.Data;
using TeamSuneat.Setting;

namespace TeamSuneat
{
    public static partial class StringGetter
    {
        public static string GetLocalizedString(this StatNames key)
        {
            return GetLocalizedString(key, GameSetting.Instance.Language.Name);
        }

        public static string GetLocalizedString(this StatNames key, LanguageNames languageName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Stat_Name_");
            stringBuilder.Append(key.ToString());

            string result = JsonDataManager.FindStringClone(stringBuilder.ToString(), languageName);
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            return key.ToString();
        }

        public static string GetStatusDetailsString(this StatNames key)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Status_Details_Stat_Name_");
            stringBuilder.Append(key.ToString());

            return JsonDataManager.FindStringClone(stringBuilder.ToString());
        }

        #region Status Category Details

        public static string GetStatusDetailsCategoryString(this StatNames statName)
        {
            return GetStatusDetailsString(statName, "Status_Details_Category_");
        }

        public static string GetStatusDetailsCategoryString(this StatNames statName, string[] values)
        {
            return GetStatusDetailsString(statName, "Status_Details_Category_", values);
        }

        public static string GetStatusDetailsCalculationString(this StatNames statName, string[] values)
        {
            if (GameSetting.Instance.Play.ShowStatusCalculations)
            {
                string content = GetStatusDetailsString(statName, "Status_Details_Category_Dev_", values);
                if (!string.IsNullOrEmpty(content))
                {
                    return content;
                }
            }

            return string.Empty;
        }

        private static string GetStatusDetailsString(this StatNames statName, string key, params string[] values)
        {
            string stringKey = key + statName.ToString();
            string content;

            if (values == null || values.Length == 0)
            {
                content = JsonDataManager.FindStringClone(stringKey);
            }
            else
            {
                StringData stringData = JsonDataManager.FindStringData(stringKey);
                content = Format(stringData, values);
            }

            content = ReplacePlaceholders(content);
            return content;
        }

        private static string ReplacePlaceholders(string content)
        {
            content = ReplaceCharacterName(content);
            return content;
        }

        #endregion Status Category Details

        public static string GetStatusString(this StatNames key)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Status_Stat_Name_");
            stringBuilder.Append(key.ToString());

            return JsonDataManager.FindStringClone(stringBuilder.ToString());
        }

        public static string ReplaceStatValue(string input, bool useColor = true)
        {
            string pattern = @"\{\$Stat\.([a-zA-Z0-9_]+)\}";

            MatchCollection matches = Regex.Matches(input, pattern);
            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    string statName = match.Groups[1].Value;
                    StatNames enumStatName;
                    if (Enum.TryParse(statName, out enumStatName))
                    {
                        if (CharacterManager.Instance != null && CharacterManager.Instance.Player != null)
                        {
                            float statValue = CharacterManager.Instance.Player.Stat.FindValueOrDefault(enumStatName);
                            string replacement = enumStatName.GetStatValueString(statValue, useColor);

                            input = input.Replace(match.Value, replacement);
                        }
                        else
                        {
                            input = input.Replace(match.Value, "0");
                        }
                    }
                }
            }

            return input;
        }

        public static string ReplaceStatValue(string input, float multiplier, bool useColor = true)
        {
            string pattern = @"\{\$Stat\.([a-zA-Z0-9_]+)\}";

            MatchCollection matches = Regex.Matches(input, pattern);
            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    string statName = match.Groups[1].Value;
                    StatNames enumStatName;
                    if (Enum.TryParse(statName, out enumStatName))
                    {
                        if (CharacterManager.Instance != null && CharacterManager.Instance.Player != null)
                        {
                            float statValue = CharacterManager.Instance.Player.Stat.FindValueOrDefault(enumStatName);
                            string replacement;
                            if (multiplier.IsEqual(1f))
                            {
                                replacement = enumStatName.GetStatValueString(statValue, useColor);
                            }
                            else
                            {
                                replacement = $"{enumStatName.GetStatValueString(statValue, useColor)}({enumStatName.GetStatValueString(statValue * multiplier, useColor)})";
                            }

                            input = input.Replace(match.Value, replacement);
                        }
                        else
                        {
                            input = input.Replace(match.Value, "0");
                        }
                    }
                }
            }

            return input;
        }
    }
}