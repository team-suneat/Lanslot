using System.Collections.Generic;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public class VCharacterStat
    {
        public int UseDeathDefianceCount;
        public List<string> UseDeathDefianceSources = new List<string>();

        public void OnLoadGameData()
        {
            Log.Info(LogTags.GameData_BattleResource, "세이브 데이터에 플레이어 캐릭터의 사용한 죽음 저항 횟수를 불러옵니다. {0}", UseDeathDefianceCount);
            Log.Info(LogTags.GameData_BattleResource, "세이브 데이터에 플레이어 캐릭터의 사용한 죽음 저항 출처를 불러옵니다. {0}", UseDeathDefianceSources.JoinToString());
        }

        public void ClearIngameData()
        {
            UseDeathDefianceCount = 0;
            UseDeathDefianceSources.Clear();
        }

        public void AddDeathDefianceCount(int value)
        {
            UseDeathDefianceCount += value;

            Log.Info(LogTags.GameData_BattleResource, "세이브 데이터에 플레이어 캐릭터의 사용한 죽음 저항 횟수를 {0} 추가합니다. 총 죽음 저항 횟수: {1}", value.ToSelectString(), UseDeathDefianceCount);
        }

        public void RegisterDeathDefianceSource(string sourceName)
        {
            if (UseDeathDefianceSources.Contains(sourceName))
            {
                Log.Error("등록된 죽음 저항 출처를 다시 등록하려합니다. 등록에 실패했습니다: {0}", sourceName.ToSelectString());
                return;
            }

            UseDeathDefianceSources.Add(sourceName);
        }

        public bool ContainsDeathDefianceSource(string sourceName)
        {
            return UseDeathDefianceSources.Contains(sourceName);
        }

        public static VCharacterStat CreateDefault()
        {
            return new VCharacterStat(); 
        }
    }
}