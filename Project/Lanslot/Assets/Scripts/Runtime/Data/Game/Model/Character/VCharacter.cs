using System;
using System.Collections.Generic;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public partial class VCharacter
    {
        [NonSerialized] public List<CharacterNames> CharacterNames = new List<CharacterNames>();
        [NonSerialized] public CharacterNames SelectedCharacterName;

        public List<string> Characters = new List<string>();
        public string SelectedCharacter;

        public void OnLoadGameData()
        {
            EnumEx.ConvertTo(ref CharacterNames, Characters);
            EnumEx.ConvertTo(ref SelectedCharacterName, SelectedCharacter);
        }

        public void ClearIngameData()
        {
        }

        public void Register(CharacterNames characterName)
        {
            string key = characterName.ToString();
            if (!Characters.Contains(key))
            {
                Characters.Add(key);
                CharacterNames.Add(characterName);

                Log.Info(LogTags.GameData, "{0} 캐릭터를 추가합니다. 캐릭터 수: {1}", characterName.ToLogString(), Characters.Count);

                GlobalEvent<int>.Send(GlobalEventType.PLAYER_CHARACTER_ADDED, Characters.Count);
            }
        }

        public void Unregister(CharacterNames characterName)
        {
            string key = characterName.ToString();
            if (Characters.Contains(key))
            {
                Characters.Remove(key);
                CharacterNames.Remove(characterName);

                Log.Info(LogTags.GameData, "{0} 캐릭터를 삭제합니다. 캐릭터 수: {1}", characterName.ToLogString(), Characters.Count);
            }
        }

        public void Select(CharacterNames characterName)
        {
            SelectedCharacter = characterName.ToString();
            SelectedCharacterName = characterName;
            Log.Info(LogTags.GameData, $"[Character] 캐릭터를 선택합니다. {characterName}");
        }
    }
}