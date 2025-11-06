using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class StageSystem : XBehaviour
    {
        private const float DEFAULT_TILE_SIZE = 1.0f;

        public StageNames Name;
        public string NameString;

        public BattlefieldManager BattlefieldManager { get; private set; }
        public StageData CurrentStageData { get; private set; }
        public int CurrentWaveNumber { get; private set; }

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
            CurrentStageData = JsonDataManager.FindStageDataClone(Name);
            if (CurrentStageData == null)
            {
                Log.Error(LogTags.Stage, "스테이지 데이터를 찾을 수 없습니다: {0}", Name);
                return;
            }

            // Width 홀수 검증
            if (CurrentStageData.Width % 2 == 0)
            {
                Log.Error(LogTags.Stage, "스테이지 Width는 홀수여야 합니다: {0}", CurrentStageData.Width);
                return;
            }

            // BattlefieldManager 생성 및 초기화
            BattlefieldManager = new BattlefieldManager();
            Vector3 originPosition = transform.position;
            BattlefieldManager.Initialize(CurrentStageData.Width, originPosition, DEFAULT_TILE_SIZE);

            // 1~10웨이브 초기 세팅
            SetupInitialWaves(Name);

            Log.Info(LogTags.Stage, "스테이지 초기화 완료: {0}, Width={1}", Name, CurrentStageData.Width);
        }

        public void StartStage()
        {
            if (BattlefieldManager == null)
            {
                Log.Warning(LogTags.Stage, "전장이 초기화되지 않았습니다.");
                return;
            }

            CurrentWaveNumber = 1;
            Log.Info(LogTags.Stage, "스테이지 시작: Wave {0}", CurrentWaveNumber);
        }

        public void CleanupStage()
        {
            BattlefieldManager?.Clear();
            BattlefieldManager = null;

            CurrentStageData = null;
            CurrentWaveNumber = 0;
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
            if (CurrentStageData == null)
            {
                return null;
            }

            int waveNumber = GetWaveNumberFromRow(row);
            return JsonDataManager.GetWaveDataByNumber(CurrentStageData.Name, waveNumber);
        }

        /// <summary>
        /// 1~10웨이브를 초기 세팅합니다.
        /// </summary>
        private void SetupInitialWaves(StageNames stageName)
        {
            for (int row = 0; row < BattlefieldManager.HEIGHT; row++)
            {
                int waveNumber = GetWaveNumberFromRow(row);
                WaveData waveData = JsonDataManager.GetWaveDataByNumber(stageName, waveNumber);

                if (waveData != null)
                {
                    Log.Info(LogTags.Stage, "웨이브 데이터 연결: Row {0} → Wave {1}", row, waveNumber);
                }
                else
                {
                    Log.Warning(LogTags.Stage, "웨이브 데이터를 찾을 수 없습니다: Row {0}, Wave {1}", row, waveNumber);
                }
            }
        }
    }
}