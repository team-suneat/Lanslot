using System;
using System.Collections.Generic;

namespace TeamSuneat.Data
{
    [Serializable]
    public class VArea
    {
        /// <summary>
        /// 지역 이름 (Key)
        /// </summary>
        [NonSerialized]
        public AreaNames AreaName;

        public string AreaNameString;

        [NonSerialized]
        public StageNames[] StageNamesInArea;

        public string[] StageNamesStringInArea;

        public bool IsVisited;

        #region 생성자

        public VArea()
        { }

        public VArea(AreaNames areaName, StageNames[] stageNames)
        {
            AreaName = areaName;
            AreaNameString = areaName.ToString();

            if (stageNames != null)
            {
                StageNamesInArea = new StageNames[stageNames.Length];
                StageNamesStringInArea = new string[stageNames.Length];

                for (int i = 0; i < stageNames.Length; i++)
                {
                    StageNamesInArea[i] = stageNames[i];
                    StageNamesStringInArea[i] = stageNames[i].ToString();
                }
            }
        }

        public VArea(AreaNames areaName, List<StageAssetData> stageDatas)
        {
            AreaName = areaName;
            AreaNameString = areaName.ToString();

            if (stageDatas != null)
            {
                StageNamesInArea = new StageNames[stageDatas.Count];
                StageNamesStringInArea = new string[stageDatas.Count];

                for (int i = 0; i < stageDatas.Count; i++)
                {
                    StageNamesInArea[i] = stageDatas[i].Name;
                    StageNamesStringInArea[i] = stageDatas[i].Name.ToString();
                }
            }
        }

        #endregion 생성자

        #region 데이터 로드 및 변환

        public void OnLoadGameData()
        {
            EnumEx.ConvertTo(ref AreaName, AreaNameString);
            EnumEx.ConvertTo(ref StageNamesInArea, StageNamesStringInArea);

            SortStagesByOrder();
        }

        private void SortStagesByOrder()
        {
            if (!StageNamesInArea.IsValid())
            {
                return;
            }

            // 스테이지 데이터를 Order로 정렬하기 위한 임시 리스트 생성
            List<(StageNames stageName, string stageNameString, int order)> stageDataList = new();

            for (int i = 0; i < StageNamesInArea.Length; i++)
            {
                StageNames stageName = StageNamesInArea[i];
                string stageNameString = StageNamesStringInArea[i];
                StageAsset stageAsset = ScriptableDataManager.Instance.FindStage(stageName);
                if (stageAsset.IsValid())
                {
                    stageDataList.Add((stageName, stageNameString, stageAsset.Data.Order));
                }
                else
                {
                    // 스테이지 에셋을 찾을 수 없는 경우 기본값 사용
                    stageDataList.Add((stageName, stageNameString, 0));
                }
            }

            // Order 값에 따라 정렬
            stageDataList.Sort((a, b) => a.order.CompareTo(b.order));

            // 정렬된 결과를 다시 배열에 저장
            for (int i = 0; i < stageDataList.Count; i++)
            {
                StageNamesInArea[i] = stageDataList[i].stageName;
                StageNamesStringInArea[i] = stageDataList[i].stageNameString;
            }

            Log.Progress(LogTags.GameData_Stage, $"지역({AreaName})의 스테이지들을 Order 순서로 정렬했습니다.\n스테이지: {StageNamesInArea.JoinToString()}");
        }

        #endregion 데이터 로드 및 변환

        #region 스테이지 조회

        public int GetStageIndex(StageNames stageName)
        {
            if (StageNamesInArea.IsValid())
            {
                for (int i = 0; i < StageNamesInArea.Length; i++)
                {
                    if (StageNamesInArea[i] == stageName)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public StageNames FindAt(int index)
        {
            if (StageNamesInArea != null && StageNamesInArea.Length > index)
            {
                return StageNamesInArea[index];
            }

            return StageNames.None;
        }

        public StageNames GetFirstStageName()
        {
            if (StageNamesInArea.IsValid())
            {
                return StageNamesInArea[0];
            }
            else
            {
                Log.Error("지역 내 스테이지가 설정되지 않았습니다.");
            }

            return StageNames.None;
        }

        public StageNames GetLastStageName()
        {
            if (StageNamesInArea.IsValid())
            {
                return StageNamesInArea[StageNamesInArea.Length - 1];
            }
            else
            {
                Log.Error("지역 내 스테이지가 설정되지 않았습니다.");
            }

            return StageNames.None;
        }

        public StageNames GetNextStageName(StageNames currentStageName)
        {
            if (StageNamesInArea != null)
            {
                for (int i = 0; i < StageNamesInArea.Length; i++)
                {
                    if (StageNamesInArea[i] == currentStageName)
                    {
                        if (StageNamesInArea.Length > i + 1)
                        {
                            Log.Progress(LogTags.GameData_Stage, "현재 지역({1}:{2}) 내 현재 스테이지의 다음 스테이지를 반환합니다. {0} ▶ {3}"
                                , StageNamesInArea[i], AreaName, AreaName.ToLogString(), StageNamesInArea[i + 1]);

                            return StageNamesInArea[i + 1];
                        }
                        else
                        {
                            Log.Progress(LogTags.GameData_Stage, "현재 스테이지({0})가 현재 지역({1}:{2})의 마지막 스테이지입니다. 지역 내 다음 스테이지를 반환할 수 없습니다."
                                , StageNamesInArea[i], AreaName, AreaName.ToLogString());
                        }
                    }
                }
            }

            return StageNames.None;
        }

        #endregion 스테이지 조회
    }
}