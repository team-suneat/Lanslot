using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class DamageResult
    {
        /// <summary> 치명타 여부 </summary>
        public bool IsCritical;

        /// <summary> 회피 여부 </summary>
        public bool IsEvasion;

        //───────────────────────────────────────────────────────────────────────────────────────────────────=

        /// <summary> 피해량 </summary>
        public float DamageValue;

        /// <summary> 줄어든 피해량 </summary>
        public float ReducedDamageValue;

        public int DamageValueToInt
        {
            get
            {
                if (DamageValue is float.MaxValue)
                {
                    return int.MaxValue;
                }
                else if (DamageValue.IsZero())
                {
                    return 0;
                }

                return Mathf.RoundToInt(DamageValue);
            }
        }

        public int ReducedDamageValueToInt => Mathf.RoundToInt(ReducedDamageValue);

        //───────────────────────────────────────────────────────────────────────────────────────────────────=

        /// <summary> 공격자 </summary>
        public Character Attacker;

        /// <summary> 공격 독립체 </summary>
        public AttackEntity AttackEntity;

        /// <summary> 피격자 바이탈 </summary>
        public Vital TargetVital;

        /// <summary> 피격자 바이탈 충돌체 </summary>
        public Collider2D TargetVitalCollider;

        /// <summary> 피격자 바이탈 충돌체 인덱스 </summary>
        public int TargetVitalColliderIndex;

        //───────────────────────────────────────────────────────────────────────────────────────────────────

        /// <summary> 히트마크 레벨 </summary>
        public int HitmarkLevel;

        /// <summary> 피해 에셋 데이터 </summary>
        public DamageAssetData Asset;

        //───────────────────────────────────────────────────────────────────────────────────────────────────

        public Vector3 AttackPosition
        {
            get
            {
                if (AttackEntity != null)
                {
                    return AttackEntity.transform.position;
                }

                return Vector3.zero;
            }
        }

        public Vector3 DamagePosition
        {
            get
            {
                if (TargetVitalCollider != null)
                {
                    return TargetVitalCollider.transform.position;
                }

                if (TargetVital != null)
                {
                    Collider2D vitalCollider = TargetVital.GetNotGuardCollider();
                    if (vitalCollider != null)
                    {
                        return vitalCollider.transform.position;
                    }
                }

                return Vector3.zero;
            }
        }

        public Character TargetCharacter
        {
            get
            {
                if (TargetVital != null)
                {
                    return TargetVital.Owner;
                }

                return null;
            }
        }

        // 에셋 데이터 (AssetData)

        public HitmarkNames HitmarkName
        {
            get
            {
                if (Asset != null)
                {
                    return Asset.Hitmark;
                }

                return HitmarkNames.None;
            }
        }

        public DamageTypes DamageType
        {
            get
            {
                if (Asset != null)
                {
                    return Asset.DamageType;
                }

                return DamageTypes.None;
            }
        }

        public HitmarkAssetData HitmarkAssetOnHit
        {
            get
            {
                if (Asset != null)
                {
                    if (Asset.HitmarkAssetOnHit != null)
                    {
                        if (Asset.HitmarkAssetOnHit.Name != HitmarkNames.None)
                        {
                            return Asset.HitmarkAssetOnHit;
                        }
                    }

                    if (Asset.NameOnHit != HitmarkNames.None)
                    {
                        return ScriptableDataManager.Instance.FindHitmarkClone(Asset.NameOnHit);
                    }
                }
                return null;
            }
        }

        public BuffAssetData BuffAssetOnHit
        {
            get
            {
                if (Asset != null)
                {
                    if (Asset.BuffAssetOnHit != null)
                    {
                        if (Asset.BuffAssetOnHit.Name != BuffNames.None)
                        {
                            return Asset.BuffAssetOnHit;
                        }
                    }

                    if (Asset.BuffOnHit != BuffNames.None)
                    {
                        return ScriptableDataManager.Instance.FindBuffClone(Asset.BuffOnHit);
                    }
                }

                return null;
            }
        }

        public VFXInstantDamageType InstantVFXDamageType { get; internal set; }
    }
}