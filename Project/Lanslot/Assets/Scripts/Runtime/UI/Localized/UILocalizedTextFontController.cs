using TeamSuneat.Data;
using TeamSuneat.Setting;
using TMPro;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    [DisallowMultipleComponent]
    public class UILocalizedTextFontController : XBehaviour
    {
        [SerializeField]
        private UILocalizedText _localizedText;

        private float _defaultFontSize;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            AssignComponents();
        }

        protected void Awake()
        {
            AssignComponents();
        }

        private void AssignComponents()
        {
            if (_localizedText == null)
            {
                _localizedText = GetComponent<UILocalizedText>();
            }
        }

        public void InitializeDefaultFontSize(float fontSize)
        {
            _defaultFontSize = fontSize;
        }

        public void SetDefaultFontSize(float fontSize)
        {
            _defaultFontSize = fontSize;
            RefreshFontSize();
        }

        public void RefreshFont(LanguageNames languageName)
        {
            if (_localizedText == null)
            {
                return;
            }

            if (_localizedText.FontType == GameFontTypes.None)
            {
                return;
            }

            if (_localizedText.UseCustomLanguage)
            {
                languageName = _localizedText.CustomLanguage;
            }

            FontAsset fontData = ScriptableDataManager.Instance.FindFont(languageName);
            if (fontData == null)
            {
                return;
            }

            TMP_FontAsset fontAsset = fontData.FindFont(_localizedText.FontType);
            if (fontAsset == null)
            {
                return;
            }

            if (_localizedText.TextPro != null)
            {
                _localizedText.TextPro.font = fontAsset;
                if (fontData.FindItalic(_localizedText.FontType))
                {
                    _localizedText.TextPro.fontStyle = FontStyles.Italic;
                }
                RefreshFontSize(languageName, fontData);
            }
        }

        public void RefreshFontSize()
        {
            LanguageNames languageName = GameSetting.Instance.Language.Name;
            FontAsset fontData = ScriptableDataManager.Instance.FindFont(languageName);
            RefreshFontSize(languageName, fontData);
        }

        public void RefreshFontSize(LanguageNames languageName, FontAsset fontData)
        {
            if (_localizedText == null || _localizedText.TextPro == null)
            {
                return;
            }

            if (fontData != null)
            {
                FontAsset.FontAssetData? fontAssetData = fontData.GetFontAssetData(_localizedText.FontType);
                if (fontAssetData != null)
                {
                    float fontSize;
                    if (_localizedText.CustomFontSize > 0)
                    {
                        fontSize = _localizedText.CustomFontSize;
                    }
                    else
                    {
                        fontSize = fontAssetData.Value.FontSize + _localizedText.CustomAddtionalFontSize;
                    }

                    if (!Mathf.Approximately(_localizedText.TextPro.fontSize, fontSize))
                    {
                        _localizedText.TextPro.enableAutoSizing = false;
                        _localizedText.TextPro.fontSize = fontSize;

                        Log.Info(LogTags.Font, "폰트 타입({0})에 맞는 폰트 크기를 적용합니다: {1} + {2} = {3}, {4}",
                            _localizedText.FontType, fontAssetData.Value.FontSize, _localizedText.CustomAddtionalFontSize, _localizedText.TextPro.fontSize, _localizedText.GetHierarchyPath());
                    }
                    return;
                }
            }

            if (_defaultFontSize > 0)
            {
                _localizedText.TextPro.fontSize = _defaultFontSize + _localizedText.CustomAddtionalFontSize;
            }
        }

        public void SetClosestFontTypeByCurrentFontSize(LanguageNames languageName)
        {
            if (_localizedText == null || _localizedText.TextPro == null)
            {
                return;
            }

            if (_localizedText.FontType is GameFontTypes.Difficulty or GameFontTypes.Number or GameFontTypes.Content_DialogueTitle)
            {
                return;
            }

            float currentFontSize = _localizedText.TextPro.fontSize;
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
                GameFontTypes prevType = _localizedText.FontType;

                if (prevType == GameFontTypes.Title_GrayShadow && closestType == GameFontTypes.Title)
                {
                    closestType = GameFontTypes.Title_GrayShadow;
                }
                else if (prevType == GameFontTypes.Content_DefaultSize_GrayShadow && closestType == GameFontTypes.Content_DefaultSize)
                {
                    closestType = GameFontTypes.Content_DefaultSize_GrayShadow;
                }

                _localizedText.FontType = closestType;
                _localizedText.FontTypeString = closestType.ToString();
                if (prevType != closestType)
                {
                    Log.Info(LogTags.Font, $"[UILocalizedText] FontType이 자동으로 변경: {prevType} → {closestType} (현재 폰트 크기: {currentFontSize})");
                }
            }
        }
    }
}