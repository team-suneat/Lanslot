namespace TeamSuneat.Data
{
    public partial class VCharacterStage
    {
        public void MoveToNextStage()
        {
            StageNames nextStageName = GetNextStageName();
            MoveToTargetStage(nextStageName);
        }

        public void MoveToTargetStage(StageNames stageName)
        {
            if (stageName != CurrentStageName)
            {
                MoveToStage(stageName);
                LogInfo("지정한 스테이지로 이동합니다. 스테이지: {0}", CurrentStageName.ToLogString());
            }
        }

        private void MoveToStage(StageNames nextStageName)
        {
            if (nextStageName == StageNames.None)
            {
                LogWarning("이동하려는 스테이지의 설정되지 않았습니다. 현재 지역: {0}, 현재 스테이지: {1}", CurrentAreaName.ToLogString(), CurrentStageName.ToLogString());
                return;
            }
            if (CurrentStageName == nextStageName)
            {
                LogWarning("이동하려는 스테이지의 이름이 같습니다. 이동할 수 없습니다. 현재 지역: {0}, 현재 스테이지: {1}", CurrentAreaName.ToLogString(), CurrentStageName.ToLogString());
                return;
            }
            StageAsset stageAsset = ScriptableDataManager.Instance.FindStage(nextStageName);
            if (stageAsset.IsValid())
            {
                RegisterVisitedStage();

                PrevStageName = CurrentStageName;
                CurrentAreaName = stageAsset.Data.Area;
                CurrentStageName = nextStageName;
                PrevStageNameString = PrevStageName.ToString();
                CurrentAreaNameString = CurrentAreaName.ToString();
                CurrentStageNameString = CurrentStageName.ToString();

                SetMaxReachedTreasureClass(stageAsset.Data.TreasureClass);
                RegisterVisitedArea();
                AddChallengeCount();

                LogInfo("스테이지를 이동합니다. 현재 지역: {0}, 이전 스테이지: {1}, 현재 스테이지: {2}", CurrentAreaName.ToLogString(), PrevStageName.ToLogString(), CurrentStageName.ToLogString());
            }
            else
            {
                LogError("[Stage-GameData] {0} 스테이지를 이동할 수 없습니다. StageAssetData를 찾을 수 없습니다.", nextStageName);
            }
        }

        public void SetMoveType(StageMoveTypes moveType)
        {
            MoveType = moveType;
            MoveTypeString = moveType.ToString();

            LogInfo("스테이지 이동 방식을 설정합니다. {0}", MoveType.ToLogString());
        }
    }
}