using TeamSuneat;

namespace TeamSuneat.Data
{
    public partial class VCharacterStage
    {
        #region Log

        public void LogProgress(string message)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.GameData_Stage, message);
            }
        }

        public void LogInfo(string message)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.GameData_Stage, message);
            }
        }

        public void LogWarning(string message)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.GameData_Stage, message);
            }
        }

        public void LogError(string message)
        {
            if (Log.LevelError)
            {
                Log.Error(LogTags.GameData_Stage, message);
            }
        }

        public void LogProgress(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.GameData_Stage, format, args);
            }
        }

        public void LogInfo(string format, params object[] args)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.GameData_Stage, format, args);
            }
        }

        public void LogWarning(string format, params object[] args)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.GameData_Stage, format, args);
            }
        }

        public void LogError(string format, params object[] args)
        {
            if (Log.LevelError)
            {
                Log.Error(LogTags.GameData_Stage, format, args);
            }
        }

        #endregion Log
    }
}