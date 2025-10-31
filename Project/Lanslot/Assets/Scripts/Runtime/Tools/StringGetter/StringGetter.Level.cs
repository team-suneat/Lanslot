using TeamSuneat.Data;

namespace TeamSuneat
{
    public static partial class StringGetter
    {
        public static string GetLevelString(this int level)
        {
            string format = JsonDataManager.FindStringClone("LevelFormat");

            if (!string.IsNullOrEmpty(format))
            {
                return string.Format(format, level);
            }

            return level.ToString();
        }

        public static string GetItemLevelString(this int level)
        {
            string format = JsonDataManager.FindStringClone("ItemLevel");

            if (!string.IsNullOrEmpty(format))
            {
                return string.Format(format, level);
            }

            return level.ToString();
        }

        public static string GetPopupItemLevelString(this float level)
        {
            string format = JsonDataManager.FindStringClone("PopupItemLevel");

            if (!string.IsNullOrEmpty(format))
            {
                return string.Format(format, level);
            }

            return level.ToString();
        }

        public static string GetCraftingItemLevelString(float minLevel, float maxLevel)
        {
            string format = JsonDataManager.FindStringClone("CraftingItemLevel");

            if (!string.IsNullOrEmpty(format))
            {
                return string.Format(format, minLevel, maxLevel);
            }

            return minLevel.ToString();
        }

        public static string GetFirstLevelString()
        {
            return JsonDataManager.FindStringClone("FirstLevel");
        }

        public static string GetNextLevelString()
        {
            return JsonDataManager.FindStringClone("NextLevel");
        }

        public static string GetCurrentLevelString(this int level)
        {
            string format = JsonDataManager.FindStringClone("CurrentLevel");

            if (!string.IsNullOrEmpty(format))
            {
                return string.Format(format, level);
            }

            return level.ToString();
        }
    }
}