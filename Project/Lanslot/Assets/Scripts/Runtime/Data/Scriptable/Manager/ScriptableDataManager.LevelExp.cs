namespace TeamSuneat.Data
{
    public partial class ScriptableDataManager
    {
        public LevelExpAsset GetLevelExpAsset()
        {
            if (_levelExp != null) { return _levelExp; }
            Log.Warning(LogTags.ScriptableData, "LevelExpAsset을 찾을 수 없습니다.");
            return null;
        }

        private bool LoadLevelExpSync(string filePath)
        {
            if (!filePath.Contains("LevelExp"))
            {
                return false;
            }

            _levelExp = ResourcesManager.LoadResource<LevelExpAsset>(filePath);
            if (_levelExp != null)
            {
                Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                return true;
            }

            return false;
        }
    }
}