using System;
using System.Collections.Generic;

namespace TeamSuneat.Data
{
    [Serializable]
    public partial class VCharacterStage
    {
        // 현재 지역, 스테이지의 정보를 저장합니다.

        /// <summary> 현재 지역 이름 </summary>
        [NonSerialized] private AreaNames _currentAreaName;
        public string CurrentAreaNameString;

        /// <summary> 현재 스테이지 이름 </summary>
        [NonSerialized] private StageNames _currentStageName;
        public string CurrentStageNameString;

        /// <summary> 이전 스테이지 이름 </summary>
        [NonSerialized] private StageNames _prevStageName;
        public string PrevStageNameString;

        /// <summary> 스테이지 이동 방식 </summary>
        [NonSerialized] private StageMoveTypes _moveType;
        public string MoveTypeString;

        // 프로퍼티로 enum과 문자열 동기화
        public AreaNames CurrentAreaName
        {
            get => _currentAreaName;
            set
            {
                _currentAreaName = value;
                CurrentAreaNameString = value.ToString();
            }
        }

        public StageNames CurrentStageName
        {
            get => _currentStageName;
            set
            {
                _currentStageName = value;
                CurrentStageNameString = value.ToString();
            }
        }

        public StageNames PrevStageName
        {
            get => _prevStageName;
            set
            {
                _prevStageName = value;
                PrevStageNameString = value.ToString();
            }
        }

        public StageMoveTypes MoveType
        {
            get => _moveType;
            set
            {
                _moveType = value;
                MoveTypeString = value.ToString();
            }
        }

        // 게임 진행에 필요한 지역, 스테이지 정보를 저장합니다.

        /// <summary> 지역 정보 </summary>
        public List<VArea> Areas = new();

        /// <summary> 스테이지 정보 </summary>
        public List<VStage> Stages = new();

        // 게임 전반에 필요한 스테이지 정보를 저장합니다.

        [NonSerialized]
        private HashSet<StageNames> _visitedStages = new();

        /// <summary> 최대 난이도의 최대 도달 TC </summary>
        public int MaxReachedTreasureClass = 1;

        private const int NEXT_STAGE_SAFETY_LIMIT = 10;

        public static VCharacterStage CreateDefault()
        {
            VCharacterStage stageInfo = new();
            stageInfo.Initialize(true);
            return stageInfo;
        }

        public VCharacterStage()
        { }

        /// <summary>
        /// 문자열 필드에서 enum 값을 복원합니다. (데이터 로드 시 사용)
        /// </summary>
        public void RestoreEnumValuesFromStrings()
        {
            if (!string.IsNullOrEmpty(CurrentAreaNameString) &&
                Enum.TryParse<AreaNames>(CurrentAreaNameString, out var areaName))
            {
                _currentAreaName = areaName;
            }

            if (!string.IsNullOrEmpty(CurrentStageNameString) &&
                Enum.TryParse<StageNames>(CurrentStageNameString, out var stageName))
            {
                _currentStageName = stageName;
            }

            if (!string.IsNullOrEmpty(PrevStageNameString) &&
                Enum.TryParse<StageNames>(PrevStageNameString, out var prevStageName))
            {
                _prevStageName = prevStageName;
            }

            if (!string.IsNullOrEmpty(MoveTypeString) &&
                Enum.TryParse<StageMoveTypes>(MoveTypeString, out var moveType))
            {
                _moveType = moveType;
            }
        }
    }
}