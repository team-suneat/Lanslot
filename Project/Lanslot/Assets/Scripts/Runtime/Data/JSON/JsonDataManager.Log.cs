namespace TeamSuneat.Data
{
    public partial class JsonDataManager
    {
        private static void LogErrorParseJsonData()
        {
#if UNITY_EDITOR
            if (!_statSheetData.IsValid())
            {
                Log.Error("[JsonDataManager] Stat 시트 데이터를 읽어오지 못했습니다.");
            }
            if (!_stringSheetData.IsValid())
            {
                Log.Error("[JsonDataManager] String 시트 데이터를 읽어오지 못했습니다.");
            }
#endif
        }

        private static void LogWarningParseJsonData()
        {
#if UNITY_EDITOR
#endif
        }

        private static void LogErrorSameKeyAlreadyExists(string dataName, string sheetName)
        {
#if UNITY_EDITOR
            Log.Error(dataName + ", 같은 키를 가진 데이터가 이미 존재합니다. 시트: " + sheetName.ToString());
#endif
        }

        private static void LogProgress(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.JsonData, format, args);
            }
        }

        private static void LogWarning(string format, params object[] args)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.JsonData, format, args);
            }
        }
    }
}