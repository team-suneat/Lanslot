using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamSuneat.Data
{
    public partial class VCharacterStage
    {
        private void SetSortedStageInArea(AreaNames areaName)
        {
            AreaAsset areaAsset = GetAreaAsset(areaName);
            if (areaAsset == null)
            {
                return;
            }

            List<StageAssetData> stageDatasInArea = GetStageDatasInArea(areaName);
            if (stageDatasInArea == null)
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{areaName}({areaName.ToLogString()}) 지역의 스테이지를 정렬합니다. ───");

            List<StageAssetData> sortedStages = SortStages(stageDatasInArea);
            if (areaAsset.Data.IsBattleArea)
            {
                List<StageAssetData> pickedBattleStages = PickBattleStages(areaAsset, sortedStages);
                if (pickedBattleStages.Count == 0)
                {
                    LogWarning("{0} 지역에 유효한 전투 스테이지가 없습니다.", areaName.ToLogString());
                    return;
                }
                StageNames eliteStage = SetEliteStage(areaAsset, pickedBattleStages);
                List<StageAssetData> filteredStages = MergeStagesPreservingOrder(sortedStages, pickedBattleStages);

                RegisterArea(areaName, filteredStages, sb);
                RegisterStagesInArea(filteredStages, eliteStage, areaAsset.Data.EliteCount, sb);
            }
            else
            {
                RegisterArea(areaName, sortedStages, sb);
                RegisterStagesInArea(sortedStages, StageNames.None, 0, sb);
            }

            sb.AppendLine($"─── {areaName}({areaName.ToLogString()}) 지역의 스테이지를 정렬을 마칩니다.");
            LogInfo(sb.ToString());
        }

        private List<StageAssetData> SortStages(List<StageAssetData> stages)
        {
            List<StageAssetData> sorted = new(stages);
            sorted.Sort((a, b) => a.Order.CompareTo(b.Order));
            return sorted;
        }

        private AreaAsset GetAreaAsset(AreaNames areaName)
        {
            AreaAsset areaAsset = ScriptableDataManager.Instance.FindArea(areaName);
            if (areaAsset == null)
            {
                LogError("해당 지역 데이터를 찾을 수 없습니다: {0}", areaName);
            }
            return areaAsset;
        }

        private List<StageAssetData> GetStageDatasInArea(AreaNames areaName)
        {
            List<StageAssetData> stageDatasInArea = ScriptableDataManager.Instance.GetStagesInArea(areaName);
            if (stageDatasInArea == null || stageDatasInArea.Count == 0)
            {
                LogError("해당 지역의 스테이지 데이터를 찾을 수 없거나 비어 있습니다: {0}", areaName);
                return null;
            }
            return stageDatasInArea;
        }

        private StageNames SetEliteStage(AreaAsset areaAsset, List<StageAssetData> battleStages)
        {
            if (areaAsset.Data.EliteCount <= 0 || battleStages.Count == 0)
            {
                return StageNames.None;
            }
            int index = areaAsset.Data.IgnoreEliteInFirstStage ? battleStages.Count - 1 : RandomEx.Range(0, battleStages.Count);
            if (index < 0 || index >= battleStages.Count)
            {
                LogError("정예 몬스터를 배치할 인덱스가 유효하지 않습니다. Count: {0}, Index: {1}", battleStages.Count, index);
                return StageNames.None;
            }
            StageNames eliteStage = battleStages[index].Name;
            LogInfo("정예 몬스터가 등장할 스테이지: {0}", eliteStage.ToLogString());
            return eliteStage;
        }

        private List<StageAssetData> MergeStagesPreservingOrder(List<StageAssetData> allSortedStages, List<StageAssetData> pickedBattleStages)
        {
            List<StageAssetData> result = new();
            foreach (StageAssetData v in allSortedStages)
            {
                result.Add(v);
            }
            return result;
        }

        private List<StageAssetData> PickBattleStages(AreaAsset areaAsset, List<StageAssetData> candidates)
        {
            List<StageAssetData> selectedStages = new();
            HashSet<StageAssetData> used = new();
            List<int> slotNumbers = areaAsset.Data.BattleSlots.GetSlotNumbers();
            int requiredCount = areaAsset.Data.BattleStageCount;
            foreach (int slot in slotNumbers)
            {
                if (selectedStages.Count >= requiredCount)
                {
                    break;
                }
                List<StageAssetData> slotCandidates = candidates
                    .Where(stage => !used.Contains(stage))
                    .ToList();
                StageAssetData chosen = SelectStage(slotCandidates, $"{slot}번째 전투 슬롯");
                if (chosen != null)
                {
                    selectedStages.Add(chosen);
                    _ = used.Add(chosen);
                }
            }
            return selectedStages;
        }

        private StageAssetData SelectStage(List<StageAssetData> candidates, string description)
        {
            if (candidates.Count == 0)
            {
                LogWarning("{0} 후보가 없습니다.", description);
                return null;
            }
            StageAssetData selected = (candidates.Count == 1)
                ? candidates[0]
                : candidates[RandomEx.Range(0, candidates.Count)];

            if (Log.LevelInfo)
            {
                LogInfo("{0} 스테이지로 선택됨: {1}", description, selected.Name.ToLogString());
            }
            return selected;
        }
    }
}