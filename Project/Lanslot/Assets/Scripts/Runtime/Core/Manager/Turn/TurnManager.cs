using UnityEngine.Events;

namespace TeamSuneat
{
    /// <summary>
    /// 턴제 전투 시스템의 턴을 관리하는 싱글톤 매니저
    /// </summary>
    public class TurnManager : Singleton<TurnManager>
    {
        #region Public Properties

        /// <summary>현재 턴 상태</summary>
        public TurnState CurrentState { get; private set; } = TurnState.None;

        /// <summary>현재 턴 번호 (1부터 시작)</summary>
        public int CurrentTurnNumber { get; private set; } = 0;

        /// <summary>게임이 초기화되었는지 여부</summary>
        public bool IsInitialized { get; private set; } = false;

        /// <summary>게임이 종료되었는지 여부</summary>
        public bool IsGameEnded { get; private set; } = false;

        #endregion Public Properties

        #region Events

        /// <summary>플레이어 턴 시작 이벤트</summary>
        public UnityEvent OnPlayerTurnStart = new();

        /// <summary>플레이어 턴 종료 이벤트</summary>
        public UnityEvent OnPlayerTurnEnd = new();

        /// <summary>보상 턴 시작 이벤트</summary>
        public UnityEvent OnRewardTurnStart = new();

        /// <summary>보상 턴 종료 이벤트</summary>
        public UnityEvent OnRewardTurnEnd = new();

        /// <summary>몬스터 턴 시작 이벤트</summary>
        public UnityEvent OnMonsterTurnStart = new();

        /// <summary>몬스터 턴 종료 이벤트</summary>
        public UnityEvent OnMonsterTurnEnd = new();

        /// <summary>게임 클리어 이벤트</summary>
        public UnityEvent OnGameClear = new();

        /// <summary>게임 오버 이벤트</summary>
        public UnityEvent OnGameOver = new();

        #endregion Events

        #region Initialization

        /// <summary>
        /// 턴 매니저를 초기화합니다.
        /// </summary>
        public void Initialize()
        {
            if (IsInitialized)
            {
                Log.Warning(LogTags.Turn, "턴 매니저가 이미 초기화되었습니다.");
                return;
            }

            CurrentState = TurnState.None;
            CurrentTurnNumber = 0;
            IsGameEnded = false;
            IsInitialized = true;

            Log.Info(LogTags.Turn, "턴 매니저 초기화 완료");
        }

        /// <summary>
        /// 턴 매니저를 리셋합니다.
        /// </summary>
        public void Reset()
        {
            CurrentState = TurnState.None;
            CurrentTurnNumber = 0;
            IsGameEnded = false;
            IsInitialized = false;

            // 이벤트 구독 해제
            OnPlayerTurnStart = null;
            OnPlayerTurnEnd = null;
            OnRewardTurnStart = null;
            OnRewardTurnEnd = null;
            OnMonsterTurnStart = null;
            OnMonsterTurnEnd = null;
            OnGameClear = null;
            OnGameOver = null;

            Log.Info(LogTags.Turn, "턴 매니저 리셋 완료");
        }

        #endregion Initialization

        #region Player Turn

        /// <summary>
        /// 플레이어 턴을 시작합니다.
        /// </summary>
        public void StartPlayerTurn()
        {
            if (!IsInitialized)
            {
                Log.Warning(LogTags.Turn, "턴 매니저가 초기화되지 않았습니다.");
                return;
            }

            if (IsGameEnded)
            {
                Log.Warning(LogTags.Turn, "게임이 종료되어 플레이어 턴을 시작할 수 없습니다.");
                return;
            }

            CurrentState = TurnState.PlayerTurn;
            CurrentTurnNumber++;

            Log.Info(LogTags.Turn, "플레이어 턴 시작: Turn {0}", CurrentTurnNumber);
            OnPlayerTurnStart?.Invoke();
        }

        /// <summary>
        /// 플레이어 턴을 종료합니다.
        /// </summary>
        public void EndPlayerTurn()
        {
            if (CurrentState != TurnState.PlayerTurn)
            {
                Log.Warning(LogTags.Turn, "플레이어 턴이 아닌 상태에서 플레이어 턴 종료를 시도했습니다. 현재 상태: {0}", CurrentState);
                return;
            }

            Log.Info(LogTags.Turn, "플레이어 턴 종료: Turn {0}", CurrentTurnNumber);
            OnPlayerTurnEnd?.Invoke();

            // 게임 종료 조건 확인
            if (CheckGameEndConditions())
            {
                return;
            }
        }

        #endregion Player Turn

        #region Reward Turn

        /// <summary>
        /// 보상 턴을 시작합니다.
        /// </summary>
        public void StartRewardTurn()
        {
            if (!IsInitialized)
            {
                Log.Warning(LogTags.Turn, "턴 매니저가 초기화되지 않았습니다.");
                return;
            }

            if (IsGameEnded)
            {
                Log.Warning(LogTags.Turn, "게임이 종료되어 보상 턴을 시작할 수 없습니다.");
                return;
            }

            CurrentState = TurnState.RewardTurn;

            Log.Info(LogTags.Turn, "보상 턴 시작: Turn {0}", CurrentTurnNumber);
            OnRewardTurnStart?.Invoke();
        }

        /// <summary>
        /// 보상 턴을 종료합니다.
        /// </summary>
        public void EndRewardTurn()
        {
            if (CurrentState != TurnState.RewardTurn)
            {
                Log.Warning(LogTags.Turn, "보상 턴이 아닌 상태에서 보상 턴 종료를 시도했습니다. 현재 상태: {0}", CurrentState);
                return;
            }

            Log.Info(LogTags.Turn, "보상 턴 종료: Turn {0}", CurrentTurnNumber);
            OnRewardTurnEnd?.Invoke();
        }

        #endregion Reward Turn

        #region Monster Turn

        /// <summary>
        /// 몬스터 턴을 시작합니다.
        /// </summary>
        public void StartMonsterTurn()
        {
            if (!IsInitialized)
            {
                Log.Warning(LogTags.Turn, "턴 매니저가 초기화되지 않았습니다.");
                return;
            }

            if (IsGameEnded)
            {
                Log.Warning(LogTags.Turn, "게임이 종료되어 몬스터 턴을 시작할 수 없습니다.");
                return;
            }

            CurrentState = TurnState.MonsterTurn;

            Log.Info(LogTags.Turn, "몬스터 턴 시작: Turn {0}", CurrentTurnNumber);
            OnMonsterTurnStart?.Invoke();
        }

        /// <summary>
        /// 몬스터 턴을 종료합니다.
        /// </summary>
        public void EndMonsterTurn()
        {
            if (CurrentState != TurnState.MonsterTurn)
            {
                Log.Warning(LogTags.Turn, "몬스터 턴이 아닌 상태에서 몬스터 턴 종료를 시도했습니다. 현재 상태: {0}", CurrentState);
                return;
            }

            Log.Info(LogTags.Turn, "몬스터 턴 종료: Turn {0}", CurrentTurnNumber);
            OnMonsterTurnEnd?.Invoke();

            // 게임 종료 조건 확인
            if (CheckGameEndConditions())
            {
                return;
            }
        }

        #endregion Monster Turn

        #region Game End Conditions

        /// <summary>
        /// 게임 클리어/오버 조건을 확인합니다.
        /// </summary>
        /// <returns>게임이 종료되었는지 여부</returns>
        public bool CheckGameEndConditions()
        {
            if (IsGameEnded)
            {
                return true;
            }

            // 게임 오버 확인 (플레이어 사망)
            if (CheckGameOver())
            {
                return true;
            }

            // 게임 클리어 확인 (모든 몬스터 처치 또는 보스 처치)
            if (CheckGameClear())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 게임 오버 조건을 확인합니다.
        /// </summary>
        /// <returns>게임 오버 여부</returns>
        private bool CheckGameOver()
        {
            PlayerCharacter player = CharacterManager.Instance?.Player;

            if (player == null)
            {
                return false;
            }

            // 플레이어가 사망했는지 확인
            if (!player.IsAlive)
            {
                IsGameEnded = true;
                CurrentState = TurnState.GameEnd;

                Log.Info(LogTags.Turn, "게임 오버: 플레이어 사망");
                OnGameOver?.Invoke();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 게임 클리어 조건을 확인합니다.
        /// </summary>
        /// <returns>게임 클리어 여부</returns>
        private bool CheckGameClear()
        {
            CharacterManager characterManager = CharacterManager.Instance;

            if (characterManager == null)
            {
                return false;
            }

            // 모든 몬스터가 처치되었는지 확인
            if (characterManager.MonsterCount <= 0)
            {
                IsGameEnded = true;
                CurrentState = TurnState.GameEnd;

                Log.Info(LogTags.Turn, "게임 클리어: 모든 몬스터 처치");
                OnGameClear?.Invoke();
                return true;
            }

            return false;
        }

        #endregion Game End Conditions
    }
}