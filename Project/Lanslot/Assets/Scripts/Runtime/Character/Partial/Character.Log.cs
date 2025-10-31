using TeamSuneat.Data;

namespace TeamSuneat
{
    public partial class Character
    {
        protected virtual void LogProgress(string content)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.Character, StringGetter.ConcatStringWithComma(Name.ToLogString(), content));
            }
        }

        protected virtual void LogProgress(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                string content = string.Format(format, args);
                Log.Progress(LogTags.Character, StringGetter.ConcatStringWithComma(Name.ToLogString(), content));
            }
        }

        protected virtual void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Character, StringGetter.ConcatStringWithComma(Name.ToLogString(), content));
            }
        }

        protected virtual void LogInfo(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                string content = string.Format(format, args);
                Log.Progress(LogTags.Character, StringGetter.ConcatStringWithComma(Name.ToLogString(), content));
            }
        }

        protected virtual void LogWarning(string content)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Character, StringGetter.ConcatStringWithComma(Name.ToLogString(), content));
            }
        }

        protected virtual void LogWarning(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                string content = string.Format(format, args);
                Log.Progress(LogTags.Character, StringGetter.ConcatStringWithComma(Name.ToLogString(), content));
            }
        }

        protected virtual void LogError(string content)
        {
            if (Log.LevelError)
            {
                Log.Error(LogTags.Character, StringGetter.ConcatStringWithComma(Name.ToLogString(), content));
            }
        }

        protected virtual void LogError(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                string content = string.Format(format, args);
                Log.Progress(LogTags.Character, StringGetter.ConcatStringWithComma(Name.ToLogString(), content));
            }
        }
    }
}