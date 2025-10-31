using TeamSuneat;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 전투 상태와 전투 로직을 관리하는 매니저
    /// GameManager를 통해 접근하며, 직접 접근은 권장하지 않습니다.
    /// </summary>
    public class BattleManager
    {
        #region 전투 상태

        public bool _autoStartBattle = true;
        public float _battleStartDelay = 1f;

        public enum BattleState
        {
            None,           // 전투 없음
            Preparing,      // 전투 준비 중
            InProgress,     // 전투 진행 중
            Paused,         // 전투 일시정지
            Ended           // 전투 종료
        }

        private BattleState _currentBattleState = BattleState.None;
        public BattleState CurrentBattleState => _currentBattleState;

        private float m_battleStartTimer = 0f;

        #endregion 전투 상태

        #region 이벤트

        public System.Action<BattleState> OnBattleStateChanged;
        public System.Action OnBattleStarted;
        public System.Action OnBattleEnded;
        public System.Action OnBattlePaused;
        public System.Action OnBattleResumed;

        #endregion 이벤트

        #region 초기화

        /// <summary>
        /// BattleManager를 초기화합니다.
        /// </summary>
        public void Initialize()
        {
            Log.Info(LogTags.Battle, "BattleManager 초기화 시작");

            // WorldClock이 없으면 생성
            EnsureWorldClock();

            Log.Info(LogTags.Battle, "BattleManager 초기화 완료");
        }

        /// <summary>
        /// BattleManager를 업데이트합니다.
        /// </summary>
        public void LogicUpdate()
        {
            UpdateBattleStartTimer();
        }

        #endregion 초기화

        #region 내부 메서드

        private void EnsureWorldClock()
        {
            if (WorldClock.GameInstance == null)
            {
                var clockGO = new GameObject("WorldClock");
                var clock = clockGO.AddComponent<WorldClock>();
                clock.TicksPerSecond = 20f;
                clock.TimeScale = 1;
                clock.IsPaused = false;

                Log.Info(LogTags.Battle, "WorldClock 자동 생성됨");
            }
        }

        /// <summary>
        /// 전투 시작 타이머를 업데이트합니다.
        /// </summary>
        private void UpdateBattleStartTimer()
        {
            if (m_battleStartTimer > 0f)
            {
                m_battleStartTimer -= Time.deltaTime;
                if (m_battleStartTimer <= 0f)
                {
                    ExecuteBattleStart();
                }
            }
        }

        #endregion 내부 메서드

        #region 전투 관리

        /// <summary>
        /// 전투를 시작합니다.
        /// </summary>
        public void StartBattle()
        {
            if (_currentBattleState == BattleState.InProgress)
            {
                Log.Warning(LogTags.Battle, "이미 전투가 진행 중입니다.");
                return;
            }

            Log.Info(LogTags.Battle, "전투 시작 준비");
            ChangeBattleState(BattleState.Preparing);

            // 지연 후 전투 시작
            m_battleStartTimer = _battleStartDelay;
        }

        /// <summary>
        /// 전투 시작을 실행합니다.
        /// </summary>
        private void ExecuteBattleStart()
        {
            ChangeBattleState(BattleState.InProgress);
            OnBattleStarted?.Invoke();

            Log.Info(LogTags.Battle, "전투 시작!");
        }

        /// <summary>
        /// 전투를 종료합니다.
        /// </summary>
        public void EndBattle()
        {
            if (_currentBattleState != BattleState.InProgress)
            {
                Log.Warning(LogTags.Battle, "현재 전투가 진행 중이 아닙니다.");
                return;
            }

            ChangeBattleState(BattleState.Ended);
            OnBattleEnded?.Invoke();

            Log.Info(LogTags.Battle, "전투 종료");
        }

        /// <summary>
        /// 전투를 일시정지합니다.
        /// </summary>
        public void PauseBattle()
        {
            if (_currentBattleState != BattleState.InProgress)
            {
                Log.Warning(LogTags.Battle, "전투가 진행 중이 아닙니다.");
                return;
            }

            ChangeBattleState(BattleState.Paused);
            OnBattlePaused?.Invoke();

            // WorldClock 일시정지
            if (WorldClock.GameInstance != null)
            {
                WorldClock.GameInstance.IsPaused = true;
            }

            Log.Info(LogTags.Battle, "전투 일시정지");
        }

        /// <summary>
        /// 전투를 재개합니다.
        /// </summary>
        public void ResumeBattle()
        {
            if (_currentBattleState != BattleState.Paused)
            {
                Log.Warning(LogTags.Battle, "전투가 일시정지 상태가 아닙니다.");
                return;
            }

            ChangeBattleState(BattleState.InProgress);
            OnBattleResumed?.Invoke();

            // WorldClock 재개
            if (WorldClock.GameInstance != null)
            {
                WorldClock.GameInstance.IsPaused = false;
            }

            Log.Info(LogTags.Battle, "전투 재개");
        }

        /// <summary>
        /// 전투 상태를 변경합니다.
        /// </summary>
        private void ChangeBattleState(BattleState newState)
        {
            if (_currentBattleState == newState) return;

            var oldState = _currentBattleState;
            _currentBattleState = newState;

            OnBattleStateChanged?.Invoke(newState);

            Log.Info(LogTags.Battle, $"전투 상태 변경: {oldState} -> {newState}");
        }

        #endregion 전투 관리

        #region 유틸리티

        /// <summary>
        /// 전투가 활성화되어 있는지 확인합니다.
        /// </summary>
        public bool IsBattleActive => _currentBattleState == BattleState.InProgress;

        /// <summary>
        /// 전투가 일시정지되어 있는지 확인합니다.
        /// </summary>
        public bool IsBattlePaused => _currentBattleState == BattleState.Paused;

        /// <summary>
        /// 전투가 종료되었는지 확인합니다.
        /// </summary>
        public bool IsBattleEnded => _currentBattleState == BattleState.Ended;

        /// <summary>
        /// 전투가 진행 중인지 확인합니다.
        /// </summary>
        public bool IsBattleInProgress => _currentBattleState == BattleState.InProgress;

        #endregion 유틸리티
    }
}