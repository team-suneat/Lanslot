using System.Collections.Generic;
using System.Linq;
using TeamSuneat;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 캐릭터 스탯 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {


        /// <summary>
        /// 캐릭터 스탯 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadCharacterStatSync(string filePath)
        {
            if (!filePath.Contains("CharacterStat_"))
            {
                return false;
            }

            CharacterStatAsset asset = ResourcesManager.LoadResource<CharacterStatAsset>(filePath);
            if (asset != null)
            {
                int characterKey = BitConvert.Enum32ToInt(asset.Name);
                if (characterKey == 0)
                {
                    Log.Warning(LogTags.ScriptableData, "{0}, 캐릭터 이름이 설정되어있지 않습니다. {1}", asset.name, filePath);
                }
                else if (_characterStats.ContainsKey(characterKey))
                {
                    Log.Warning(LogTags.ScriptableData, "같은 캐릭터로 중복 CharacterStat이 로드 되고 있습니다. Character: {0}, 기존: {1}, 새로운 이름: {2}",
                         asset.Name, _characterStats[characterKey].name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _characterStats[characterKey] = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }


        /// <summary>
        /// 모든 캐릭터 스탯 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllCharacterStats()
        {
            foreach (KeyValuePair<int, CharacterStatAsset> item in _characterStats)
            {
                Refresh(item.Value);
            }
        }

        /// <summary>
        /// 특정 캐릭터 스탯 에셋을 리프레시합니다.
        /// </summary>
        private void Refresh(CharacterStatAsset characterStatAsset)
        {
            characterStatAsset?.Refresh();
        }

        /// <summary>
        /// 캐릭터 스탯 에셋을 가져옵니다.
        /// </summary>
        public CharacterStatAsset GetCharacterStatAsset(CharacterNames characterName)
        {
            int key = BitConvert.Enum32ToInt(characterName);
            return _characterStats.TryGetValue(key, out var asset) ? asset : null;
        }


    }
}