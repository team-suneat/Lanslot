using System.Collections.Generic;

namespace TeamSuneat.Data
{
    public partial class VCharacterStage
    {
        /// <summary>
        /// 직렬화된 스테이지/지역 관련 문자열 값을 최신 규격으로 정규화(정상화)하고
        /// 열거형 필드로 역직렬화한 뒤 컬렉션 및 상태를 복원합니다.
        /// </summary>
        public void OnLoadGameData()
        {
            // 직렬화된 데이터를 다시 메모리에 복원합니다.
            EnumEx.ConvertTo(ref _currentAreaName, CurrentAreaNameString);
            EnumEx.ConvertTo(ref _currentStageName, CurrentStageNameString);
            EnumEx.ConvertTo(ref _prevStageName, PrevStageNameString);
            EnumEx.ConvertTo(ref _moveType, MoveTypeString);

            InvokeOnLoadGameDataForAreas();
            InvokeOnLoadGameDataForStages();
            ClampMaxTreasureClassToLimit();
        }

        /// <summary>
        /// 각 지역의 OnLoadGameData를 호출합니다.
        /// </summary>
        private void InvokeOnLoadGameDataForAreas()
        {
            if (!Areas.IsValid())
            {
                return;
            }

            for (int i = 0; i < Areas.Count; i++)
            {
                Areas[i].OnLoadGameData();
            }
        }

        /// <summary>
        /// 각 스테이지의 OnLoadGameData를 호출합니다.
        /// </summary>
        private void InvokeOnLoadGameDataForStages()
        {
            if (Stages != null)
            {
                for (int i = 0; i < Stages.Count; i++)
                {
                    Stages[i].OnLoadGameData();
                }
            }
        }

        /// <summary>
        /// 저장된 최대 보물 등급이 게임 상한을 초과하면 상한으로 클램프합니다.
        /// </summary>
        private void ClampMaxTreasureClassToLimit()
        {
            if (MaxReachedTreasureClass > GameDefine.MAX_TREASURE_CLASS)
            {
                Log.Info(LogTags.Difficulty, "해금된 최대 TC가 게임의 최대 TC보다 높다면 조정합니다. {0} ▶ {1}", MaxReachedTreasureClass, GameDefine.MAX_TREASURE_CLASS);
                MaxReachedTreasureClass = GameDefine.MAX_TREASURE_CLASS;
            }
        }

        /// <summary>
        /// 스테이지 진행 정보를 초기화하고 시작 지점을 설정합니다.
        /// </summary>
        /// <param name="toPrologue">프롤로그로 시작할지 여부</param>
        public void Initialize(bool toPrologue = false)
        {
            Areas.Clear();
            Stages.Clear();
            GenerateStage(toPrologue);
            LogInfo("플레이어의 스테이지 정보를 초기화합니다.");
        }

        /// <summary>
        /// 현재 지역/스테이지 및 이동/맵 타입을 설정하고,
        /// 지역별 스테이지를 정렬하여 등록합니다.
        /// </summary>
        /// <param name="toPrologue">프롤로그 시작 여부</param>
        private void GenerateStage(bool toPrologue)
        {
            AreaNames[] areaNames = GenerateArea(toPrologue);
            if (toPrologue)
            {
                CurrentAreaName = AreaNames.Prologue;
                CurrentStageName = StageNames.Stage1;
                MoveType = StageMoveTypes.Run;
            }
            else
            {
                CurrentAreaName = AreaNames.OutGame;
                CurrentStageName = StageNames.Stage1;
                MoveType = StageMoveTypes.Death;
            }

            CurrentAreaNameString = CurrentAreaName.ToString();
            CurrentStageNameString = CurrentStageName.ToString();
            MoveTypeString = MoveType.ToString();

            for (int i = 0; i < areaNames.Length; i++)
            {
                AreaNames areaName = areaNames[i];
                SetSortedStageInArea(areaName);
            }
        }

        /// <summary>
        /// 초기 등록해야 할 지역 목록을 생성합니다.
        /// </summary>
        /// <param name="toPrologue">프롤로그 포함 여부</param>
        /// <returns>등록할 지역 이름 배열</returns>
        private AreaNames[] GenerateArea(bool toPrologue)
        {
            List<AreaNames> result = new();

            // 기본 지역 등록
            AddBaseArea(result, toPrologue);

            // 전투 지역 등록
            AddBattleAreas(result);

            return result.ToArray();
        }

        /// <summary>
        /// 기본 지역을 결과 목록에 추가합니다.
        /// </summary>
        /// <param name="result">지역 결과 목록</param>
        /// <param name="toPrologue">프롤로그 포함 여부</param>
        private void AddBaseArea(List<AreaNames> result, bool toPrologue)
        {
            if (toPrologue)
            {
                result.Add(AreaNames.Prologue);
            }
            else
            {
                result.Add(AreaNames.OutGame);
            }
        }

        /// <summary>
        /// 전투 지역을 조건에 맞게 결과 목록에 추가합니다.
        /// </summary>
        /// <param name="result">지역 결과 목록</param>
        private void AddBattleAreas(List<AreaNames> result)
        {
            AreaNames[] areaNames = EnumEx.GetValues<AreaNames>();
            int firstIndex = GameDefine.FIRST_BATTLE_AREA_NAME.ToInt();
            int lastIndex = GameDefine.LAST_BATTLE_AREA_NAME.ToInt();

            for (int i = firstIndex; i <= lastIndex; i++)
            {
                AreaNames areaName = areaNames[i];
                AreaAsset areaAsset = ScriptableDataManager.Instance.FindArea(areaName);
                if (!areaAsset.IsValid() || areaAsset.Data.IsBlock)
                {
                    continue;
                }
                result.Add(areaName);
            }
        }

        /// <summary>
        /// 지정한 지역과 해당 지역에 속한 스테이지들을 제거합니다.
        /// </summary>
        /// <param name="areaName">제거할 지역 이름</param>
        public void RemoveArea(AreaNames areaName)
        {
            if (!Areas.IsValid())
            {
                return;
            }

            // 제거할 지역 찾기
            VArea targetArea = FindArea(areaName);
            if (targetArea == null)
            {
                LogWarning($"제거할 지역을 찾을 수 없습니다: {areaName.ToLogString()}");
                return;
            }

            LogInfo($"기존 지역을 제거합니다: {targetArea.AreaName.ToLogString()}");

            // 해당 지역의 스테이지들 제거
            RemoveStagesInArea(targetArea);

            // 지역 제거
            Areas.Remove(targetArea);
        }

        /// <summary>
        /// 지정한 지역에 속한 모든 스테이지를 제거합니다.
        /// </summary>
        /// <param name="area">스테이지를 제거할 지역</param>
        private void RemoveStagesInArea(VArea area)
        {
            if (!area.StageNamesInArea.IsValid())
            {
                return;
            }

            // 지역에 속한 스테이지 이름들을 HashSet으로 변환하여 빠른 검색
            HashSet<StageNames> stageNamesToRemove = new HashSet<StageNames>(area.StageNamesInArea);

            // Stages 컬렉션에서 해당 스테이지들 제거 (역순으로 순회)
            for (int i = Stages.Count - 1; i >= 0; i--)
            {
                if (stageNamesToRemove.Contains(Stages[i].StageName))
                {
                    LogInfo($"{area.AreaName.ToLogString()} 지역의 스테이지를 제거합니다: {Stages[i].StageName.ToLogString()}");
                    Stages.RemoveAt(i);
                }
            }
        }
    }
}