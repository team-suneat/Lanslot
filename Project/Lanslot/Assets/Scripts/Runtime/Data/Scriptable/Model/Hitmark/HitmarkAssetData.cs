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

        #region 피해 정보 (Damage Information)

        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#피해 정보")]
        [GUIColor("GetHitmarkColor")]
        [SuffixLabel("히트마크")]
        public HitmarkNames Hitmark;

        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#피해 정보")]
        [SuffixLabel("피해 종류")]
        [GUIColor("GetDamageTypeColor")]
        public DamageTypes DamageType;

        [FoldoutGroup("#피해 정보 - 토글")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("회피 불가 공격")]
        public bool IgnoreEvasion;

        [FoldoutGroup("#피해 정보 - 토글")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("압도 보호막 필요 여부")]
        [EnableIf("DamageType", DamageTypes.Overwhelm)]
        public bool IsRequireShieldForOverwhelm;

        // 피격

        [FoldoutGroup("#피해 정보 - 토글/애니메이션", true)]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("피격자의 피격 애니메이션 종류 : 강한 공격 여부")]
        public bool IsPowerfulAttack;

        [FoldoutGroup("#피해 정보 - 토글/애니메이션", true)]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("피격 역방향 적용")]
        public bool IsReverseDamageDirection;

        [FoldoutGroup("#피해 정보 - 토글/애니메이션", true)]
        [GUIColor("GetBoolColor")]
        [Tooltip("피격 애니메이션을 재생하지 않으면 피격 FV 또한 적용하지 않습니다.")]
        [SuffixLabel("피격 애니메이션 사용 안함*")]
        public bool NotPlayDamageAnimation;

        [FoldoutGroup("#피해 정보 - 토글/애니메이션", true)]
        [GUIColor("GetIntColor")]
        [SuffixLabel("이 공격에 비롯된 피격 애니메이션의 우선순위")]
        public int DamageAnimationPriority;

        [FoldoutGroup("#피해 정보 - 토글")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("자기 자신에게 적용")]
        public bool ApplyToSelf;

        [FoldoutGroup("#피해 정보 - 토글")]
        [GUIColor("GetFloatColor")]
        [EnableIf("ApplyToSelf")]
        [SuffixLabel("자기 자신에게 적용 배율(%)")]
        public float ApplyMultiplierToSelf;

        // 피해

        [FoldoutGroup("#피해량")]
        [GUIColor("GetFloatColor")]
        [Tooltip("피해 종류가 물리 또는 마법일 때 피격자의 생명력 비율이 일정 이하라면 적을 처형합니다.")]
        [SuffixLabel("피격자의 처형 조건 생명력 비율*")]
        [Range(0f, 1f)]
        public float ExecutionConditionalTargetLifeRate;

        [FoldoutGroup("#피해량")]
        [GUIColor("GetIntColor")]
        [SuffixLabel("최소 피해량")]
        public int MinDamageValue = 1;

        [FoldoutGroup("#피해량")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("고정 피해")]
        public float FixedDamage;

        [FoldoutGroup("#피해량")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("고정 성장 피해")]
        public float FixedDamageByLevel;

        [FoldoutGroup("#피해량")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("무기 피해 사용")]
        public bool UseWeaponDamage;

        // 연결된 값

        [FoldoutGroup("연결된 참조 피해량")]
        [Tooltip("피해량을 고정으로 설정하지 않고, 해당 값을 찾아 비례 피해를 입힙니다")]
        [GUIColor("GetLinkedDamageTypeColor")]
        [SuffixLabel("연결된 값 종류*")]
        public LinkedDamageTypes LinkedDamageType;

        [FoldoutGroup("연결된 참조 피해량")]
        [DisableIf("LinkedDamageType", LinkedDamageTypes.None)]
        [GUIColor("GetStateEffectColor")]
        [SuffixLabel("연결된 상태 이상")]
        public StateEffects LinkedStateEffect;

        [FoldoutGroup("연결된 참조 피해량")]
        [GUIColor("GetFloatColor")]
        [DisableIf("LinkedDamageType", LinkedDamageTypes.None)]
        [SuffixLabel("연결된 값 배율(%)")]
        public float LinkedHitmarkMagnification;

        [FoldoutGroup("연결된 참조 피해량")]
        [GUIColor("GetFloatColor")]
        [DisableIf("LinkedDamageType", LinkedDamageTypes.None)]
        [SuffixLabel("연결된 값 배율(%) (레벨별)")]
        public float LinkedValueMagnificationByLevel;

        [FoldoutGroup("연결된 참조 피해량")]
        [GUIColor("GetFloatColor")]
        [DisableIf("LinkedDamageType", LinkedDamageTypes.None)]
        [SuffixLabel("연결된 값 배율(%) (스택별)")]
        public float LinkedValueMagnificationByStack;

        // 적중시 효과

        [FoldoutGroup("#적중 시(On Hit)")]
        [GUIColor("GetHitmarkColor")]
        [SuffixLabel("적중시 추가 공격 히트마크")]
        public HitmarkNames NameOnHit;

        [FoldoutGroup("#적중 시(On Hit)")]
        [GUIColor("GetBuffNameColor")]
        [SuffixLabel("적중시 추가 버프")]
        public BuffNames BuffOnHit;

        [FoldoutGroup("#적중 시(On Hit)")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("적중시 추가 공격 지연 시간")]
        public float DelayTimeOfAttackOnHit;

        [FoldoutGroup("#적중 시(On Hit)")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("피해 없이도 적중시 효과 적용")]
        public bool ApplyOnHitEventIfNoDamage;

        [FoldoutGroup("#적중 시(On Hit)")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("공격자와 대상이 같아도 효과 적용")]
        public bool CanApplyToSelf;

        [NonSerialized] public HitmarkAssetData HitmarkAssetOnHit;
        [NonSerialized] public BuffAssetData BuffAssetOnHit;

        // 스트링

        [FoldoutGroup("#String")] public string DamageTypeString;
        [FoldoutGroup("#String")] public string LinkedDamageTypeString;
        [FoldoutGroup("#String")] public string LinkedStateEffectString;
        [FoldoutGroup("#String")] public string NameOnHitString;
        [FoldoutGroup("#String")] public string BuffOnHitString;
        [FoldoutGroup("#String")] public string UseWeaponDamageString;

        #endregion 피해 정보 (Damage Information)

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
            Hitmark = Name;

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

                ValidateDamageData();
                RefreshDecrescenceTypeMassage();
            }
        }

        public override void Refresh()
        {
            base.Refresh();

            Hitmark = Name;
            AttackTargetTypeString = AttackTargetType.ToString();
            DecrescenceTypeString = DecrescenceType.ToString();
            ResourceConsumeTypeString = ResourceConsumeType.ToString();

            RefreshDamageString();

            IsChangingAsset = false;
        }

        public override void OnLoadData()
        {
            base.OnLoadData();

            DamageTypeLog();
            EnumLog();
        }

        public HitmarkAssetData Clone()
        {
            HitmarkAssetData clone = new()
            {
                Name = Name,
                Hitmark = Hitmark,
                AttackTargetType = AttackTargetType,

                IsCrowdControl = IsCrowdControl,

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

                DamageType = DamageType,
                IgnoreEvasion = IgnoreEvasion,
                IsRequireShieldForOverwhelm = IsRequireShieldForOverwhelm,
                IsPowerfulAttack = IsPowerfulAttack,
                IsReverseDamageDirection = IsReverseDamageDirection,
                NotPlayDamageAnimation = NotPlayDamageAnimation,
                DamageAnimationPriority = DamageAnimationPriority,
                ApplyToSelf = ApplyToSelf,
                ApplyMultiplierToSelf = ApplyMultiplierToSelf,

                ExecutionConditionalTargetLifeRate = ExecutionConditionalTargetLifeRate,
                MinDamageValue = MinDamageValue,
                FixedDamage = FixedDamage,
                FixedDamageByLevel = FixedDamageByLevel,
                UseWeaponDamage = UseWeaponDamage,

                LinkedDamageType = LinkedDamageType,
                LinkedStateEffect = LinkedStateEffect,
                LinkedHitmarkMagnification = LinkedHitmarkMagnification,
                LinkedValueMagnificationByLevel = LinkedValueMagnificationByLevel,
                LinkedValueMagnificationByStack = LinkedValueMagnificationByStack,

                NameOnHit = NameOnHit,
                BuffOnHit = BuffOnHit,
                DelayTimeOfAttackOnHit = DelayTimeOfAttackOnHit,
                ApplyOnHitEventIfNoDamage = ApplyOnHitEventIfNoDamage,
                CanApplyToSelf = CanApplyToSelf,
            };

            CloneOnHitAssets(clone);

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

        private void ValidateDamageData()
        {
            if (!EnumEx.ConvertTo(ref DamageType, DamageTypeString))
            {
                Log.Error("HitmarkAssetData의 DamageType을 변환하지 못합니다. Hitmark:{0}, {1}", Hitmark, DamageTypeString);
            }
            if (!EnumEx.ConvertTo(ref LinkedDamageType, LinkedDamageTypeString))
            {
                Log.Error("HitmarkAssetData의 LinkedDamageType을 변환하지 못합니다. Hitmark:{0}, {1}", Hitmark, LinkedDamageTypeString);
            }
            if (!EnumEx.ConvertTo(ref LinkedStateEffect, LinkedStateEffectString))
            {
                Log.Error("HitmarkAssetData의 LinkedStateEffect을 변환하지 못합니다. Hitmark:{0}, {1}", Hitmark, LinkedStateEffectString);
            }

            if (!string.IsNullOrWhiteSpace(UseWeaponDamageString))
            {
                if (!bool.TryParse(UseWeaponDamageString, out bool parsedUseWeaponDamage))
                {
                    Log.Error("HitmarkAssetData의 UseWeaponDamage를 변환하지 못합니다. Hitmark:{0}, {1}", Hitmark, UseWeaponDamageString);
                }
                else
                {
                    UseWeaponDamage = parsedUseWeaponDamage;
                }
            }

            if (!EnumEx.ConvertTo(ref NameOnHit, NameOnHitString))
            {
                Log.Error("HitmarkAssetData의 NameOnHit을 변환하지 못합니다. Hitmark:{0}, {1}", Hitmark, NameOnHitString);
            }
            if (!EnumEx.ConvertTo(ref BuffOnHit, BuffOnHitString))
            {
                Log.Error("HitmarkAssetData의 BuffOnHit를 변환하지 못합니다. Hitmark:{0}, {1}", Hitmark, BuffOnHitString);
            }

            DamageTypeLog();
            EnumLog();
        }

        private void RefreshDamageString()
        {
            DamageTypeString = DamageType.ToString();
            LinkedDamageTypeString = LinkedDamageType.ToString();
            LinkedStateEffectString = LinkedStateEffect.ToString();
            NameOnHitString = NameOnHit.ToString();
            BuffOnHitString = BuffOnHit.ToString();
            UseWeaponDamageString = UseWeaponDamage.ToString();
        }

        private void CloneOnHitAssets(HitmarkAssetData clone)
        {
            if (NameOnHit != HitmarkNames.None)
            {
                HitmarkAsset asset = ScriptableDataManager.Instance.FindHitmark(NameOnHit);
                if (asset != null)
                {
                    clone.HitmarkAssetOnHit = asset.Data;
                }
            }
            if (BuffOnHit != BuffNames.None)
            {
                BuffAsset asset = ScriptableDataManager.Instance.FindBuff(BuffOnHit);
                if (asset != null)
                {
                    clone.BuffAssetOnHit = asset.Data;
                }
            }
        }

        private void DamageTypeLog()
        {
#if UNITY_EDITOR
            if (DamageType == DamageTypes.None)
            {
                Log.Warning("HitmarkAssetData의 DamageType이 올바르지 않을 수 있습니다. Hitmark:{0}, {1}", Hitmark.ToLogString(), DamageType);
            }
#endif
        }

        private void EnumLog()
        {
#if UNITY_EDITOR
            string type = "HitmarkAssetData".ToSelectString();
            EnumExplorer.LogBuff(type, Hitmark.ToString(), BuffOnHit);
#endif
        }

        public bool CompareDamage(HitmarkAssetData another)
        {
            if (Hitmark != another.Hitmark) { return false; }
            if (DamageType != another.DamageType) { return false; }
            if (IgnoreEvasion != another.IgnoreEvasion) { return false; }
            if (ApplyToSelf != another.ApplyToSelf) { return false; }
            if (ApplyMultiplierToSelf != another.ApplyMultiplierToSelf) { return false; }
            if (ExecutionConditionalTargetLifeRate != another.ExecutionConditionalTargetLifeRate) { return false; }
            if (MinDamageValue != another.MinDamageValue) { return false; }
            if (FixedDamage != another.FixedDamage) { return false; }
            if (FixedDamageByLevel != another.FixedDamageByLevel) { return false; }
            if (UseWeaponDamage != another.UseWeaponDamage) { return false; }
            if (LinkedDamageType != another.LinkedDamageType) { return false; }
            if (LinkedStateEffect != another.LinkedStateEffect) { return false; }
            if (LinkedHitmarkMagnification != another.LinkedHitmarkMagnification) { return false; }
            if (LinkedValueMagnificationByLevel != another.LinkedValueMagnificationByLevel) { return false; }
            if (LinkedValueMagnificationByStack != another.LinkedValueMagnificationByStack) { return false; }
            if (NameOnHit != another.NameOnHit) { return false; }
            if (BuffOnHit != another.BuffOnHit) { return false; }
            if (DelayTimeOfAttackOnHit != another.DelayTimeOfAttackOnHit) { return false; }
            if (ApplyOnHitEventIfNoDamage != another.ApplyOnHitEventIfNoDamage) { return false; }
            if (CanApplyToSelf != another.CanApplyToSelf) { return false; }
            if (IsPowerfulAttack != another.IsPowerfulAttack) { return false; }
            if (IsReverseDamageDirection != another.IsReverseDamageDirection) { return false; }
            if (NotPlayDamageAnimation != another.NotPlayDamageAnimation) { return false; }
            if (DamageAnimationPriority != another.DamageAnimationPriority) { return false; }

            return true;
        }

#if UNITY_EDITOR

        public bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            UpdateIfChanged(ref AttackTargetTypeString, AttackTargetType);
            UpdateIfChanged(ref DecrescenceTypeString, DecrescenceType);
            UpdateIfChanged(ref ResourceConsumeTypeString, ResourceConsumeType);

            UpdateIfChanged(ref DamageTypeString, DamageType);
            UpdateIfChanged(ref LinkedDamageTypeString, LinkedDamageType);
            UpdateIfChanged(ref LinkedStateEffectString, LinkedStateEffect);
            UpdateIfChanged(ref NameOnHitString, NameOnHit);
            UpdateIfChanged(ref BuffOnHitString, BuffOnHit);
            UpdateIfChangedBool(ref UseWeaponDamageString, UseWeaponDamage);

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

        private void UpdateIfChangedBool(ref string target, bool newValue)
        {
            string newString = newValue.ToString();
            if (target != newString)
            {
                target = newString;
                _hasChangedWhiteRefreshAll = true;
            }
        }

        private Color GetLinkedDamageTypeColor(LinkedDamageTypes key)
        {
            return GetFieldColor(key);
        }

#endif
    }
}