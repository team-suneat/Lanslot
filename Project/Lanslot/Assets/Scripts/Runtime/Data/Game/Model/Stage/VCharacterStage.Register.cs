using System.Collections.Generic;
using System.Text;

namespace TeamSuneat.Data
{
    public partial class VCharacterStage
    {
        private void RegisterArea(AreaNames areaName, StageNames[] stageNames)
        {
            Areas.Add(new VArea(areaName, stageNames));
            LogInfo("지역을 등록합니다. {0}({1})", areaName, areaName.ToLogString());
        }

        private void RegisterStage(StageNames stageName)
        {
            StageAsset stageAsset = ScriptableDataManager.Instance.FindStage(stageName);
            if (stageAsset != null && stageAsset.Data != null)
            {
                VStage newStageInfo = new(stageAsset.Data);
                Stages.Add(newStageInfo);
                LogInfo("스테이지를 등록합니다. {0}({1})", stageAsset.Name, stageAsset.Name.ToLogString());
            }
            else
            {
                LogWarning("스테이지 에셋을 찾을 수 없어 Stages에 등록하지 못했습니다: {0}({1})", stageName, stageName.ToLogString());
            }
        }

        // For Generate

        private void RegisterArea(AreaNames areaName, List<StageAssetData> stageDatas, StringBuilder sb)
        {
            Areas.Add(new VArea(areaName, stageDatas));
            sb.AppendLine($"지역을 등록합니다. {areaName}({areaName.ToLogString()})");
        }

        private void RegisterStagesInArea(List<StageAssetData> stageDatas, StageNames eliteStageName, int eliteCount, StringBuilder sb)
        {
            for (int i = 0; i < stageDatas.Count; i++)
            {
                StageAssetData stageData = stageDatas[i];
                VStage stageInfo = new(stageData)
                {
                    EliteCount = stageData.Name == eliteStageName ? eliteCount : 0
                };

                Stages.Add(stageInfo);
                sb.AppendLine($"지역의 스테이지를 등록합니다. {stageData.Name}({stageData.Name.ToLogString()})");
            }
        }
    }
}