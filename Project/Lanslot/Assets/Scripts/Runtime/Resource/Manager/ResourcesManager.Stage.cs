using UnityEngine;

namespace TeamSuneat
{
    public partial class ResourcesManager
    {
        internal static StageSystem SpawnStage(StageNames stageName, Transform point)
        {
            return SpawnPrefab<StageSystem>(stageName.ToString(), point);
        }
    }
}