using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "Font", menuName = "TeamSuneat/Scriptable/Font")]
    public class FontAsset : XScriptableObject
    {
        [Title("Font")]
        public LanguageNames Language;

        [Title("Font-Data")]
        public FontAssetData Title;

        public FontAssetData Title_GrayShadow;
        public FontAssetData Content_DefaultSize;
        public FontAssetData Content_DefaultSize_GrayShadow;
        public FontAssetData Content_LargeSize;
        public FontAssetData Content_SmallSize;
        public FontAssetData Content_XSmallSize;
        public FontAssetData Content_DialogueTitle;
        public FontAssetData Number;
        public FontAssetData Difficulty;

        public int TID => BitConvert.Enum32ToInt(Language);

        public override void Refresh()
        {
            base.Refresh();

            // Title.SetDefaultFontSize(GameFontTypes.Title);
            // Title_GrayShadow.SetDefaultFontSize(GameFontTypes.Title_GrayShadow);
            // Content_DefaultSize.SetDefaultFontSize(GameFontTypes.Content_DefaultSize);
            // Content_DefaultSize_GrayShadow.SetDefaultFontSize(GameFontTypes.Content_DefaultSize_GrayShadow);
            // Content_LargeSize.SetDefaultFontSize(GameFontTypes.Content_LargeSize);
            // Content_SmallSize.SetDefaultFontSize(GameFontTypes.Content_SmallSize);
            // Content_XSmallSize.SetDefaultFontSize(GameFontTypes.Content_XSmallSize);
            // Content_DialogueTitle.SetDefaultFontSize(GameFontTypes.Content_DialogueTitle);
            // Number.SetDefaultFontSize(GameFontTypes.Number);
            // Difficulty.SetDefaultFontSize(GameFontTypes.Difficulty);
        }

        public TMP_FontAsset FindFont(GameFontTypes fontType)
        {
            switch (fontType)
            {
                case GameFontTypes.Title:
                    {
                        return Title.Font;
                    }
                case GameFontTypes.Title_GrayShadow:
                    {
                        return Title_GrayShadow.Font;
                    }

                case GameFontTypes.Content_DefaultSize:
                    {
                        return Content_DefaultSize.Font;
                    }
                case GameFontTypes.Content_DefaultSize_GrayShadow:
                    {
                        return Content_DefaultSize_GrayShadow.Font;
                    }
                case GameFontTypes.Content_LargeSize:
                    {
                        return Content_LargeSize.Font;
                    }
                case GameFontTypes.Content_SmallSize:
                    {
                        return Content_SmallSize.Font;
                    }
                case GameFontTypes.Content_XSmallSize:
                    {
                        return Content_XSmallSize.Font;
                    }
                case GameFontTypes.Content_DialogueTitle:
                    {
                        return Content_DialogueTitle.Font;
                    }
                case GameFontTypes.Number:
                    {
                        return Number.Font;
                    }
                case GameFontTypes.Difficulty:
                    {
                        return Difficulty.Font;
                    }
            }

            return null;
        }

        public bool FindItalic(GameFontTypes fontType)
        {
            switch (fontType)
            {
                case GameFontTypes.Title:
                    {
                        return Title.UseItalicText;
                    }
                case GameFontTypes.Title_GrayShadow:
                    {
                        return Title_GrayShadow.UseItalicText;
                    }

                case GameFontTypes.Content_DefaultSize:
                    {
                        return Content_DefaultSize.UseItalicText;
                    }
                case GameFontTypes.Content_DefaultSize_GrayShadow:
                    {
                        return Content_DefaultSize_GrayShadow.UseItalicText;
                    }
                case GameFontTypes.Content_LargeSize:
                    {
                        return Content_LargeSize.UseItalicText;
                    }
                case GameFontTypes.Content_SmallSize:
                    {
                        return Content_SmallSize.UseItalicText;
                    }
                case GameFontTypes.Content_XSmallSize:
                    {
                        return Content_XSmallSize.UseItalicText;
                    }
                case GameFontTypes.Content_DialogueTitle:
                    {
                        return Content_DialogueTitle.UseItalicText;
                    }
                case GameFontTypes.Number:
                    {
                        return Number.UseItalicText;
                    }
                case GameFontTypes.Difficulty:
                    {
                        return Difficulty.UseItalicText;
                    }
            }

            return false;
        }

        public FontAssetData? GetFontAssetData(GameFontTypes type)
        {
            switch (type)
            {
                case GameFontTypes.Title: return Title;
                case GameFontTypes.Title_GrayShadow: return Title_GrayShadow;
                case GameFontTypes.Content_DefaultSize: return Content_DefaultSize;
                case GameFontTypes.Content_DefaultSize_GrayShadow: return Content_DefaultSize_GrayShadow;
                case GameFontTypes.Content_LargeSize: return Content_LargeSize;
                case GameFontTypes.Content_SmallSize: return Content_SmallSize;
                case GameFontTypes.Content_XSmallSize: return Content_XSmallSize;
                case GameFontTypes.Content_DialogueTitle: return Content_DialogueTitle;
                case GameFontTypes.Number: return Number;
                case GameFontTypes.Difficulty: return Difficulty;
                default: return null;
            }
        }

#if UNITY_EDITOR

        public override void Rename()
        {
            Rename("Font");
        }

#endif

        [System.Serializable]
        public struct FontAssetData
        {
            public GameFontTypes Type;

            [SuffixLabel("폰트가 아닌 텍스트에서 이탤릭체를 지원")]
            public bool UseItalicText;

            public TMP_FontAsset Font;

            // 폰트 크기 관련 필드 추가
            [Title("Font Size 설정")]
            [Tooltip("고정 폰트 크기 (UseAutoSize가 false일 때 적용)")]
            public float FontSize;

            /// <summary>
            /// 타입에 따라 기본 폰트 크기 및 오토사이즈 값을 설정합니다.
            /// </summary>
            public void SetDefaultFontSize(GameFontTypes type)
            {
                Type = type;

                switch (type)
                {
                    case GameFontTypes.Title:
                    case GameFontTypes.Title_GrayShadow:
                        FontSize = 36f;
                        break;

                    case GameFontTypes.Content_DefaultSize:
                    case GameFontTypes.Content_DefaultSize_GrayShadow:
                        FontSize = 20f;
                        break;

                    case GameFontTypes.Content_LargeSize:
                        FontSize = 28f;
                        break;

                    case GameFontTypes.Content_SmallSize:
                        FontSize = 16f;
                        break;

                    case GameFontTypes.Content_XSmallSize:
                        FontSize = 12f;
                        break;

                    case GameFontTypes.Content_DialogueTitle:
                        FontSize = 24f;
                        break;

                    case GameFontTypes.Number:
                        FontSize = 20f;
                        break;

                    case GameFontTypes.Difficulty:
                        FontSize = 16f;
                        break;

                    default:
                        FontSize = 20f;
                        break;
                }
            }
        }
    }
}