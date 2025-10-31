using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityEngine;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public class HitmarkAssetData : ScriptableData<int>
    {
        [SuffixLabel("개별 에셋 변경 모드")]
        public bool IsChangingAsset;

        [EnableIf("IsChangingAsset")]
        [SuffixLabel("히트마크 이름")]
        public HitmarkNames Name;

        [EnableIf("IsChangingAsset")]
        [SuffixLabel("목표 설정 방식")]
        public AttackTargetTypes AttackTargetType;

        [SuffixLabel("군중제어기 여부")]
        public bool IsCrowdControl;

        [SuffixLabel("공격 가능한 거리")]
        [GUIColor("GetFloatColor")]
        public float AttackRange;

        #region 피해 정보 (Damage Information)

        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#피해 정보 (Damage Information)")]
        public DamageAssetData Damage;

        #endregion 피해 정보 (Damage Information)

        #region 충돌 횟수 (Hit Count)

        [FoldoutGroup("#충돌 횟수 (Hit Count)")]
        [EnableIf("@EntityType == AttackEntityTypes.Area " +
            "|| EntityType == AttackEntityTypes.Projectile")]
        [SuffixLabel("1회 공격시 최대 공격 수")]
        [GUIColor("GetIntColor")]
        public int HitCountAtTime;

        [FoldoutGroup("#충돌 횟수 (Hit Count)")]
        [SuffixLabel("충돌시 영역 공격 비활성화")]
        public bool UseDeactivateOnHit;

        [FoldoutGroup("#충돌 횟수 (Hit Count)")]
        [EnableIf("UseDeactivateOnHit")]
        [SuffixLabel("영역 공격 비활성화 충돌 횟수")]
        [GUIColor("GetIntColor")]
        public int DeactivateHitCount;

        #endregion 충돌 횟수 (Hit Count)

        #region 피해 점감 (Damage Decrescence)

        public enum DecrescenceTypes
        { None, HitCount, ApplyCount }

        [FoldoutGroup("#피해 점감 (Damage Decrescence)")]
        [SuffixLabel("피해량 점감 종류")]
        [InfoBox("$DecrescenceTypeMassage")]
        public DecrescenceTypes DecrescenceType;

        [NonSerialized]
        public string DecrescenceTypeMassage;

        [FoldoutGroup("#피해 점감 (Damage Decrescence)")]
        [DisableIf("DecrescenceType", DecrescenceTypes.None)]
        [SuffixLabel("피해량 점감 배율")]
        [Range(0, 1)]
        [GUIColor("GetFloatColor")]
        public float DecrescenceRate;

        [FoldoutGroup("#피해 점감 (Damage Decrescence)")]
        [DisableIf("DecrescenceType", DecrescenceTypes.None)]
        [SuffixLabel("최초 피해부터 피해량 점감")]
        public bool ApplyDecrescenceFromFirst;

        #endregion 피해 점감 (Damage Decrescence)

        #region 자원 (Resource)

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("자원이 부족하다면 공격 애니메이션을 멈춥니다")]
        public bool StopAttackAnimationOnResourceLack;

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("공격 활성화 자원 소모*")]
        public bool UseResourceOnActivate;

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("공격 적용시 자원 사용*")]
        public bool UseResourceOnApply;

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("공격시 자원 사용*")]
        public bool UseResourceOnAttack;

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("공격 성공시 자원 사용*")]
        public bool UseResourceOnAttackSuccessed;

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("공격 실패시 자원 사용*")]
        public bool UseResourceOnAttackFailed;

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("공격 비활성화시 자원 소모")]
        public bool UseResourceOnDeactive;

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("자원 소모량")]
        [GUIColor("GetFloatColor")]
        public float UseResourceValue;

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("자원 회복량")]
        [GUIColor("GetFloatColor")]
        public float RestoreResourceValue;

        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("자원 소모 방식")]
        public VitalConsumeTypes ResourceConsumeType;

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("강제 자원 소모*")]
        [Tooltip("자원이 부족해도 잔여 모든 자원을 사용합니다.")]
        public bool ForceResourceConsume;

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("소모를 통한 죽음 무시*")]
        public bool IgnoreDeathByConsume;

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("자원 소모 지연시간")]
        [GUIColor("GetFloatColor")]
        public float ConsumeDealyTime;

        #endregion 자원 (Resource)

        #region 스트링 (String)

        [FoldoutGroup("#String")] public string AttackTargetTypeString;
        [FoldoutGroup("#String")] public string DecrescenceTypeString;
        [FoldoutGroup("#String")] public string ResourceConsumeTypeString;

        #endregion 스트링 (String)

        public override int GetKey()
        {
            return BitConvert.Enum32ToInt(Name);
        }

        public void Validate()
        {
            if (!IsChangingAsset)
            {
                if (!EnumEx.ConvertTo(ref AttackTargetType, AttackTargetTypeString))
                {
                    Log.Error("Hitmark 에셋 데이터의 AttackTargetTypeString 변수를 변환할 수 없습니다. {0} ({1}), {2}", Name, Name.ToLogString(), AttackTargetTypeString);
                }
                if (!EnumEx.ConvertTo(ref DecrescenceType, DecrescenceTypeString))
                {
                    Log.Error("Hitmark 에셋 데이터의 DecrescenceTypeString 변수를 변환할 수 없습니다. {0} ({1}), {2}", Name, Name.ToLogString(), DecrescenceTypeString);
                }
                if (!EnumEx.ConvertTo(ref ResourceConsumeType, ResourceConsumeTypeString))
                {
                    Log.Error("Hitmark 에셋 데이터의 ResourceConsumeTypeString 변수를 변환할 수 없습니다. {0} ({1}), {2}", Name, Name.ToLogString(), ResourceConsumeTypeString);
                }

                if (Damage != null)
                {
                    Damage.Hitmark = Name;
                    Damage.Validate();
                }

                RefreshDecrescenceTypeMassage();
            }
        }

        public override void Refresh()
        {
            base.Refresh();

            AttackTargetTypeString = AttackTargetType.ToString();
            DecrescenceTypeString = DecrescenceType.ToString();
            ResourceConsumeTypeString = ResourceConsumeType.ToString();

            if (Damage != null)
            {
                Damage.Refresh();
            }

            IsChangingAsset = false;
        }

        public override void OnLoadData()
        {
            base.OnLoadData();

            if (Damage.IsValid())
            {
                Damage.OnLoadData();
            }
        }

        public HitmarkAssetData Clone()
        {
            HitmarkAssetData clone = new()
            {
                Name = Name,
                AttackTargetType = AttackTargetType,

                IsCrowdControl = IsCrowdControl,
                AttackRange = AttackRange,

                HitCountAtTime = HitCountAtTime,
                UseDeactivateOnHit = UseDeactivateOnHit,
                DeactivateHitCount = DeactivateHitCount,

                DecrescenceType = DecrescenceType,
                DecrescenceRate = DecrescenceRate,
                ApplyDecrescenceFromFirst = ApplyDecrescenceFromFirst,

                StopAttackAnimationOnResourceLack = StopAttackAnimationOnResourceLack,
                UseResourceOnActivate = UseResourceOnActivate,
                UseResourceOnDeactive = UseResourceOnDeactive,
                UseResourceOnApply = UseResourceOnApply,
                UseResourceOnAttack = UseResourceOnAttack,
                UseResourceOnAttackSuccessed = UseResourceOnAttackSuccessed,
                UseResourceOnAttackFailed = UseResourceOnAttackFailed,

                ResourceConsumeType = ResourceConsumeType,
                ForceResourceConsume = ForceResourceConsume,
                IgnoreDeathByConsume = IgnoreDeathByConsume,
                ConsumeDealyTime = ConsumeDealyTime,
                UseResourceValue = UseResourceValue,
                RestoreResourceValue = RestoreResourceValue,
            };

            if (Damage.IsValid())
            {
                clone.Damage = Damage.Clone();
            }

            return clone;
        }

        //

        private void RefreshDecrescenceTypeMassage()
        {
            if (DecrescenceType == DecrescenceTypes.HitCount)
            {
                DecrescenceTypeMassage = "목표 또는 영역 공격 충돌 횟수에 따라 점감이 적용됩니다.\n독립체의 적용 함수에서 충돌 횟수가 초기화됩니다.";
            }
            else if (DecrescenceType == DecrescenceTypes.ApplyCount)
            {
                DecrescenceTypeMassage = "목표 또는 영역 공격 적용 횟수에 따라 점감이 적용됩니다.\n독립체의 초기화 함수에서 적용 횟수가 초기화됩니다.";
            }
            else
            {
                DecrescenceTypeMassage = "피해 점감 기능을 사용하지 않습니다.";
            }
        }

#if UNITY_EDITOR

        public bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            UpdateIfChanged(ref AttackTargetTypeString, AttackTargetType);
            UpdateIfChanged(ref DecrescenceTypeString, DecrescenceType);
            UpdateIfChanged(ref ResourceConsumeTypeString, ResourceConsumeType);

            if (Damage != null)
            {
                if (Damage.RefreshWithoutSave())
                {
                    _hasChangedWhiteRefreshAll = true;
                }
            }

            return _hasChangedWhiteRefreshAll;
        }

        private bool _hasChangedWhiteRefreshAll = false;

        private void UpdateIfChanged<TEnum>(ref string target, TEnum newValue) where TEnum : Enum
        {
            string newString = newValue?.ToString();
            if (target != newString)
            {
                target = newString;
                _hasChangedWhiteRefreshAll = true;
            }
        }

        private void UpdateIfChangedArray(ref string[] target, string[] newArray)
        {
            if (!target.SequenceEqual(newArray))
            {
                target = newArray;
                _hasChangedWhiteRefreshAll = true;
            }
        }

#endif
    }
}