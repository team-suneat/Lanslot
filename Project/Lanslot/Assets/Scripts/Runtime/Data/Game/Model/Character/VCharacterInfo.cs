using System;
using TeamSuneat.Data;

namespace TeamSuneat.Data.Game
{
    /// <summary>
    /// 캐릭터 정보를 저장하는 클래스 (이름, 랭크, 랭크 경험치, 플레이 횟수)
    /// </summary>
    [System.Serializable]
    public class VCharacterInfo
    {
        [NonSerialized]
        public CharacterNames CharacterName;
        public string CharacterNameString;

        public int Rank;
        public int RankExperience;
        public int PlayCount;

        public VCharacterInfo()
        {
            Rank = 1;
            RankExperience = 0;
            PlayCount = 0;
        }

        public VCharacterInfo(CharacterNames characterName)
        {
            CharacterName = characterName;
            CharacterNameString = characterName.ToString();
            Rank = 1;
            RankExperience = 0;
            PlayCount = 0;
        }

        public void OnLoadGameData()
        {
            _ = EnumEx.ConvertTo(ref CharacterName, CharacterNameString);
        }

        /// <summary>
        /// 랭크 경험치를 추가하고 필요 시 랭크업을 수행합니다.
        /// </summary>
        /// <param name="experience">추가할 경험치</param>
        /// <returns>증가한 랭크 수</returns>
        public int AddRankExperience(int experience)
        {
            if (experience <= 0)
            {
                return 0;
            }

            int addedRank = 0;
            RankExperience += experience;

            // 여러 번의 랭크업이 가능하도록 루프 처리
            while (true)
            {
                CharacterRankExpData rankExpData = JsonDataManager.FindCharacterRankExpDataClone(Rank);
                
                // 다음 랭크 데이터가 없거나 필요 경험치가 0이면 최대 랭크에 도달한 것으로 간주
                if (rankExpData == null || rankExpData.RequiredExperience <= 0)
                {
                    break;
                }

                // 현재 경험치가 필요 경험치 이상이면 랭크업
                if (RankExperience >= rankExpData.RequiredExperience)
                {
                    RankExperience -= rankExpData.RequiredExperience;
                    Rank++;
                    addedRank++;

                    Log.Info(LogTags.GameData, "[Character] {0} 캐릭터의 랭크가 올랐습니다. 랭크: {1}, 남은 경험치: {2}", 
                        CharacterName.ToLogString(), Rank, RankExperience);
                }
                else
                {
                    // 경험치 부족하면 종료
                    break;
                }
            }

            if (addedRank > 0)
            {
                Log.Info(LogTags.GameData, "[Character] {0} 캐릭터가 {1} 랭크 상승했습니다. 현재 랭크: {2}", 
                    CharacterName.ToLogString(), addedRank, Rank);
            }

            return addedRank;
        }

        /// <summary>
        /// 플레이 횟수를 증가시킵니다.
        /// </summary>
        /// <param name="count">증가시킬 횟수 (기본값: 1)</param>
        public void AddPlayCount(int count = 1)
        {
            if (count <= 0)
            {
                return;
            }

            PlayCount += count;
            Log.Info(LogTags.GameData, "[Character] {0} 캐릭터의 플레이 횟수가 증가했습니다. 현재 플레이 횟수: {1}", 
                CharacterName.ToLogString(), PlayCount);
        }
    }
}

