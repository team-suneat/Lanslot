namespace TeamSuneat
{
    /// <summary>
    /// 게임 시간 추적 상태를 나타내는 열거형
    /// </summary>
    public enum GameTimeState
    {
        /// <summary>시간 추적하지 않음 (초기 상태, 마을, 부활 지역 등)</summary>
        None = 0,

        /// <summary>일반 게임플레이 시간 추적 중</summary>
        Tracking = 1,

        /// <summary>전투 시간 추적 중</summary>
        InCombat = 2
    }

    /// <summary>
    /// 게임 시간 추적을 관리하는 싱글톤 클래스
    /// 스테이지 타입과 진행 상태에 따라 자동으로 시간을 기록하거나 중지합니다.
    /// </summary>
    public partial class GameTimeManager : Singleton<GameTimeManager>
    {
        #region Public Properties

        /// <summary>현재 시간 추적 상태</summary>
        public GameTimeState CurrentState { get; private set; } = GameTimeState.None;

        /// <summary>총 게임플레이 시간 (초)</summary>
        public float TotalGameplayTime { get; private set; } = 0f;

        /// <summary>일시정지 상태 여부</summary>
        public bool IsPaused { get; private set; } = false;

        /// <summary>현재 시간을 추적 중인지 여부 (일시정지가 아니고 추적 가능한 상태)</summary>
        public bool IsTracking => !IsPaused && (CurrentState == GameTimeState.Tracking || CurrentState == GameTimeState.InCombat);

        #endregion Public Properties

        #region Private Fields

        private float _lastLogTime = 0f;
        private const float LOG_INTERVAL = 1f;

        #endregion Private Fields
    }
}