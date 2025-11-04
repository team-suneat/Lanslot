using System;
using System.Collections.Generic;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public partial class VCharacter
    {
        public Dictionary<string, VCharacterInfo> UnlockedCharacters = new();

        [NonSerialized]
        public CharacterNames SelectedCharacterName;
        public string SelectedCharacterString;

        public void OnLoadGameData()
        {
            _ = EnumEx.ConvertTo(ref SelectedCharacterName, SelectedCharacterString);

            // 딕셔너리 내부의 캐릭터 정보들도 로드
            foreach (VCharacterInfo characterInfo in UnlockedCharacters.Values)
            {
                characterInfo.OnLoadGameData();
            }
        }

        public void ClearIngameData()
        {
        }

        public bool Contains(CharacterNames characterName)
        {
            string key = characterName.ToString();
            return UnlockedCharacters.ContainsKey(key);
        }

        public VCharacterInfo GetCharacterInfo(CharacterNames characterName)
        {
            string key = characterName.ToString();
            UnlockedCharacters.TryGetValue(key, out VCharacterInfo characterInfo);
            return characterInfo;
        }

        public void Unlock(CharacterNames characterName)
        {
            string key = characterName.ToString();
            if (!UnlockedCharacters.ContainsKey(key))
            {
                VCharacterInfo characterInfo = new VCharacterInfo(characterName);
                UnlockedCharacters.Add(key, characterInfo);
                Log.Info(LogTags.GameData, "{0} 캐릭터를 추가합니다. 캐릭터 수: {1}", characterName.ToLogString(), UnlockedCharacters.Count);
                GlobalEvent<int>.Send(GlobalEventType.PLAYER_CHARACTER_ADDED, UnlockedCharacters.Count);
            }
        }

        public void Select(CharacterNames characterName)
        {
            if (Contains(characterName))
            {
                SelectedCharacterString = characterName.ToString();
                SelectedCharacterName = characterName;
                Log.Info(LogTags.GameData, $"[Character] 캐릭터를 선택합니다. {characterName}");
            }
            else
            {
                Log.Info(LogTags.GameData, $"[Character] 해금되지 않은 캐릭터를 선택할 수 없습니다. {characterName}");
            }
        }

        /// <summary>
        /// 특정 캐릭터의 랭크 경험치를 추가합니다.
        /// </summary>
        /// <param name="characterName">캐릭터 이름</param>
        /// <param name="experience">추가할 경험치</param>
        /// <returns>증가한 랭크 수 (캐릭터가 없거나 해금되지 않은 경우 0)</returns>
        public int AddRankExperience(CharacterNames characterName, int experience)
        {
            VCharacterInfo characterInfo = GetCharacterInfo(characterName);
            if (characterInfo == null)
            {
                Log.Warning(LogTags.GameData, "[Character] {0} 캐릭터를 찾을 수 없습니다.", characterName.ToLogString());
                return 0;
            }

            return characterInfo.AddRankExperience(experience);
        }

        /// <summary>
        /// 특정 캐릭터의 플레이 횟수를 증가시킵니다.
        /// </summary>
        /// <param name="characterName">캐릭터 이름</param>
        /// <param name="count">증가시킬 횟수 (기본값: 1)</param>
        public void AddPlayCount(CharacterNames characterName, int count = 1)
        {
            VCharacterInfo characterInfo = GetCharacterInfo(characterName);
            if (characterInfo == null)
            {
                Log.Warning(LogTags.GameData, "[Character] {0} 캐릭터를 찾을 수 없습니다.", characterName.ToLogString());
                return;
            }

            characterInfo.AddPlayCount(count);
        }

        public static VCharacter CreateDefault()
        {
            VCharacter defaultCharacter = new()
            {
                UnlockedCharacters = new Dictionary<string, VCharacterInfo>()
            };

            // 기본 캐릭터들 추가
            defaultCharacter.Unlock(CharacterNames.IronWarden);
            defaultCharacter.Unlock(CharacterNames.ShadowAssassin);
            defaultCharacter.Unlock(CharacterNames.BloodRaven);

            return defaultCharacter;
        }
    }
}