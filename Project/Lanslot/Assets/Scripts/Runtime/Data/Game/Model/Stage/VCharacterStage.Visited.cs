namespace TeamSuneat.Data
{
    public partial class VCharacterStage
    {
        private void RegisterVisitedArea()
        {
            VArea areaInfo = Areas.Find(x => x.AreaName == CurrentAreaName);
            if (areaInfo != null)
            {
                areaInfo.IsVisited = true;
            }
        }

        public bool CheckVisitedArea(AreaNames areaName)
        {
            VArea areaInfo = Areas.Find(x => x.AreaName == areaName);
            if (areaInfo != null)
            {
                if (areaInfo.IsVisited)
                {
                    return true;
                }
            }
            return false;
        }

        private void RegisterVisitedStage()
        {
            StageAsset stageAsset = ScriptableDataManager.Instance.FindStage(CurrentStageName);
            if (!stageAsset.IsValid())
            {
                return;
            }
            if (!_visitedStages.Contains(CurrentStageName))
            {
                _ = _visitedStages.Add(CurrentStageName);
            }
        }

        public bool CheckVisitedStage(StageNames stageName)
        {
            VStage stageInfo = Stages.Find(x => x.StageName == stageName);
            if (stageInfo != null)
            {
                if (stageInfo.Progress == StageProgress.Rewarded)
                {
                    return true;
                }
            }
            return false;
        }
    }
}