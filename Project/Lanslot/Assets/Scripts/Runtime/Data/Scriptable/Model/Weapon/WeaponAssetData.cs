using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace TeamSuneat.Data
{
    [Serializable]
    public partial class WeaponAssetData : ScriptableData<int>
    {
        public bool IsChangingAsset;

        [GUIColor("GetItemNameColor")]
        public ItemNames Name;

        [EnableIf("IsChangingAsset")]
        [GUIColor("GetHitmarkNamesColor")]
        [SuffixLabel("무기 공격 시 사용되는 히트마크")]
        public HitmarkNames[] Hitmarks;

        [GUIColor("GetFloatColor")]
        [SuffixLabel("초")]
        [Tooltip("무기의 기본 공격 속도 (몇 초에 한 번 공격하는지)")]
        public float AttackSpeed;

        [GUIColor("GetIntColor")]
        [SuffixLabel("회")]
        [Tooltip("무기의 기본 공격 횟수 (공격 시 몇 번 공격하는지)")]
        public int AttackCount;

        [SuffixLabel("무기 설명")]
        [TextArea(3, 5)]
        public string WeaponDescription;

        #region String

        [FoldoutGroup("#String")]
        public string[] HitmarkAsStrings;

        #endregion String

        public override int GetKey()
        {
            return Name.ToInt();
        }

        #region Validate & Refresh

        public void Validate()
        {
            if (Hitmarks != null && HitmarkAsStrings != null)
            {
                for (int i = 0; i < Hitmarks.Length && i < HitmarkAsStrings.Length; i++)
                {
                    if (!EnumEx.ConvertTo(ref Hitmarks[i], HitmarkAsStrings[i]))
                    {
                        Log.Error("WeaponAssetData의 Hitmark[{0}]을 변환하지 못합니다. {1}", i, HitmarkAsStrings[i]);
                    }
                }
            }
        }

        public override void Refresh()
        {
            base.Refresh();

            if (Hitmarks != null)
            {
                HitmarkAsStrings = Hitmarks.ToStringArray();
            }

            IsChangingAsset = false;
        }

        #endregion Validate & Refresh

        #region Clone

        public WeaponAssetData Clone()
        {
            WeaponAssetData clonedData = new()
            {
                Name = Name,
                Hitmarks = Hitmarks?.Clone() as HitmarkNames[],

                // 무기 기본 정보
                AttackSpeed = AttackSpeed,
                AttackCount = AttackCount,
            };

            return clonedData;
        }

        #endregion Clone

        protected Color GetHitmarkNamesColor()
        {
            if (Hitmarks.IsValidArray())
            {
                return GameColors.GreenYellow;
            }
            return GameColors.CreamIvory;
        }
    }
}