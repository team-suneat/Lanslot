using System;
using System.Globalization;

namespace TeamSuneat.Data
{
    public partial class VStatistics
    {
        #region Get

        /// <summary>
        /// 총 플레이 시간을 반환합니다.
        /// 게임 시작 시간과 마지막으로 저장한 시간의 차이를 계산합니다.
        /// </summary>
        /// <returns>총 플레이 시간</returns>
        public TimeSpan GetTotalPlayTime()
        {
            TimeSpan totalPlayTime = LastSaveTime - GameStartTime;
            return totalPlayTime;
        }

        /// <summary>
        /// 마지막 저장 시간을 문자열로 반환합니다.
        /// </summary>
        /// <returns>마지막 저장 시간 문자열</returns>
        public string GetLastSaveTimeString()
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            return GameStartTime.ToString("yyyy/MM/dd HH:mm:ss", culture);
        }

        /// <summary>
        /// 총 플레이 시간을 문자열로 반환합니다.
        /// </summary>
        /// <returns>총 플레이 시간 문자열</returns>
        public string GetTotalPlayTimeString()
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            return GetTotalPlayTime().ToString(@"hh\:mm", culture);
        }

        /// <summary>
        /// 현재 진행 중인 도전의 실제 게임플레이 시간을 반환합니다.
        /// </summary>
        /// <returns>도전 시간</returns>
        public TimeSpan GetChallengeTime()
        {
            return TimeSpan.FromSeconds(GameTimeManager.Instance.TotalGameplayTime);
        }

        /// <summary>
        /// 도전 시간을 문자열로 반환합니다.
        /// </summary>
        /// <returns>도전 시간 문자열</returns>
        public string GetChallengeTimeString()
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            return GetChallengeTime().ToString(@"hh\:mm", culture);
        }

        /// <summary>
        /// 스피드런 클리어 시간을 문자열로 반환합니다.
        /// </summary>
        /// <returns>스피드런 클리어 시간 문자열 또는 "-"</returns>
        public string GetSpeedrunClearTimeString()
        {
            if (SpeedrunClearTime == TimeSpan.Zero)
            {
                return "-";
            }

            CultureInfo culture = CultureInfo.InvariantCulture;
            return SpeedrunClearTime.ToString(@"hh\:mm", culture);
        }

        #endregion Get

        /// <summary>
        /// 게임 시작 시간을 등록합니다.
        /// </summary>
        public void RegisterGameStartTime()
        {
            GameStartTime = DateTime.Now;
        }

        /// <summary>
        /// 마지막 저장 시간을 등록합니다.
        /// </summary>
        public void RegisterLastSaveTime()
        {
            LastSaveTime = DateTime.Now;
        }

        //

        /// <summary>
        /// 도전 시작 시간을 등록합니다.
        /// </summary>
        public void RegisterChallengeStartTime()
        {
            ChallengeStartTime = DateTime.Now;
            GameTimeManager.Instance.StartGameplayTracking();
            Log.Info(LogTags.GameData_Statistics, "(Time) 도전 시작 시간을 기록합니다.");
        }

        /// <summary>
        /// 도전이 시작되었는지 확인합니다.
        /// </summary>
        /// <returns>도전 시작 여부</returns>
        public bool IsChallengeStarted()
        {
            return ChallengeStartTime != default(DateTime);
        }

        //

        /// <summary>
        /// 게임 클리어 시간을 등록합니다.
        /// </summary>
        /// <param name="totalStepLevel">총 단계 레벨</param>
        public void RegisterGameClearTime(int totalStepLevel)
        {
            TimeSpan clearTime = TimeSpan.FromSeconds(GameTimeManager.Instance.TotalGameplayTime);
            if (!DifficultyStepClearTimes.ContainsKey(totalStepLevel) || DifficultyStepClearTimes[totalStepLevel] > clearTime)
            {
                DifficultyStepClearTimes[totalStepLevel] = clearTime;
                Log.Info(LogTags.GameData_Statistics, $"(Time) 악몽 난이도 단계 {totalStepLevel} 스피드런 기록 갱신: {clearTime:hh\\:mm\\:ss}");
            }

            GameTimeManager.Instance.StopGameplayTracking();
            GameTimeManager.Instance.ResetGameplayTime();
        }

        /// <summary>
        /// 현재 게임플레이 시간을 저장합니다.
        /// </summary>
        /// <param name="gameplayTime">게임플레이 시간</param>
        public void SaveCurrentGameplayTime(float gameplayTime)
        {
            CurrentGameplayTime = gameplayTime;
            Log.Info(LogTags.GameData_Statistics, $"(Time) 현재 게임플레이 시간을 {gameplayTime:F2}초로 저장합니다.");
        }

        /// <summary>
        /// 현재 게임플레이 시간을 반환합니다.
        /// </summary>
        /// <returns>현재 게임플레이 시간</returns>
        public float GetCurrentGameplayTime()
        {
            return CurrentGameplayTime;
        }

        /// <summary>
        /// 현재 게임플레이 시간을 초기화합니다.
        /// </summary>
        public void ResetCurrentGameplayTime()
        {
            CurrentGameplayTime = 0f;
            IsValidGameplayTimeSaved = false;
            Log.Info(LogTags.GameData_Statistics, "(Time) 현재 게임플레이 시간과 유효 저장 플래그를 초기화합니다.");
        }

        //

        /// <summary>
        /// 게임플레이 시간 저장 유효성을 설정합니다.
        /// 클리프샤이어 지역에서만 true로 설정할 수 있습니다.
        /// </summary>
        /// <param name="isValid">유효성 여부</param>
        public void SetGameplayTimeSaveValidity(bool isValid)
        {
            IsValidGameplayTimeSaved = isValid;
            Log.Info(LogTags.GameData_Statistics, "(Time) 게임플레이 시간 저장 유효성을 {0}로 설정합니다.", isValid);
        }

        internal int FindStack(BuffNames name)
        {
            return 0;
        }

        internal bool Contains(BuffNames buffName)
        {
            return false;
        }

        internal void AddChallengeCount()
        {
            
        }
    }
}