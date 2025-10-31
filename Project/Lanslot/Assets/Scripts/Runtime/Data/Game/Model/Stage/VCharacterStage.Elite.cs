using TeamSuneat;

namespace TeamSuneat.Data
{
    public partial class VCharacterStage
    {
        public bool DetermineSpawnElite()
        {
            VStage stageInfo = Stages.Find(x => x.StageName == CurrentStageName);
            if (stageInfo != null)
            {
                if (stageInfo.EliteCount > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public void OnSpawnElite()
        {
            VStage stageInfo = Stages.Find(x => x.StageName == CurrentStageName);
            if (stageInfo != null)
            {
                if (stageInfo.EliteCount > 0)
                {
                    stageInfo.EliteCount -= 1;
                    LogInfo( "정예를 생성했다면 스테이지({0})에 등록된 정예 생성 수를 감소시킵니다. 남은 정예 몬스터 수: {1}",
                        CurrentStageName.ToLogString(), stageInfo.EliteCount);
                }
                else
                {
                    LogWarning( "등록된 스테이지({0})의 정예 생성 수를 감소시킬 수 없습니다. 남은 정예 몬스터 수: {1}",
                        CurrentStageName.ToLogString(), stageInfo.EliteCount);
                }
            }
        }
    }
}