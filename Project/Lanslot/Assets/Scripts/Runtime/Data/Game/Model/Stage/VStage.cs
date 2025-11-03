using System;
using System.Collections.Generic;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VStage
    {
        [NonSerialized]
        public List<StageNames> Stages = new();
        public List<string> StageStrings = new();

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
            }

            if (!StageStrings.Contains(stageName.ToString()))
            {
                StageStrings.Add(stageName.ToString());
            }
        }

        public void Unregister(StageNames stageName)
        {
            if (Stages.Contains(stageName))
            {
                Stages.Remove(stageName);
            }

            if (StageStrings.Contains(stageName.ToString()))
            {
                StageStrings.Remove(stageName.ToString());
            }
        }

        public void Select(StageNames stageName)
        {
            CurrentStage = stageName;
            CurrentStageString = stageName.ToString();
        }
    }
}