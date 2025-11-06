using System;
using System.Collections.Generic;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VCharacterStage
    {
        [NonSerialized]
        public List<StageNames> Stages = new();
        public List<string> StageStrings = new();

        [NonSerialized]
        public StageNames CurrentStage;
        public string CurrentStageString;

        public void OnLoadGameData()
        {
            EnumEx.ConvertTo(ref Stages, StageStrings);
            EnumEx.ConvertTo(ref CurrentStage, CurrentStageString);
        }

        public void ClearIngameData()
        {
        }

        public void Register(StageNames stageName)
        {
            if (!Stages.Contains(stageName))
            {
                Stages.Add(stageName);
                string stageString = stageName.ToString();
                if (!StageStrings.Contains(stageString))
                {
                    StageStrings.Add(stageString);
                }
            }
        }

        public void Unregister(StageNames stageName)
        {
            if (Stages.Contains(stageName))
            {
                Stages.Remove(stageName);
                string stageString = stageName.ToString();
                if (StageStrings.Contains(stageString))
                {
                    StageStrings.Remove(stageString);
                }
            }
        }

        public void Select(StageNames stageName)
        {
            CurrentStage = stageName;
            CurrentStageString = stageName.ToString();
        }

        public static VCharacterStage CreateDefault()
        {
            return new VCharacterStage()
            {
                CurrentStage = StageNames.Stage1,
                CurrentStageString = StageNames.Stage1.ToString(),
            };
        }
    }
}