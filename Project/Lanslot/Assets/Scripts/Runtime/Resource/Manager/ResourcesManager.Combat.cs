using UnityEngine;

namespace TeamSuneat
{
    public partial class ResourcesManager
    {
        internal static MonsterCharacter SpawnMonsterCharacter(CharacterNames characterName, Transform parent)
        {
            string prefabName = characterName.ToString();
            MonsterCharacter monster = SpawnPrefab<MonsterCharacter>(prefabName, parent);
            if (monster != null)
            {
                monster.ResetLocalTransform();
            }

            return monster;
        }

        internal static DropEXP SpawnDropExp(Vector3 spawnPosition)
        {
            return SpawnPrefab<DropEXP>("DropEXP", spawnPosition);
        }
    }
}