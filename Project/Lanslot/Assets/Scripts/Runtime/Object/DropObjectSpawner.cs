using UnityEngine;

namespace TeamSuneat
{
    public class DropObjectSpawner : MonoBehaviour
    {
        public DropEXP SpawnDropEXP(Vector3 spawnPosition, int expAmount = 1)
        {
            DropEXP dropEXP = ResourcesManager.SpawnDropExp(spawnPosition);
            if (dropEXP == null)
            {
                Log.Warning(LogTags.DropObject, "사용 가능한 DropEXP가 없습니다.");
                return null;
            }

            dropEXP.transform.position = spawnPosition;
            dropEXP.SetExpAmount(expAmount);
            dropEXP.Initialize();
            dropEXP.Activate();

            Log.Info(LogTags.DropObject, "DropEXP 생성됨 - 위치: {0}, 경험치: {1}", spawnPosition, expAmount);

            return dropEXP;
        }
    }
}