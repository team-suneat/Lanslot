using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class StageSystem : XBehaviour
    {
        public StageNames Name;
        public string NameString;

        [SerializeField]
        private BattlefieldTileGroup _battlefieldTileGroup;
        private StageData _currentStageData;
        private int _currentWaveNumber;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
            _battlefieldTileGroup = GetComponentInChildren<BattlefieldTileGroup>();
        }

        public override void AutoSetting()
        {
            base.AutoSetting();
            NameString = Name.ToString();
        }

        private void OnValidate()
        {
            _ = EnumEx.ConvertTo(ref Name, NameString);
        }

        public override void AutoNaming()
        {
            SetGameObjectName(NameString);
        }

        public void Initialize()
        {
            // StageData 로드
            _currentStageData = JsonDataManager.FindStageDataClone(Name);
            if (_currentStageData == null)
            {
                Log.Error(LogTags.Stage, "스테이지 데이터를 찾을 수 없습니다: {0}", Name);
                return;
            }

            // Width 홀수 검증
            if (_currentStageData.Width % 2 == 0)
            {
                Log.Error(LogTags.Stage, "스테이지 Width는 홀수여야 합니다: {0}", _currentStageData.Width);
                return;
            }

            // BattlefieldTileGroup 검증
            if (_battlefieldTileGroup == null)
            {
                Log.Error(LogTags.Stage, "BattlefieldTileGroup 컴포넌트가 할당되지 않았습니다.");
                return;
            }

            // BattlefieldTileGroup 초기화
            Vector3 originPosition = transform.position;
            _battlefieldTileGroup.Initialize(_currentStageData.Width, originPosition);

            // 1~10웨이브 초기 세팅
            SetupInitialWaves(Name);

            Log.Info(LogTags.Stage, "스테이지 초기화 완료: {0}, Width={1}", Name, _currentStageData.Width);
        }

        public void StartStage()
        {
            if (_battlefieldTileGroup == null)
            {
                Log.Warning(LogTags.Stage, "전장이 초기화되지 않았습니다.");
                return;
            }

            _currentWaveNumber = 1;
            Log.Info(LogTags.Stage, "스테이지 시작: Wave {0}", _currentWaveNumber);
        }

        public void CleanupStage()
        {
            _battlefieldTileGroup?.Clear();

            _currentStageData = null;
            _currentWaveNumber = 0;
        }

        /// <summary>
        /// Row를 웨이브 번호로 변환합니다.
        /// </summary>
        public int GetWaveNumberFromRow(int row)
        {
            return row + 1;
        }

        /// <summary>
        /// 웨이브 번호를 Row로 변환합니다.
        /// </summary>
        public int GetRowFromWaveNumber(int waveNumber)
        {
            return waveNumber - 1;
        }

        /// <summary>
        /// 해당 Row의 웨이브 데이터를 조회합니다.
        /// </summary>
        public WaveData GetWaveDataForRow(int row)
        {
            if (_currentStageData == null)
            {
                return null;
            }

            int waveNumber = GetWaveNumberFromRow(row);
            return JsonDataManager.GetWaveDataByNumber(_currentStageData.Name, waveNumber);
        }

        /// <summary>
        /// 1~10웨이브를 초기 세팅합니다.
        /// </summary>
        private void SetupInitialWaves(StageNames stageName)
        {
            if (_battlefieldTileGroup == null)
            {
                return;
            }

            WaveData waveData = null;
            for (int row = 0; row < _battlefieldTileGroup.Height; row++)
            {
                int waveNumber = GetWaveNumberFromRow(row);
                WaveData tempWaveData = JsonDataManager.GetWaveDataByNumber(stageName, waveNumber);
                if (tempWaveData != null)
                {
                    waveData = tempWaveData;
                    Log.Info(LogTags.Stage, "웨이브 데이터 연결: Row {0} → Wave {1}", row, waveNumber);
                }

                int monsterCount = waveData.GetMonsterCount();
                if (monsterCount <= 0)
                {
                    continue;
                }

                Deck<int> deck = new();
                for (int column = 0; column < _battlefieldTileGroup.Width; column++)
                {
                    deck.Add(column);
                }
                deck.Shuffle();

                for (int i = 0; i < monsterCount; i++)
                {
                    int column = deck.Get(i);
                    CharacterNames characterName = waveData.GetRandomMonster();
                    SpawnMonster(row, column, characterName);
                }
            }
        }

        private void SpawnMonster(int row, int column, CharacterNames characterName)
        {
            BattlefieldTile tile = _battlefieldTileGroup.GetTile(row, column);
            MonsterCharacter monster = ResourcesManager.SpawnMonsterCharacter(characterName, tile.transform);
            _battlefieldTileGroup.SetTileOccupied(row, column, monster);
        }
    }
}