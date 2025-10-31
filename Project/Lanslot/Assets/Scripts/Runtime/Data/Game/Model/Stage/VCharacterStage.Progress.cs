namespace TeamSuneat.Data
{
    public partial class VCharacterStage
    {
        public StageProgress FindProgress(StageNames stageName)
        {
            VStage stageInfo = FindStage(stageName);
            if (stageInfo != null)
            {
                return stageInfo.Progress;
            }

            return StageProgress.None;
        }

        public StageProgress GetProgress()
        {
            return FindProgress(CurrentStageName);
        }

        public bool CheckProgress(StageProgress progress)
        {
            return GetProgress() == progress;
        }

        public bool CheckProgress(params StageProgress[] progress)
        {
            if (progress.IsValid())
            {
                for (int i = 0; i < progress.Length; i++)
                {
                    if (GetProgress() == progress[i])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void SetStageProgress(StageProgress progress)
        {
            VStage stageInfo = Stages.Find(x => x.StageName == CurrentStageName);
            if (stageInfo != null)
            {
                if (stageInfo.TrySetProgress(progress))
                {
                    string areaContent = CurrentAreaName.ToLogString();
                    string stageContent = CurrentStageName.ToLogString();

                    if (string.IsNullOrEmpty(stageContent))
                    {
                        stageContent = CurrentStageName.ToString();
                    }

                    LogInfo(
                        "현재 스테이지의 진행도를 설정합니다. 현재 지역: {0}, 현재 스테이지: {1}, 진행도: {2}",
                        areaContent, stageContent, progress);

                    if (progress == StageProgress.Completed)
                    {
                        GlobalEvent.Send(GlobalEventType.STAGE_COMPLETED);
                    }
                    else if (progress == StageProgress.Rewarded)
                    {
                        GlobalEvent.Send(GlobalEventType.STAGE_REWARDED);
                    }
                }
            }
        }
    }
}