namespace TeamSuneat
{
    /// <summary>
    /// GameTimeManager의 상태 관리 기능을 담당하는 파셜 클래스
    /// </summary>
    public partial class GameTimeManager
    {
        #region Public Methods

        /// <summary>
        /// 전투 상태를 설정합니다.
        /// </summary>
        /// <param name="inCombat">전투 중인지 여부</param>
        public void SetCombatState(bool inCombat)
        {
            GameTimeState newState = inCombat ? GameTimeState.InCombat : GameTimeState.Tracking;
            SetState(newState);
        }

        /// <summary>
        /// 시간 추적 상태를 변경합니다.
        /// </summary>
        /// <param name="newState">새로운 상태</param>
        public void SetState(GameTimeState newState)
        {
            if (CurrentState == newState)
            {
                return;
            }

            GameTimeState previousState = CurrentState;
            CurrentState = newState;

            if (newState != GameTimeState.None)
            {
                IsPaused = false;
            }

            OnStateChanged(previousState, newState);
            Log.Info(LogTags.Time, "(Manager) 시간 추적 상태 변경: {0} → {1}", previousState, newState);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// 상태 변경 시 호출되는 이벤트 핸들러
        /// </summary>
        private void OnStateChanged(GameTimeState from, GameTimeState to)
        {
            switch (to)
            {
                case GameTimeState.Tracking:
                    InitializeTimeIfNeeded();
                    break;

                case GameTimeState.InCombat:
                    InitializeTimeIfNeeded();
                    SaveGameplayTimeToStatistics();
                    break;
            }
        }

        #endregion Private Methods
    }
}