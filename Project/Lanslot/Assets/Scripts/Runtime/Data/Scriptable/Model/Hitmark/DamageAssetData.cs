using Sirenix.OdinInspector;
using System;
using System.Linq;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    [System.Serializable]
    public class DamageAssetData : ScriptableData<int>
    {
        [FoldoutGroup("#피해 정보")]
        [GUIColor("GetHitmarkColor")]
        [SuffixLabel("히트마크")]
        public HitmarkNames Hitmark;

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
        [GUIColor("GetFloatColor")]
        [SuffixLabel("피해 계수")]
        public float DamageRatio = 1;

        [FoldoutGroup("#피해량")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("피해 성장 계수")]
        public float DamageRatioByLevel;

        // 연결된 값

        [FoldoutGroup("연결된 참조 피해량")]
        [Tooltip("피해량을 고정으로 설정하지 않고, 해당 값을 찾아 비례 피해를 입힙니다")]
        [GUIColor("GetLinkedDamageTypeColor")]
        [SuffixLabel("연결된 값 종류*")]
        public LinkedDamageTypes LinkedDamageType;

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

        public override int GetKey()
        {
            return Hitmark.ToInt();
        }

        public override void OnLoadData()
        {
            DamageTypeLog();
            EnumLog();
        }

        private void DamageTypeLog()
        {
#if UNITY_EDITOR
            if (DamageType == DamageTypes.None)
            {
                Log.Warning("DamageAssetData의 DamageType이 올바르지 않을 수 있습니다. Hitmark:{0}, {1}", Hitmark.ToLogString(), DamageType);
            }
#endif
        }

        private void EnumLog()
        {
#if UNITY_EDITOR
            string type = "DamageAssetData".ToSelectString();
            EnumExplorer.LogBuff(type, Hitmark.ToString(), BuffOnHit);
#endif
        }

        public void Validate()
        {
            if (!EnumEx.ConvertTo(ref DamageType, DamageTypeString))
            {
                Log.Error("DamageAssetData의 DamageType을 변환하지 못합니다. Hitmark:{0}, {1}", Hitmark, DamageTypeString);
            }
            if (!EnumEx.ConvertTo(ref LinkedDamageType, LinkedDamageTypeString))
            {
                Log.Error("DamageAssetData의 LinkedDamageType을 변환하지 못합니다. Hitmark:{0}, {1}", Hitmark, LinkedDamageTypeString);
            }
            if (!EnumEx.ConvertTo(ref LinkedStateEffect, LinkedStateEffectString))
            {
                Log.Error("DamageAssetData의 LinkedStateEffect을 변환하지 못합니다. Hitmark:{0}, {1}", Hitmark, LinkedStateEffectString);
            }

            if (!EnumEx.ConvertTo(ref NameOnHit, NameOnHitString))
            {
                Log.Error("DamageAssetData의 NameOnHit을 변환하지 못합니다. Hitmark:{0}, {1}", Hitmark, NameOnHitString);
            }
            if (!EnumEx.ConvertTo(ref BuffOnHit, BuffOnHitString))
            {
                Log.Error("DamageAssetData의 BuffOnHit를 변환하지 못합니다. Hitmark:{0}, {1}", Hitmark, BuffOnHitString);
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            DamageTypeString = DamageType.ToString();
            LinkedDamageTypeString = LinkedDamageType.ToString();
            LinkedStateEffectString = LinkedStateEffect.ToString();
            NameOnHitString = NameOnHit.ToString();
            BuffOnHitString = BuffOnHit.ToString();
        }

        public DamageAssetData Clone()
        {
            DamageAssetData assetData = new()
            {
                Hitmark = Hitmark,
                DamageType = DamageType,

                IgnoreEvasion = IgnoreEvasion,
                IsRequireShieldForOverwhelm = IsRequireShieldForOverwhelm,
                ApplyToSelf = ApplyToSelf,
                ApplyMultiplierToSelf = ApplyMultiplierToSelf,

                ExecutionConditionalTargetLifeRate = ExecutionConditionalTargetLifeRate,

                MinDamageValue = MinDamageValue,
                FixedDamage = FixedDamage,
                FixedDamageByLevel = FixedDamageByLevel,
                DamageRatio = DamageRatio,
                DamageRatioByLevel = DamageRatioByLevel,

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

                IsPowerfulAttack = IsPowerfulAttack,
                IsReverseDamageDirection = IsReverseDamageDirection,
                NotPlayDamageAnimation = NotPlayDamageAnimation,
                DamageAnimationPriority = DamageAnimationPriority,
            };

            if (NameOnHit != HitmarkNames.None)
            {
                HitmarkAsset asset = ScriptableDataManager.Instance.FindHitmark(NameOnHit);
                if (asset != null)
                {
                    assetData.HitmarkAssetOnHit = asset.Data;
                }
            }
            if (BuffOnHit != BuffNames.None)
            {
                BuffAsset asset = ScriptableDataManager.Instance.FindBuff(BuffOnHit);
                if (asset != null)
                {
                    assetData.BuffAssetOnHit = asset.Data;
                }
            }

            return assetData;
        }

        public bool Compare(DamageAssetData another)
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
            if (DamageRatio != another.DamageRatio) { return false; }
            if (DamageRatioByLevel != another.DamageRatioByLevel) { return false; }
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

            UpdateIfChanged(ref DamageTypeString, DamageType);
            UpdateIfChanged(ref LinkedDamageTypeString, LinkedDamageType);
            UpdateIfChanged(ref LinkedStateEffectString, LinkedStateEffect);
            UpdateIfChanged(ref NameOnHitString, NameOnHit);
            UpdateIfChanged(ref BuffOnHitString, BuffOnHit);

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

        private Color GetLinkedDamageTypeColor(LinkedDamageTypes key)
        {
            return GetFieldColor(key);
        }

#endif
    }
}