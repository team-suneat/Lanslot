using Sirenix.OdinInspector;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class StageSystem : MonoBehaviour
    {
        #region Public Properties

        public StageAssetData StageData => _stageAsset.Data ?? null;

        /// <summary>스테이지가 초기화되었는지 여부</summary>
        public bool IsInitialized { get; private set; }

        /// <summary>스테이지가 활성화되었는지 여부</summary>
        public bool IsActive { get; private set; }

        #endregion Public Properties

        #region Private Fields

        [SerializeField] private StageAsset _stageAsset;
        [SerializeField] private PlayerCharacterSpawner _playerSpawner;
        [SerializeField] private MonsterCharacterSpawner _monsterSpawner;
        [SerializeField] private WaveController _waveController;

        #endregion Private Fields

        #region Button Methods

        [Button]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void AutoGetComponents()
        {
            FindComponents();
        }

        #endregion Button Methods

        #region Public Methods

        /// <summary>
        /// 스테이지 시스템을 초기화합니다
        /// </summary>
        /// <param name="stageData">스테이지 데이터</param>
        public void Initialize()
        {
            if (_stageAsset == null)
            {
                Log.Error(LogTags.Stage, "스테이지 데이터가 null입니다.");
                return;
            }

            if (!_stageAsset.Data.HasValidWaves)
            {
                Log.Error(LogTags.Stage, "유효한 웨이브가 없는 스테이지입니다: {0}", _stageAsset.Data.Name);
                return;
            }

            IsInitialized = true;

            // 웨이브 컨트롤러 초기화
            if (_waveController != null)
            {
                _waveController.Initialize(_stageAsset.Data);
            }

            Log.Info(LogTags.Stage, "스테이지 시스템 초기화 완료: {0}, 웨이브 수: {1}", _stageAsset.Data.Name, _stageAsset.Data.WaveCount);
        }

        /// <summary>
        /// 스테이지를 시작합니다
        /// </summary>
        public void StartStage()
        {
            if (!IsInitialized)
            {
                Log.Warning(LogTags.Stage, "스테이지 시스템이 초기화되지 않았습니다.");
                return;
            }

            if (IsActive)
            {
                Log.Warning(LogTags.Stage, "스테이지가 이미 활성화되어 있습니다.");
                return;
            }

            IsActive = true;

            Log.Info(LogTags.Stage, "스테이지 시작: {0}", StageData.Name);

            if (_playerSpawner != null)
            {
                _playerSpawner.StartSpawnCharacter();
            }

            if (_waveController != null)
            {
                _waveController.StartWaves();
            }
        }

        /// <summary>
        /// 스테이지를 중지합니다
        /// </summary>
        public void StopStage()
        {
            if (!IsActive) return;

            IsActive = false;

            // 웨이브 중지
            if (_waveController != null)
            {
                _waveController.StopWaves();
            }

            Log.Info(LogTags.Stage, "스테이지 중지: {0}", StageData.Name.ToLogString() ?? "Unknown");
        }

        /// <summary>
        /// 스테이지를 정리합니다
        /// </summary>
        public void CleanupStage()
        {
            StopStage();
            IsInitialized = false;

            Log.Info(LogTags.Stage, "스테이지 정리 완료");
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// 자식 컴포넌트들을 찾습니다
        /// </summary>
        private void FindComponents()
        {
            // 플레이어 스포너 찾기 (StageSystem 직속)
            if (_playerSpawner == null)
            {
                _playerSpawner = GetComponentInChildren<PlayerCharacterSpawner>();
            }

            // 웨이브 컨트롤러 찾기 (StageSystem 직속)
            if (_waveController == null)
            {
                _waveController = GetComponentInChildren<WaveController>();
            }

            // 몬스터 스포너는 웨이브 컨트롤러의 자식이므로 웨이브 컨트롤러를 통해 접근
            if (_waveController != null && _monsterSpawner == null)
            {
                _monsterSpawner = _waveController.GetComponentInChildren<MonsterCharacterSpawner>();
            }

            // 컴포넌트 검증
            if (_playerSpawner == null)
            {
                Log.Warning(LogTags.Stage, "PlayerCharacterSpawner를 찾을 수 없습니다.");
            }

            if (_monsterSpawner == null)
            {
                Log.Warning(LogTags.Stage, "MonsterCharacterSpawner를 찾을 수 없습니다.");
            }

            if (_waveController == null)
            {
                Log.Warning(LogTags.Stage, "WaveController를 찾을 수 없습니다.");
            }
        }

        #endregion Private Methods

        #region Unity Lifecycle

        private void OnDestroy()
        {
            CleanupStage();
        }

        #endregion Unity Lifecycle
    }
}