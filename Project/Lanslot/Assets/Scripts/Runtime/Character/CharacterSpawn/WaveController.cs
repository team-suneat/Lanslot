using System;
using System.Collections;
using TeamSuneat.Data;
using TeamSuneat.UserInterface;
using UnityEngine;

namespace TeamSuneat
{
    public class WaveController : MonoBehaviour
    {
        #region Events

        /// <summary>웨이브가 시작될 때 발생하는 이벤트</summary>
        public static event Action<int, WaveData> OnWaveStarted;

        /// <summary>웨이브가 종료될 때 발생하는 이벤트</summary>
        public static event Action<int> OnWaveEnded;

        /// <summary>모든 웨이브가 완료될 때 발생하는 이벤트</summary>
        public static event Action OnAllWavesCompleted;

        #endregion Events

        #region Public Properties

        /// <summary>현재 웨이브 인덱스</summary>
        public int CurrentWaveIndex { get; private set; } = -1;

        /// <summary>현재 웨이브 데이터</summary>
        public WaveData CurrentWave { get; private set; }

        /// <summary>웨이브가 진행 중인지 여부</summary>
        public bool IsWaveActive { get; private set; }

        /// <summary>모든 웨이브가 완료되었는지 여부</summary>
        public bool IsAllWavesCompleted { get; private set; }

        /// <summary>현재 웨이브 시작 시간</summary>
        public float CurrentWaveStartTime { get; private set; }

        /// <summary>현재 웨이브 경과 시간</summary>
        public float CurrentWaveElapsedTime => IsWaveActive ? Time.time - CurrentWaveStartTime : 0f;

        #endregion Public Properties

        #region Private Fields

        private StageAssetData _stageData;
        private Coroutine _waveCoroutine;
        private bool _isInitialized = false;

        private float _elapsedTimeTotal = 0;
        private float _elapsedTimePerWave = 0;

        [Header("스포너 참조")]
        [SerializeField] private MonsterCharacterSpawner _monsterSpawner;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// 웨이브 컨트롤러를 초기화합니다
        /// </summary>
        /// <param name="stageData">스테이지 데이터</param>
        public void Initialize(StageAssetData stageData)
        {
            if (stageData == null)
            {
                Log.Error(LogTags.CharacterWave, "스테이지 데이터가 null입니다.");
                return;
            }

            if (!stageData.HasValidWaves)
            {
                Log.Error(LogTags.CharacterWave, "유효한 웨이브가 없습니다: {0}", stageData.Name);
                return;
            }

            // 몬스터 스포너 자동 찾기
            if (_monsterSpawner == null)
            {
                _monsterSpawner = GetComponentInChildren<MonsterCharacterSpawner>();
            }

            if (_monsterSpawner == null)
            {
                Log.Warning(LogTags.CharacterWave, "MonsterCharacterSpawner를 찾을 수 없습니다.");
            }

            _stageData = stageData;
            CurrentWaveIndex = -1;
            IsAllWavesCompleted = false;
            _isInitialized = true;
            _elapsedTimePerWave = 0;
            _elapsedTimeTotal = 0;

            Log.Info(LogTags.CharacterWave, "웨이브 컨트롤러 초기화 완료: {0}, 웨이브 수: {1}",
                stageData.Name, stageData.WaveCount);
        }

        /// <summary>
        /// 웨이브를 시작합니다
        /// </summary>
        public void StartWaves()
        {
            if (!_isInitialized)
            {
                Log.Warning(LogTags.CharacterWave, "웨이브 컨트롤러가 초기화되지 않았습니다.");
                return;
            }

            if (_stageData == null)
            {
                Log.Error(LogTags.CharacterWave, "스테이지 데이터가 설정되지 않았습니다.");
                return;
            }

            Log.Info(LogTags.CharacterWave, "웨이브 시작: {0}", _stageData.Name);
            StartNextWave();
        }

        /// <summary>
        /// 웨이브를 중지합니다
        /// </summary>
        public void StopWaves()
        {
            if (_waveCoroutine != null)
            {
                StopCoroutine(_waveCoroutine);
                _waveCoroutine = null;
            }

            if (IsWaveActive)
            {
                EndCurrentWave();
            }

            Log.Info(LogTags.CharacterWave, "웨이브 중지됨");
        }

        /// <summary>
        /// 현재 웨이브를 강제로 종료합니다
        /// </summary>
        public void ForceEndCurrentWave()
        {
            if (!IsWaveActive) return;

            Log.Info(LogTags.CharacterWave, "현재 웨이브를 강제 종료합니다: {0}", CurrentWaveIndex);
            EndCurrentWave();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// 다음 웨이브를 시작합니다
        /// </summary>
        private void StartNextWave()
        {
            CurrentWaveIndex++;

            // 모든 웨이브가 완료된 경우
            if (CurrentWaveIndex >= _stageData.WaveCount)
            {
                CompleteAllWaves();
                return;
            }

            // 현재 웨이브 데이터 가져오기
            CurrentWave = _stageData.GetWave(CurrentWaveIndex);
            if (CurrentWave == null)
            {
                Log.Error(LogTags.CharacterWave, "웨이브 데이터를 가져올 수 없습니다: 인덱스 {0}", CurrentWaveIndex);
                return;
            }

            // 웨이브 시작
            StartWave(CurrentWave);
        }

        /// <summary>
        /// 웨이브를 시작합니다
        /// </summary>
        private void StartWave(WaveData waveData)
        {
            IsWaveActive = true;
            CurrentWaveStartTime = Time.time;

            Log.Info(LogTags.CharacterWave, "웨이브 시작: {0} (인덱스: {1}, 지속시간: {2}초)",
                waveData.WaveName, CurrentWaveIndex, waveData.WaveDuration);

            // 몬스터 스포너에 웨이브 데이터 설정
            if (_monsterSpawner != null)
            {
                _monsterSpawner.SetCurrentWave(waveData);
            }

            OnWaveStarted?.Invoke(CurrentWaveIndex, waveData);

            // 웨이브 지속 시간 후 자동 종료
            _waveCoroutine = StartCoroutine(WaveTimerCoroutine());
        }

        /// <summary>
        /// 현재 웨이브를 종료합니다
        /// </summary>
        private void EndCurrentWave()
        {
            if (!IsWaveActive) return;

            IsWaveActive = false;
            _elapsedTimePerWave = 0;

            Log.Info(LogTags.CharacterWave, "웨이브 종료: {0} (인덱스: {1}, 경과시간: {2:F1}초)",
                CurrentWave.WaveName, CurrentWaveIndex, CurrentWaveElapsedTime);

            // 몬스터 스포너에 웨이브 종료 알림
            if (_monsterSpawner != null)
            {
                _monsterSpawner.OnWaveEnded();
            }

            OnWaveEnded?.Invoke(CurrentWaveIndex);

            // 다음 웨이브 시작 (모든 웨이브가 완료되지 않은 경우)
            if (!IsAllWavesCompleted)
            {
                StartNextWave();
            }
        }

        /// <summary>
        /// 모든 웨이브 완료 처리
        /// </summary>
        private void CompleteAllWaves()
        {
            IsAllWavesCompleted = true;
            IsWaveActive = false;

            // 몬스터 스포너에 모든 웨이브 완료 알림
            if (_monsterSpawner != null)
            {
                _monsterSpawner.OnAllWavesCompleted();
            }

            Log.Info(LogTags.CharacterWave, "모든 웨이브가 완료되었습니다: {0}", _stageData.Name);

            OnAllWavesCompleted?.Invoke();
        }

        /// <summary>
        /// 웨이브 타이머 코루틴
        /// </summary>
        private IEnumerator WaveTimerCoroutine()
        {
            while (CurrentWaveElapsedTime < CurrentWave.WaveDuration)
            {
                yield return new WaitForSeconds(1);
                _elapsedTimeTotal += 1;
                UIManager.Instance.Timer.UpdateTimeText(_elapsedTimeTotal);
            }

            EndCurrentWave();
        }

        #endregion Private Methods

        #region Unity Lifecycle

        private void OnDestroy()
        {
            IsWaveActive = false;
            StopWaves();
        }

        #endregion Unity Lifecycle
    }
}