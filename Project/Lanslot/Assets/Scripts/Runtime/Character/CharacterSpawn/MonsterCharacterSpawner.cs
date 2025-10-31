using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 몬스터를 생성하는 스포너 클래스입니다.
    /// WaveController와 연동하여 플레이어 주변에 일정 간격으로 몬스터를 생성합니다.
    /// </summary>
    public class MonsterCharacterSpawner : MonoBehaviour
    {
        [Title("디버그")]
        [SerializeField] private bool _showSpawnRadius = true;

        private float _lastSpawnTime;
        private PlayerCharacter _player;
        private WaveData _currentWave;

        private void OnValidate()
        {
            _currentWave?.Validate();
        }

        [Button]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void AutoSetting()
        {
            _currentWave?.Refresh();
        }

        private void Start()
        {
            StartCoroutine(InitializeSpawner());
        }

        private void OnEnable()
        {
            // 웨이브 컨트롤러 이벤트 구독
            WaveController.OnWaveStarted += OnWaveStarted;
            WaveController.OnWaveEnded += OnWaveEnded;
            WaveController.OnAllWavesCompleted += OnAllWavesCompleted;
        }

        private void OnDisable()
        {
            // 웨이브 컨트롤러 이벤트 구독 해제
            WaveController.OnWaveStarted -= OnWaveStarted;
            WaveController.OnWaveEnded -= OnWaveEnded;
            WaveController.OnAllWavesCompleted -= OnAllWavesCompleted;
        }

        private IEnumerator InitializeSpawner()
        {
            // 플레이어 캐릭터가 생성할 때까지 대기
            yield return new WaitUntil(() => CharacterManager.Instance.Player != null);

            // 플레이어 참조 가져오기
            _player = CharacterManager.Instance.Player;

            // 웨이브 데이터 초기화
            InitializeWaveData();

            Log.Info(LogTags.CharacterSpawn, "몬스터 스포너가 초기화되었습니다.");
        }

        /// <summary>
        /// 웨이브 데이터를 초기화합니다
        /// </summary>
        private void InitializeWaveData()
        {
            if (_currentWave != null)
            {
                Log.Info(LogTags.CharacterSpawn, "웨이브 데이터 설정: {0}, 스폰 간격: {1}초, 최대 몬스터 수: {2}",
                    _currentWave.WaveName, _currentWave.SpawnInterval, _currentWave.MaxMonsters);
            }
            else
            {
                Log.Warning(LogTags.CharacterSpawn, "웨이브 데이터가 설정되지 않았습니다.");
            }
        }

        private void Update()
        {
            if (_player == null) return;
            if (_currentWave == null) return;
            if (CharacterManager.Instance.MonsterCount >= _currentWave.MaxMonsters) return;

            if (Time.time - _lastSpawnTime >= _currentWave.SpawnInterval)
            {
                SpawnMonster();
                _lastSpawnTime = Time.time;
            }
        }

        private async void SpawnMonster()
        {
            Vector3 spawnPos = GetRandomSpawnPosition();
            CharacterNames monsterType = _currentWave.GetRandomMonsterType();
            string monsterTypeString = monsterType.ToString();

            MonsterCharacter monster = await ResourcesManager.SpawnPrefabAsync<MonsterCharacter>(
                monsterTypeString, spawnPos);

            if (monster != null)
            {
                monster.Initialize();
                monster.Face(_player.position);
                monster.SetTarget(_player);

                Log.Info(LogTags.CharacterSpawn, "몬스터를 생성했습니다: {0} at {1}",
                    monsterTypeString, spawnPos);
            }
            else
            {
                Log.Error(LogTags.CharacterSpawn, "몬스터 생성에 실패했습니다: {0}", monsterTypeString);
            }
        }

        private Vector3 GetRandomSpawnPosition()
        {
            Vector3 playerPos = _player.transform.position;

            // 플레이어로부터 최소 거리 이상 떨어진 위치에서 스폰
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(_currentWave.MinDistanceFromPlayer, _currentWave.SpawnRadius);
            Vector3 spawnPos = playerPos + new Vector3(randomDirection.x, randomDirection.y, 0) * randomDistance;

            return spawnPos;
        }

        /// <summary>
        /// 웨이브가 시작될 때 호출되는 이벤트 핸들러
        /// </summary>
        private void OnWaveStarted(int waveIndex, WaveData waveData)
        {
            _currentWave = waveData;
            Log.Info(LogTags.CharacterSpawn, "웨이브 시작됨: {0} (인덱스: {1})", waveData.WaveName, waveIndex);
        }

        /// <summary>
        /// 웨이브가 종료될 때 호출되는 이벤트 핸들러
        /// </summary>
        private void OnWaveEnded(int waveIndex)
        {
            Log.Info(LogTags.CharacterSpawn, "웨이브 종료됨: 인덱스 {0}", waveIndex);
        }

        /// <summary>
        /// 모든 웨이브가 완료될 때 호출되는 이벤트 핸들러
        /// </summary>
        public void OnAllWavesCompleted()
        {
            _currentWave = null; // 스폰 중지
            Log.Info(LogTags.CharacterSpawn, "모든 웨이브가 완료되어 몬스터 스폰을 중지합니다.");
        }

        #region WaveController Direct Control

        /// <summary>
        /// 웨이브 컨트롤러에서 직접 호출하는 메서드 - 현재 웨이브 설정
        /// </summary>
        public void SetCurrentWave(WaveData waveData)
        {
            _currentWave = waveData;
            Log.Info(LogTags.CharacterSpawn, "웨이브 설정됨: {0}", waveData.WaveName);
        }

        /// <summary>
        /// 웨이브 컨트롤러에서 직접 호출하는 메서드 - 웨이브 종료
        /// </summary>
        public void OnWaveEnded()
        {
            Log.Info(LogTags.CharacterSpawn, "웨이브 종료됨");
        }

        #endregion WaveController Direct Control

        private void OnDrawGizmosSelected()
        {
            if (!_showSpawnRadius || _currentWave == null || _player == null) return;

            // 스폰 반경 표시
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_player.transform.position, _currentWave.SpawnRadius);

            // 최소 거리 표시
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_player.transform.position, _currentWave.MinDistanceFromPlayer);
        }
    }
}