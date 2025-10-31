using System;

namespace TeamSuneat.Data
{
    [Serializable]
    public class VStage
    {
        /// <summary> 스테이지 이름 (Key) </summary>
        [NonSerialized]
        public StageNames StageName;

        public string StageNameString;

        /// <summary> 스테이지 진행 상황 </summary>
        [NonSerialized]
        public StageProgress Progress; // (None, Completed, Rewarded)

        public string ProgressString;

        /// <summary> 스테이지에서 등장할 수 있는 정예 몬스터의 수 </summary>
        public int EliteCount;

        /// <summary> 스테이지의 웨이브 인덱스 </summary>
        public int WaveIndex;

        /// <summary> 스테이지의 전투 중단 여부 </summary>
        public bool IsBattlePause;

        public VStage()
        {
        }

        public VStage(StageAssetData assetData)
        {
            StageName = assetData.Name;
            Progress = StageProgress.None;

            StageNameString = assetData.Name.ToString();
            ProgressString = StageProgress.None.ToString();
        }

        public void OnLoadGameData()
        {
            EnumEx.ConvertTo(ref StageName, StageNameString);
            EnumEx.ConvertTo(ref Progress, ProgressString);
        }

        public bool TrySetProgress(StageProgress progress)
        {
            if (Progress != progress)
            {
                Progress = progress;
                ProgressString = progress.ToString();

                return true;
            }

            return false;
        }
    }
}