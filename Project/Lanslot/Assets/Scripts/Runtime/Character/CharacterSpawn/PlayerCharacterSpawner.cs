using System.Threading.Tasks;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat
{
    public class PlayerCharacterSpawner : MonoBehaviour
    {
        public void StartSpawnCharacter()
        {
            _ = SpawnCharacterAsync();
        }

        private async Task SpawnCharacterAsync()
        {
            VProfile profileInfo = GameApp.GetSelectedProfile();
            if (profileInfo != null)
            {
                if (profileInfo.CharacterName != CharacterNames.None)
                {
                    string prefabName = profileInfo.CharacterName.ToString();
                    PlayerCharacter character = await ResourcesManager.SpawnPrefabAsync<PlayerCharacter>(prefabName, transform);

                    if (character != null)
                    {
                        Log.Info(LogTags.Character, "플레이어 캐릭터를 성공적으로 스폰했습니다: {0}", prefabName);
                    }
                    else
                    {
                        Log.Error(LogTags.Character, "플레이어 캐릭터 스폰에 실패했습니다: {0}", prefabName);
                    }
                }
            }
        }
    }
}