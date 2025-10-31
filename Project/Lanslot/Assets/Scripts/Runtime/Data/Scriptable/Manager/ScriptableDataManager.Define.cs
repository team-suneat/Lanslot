using TeamSuneat;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 게임 정의 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region GameDefine Get Methods

        /// <summary>
        /// 게임 정의 에셋을 가져옵니다.
        /// </summary>
        public GameDefineAsset GetGameDefine()
        {
            return _gameDefine;
        }

        #endregion GameDefine Get Methods

        #region GameDefine Load Methods

        /// <summary>
        /// 게임 정의 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadGameDefineSync(string filePath)
        {
            if (!filePath.Contains("GameDefine"))
            {
                return false;
            }

            _gameDefine = ResourcesManager.LoadResource<GameDefineAsset>(filePath);
            if (_gameDefine != null)
            {
                Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                return true;
            }

            return false;
        }

        #endregion GameDefine Load Methods
    }
}
