using System;
using System.Collections.Generic;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public partial class VCharacter
    {
        [NonSerialized] public List<CharacterNames> PlayerCharacterNames = new List<CharacterNames>();
        [NonSerialized] public CharacterNames SelectedCharacterName;

        public List<string> CharacterStrings = new List<string>();
        public string SelectedCharacterString;

        public void OnLoadGameData()
        {
            EnumEx.ConvertTo(ref PlayerCharacterNames, CharacterStrings);
            EnumEx.ConvertTo(ref SelectedCharacterName, SelectedCharacterString);
        }

        public void ClearIngameData()
        {
        }

        public void Register(CharacterNames characterName)
        {
            string key = characterName.ToString();
            if (!CharacterStrings.Contains(key))
            {
                CharacterStrings.Add(key);
                PlayerCharacterNames.Add(characterName);

                Log.Info(LogTags.GameData, "{0} 캐릭터를 추가합니다. 캐릭터 수: {1}", characterName.ToLogString(), CharacterStrings.Count);

                GlobalEvent<int>.Send(GlobalEventType.PLAYER_CHARACTER_ADDED, CharacterStrings.Count);
            }
        }

        public void Unregister(CharacterNames characterName)
        {
            string key = characterName.ToString();
            if (CharacterStrings.Contains(key))
            {
                CharacterStrings.Remove(key);
                PlayerCharacterNames.Remove(characterName);

                Log.Info(LogTags.GameData, "{0} 캐릭터를 삭제합니다. 캐릭터 수: {1}", characterName.ToLogString(), CharacterStrings.Count);
            }
        }

        public void Select(CharacterNames characterName)
        {
            SelectedCharacterString = characterName.ToString();
            SelectedCharacterName = characterName;
            Log.Info(LogTags.GameData, $"[Character] 캐릭터를 선택합니다. {characterName}");
        }
    }
}