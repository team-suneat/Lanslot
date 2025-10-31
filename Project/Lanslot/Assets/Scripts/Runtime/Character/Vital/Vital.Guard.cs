using UnityEngine;

namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        // Guard : 일정 시간 피해를 입지않는 상태를 의미합니다.
        // GuardCollider : 해당 인덱스의 충돌체가 활성화되어있어야 피격합니다.
        /// <summary> 막지 않고 피해를 받을 수 있는 충돌체를 반환합니다. </summary>
        public Collider2D GetNotGuardCollider()
        {
            if (Collider != null)
            {
                return Collider;
            }

            if (Colliders != null)
            {
                for (int i = 0; i < Colliders.Length; i++)
                {
                    if (ContainsGuardIndex(i))
                    {
                        continue;
                    }

                    return Colliders[i];
                }
            }

            return null;
        }

        /// <summary> 막지 않고 피해를 받을 수 있는 충돌체 인덱스를 반환합니다. </summary>
        public int GetNotGuardColliderIndex()
        {
            if (Colliders != null)
            {
                for (int i = 0; i < Colliders.Length; i++)
                {
                    if (ContainsGuardIndex(i))
                    {
                        continue;
                    }

                    return i;
                }
            }

            return -1;
        }

        //

        public bool ContainsGuardIndex(int index)
        {
            if (GuardColliderIndexes != null)
            {
                if (GuardColliderIndexes.Contains(index))
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckAllGuardColliders()
        {
            if (Colliders.IsValid() && GuardColliderIndexes.IsValid())
            {
                if (Colliders.Length == GuardColliderIndexes.Count)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckGuardCollider(Collider2D collider)
        {
            if (GuardColliderIndexes != null)
            {
                for (int i = 0; i < GuardColliderIndexes.Count; i++)
                {
                    int colliderIndex = GuardColliderIndexes[i];
                    if (Colliders[colliderIndex] == collider)
                    {
                        GuardFeedbacks?.PlayFeedbacks(collider.transform.position, colliderIndex);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool CheckOnlyUseShieldColliderIndexes(int index)
        {
            return OnlyUseShieldColliderIndexes.IsValid(index);
        }

        //

        public void SetGuardActive(int index, bool isActiveGuard)
        {
            if (GuardColliderIndexes != null)
            {
                if (isActiveGuard)
                {
                    if (!GuardColliderIndexes.Contains(index))
                    {
                        GuardColliderIndexes.Add(index);

                        Log.Info(LogTags.Vital, "바이탈 가드 인덱스를 추가합니다. {0}", index.ToSelectString());
                    }
                }
                else
                {
                    if (GuardColliderIndexes.Contains(index))
                    {
                        GuardColliderIndexes.Remove(index);

                        Log.Info(LogTags.Vital, "바이탈 가드 인덱스를 삭제합니다. {0}", index.ToErrorString());
                    }
                }
            }
        }
    }
}