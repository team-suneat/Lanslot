using System.Collections.Generic;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public class VCharacterStat
    {
        public int CurrentLife;
        public int CurrentShield;

        public int MaxLife;
        public int MaxShield;

        public int UseDeathDefianceCount;
        public List<string> UseDeathDefianceSources = new List<string>();

        // 사용하지 않음.
        public List<VStat> SynergyStats = new();

        public void OnLoadGameData()
        {
            Log.Info(LogTags.GameData_BattleResource, "세이브 데이터에 플레이어 캐릭터의 생명력을 불러옵니다. {0}/{1}", CurrentLife, MaxLife);
            Log.Info(LogTags.GameData_BattleResource, "세이브 데이터에 플레이어 캐릭터의 보호막을 불러옵니다. {0}/{1}", CurrentShield, MaxShield);

            Log.Info(LogTags.GameData_BattleResource, "세이브 데이터에 플레이어 캐릭터의 사용한 죽음 저항 횟수를 불러옵니다. {0}", UseDeathDefianceCount);
            Log.Info(LogTags.GameData_BattleResource, "세이브 데이터에 플레이어 캐릭터의 사용한 죽음 저항 출처를 불러옵니다. {0}", UseDeathDefianceSources.JoinToString());
        }

        public void SaveVitalValues()
        {
            PlayerCharacter playerCharacter = CharacterManager.Instance.Player;
            if (playerCharacter != null)
            {
                CurrentLife = playerCharacter.MyVital.CurrentLife;
                CurrentShield = playerCharacter.MyVital.CurrentShield;

                MaxLife = playerCharacter.MyVital.MaxLife;
                MaxShield = playerCharacter.MyVital.MaxShield;

                Log.Info(LogTags.GameData_BattleResource, "세이브 데이터에 플레이어 캐릭터의 모든 자원을 저장합니다. 생명력:{0}/{1}, 보호막:{2}/{3}",
                    CurrentLife, MaxLife, CurrentShield, MaxShield);
            }
        }

        public void ResetCurrentVitalValue()
        {
            Log.Info(LogTags.GameData_BattleResource, "세이브 데이터에 플레이어 캐릭터의 생명력, 보호막, 전투 자원을 초기화합니다. 생명력:{0}/{1}, 보호막:{2}/{3}", CurrentLife, MaxLife, CurrentShield, MaxShield);
            Log.Info(LogTags.GameData_BattleResource, "세이브 데이터에 플레이어 캐릭터의 사용한 죽음 저항 횟수/출처를 초기화합니다. {0}, {1}", UseDeathDefianceCount, UseDeathDefianceSources.JoinToString());

            CurrentLife = 0;
            CurrentShield = 0;

            MaxLife = 0;
            MaxShield = 0;

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
    }
}