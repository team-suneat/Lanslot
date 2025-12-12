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

        public bool IsUnlocked(CharacterNames characterName)
        {
            VCharacterInfo characterInfo = GetCharacterInfo(characterName);
            return characterInfo != null && characterInfo.IsUnlocked();
        }

        public bool IsPurchased(CharacterNames characterName)
        {
            VCharacterInfo characterInfo = GetCharacterInfo(characterName);
            return characterInfo != null && characterInfo.IsPurchased();
        }

        public VCharacterInfo GetCharacterInfo(CharacterNames characterName)
        {
            string key = characterName.ToString();
            UnlockedCharacters.TryGetValue(key, out VCharacterInfo characterInfo);
            return characterInfo;
        }

        public void Unlock(CharacterNames characterName)
        {
            VCharacterInfo characterInfo = GetOrCreateCharacterInfo(characterName);
            if (characterInfo.State == CharacterState.Locked)
            {
                characterInfo.SetState(CharacterState.Unlocked);
                Log.Info(LogTags.GameData_Character, "{0} 캐릭터를 추가합니다. 캐릭터 수: {1}", characterName.ToLogString(), UnlockedCharacters.Count);
                GlobalEvent<int>.Send(GlobalEventType.PLAYER_CHARACTER_ADDED, UnlockedCharacters.Count);
            }
        }

        public bool Purchase(CharacterNames characterName)
        {
            VCharacterInfo characterInfo = GetOrCreateCharacterInfo(characterName);
            if (!characterInfo.IsUnlocked())
            {
                Log.Info(LogTags.GameData_Character, $"해금되지 않은 캐릭터는 구매할 수 없습니다. {characterName}");
                return false;
            }

            if (characterInfo.IsPurchased())
            {
                return false;
            }

            characterInfo.SetState(CharacterState.Purchased);
            Log.Info(LogTags.GameData_Character, $"캐릭터를 구매합니다. {characterName}");
            return true;
        }

        public void Select(CharacterNames characterName)
        {
            VCharacterInfo characterInfo = GetCharacterInfo(characterName);
            if (characterInfo != null && characterInfo.IsPurchased())
            {
                SelectedCharacterString = characterName.ToString();
                SelectedCharacterName = characterName;
                Log.Info(LogTags.GameData_Character, $"캐릭터를 선택합니다. {characterName}");
            }
            else
            {
                Log.Info(LogTags.GameData_Character, $"해금되지 않은 캐릭터를 선택할 수 없습니다. {characterName}");
            }
        }

        public int AddRankExperience(CharacterNames characterName, int experience)
        {
            VCharacterInfo characterInfo = GetCharacterInfo(characterName);
            if (characterInfo == null)
            {
                Log.Warning(LogTags.GameData_Character, "{0} 캐릭터를 찾을 수 없습니다.", characterName.ToLogString());
                return 0;
            }

            return characterInfo.AddRankExperience(experience);
        }

        public void AddPlayCount(CharacterNames characterName, int count = 1)
        {
            VCharacterInfo characterInfo = GetCharacterInfo(characterName);
            if (characterInfo == null)
            {
                Log.Warning(LogTags.GameData_Character, "{0} 캐릭터를 찾을 수 없습니다.", characterName.ToLogString());
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

            for (int i = 0; i < GameDefine.DEFAULT_UNLOCKED_CHARACTERS.Length; i++)
            {
                CharacterNames characterName = GameDefine.DEFAULT_UNLOCKED_CHARACTERS[i];
                defaultCharacter.Unlock(characterName);
                defaultCharacter.Purchase(characterName);
            }

            defaultCharacter.Unlock(CharacterNames.BloodRaven);

            return defaultCharacter;
        }

        private VCharacterInfo GetOrCreateCharacterInfo(CharacterNames characterName)
        {
            string key = characterName.ToString();
            if (!UnlockedCharacters.TryGetValue(key, out VCharacterInfo characterInfo))
            {
                characterInfo = new VCharacterInfo(characterName);
                UnlockedCharacters.Add(key, characterInfo);
            }

            return characterInfo;
        }
    }
}