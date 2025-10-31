using UnityEngine;

namespace TeamSuneat
{
    public partial class ResourcesManager
    {
        internal static DropEXP SpawnDropExp(Vector3 spawnPosition)
        {
            return SpawnPrefab<DropEXP>("DropEXP", spawnPosition);
        }
    }
}