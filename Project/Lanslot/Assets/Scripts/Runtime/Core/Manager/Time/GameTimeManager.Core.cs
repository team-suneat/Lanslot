using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// GameTimeManager의 핵심 시간 추적 기능을 담당하는 파셜 클래스
    /// </summary>
    public partial class GameTimeManager
    {
        #region Public Methods

        /// <summary>
        /// 게임플레이 시간 추적을 시작합니다.
        /// </summary>
        public void StartGameplayTracking()
        {
            SetState(GameTimeState.Tracking);
            Log.Info(LogTags.Time, "(Manager) 게임플레이 시간 추적을 시작합니다. 현재 시간: {0:F2}초", TotalGameplayTime);
        }

        /// <summary>
        /// 게임 시작 시 시간 추적을 초기화합니다.
        /// </summary>
        /// <param name="isChallengeStarted">도전이 시작되었는지 여부</param>
        public void InitializeGameplayTracking(bool isChallengeStarted)
        {
            if (isChallengeStarted)
            {
                if (LoadGameplayTimeFromStatistics())
                {
                    SetState(GameTimeState.Tracking);
                    Log.Info(LogTags.Time, "(Manager) 게임 재시작 시 게임플레이 시간 추적을 복원합니다. 복원된 시간: {0:F2}초", TotalGameplayTime);
                    return;
                }
            }

            TotalGameplayTime = -1f;
            SetState(GameTimeState.None);
            Log.Warning(LogTags.Time, "(Manager) 도전이 시작되지 않았으므로 게임플레이 시간 추적을 시작하지 않습니다.");
        }

        /// <summary>
        /// 게임플레이 시간 추적을 중지하고 현재 시간을 저장합니다.
        /// </summary>
        public void StopGameplayTracking()
        {
            SaveGameplayTimeToStatistics();
            SetState(GameTimeState.None);
            Log.Info(LogTags.Time, "(Manager) 게임플레이 시간 추적을 중지합니다. 총 시간: {0:F2}초", TotalGameplayTime);
        }

        /// <summary>
        /// 게임플레이 시간을 리셋하고 추적을 중지합니다.
        /// </summary>
        public void ResetGameplayTime()
        {
            TotalGameplayTime = -1f;
            SetState(GameTimeState.None);
            Log.Info(LogTags.Time, "(Manager) 게임플레이 시간을 리셋합니다.");
        }

        /// <summary>
        /// 매 프레임 호출되는 시간 추적 업데이트 메서드
        /// </summary>
        public void UpdateTimeTracking()
        {
            CheckPauseState();

            if (IsTracking && TotalGameplayTime >= 0)
            {
                float deltaTime = Time.deltaTime;
                TotalGameplayTime += deltaTime;
                UpdateLogTimeTracking();
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// 시간이 초기화되지 않았다면 0으로 초기화합니다.
        /// </summary>
        private void InitializeTimeIfNeeded()
        {
            if (TotalGameplayTime < 0)
            {
                TotalGameplayTime = 0f;
                Log.Info(LogTags.Time, "(Manager) 게임플레이 시간을 0으로 초기화했습니다.");
            }
        }

        /// <summary>
        /// 디버깅용 시간 추적 로그를 업데이트합니다.
        /// </summary>
        private void UpdateLogTimeTracking()
        {
#if UNITY_EDITOR
            if (CurrentState == GameTimeState.InCombat && Time.time - _lastLogTime > LOG_INTERVAL)
            {
                Log.Info(LogTags.Time, "(Manager) 전투 시간 증가: {0:F2}초", TotalGameplayTime);
                _lastLogTime = Time.time;
            }
#endif
        }

        #endregion Private Methods
    }
}