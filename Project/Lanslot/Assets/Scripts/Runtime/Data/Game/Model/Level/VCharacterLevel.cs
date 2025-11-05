using UnityEngine;

namespace TeamSuneat.Data.Game
{
    public class VCharacterLevel
    {
        public int Level;
        public int Experience;

        public void ResetValues()
        {
            Level = 1;
            Experience = 0;

            Log.Info(LogTags.GameData, "[Character] 플레이어 캐릭터의 레벨과 경험치를 초기화합니다.");
        }

        /// <returns>Current Level</returns>
        public int AddExperience(int experience)
        {
            if (CharacterManager.Instance.Player == null)
            {
                return 0;
            }

            CharacterLevelExpData data = JsonDataManager.FindCharacterLevelExpDataClone(Level);
            int nextExperience = data.RequiredExperience;
            float increasedExperience = CharacterManager.Instance.Player.Stat.FindValueOrDefault(StatNames.XPGain);

            Log.Info(LogTags.GameData, "[Character] 받는 경험치 획득량을 결정합니다. EXP: {0}, 추가 획득량 {1}", experience, increasedExperience);

            int resultLevel = 0;
            Experience += Mathf.RoundToInt(experience * increasedExperience);
            if (Experience >= nextExperience)
            {
                int overflowExperience = Experience - nextExperience;
                Experience = overflowExperience;
                resultLevel = LevelUp();
            }

            _ = GlobalEvent<int, int>.Send(GlobalEventType.GAME_DATA_CHARACTER_ADD_EXPERIENCE, Experience, nextExperience);
            return resultLevel;
        }

        public int LevelUp()
        {
            return LevelUp(1);
        }

        public int LevelUp(int addLevel)
        {
            if (Level >= GameDefine.CHARACTER_MAX_LEVEL)
            {
                return 0;
            }

            int addedLevel = 0;
            for (int i = 0; i < addLevel; i++)
            {
                if (Level < GameDefine.CHARACTER_MAX_LEVEL)
                {
                    Level += 1;
                    addedLevel += 1;
                    Log.Info(LogTags.GameData, "[Character] 플레이어 캐릭터의 레벨이 올랐습니다: Lv.{0}", Level.ToString());
                }
                else
                {
                    break;
                }
            }

            if (CharacterManager.Instance.Player != null)
            {
                CharacterManager.Instance.Player.OnLevelup(addedLevel);
            }

            _ = GlobalEvent<int>.Send(GlobalEventType.GAME_DATA_CHARACTER_LEVEL_CHANGED, Level);
            return addedLevel;
        }

        public void LevelDown()
        {
            Level = Mathf.Max(Level - 1, 1);
            Log.Info(LogTags.GameData, "[Character] 플레이어 캐릭터의 레벨이 떨어졌습니다. {0}", Level.ToString());
            CharacterManager.Instance.Player.OnLevelDown();
            _ = GlobalEvent<int>.Send(GlobalEventType.GAME_DATA_CHARACTER_LEVEL_CHANGED, Level);
        }

        public static VCharacterLevel CreateDefault()
        {
            return new VCharacterLevel()
            {
                Level = 1,
                Experience = 0,
            };
        }
    }
}