using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 스테이지 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region Stage Get Methods

        /// <summary>
        /// 지역의 첫 번째 스테이지를 가져옵니다.
        /// </summary>
        public StageNames GetFirstStageInArea(AreaNames areaName)
        {
            StageAsset[] values = _stages.Values.ToArray();
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Data.IsBlock) { continue; }
                if (values[i].Data.Area != areaName) { continue; }
                if (values[i].Data.Order == 0)
                {
                    return values[i].Name;
                }
            }

            return StageNames.None;
        }

        /// <summary>
        /// 지역의 모든 스테이지를 가져옵니다.
        /// </summary>
        public List<StageAssetData> GetStagesInArea(AreaNames areaName)
        {
            List<StageAssetData> stagesInArea = new();
            StageAsset[] values = _stages.Values.ToArray();

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Data.IsBlock)
                {
                    continue;
                }
                if (values[i].Data.Area == areaName)
                {
                    stagesInArea.Add(values[i].Data);
                }
            }

            return stagesInArea;
        }

        /// <summary>
        /// 모든 스테이지를 가져옵니다.
        /// </summary>
        public void GetAllStages(ref Dictionary<StageNames, StageAssetData> stages)
        {
            foreach (KeyValuePair<int, StageAsset> item in _stages)
            {
                stages.Add(item.Key.ToEnum<StageNames>(), item.Value.Data);
            }
        }

        #endregion Stage Get Methods

        #region Stage Find Methods

        /// <summary>
        /// 스테이지 에셋을 찾습니다.
        /// </summary>
        public StageAsset FindStage(StageNames key)
        {
            return FindStage(BitConvert.Enum32ToInt(key));
        }

        public StageAsset FindStage(int tid)
        {
            if (_stages.ContainsKey(tid))
            {
                return _stages[tid];
            }

            return null;
        }

        /// <summary>
        /// 지역 에셋을 찾습니다.
        /// </summary>
        public AreaAsset FindArea(AreaNames key)
        {
            return FindArea(BitConvert.Enum32ToInt(key));
        }

        private AreaAsset FindArea(int tid)
        {
            if (_areas.ContainsKey(tid))
            {
                return _areas[tid];
            }

            return null;
        }

        #endregion Stage Find Methods

        #region Stage Load Methods

        /// <summary>
        /// 스테이지 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadStageSync(string filePath)
        {
            if (!filePath.Contains("Stage_"))
            {
                return false;
            }

            StageAsset asset = ResourcesManager.LoadResource<StageAsset>(filePath);
            if (asset != null)
            {
                if (asset.TID == 0)
                {
                    Log.Warning(LogTags.ScriptableData, "{0}, 스테이지 아이디가 설정되어있지 않습니다. {1}", asset.name, filePath);
                }
                else if (_stages.ContainsKey(asset.TID))
                {
                    Log.Warning(LogTags.ScriptableData, "같은 TID로 중복 Stage가 로드 되고 있습니다. TID: {0}, 기존: {1}, 새로운 이름: {2}",
                         asset.TID, _stages[asset.TID].name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _stages[asset.TID] = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        /// <summary>
        /// 지역 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadAreaSync(string filePath)
        {
            if (!filePath.Contains("Area_"))
            {
                return false;
            }

            AreaAsset asset = ResourcesManager.LoadResource<AreaAsset>(filePath);
            if (asset != null)
            {
                if (asset.TID == 0)
                {
                    Log.Warning(LogTags.ScriptableData, "{0}, 영역 아이디가 설정되어있지 않습니다. {1}", asset.name, filePath);
                }
                else if (_areas.ContainsKey(asset.TID))
                {
                    Log.Warning(LogTags.ScriptableData, "같은 TID로 중복 Area가 로드 되고 있습니다. TID: {0}, 기존: {1}, 새로운 이름: {2}",
                         asset.TID, _areas[asset.TID].name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _areas[asset.TID] = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        #endregion Stage Load Methods

        #region Stage Refresh Methods

        /// <summary>
        /// 모든 스테이지 및 지역 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllArea()
        {
            foreach (KeyValuePair<int, AreaAsset> item in _areas) { Refresh(item.Value); }
            foreach (KeyValuePair<int, StageAsset> item in _stages) { Refresh(item.Value); }
        }

        private void Refresh(AreaAsset areaAsset)
        {
            areaAsset?.Refresh();
        }

        private void Refresh(StageAsset stageAsset)
        {
            stageAsset?.Refresh();
        }

        #endregion Stage Refresh Methods

        #region Stage Validation Methods

        /// <summary>
        /// 스테이지 에셋 유효성을 검사합니다.
        /// </summary>
        private void CheckValidStagesOnLoadAssets()
        {
#if UNITY_EDITOR
            BuildTypes[] buildTypes = EnumEx.GetValues<BuildTypes>(true);
            AreaNames[] areaNames = EnumEx.GetValues<AreaNames>(true);
            StageNames[] stageNames = EnumEx.GetValues<StageNames>(true);

            ListMultiMap<AreaNames, StageNames> stagesForEditor = new();
            ListMultiMap<AreaNames, StageNames> stagesForDevelopment = new();
            ListMultiMap<AreaNames, StageNames> stagesForLive = new();

            for (int stageIndex = 0; stageIndex < stageNames.Length; stageIndex++)
            {
                StageNames stageName = stageNames[stageIndex];
                int stageTID = stageName.ToInt();
                if (!_stages.ContainsKey(stageTID))
                {
                    Log.Progress(LogTags.ScriptableData, "스테이지 에셋이 설정되지 않았습니다. {0}({1})", stageName, stageName.ToLogString());
                    continue;
                }

                StageAsset stageAsset = _stages[stageTID];
                if (!stageAsset.IsValid())
                {
                    continue;
                }

                for (int buildIndex = 0; buildIndex < buildTypes.Length; buildIndex++)
                {
                    BuildTypes buildType = buildTypes[buildIndex];
                    if (!stageAsset.Data.SupportedBuildTypes.Contains(buildType))
                    {
                        continue;
                    }

                    switch (buildType)
                    {
                        case BuildTypes.Editor:
                            stagesForEditor.Add(stageAsset.Data.Area, stageName);
                            break;

                        case BuildTypes.Development:
                            stagesForDevelopment.Add(stageAsset.Data.Area, stageName);
                            break;

                        case BuildTypes.Live:
                            stagesForLive.Add(stageAsset.Data.Area, stageName);
                            break;
                    }
                }
            }

            LogBattleStageByBuildType(areaNames, stagesForEditor, BuildTypes.Editor);
            LogBattleStageByBuildType(areaNames, stagesForDevelopment, BuildTypes.Development);
            LogBattleStageByBuildType(areaNames, stagesForLive, BuildTypes.Live);
#endif
        }

        /// <summary>
        /// 빌드 타입별 전투 스테이지를 로그로 출력합니다.
        /// </summary>
        private void LogBattleStageByBuildType(AreaNames[] areaNames, ListMultiMap<AreaNames, StageNames> stageMultimap, BuildTypes buildType)
        {
            StringBuilder sb = new();
            for (int areaIndex = 0; areaIndex < areaNames.Length; areaIndex++)
            {
                AreaNames areaName = areaNames[areaIndex];
                if (!_areas.ContainsKey(areaName.ToInt()))
                {
                    Log.Warning(LogTags.Stage, "{0} 빌드에서 {1} 지역의 지역 에셋을 찾을 수 없습니다.", buildType, areaName.ToLogString());
                    continue;
                }

                AreaAsset areaAsset = _areas[areaName.ToInt()];
                if (!areaAsset.IsValid() || areaAsset.Data.IsBlock)
                {
                    Log.Warning(LogTags.Stage, "{0} 빌드에서 {1} 지역의 정보가 올바르지 않거나 차단되었습니다.", buildType, areaName.ToLogString());
                    continue;
                }

                if (!stageMultimap.TryGetValue(areaName, out List<StageNames> stageList))
                {
                    Log.Warning(LogTags.Stage, "{0} 빌드에서 {1} 지역의 스테이지 정보를 찾을 수 없습니다.", buildType, areaName.ToLogString());
                    continue;
                }

                _ = sb.Clear();
                _ = sb.AppendFormat("{0} 빌드에서 사용하는 {1} 지역 스테이지: ", buildType, areaName.ToLogString());

                Dictionary<int, int> battleStageCount = new();
                for (int stageIndex = 0; stageIndex < stageList.Count; stageIndex++)
                {
                    StageNames stageName = stageList[stageIndex];
                    int stageTID = stageName.ToInt();
                    StageAsset stageAsset = _stages[stageTID];

                    for (int k = 0; k < areaAsset.Data.BattleStageCount; k++)
                    {
                        int order = k + 1;
                        if (areaAsset.Data.IncludesSlot(order))
                        {
                            if (!battleStageCount.ContainsKey(order))
                            {
                                battleStageCount.Add(order, 1);
                            }
                            else
                            {
                                battleStageCount[order] += 1;
                            }
                        }
                    }

                    _ = sb.Append($"{stageName.ToLogString()}, ");
                }

                Log.Progress(LogTags.Stage, sb.ToString());

                if (areaAsset.Data.IsBattleArea)
                {
                    for (int battleStageIndex = 0; battleStageIndex < areaAsset.Data.BattleStageCount; battleStageIndex++)
                    {
                        int order = battleStageIndex + 1;
                        if (!battleStageCount.ContainsKey(order))
                        {
                            Log.Error($"{buildType.ToSelectString()} 빌드에서 {areaName.ToLogString()} 지역의 {order.ToSelectString()} 번째 전투 스테이지가 설정되지 않았습니다.");
                        }
                    }
                }
            }
        }

        #endregion Stage Validation Methods
    }
}