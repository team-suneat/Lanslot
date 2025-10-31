using System.Linq;

namespace TeamSuneat.Data
{
    public partial class VCharacterStage
    {
        public StageNames FindStageName(int index)
        {
            return FindStageName(CurrentAreaName, index);
        }

        public StageNames FindStageName(AreaNames areaName, int index)
        {
            VArea areaInfo = Areas.Find(x => x.AreaName == areaName);
            return areaInfo != null ? areaInfo.FindAt(index) : StageNames.None;
        }

        public VStage FindStage(StageNames stageName)
        {
            VStage result = Stages.Find(x => x.StageName == stageName);
            if (result == null)
            {
                LogWarning("스테이지의 정보를 불러올 수 없습니다: {0}({1})", stageName, stageName.ToLogString());
            }
            return result;
        }

        public VArea FindArea(AreaNames areaName)
        {
            VArea result = Areas.Find(x => x.AreaName == areaName);
            if (result == null)
            {
                LogWarning("지역의 정보를 불러올 수 없습니다: {0}({1})", areaName, areaName.ToLogString());
            }
            return result;
        }

        //

        public VStage GetCurrentStage()
        {
            return FindStage(CurrentStageName);
        }

        public StageNames GetNextStageName()
        {
            StageNames nextStageName = GetNextStageNameInternal();
            int safetyCounter = NEXT_STAGE_SAFETY_LIMIT;
            while (_visitedStages.Contains(nextStageName) && safetyCounter-- > 0)
            {
                CurrentStageName = nextStageName;
                CurrentStageNameString = nextStageName.ToString();
                nextStageName = GetNextStageNameInternal();
            }
            if (safetyCounter <= 0)
            {
                LogError("GetNextStageName() 루프가 비정상적으로 반복되었습니다. fallback 반환");
                return StageNames.None;
            }
            return nextStageName;
        }

        private StageNames GetNextStageNameInternal()
        {
            for (int i = 0; i < Areas.Count; i++)
            {
                if (Areas[i].AreaName != CurrentAreaName)
                {
                    continue;
                }
                StageNames nextStage = Areas[i].GetNextStageName(CurrentStageName);
                if (nextStage == StageNames.None && Areas.Count > i + 1)
                {
                    return Areas[i + 1].GetFirstStageName();
                }
                return nextStage;
            }
            return StageNames.None;
        }

        public StageNames GetFirstStageName(AreaNames areaName)
        {
            for (int i = 0; i < Areas.Count; i++)
            {
                if (Areas[i].AreaName != areaName)
                {
                    continue;
                }
                return Areas[i].GetFirstStageName();
            }
            LogWarning("지역의 마지막 스테이지를 찾을 수 없습니다:{0}", areaName);
            return StageNames.None;
        }

        public StageNames GetLastStageNameOfCurrentArea()
        {
            StageNames lastStageName = StageNames.None;
            VArea areaInfo = FindArea(CurrentAreaName);
            if (areaInfo != null)
            {
                lastStageName = areaInfo.GetLastStageName();
            }

            if (lastStageName == StageNames.None)
            {
                LogWarning("다음 지역의 마지막 스테이지를 찾을 수 없습니다. 현재 지역: {0}({1})",
                CurrentAreaName, CurrentAreaName.ToLogString());
            }

            return lastStageName;
        }

        public VArea GetCurrentArea()
        {
            VArea result = Areas.Find(x => x.AreaName == CurrentAreaName);
            if (result == null)
            {
                LogError("현재 지역의 정보를 불러올 수 없습니다: {0}({1})", CurrentAreaName, CurrentAreaName.ToLogString());
            }
            return result;
        }

        public AreaNames GetNextAreaName()
        {
            StageNames nextStageName = StageNames.None;
            AreaNames nextAreaName = AreaNames.None;
            for (int i = 0; i < Areas.Count; i++)
            {
                if (Areas[i].AreaName != CurrentAreaName)
                {
                    continue;
                }
                nextStageName = Areas[i].GetNextStageName(CurrentStageName);
                if (nextStageName == StageNames.None)
                {
                    if (Areas.Count > i + 1)
                    {
                        nextStageName = Areas[i + 1].GetFirstStageName();
                        nextAreaName = Areas[i + 1].AreaName;
                    }
                }
            }

            return nextAreaName;
        }

        public int GetStageOrderOfCurrentArea()
        {
            for (int i = 0; i < Areas.Count; i++)
            {
                VArea item = Areas[i];
                if (item.AreaName != CurrentAreaName)
                {
                    continue;
                }

                StageNames[] stages = item.StageNamesInArea;
                for (int j = 0; j < stages.Length; j++)
                {
                    StageNames stageName = stages[j];
                    if (stageName == CurrentStageName)
                    {
                        return j;
                    }
                }
            }

            return 0;
        }

        public StageNames PeekNextStageName()
        {
            StageNames current = CurrentStageName;
            StageNames next = InternalGetNextStageName(current);
            int safetyCounter = NEXT_STAGE_SAFETY_LIMIT;
            while (_visitedStages.Contains(next) && safetyCounter-- > 0)
            {
                current = next;
                next = InternalGetNextStageName(current);
            }
            if (safetyCounter <= 0)
            {
                LogError("PeekNextStageName() 재귀 루프가 비정상적으로 반복되었습니다. fallback 반환");
                return StageNames.None;
            }
            return next;
        }

        private StageNames InternalGetNextStageName(StageNames fromStage)
        {
            for (int i = 0; i < Areas.Count; i++)
            {
                if (!Areas[i].StageNamesInArea.Contains(fromStage))
                {
                    continue;
                }
                StageNames next = Areas[i].GetNextStageName(fromStage);
                if (next == StageNames.None && Areas.Count > i + 1)
                {
                    return Areas[i + 1].GetFirstStageName();
                }
                return next;
            }
            return StageNames.None;
        }
    }
}