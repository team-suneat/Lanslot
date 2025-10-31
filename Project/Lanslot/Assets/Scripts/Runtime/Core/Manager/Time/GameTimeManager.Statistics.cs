namespace TeamSuneat
{
    /// <summary>
    /// GameTimeManager의 통계 관리 기능을 담당하는 파셜 클래스
    /// </summary>
    public partial class GameTimeManager
    {
        #region Public Methods

        /// <summary>
        /// 현재 게임플레이 시간을 통계에 저장합니다.
        /// </summary>
        public void SaveGameplayTimeToStatistics()
        {
            Data.Game.VProfile profileInfo = GameApp.GetSelectedProfile();
            if (GameApp.Instance != null && profileInfo != null && profileInfo.Statistics != null && TotalGameplayTime >= 0)
            {
                profileInfo.Statistics.SaveCurrentGameplayTime(TotalGameplayTime);
                Log.Info(LogTags.Time, "(Manager) 게임플레이 시간을 저장했습니다: {0:F2}초", TotalGameplayTime);
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// 통계에서 저장된 게임플레이 시간을 로드합니다.
        /// </summary>
        /// <returns>로드 성공 여부</returns>
        private bool LoadGameplayTimeFromStatistics()
        {
            Data.Game.VProfile profileInfo = GameApp.GetSelectedProfile();
            if (profileInfo == null)
            {
                Log.Warning(LogTags.Time, "(Manager) GameData가 null이어서 시간 불러오기 실패");
                return false;
            }

            if (profileInfo.Statistics != null)
            {
                float savedTime = profileInfo.Statistics.GetCurrentGameplayTime();
                bool isValidSaved = profileInfo.Statistics.IsValidGameplayTimeSaved;

                if (isValidSaved)
                {
                    TotalGameplayTime = savedTime;
                    Log.Info(LogTags.Time, "(Manager) 저장된 게임플레이 시간을 로드했습니다: {0:F2}초", savedTime);
                    return true;
                }
                else
                {
                    Log.Info(LogTags.Time, "(Manager) 유효하지 않은 저장된 시간 - Time: {0:F2}, IsValid: {1}", savedTime, isValidSaved);
                }
            }
            else
            {
                Log.Warning(LogTags.Time, "(Manager) Statistics가 null이어서 시간 불러오기 실패");
            }

            return false;
        }

        #endregion Private Methods
    }
}