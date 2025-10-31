using UnityEngine;

namespace TeamSuneat
{
    public partial class ResourcesManager
    {
        public static StageSystem SpawnStage(StageNames stageName, Transform parent)
        {
            string prefabName = stageName.ToString();
            StageSystem stage = SpawnPrefab<StageSystem>(stageName.ToString(), parent);

            if (stage != null)
            {
                Log.Info(LogTags.Resource, "스테이지 프리펩을 성공적으로 스폰했습니다: {0}", prefabName);
            }
            else
            {
                Log.Error(LogTags.Resource, "스테이지 프리펩 스폰에 실패했습니다: {0}", prefabName);
            }

            return stage;
        }
    }
}