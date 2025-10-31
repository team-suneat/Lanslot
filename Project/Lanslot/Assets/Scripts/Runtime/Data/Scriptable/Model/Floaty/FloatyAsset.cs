using TeamSuneat.Setting;
using TeamSuneat.UserInterface;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "FloatyAsset", menuName = "TeamSuneat/Scriptable/Floaty")]
    public class FloatyAsset : XScriptableObject
    {
        public bool IsChangingAsset;

        [EnableIf("IsChangingAsset")]
        public UIFloatyMoveNames Name;

        [FoldoutGroup("#Font")] public float FontSize;
        [EnableIf("IsChangingAsset")][FoldoutGroup("#Font")] public GameFontTypes FontType;
        [FoldoutGroup("#Font")] public Color TextColor;

        [FoldoutGroup("#Spawn")] public Vector2 SpawnArea;
        [FoldoutGroup("#Spawn")] public Vector3 SpawnOffset;

        [EnableIf("IsChangingAsset")][FoldoutGroup("#Move")] public UIFloatyMoveTypes Type;

        [FoldoutGroup("#Move")] public bool RandomFace;
        [FoldoutGroup("#Time")] public float DelayTime;
        [FoldoutGroup("#Time")] public float Duration;
        [FoldoutGroup("#Velocity")][EnableIf("Type", UIFloatyMoveTypes.Velocity)] public Vector3 Velocity;
        [FoldoutGroup("#Jump")][EnableIf("Type", UIFloatyMoveTypes.Jump)] public Vector3 JumpEndValue;
        [FoldoutGroup("#Jump")][EnableIf("Type", UIFloatyMoveTypes.Jump)] public float JumpPower;
        [FoldoutGroup("#Jump")][EnableIf("Type", UIFloatyMoveTypes.Jump)] public int NumberOfJumps;

        [FoldoutGroup("#Fade")] public bool UseFadeOut;
        [FoldoutGroup("#Fade")][EnableIf("UseFadeOut")] public float FadeDuration;
        [FoldoutGroup("#Fade")][EnableIf("UseFadeOut")] public float FadeDelayTime;
        [FoldoutGroup("#Fade")][EnableIf("UseFadeOut")][Range(0f, 1f)] public float FadeTargetAlpha;

        [FoldoutGroup("#PunchScale")] public bool UsePunchScale;
        [FoldoutGroup("#PunchScale")][EnableIf("UsePunchScale")] public Vector2 PunchScale;
        [FoldoutGroup("#PunchScale")][EnableIf("UsePunchScale")] public float PunchScaleDuration;
        [FoldoutGroup("#PunchScale")][EnableIf("UsePunchScale")] public int PunchScaleVibrato;

        [FoldoutGroup("#String")] public string TypeString;
        [FoldoutGroup("#String")] public string FontTypeString;

        public int TID => BitConvert.Enum32ToInt(Name);

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            if (!IsChangingAsset)
            {
                EnumEx.ConvertTo(ref Name, NameString);
                EnumEx.ConvertTo(ref Type, TypeString);
                EnumEx.ConvertTo(ref FontType, FontTypeString);
            }
        }

        public override void Rename()
        {
            Rename("Floaty");
        }

        public override void Refresh()
        {
            if (Name != 0) { NameString = Name.ToString(); }
            if (Type != 0) { TypeString = Type.ToString(); }
            if (FontType != 0) { FontTypeString = FontType.ToString(); }

            SetClosestFontTypeByCurrentFontSize();

            IsChangingAsset = false;
            base.Refresh();
        }

        private void SetClosestFontTypeByCurrentFontSize()
        {
            if (FontSize == 0)
            {
                return;
            }

            if (FontType is GameFontTypes.Difficulty or GameFontTypes.Number or GameFontTypes.Content_DialogueTitle)
            {
                return;
            }

            if (!ScriptableDataManager.Instance.CheckLoaded())
            {
                ScriptableDataManager.Instance.LoadScriptableAssetsAsync();
            }

            float currentFontSize = FontSize;
            LanguageNames languageName = GameSetting.Instance.Language.Name;
            FontAsset fontAsset = ScriptableDataManager.Instance.FindFont(languageName);
            if (fontAsset == null)
            {
                return;
            }

            GameFontTypes closestType = GameFontTypes.None;
            float minDiff = float.MaxValue;

            GameFontTypes[] matchTypes = new GameFontTypes[]
            {
                GameFontTypes.Title,
                GameFontTypes.Title_GrayShadow,
                GameFontTypes.Content_DefaultSize,
                GameFontTypes.Content_DefaultSize_GrayShadow,
                GameFontTypes.Content_LargeSize,
                GameFontTypes.Content_SmallSize,
                GameFontTypes.Content_XSmallSize
            };
            for (int i = 0; i < matchTypes.Length; i++)
            {
                GameFontTypes type = matchTypes[i];
                FontAsset.FontAssetData? dataNullable = fontAsset.GetFontAssetData(type);
                if (dataNullable == null)
                {
                    continue;
                }

                FontAsset.FontAssetData data = dataNullable.Value;
                float compareSize = data.FontSize;
                float diff = Mathf.Abs(currentFontSize - compareSize);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    closestType = type;
                }
            }

            if (closestType != GameFontTypes.None)
            {
                GameFontTypes prevType = FontType;

                if (prevType == GameFontTypes.Title_GrayShadow && closestType == GameFontTypes.Title)
                {
                    closestType = GameFontTypes.Title_GrayShadow;
                }
                else if (prevType == GameFontTypes.Content_DefaultSize_GrayShadow && closestType == GameFontTypes.Content_DefaultSize)
                {
                    closestType = GameFontTypes.Content_DefaultSize_GrayShadow;
                }

                FontType = closestType;
                FontTypeString = closestType.ToString();
                if (prevType != closestType)
                {
                    Log.Info($"[UILocalizedText] FontType이 자동으로 변경됨: {prevType} → {closestType} (기준 폰트 크기: {currentFontSize})");
                }
            }
        }

#endif
    }
}