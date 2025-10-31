using DG.Tweening;
using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Setting;
using TMPro;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UILocalizedText : XBehaviour
    {
        [Title("#UI Localized Text")]
        public string StringKey;

        public GameFontTypes FontType;
        public string FontTypeString;
        public TextMeshProUGUI TextPro;
        public bool SizeToTextLenthX;
        public bool SizeToTextLenthY;
        public int CustomAddtionalFontSize;

        [Title("#UI Localized Text", "Language")]
        public bool UseCustomLanguage;

        public LanguageNames CustomLanguage;

        public float CustomFontSize { get; set; }

        private float _defaultFontSize;
        private string _content;
        private string _spriteContent;

        public Color DefaultTextColor { get; private set; }

        public int FontSize => TextPro != null ? (int)TextPro.fontSize : 0;

        #region 컴포넌트 할당

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            AssignTextComponents();
        }

        private void AssignTextComponents()
        {
            if (TextPro == null)
            {
                TextPro = GetComponent<TextMeshProUGUI>();
            }
        }

        #endregion 컴포넌트 할당

        #region 초기화 및 이벤트

        private void OnValidate()
        {
            if (!EnumEx.ConvertTo(ref FontType, FontTypeString))
            {
                Log.Error(LogTags.Font, "{0} 폰트 타입이 변환되지 않습니다. {1}", FontType, FontTypeString);
            }
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (!string.IsNullOrEmpty(StringKey))
            {
                StringKey = StringKey.Replace(" ", "");
            }
            if (FontType != GameFontTypes.None)
            {
                FontTypeString = FontType.ToString();
            }
        }

        /// <summary>
        /// 현재 설정된 FontAsset에서 TextPro의 폰트 크기에 가장 가까운 타입으로 FontType을 설정합니다.
        /// </summary>
        private void SetClosestFontTypeByCurrentFontSize(LanguageNames languageName)
        {
            if (TextPro == null)
            {
                return;
            }

            if (FontType is GameFontTypes.Difficulty or GameFontTypes.Number or GameFontTypes.Content_DialogueTitle)
            {
                return;
            }

            float currentFontSize = TextPro.fontSize;
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
                    Log.Info(LogTags.Font, $"[UILocalizedText] FontType이 자동으로 변경: {prevType} → {closestType} (현재 폰트 크기: {currentFontSize})");
                }
            }
        }

        //

        protected void Awake()
        {
            AssignTextComponents();

            if (TextPro != null)
            {
                DefaultTextColor = TextPro.color;
                _defaultFontSize = TextPro.fontSize;
                _content = TextPro.text;
            }
        }

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();
            GlobalEvent.Register(GlobalEventType.GAME_LANGUAGE_CHANGED, OnGameLanguageChanged);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();
            GlobalEvent.Unregister(GlobalEventType.GAME_LANGUAGE_CHANGED, OnGameLanguageChanged);
        }

        private void OnGameLanguageChanged()
        {
            Refresh(GameSetting.Instance.Language.Name);
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();

            Refresh(GameSetting.Instance.Language.Name);
        }

        #endregion 초기화 및 이벤트

        #region 활성화

        public void Activate()
        {
            if (TextPro != null)
            {
                TextPro.enabled = true;
            }
        }

        public void Deactivate()
        {
            if (TextPro != null)
            {
                TextPro.enabled = false;
            }
        }

        #endregion 활성화

        #region 텍스트 처리

        public void ResetText()
        {
            ResetTextInteral();
        }

        private void ResetTextInteral()
        {
            if (TextPro == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(_content))
            {
                _content = string.Empty;
            }
            OnSetText();
        }

        public void SetText(string content)
        {
            if (TextPro == null)
            {
                return;
            }

            if (_content == content)
            {
                return;
            }

            _content = content;
            OnSetText();
        }

        public void SetStringKey(string stringKey)
        {
            if (StringKey == stringKey)
            {
                return;
            }

            StringKey = stringKey;

            RefreshContent(GameSetting.Instance.Language.Name);
        }

        public void ResetStringKey()
        {
            if (string.IsNullOrEmpty(StringKey))
            {
                return;
            }

            StringKey = string.Empty;
            RefreshContent(GameSetting.Instance.Language.Name);
        }

        public void SetSpriteText(string spriteContent)
        {
            if (TextPro == null)
            {
                return;
            }

            if (_spriteContent == spriteContent)
            {
                return;
            }

            _spriteContent = spriteContent;
            OnSetText();
        }

        public void ResetSpriteText()
        {
            if (TextPro == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(_spriteContent))
            {
                return;
            }

            _spriteContent = string.Empty;
            OnSetText();
        }

        private void OnSetText()
        {
            if (!string.IsNullOrEmpty(_spriteContent) && !string.IsNullOrEmpty(_content))
            {
                TextPro.SetText($"{_spriteContent} {_content}");
            }
            else if (!string.IsNullOrEmpty(_spriteContent))
            {
                TextPro.SetText(_spriteContent);
            }
            else if (!string.IsNullOrEmpty(_content))
            {
                TextPro.SetText(_content);
            }
            else
            {
                TextPro.ResetText();
            }

            RefreshTextRectSize();
        }

        #endregion 텍스트 처리

        #region 폰트 크기 및 스타일

        public void SetDefaultFontSize(float fontSize)
        {
            _defaultFontSize = fontSize;
            RefreshFontSize();
        }

        private void RefreshFontSize()
        {
            LanguageNames languageName = GameSetting.Instance.Language.Name;
            FontAsset fontData = ScriptableDataManager.Instance.FindFont(languageName);
            RefreshFontSize(languageName, fontData);
        }

        private void RefreshFontSize(LanguageNames languageName, FontAsset fontData)
        {
            if (TextPro == null)
            {
                return;
            }

            if (fontData != null)
            {
                FontAsset.FontAssetData? fontAssetData = fontData.GetFontAssetData(FontType);
                if (fontAssetData != null)
                {
                    float fontSize;
                    if (CustomFontSize > 0)
                    {
                        fontSize = CustomFontSize;
                    }
                    else
                    {
                        fontSize = fontAssetData.Value.FontSize + CustomAddtionalFontSize;
                    }

                    if (TextPro.fontSize != fontSize)
                    {
                        TextPro.enableAutoSizing = false;
                        TextPro.fontSize = fontSize;

                        Log.Info(LogTags.Font, "폰트 타입({0})에 맞는 폰트 크기를 적용합니다: {1} + {2} = {3}, {4}",
                            FontType, fontAssetData.Value.FontSize, CustomAddtionalFontSize, TextPro.fontSize, this.GetHierarchyPath());
                    }
                }
            }
            else if (_defaultFontSize > 0)
            {
                TextPro.fontSize = _defaultFontSize + CustomAddtionalFontSize;
            }
        }

        public void SetAlignment(TextAlignmentOptions alignment)
        {
            if (TextPro != null)
            {
                TextPro.alignment = alignment;
            }
        }

        public void SetFontStyle(FontStyles fontStyle)
        {
            if (TextPro != null)
            {
                TextPro.fontStyle = fontStyle;
            }
        }

        public void SetSpriteAsset(TMP_SpriteAsset spriteAsset)
        {
            if (TextPro != null)
            {
                TextPro.spriteAsset = spriteAsset;
            }
        }

        public void SetTextColor(Color fontColor)
        {
            if (TextPro != null)
            {
                TextPro.SetTextColor(fontColor);
            }
        }

        #endregion 폰트 크기 및 스타일

        #region 페이드 효과

        public Tweener FadeOut(float targetAlpha, float duration, float delayTime)
        {
            if (TextPro != null)
            {
                return TextPro.FadeOut(targetAlpha, duration, delayTime);
            }
            return null;
        }

        #endregion 페이드 효과

        #region 언어변환 및 폰트 설정

        public void Refresh(LanguageNames languageName)
        {
            string storageContent = _content;

            ResetTextInteral();

            RefreshFont(languageName);
            RefreshContent(languageName, storageContent);
            RefreshTextRectSize();
        }

        private void RefreshContent(LanguageNames languageName, string storageContent)
        {
            if (string.IsNullOrEmpty(StringKey))
            {
                if (!string.IsNullOrEmpty(storageContent))
                {
                    SetText(storageContent);
                }
                return;
            }

            string content = JsonDataManager.FindStringClone(StringKey, languageName);
            SetText(content);
        }

        private void RefreshContent(LanguageNames languageName)
        {
            if (string.IsNullOrEmpty(StringKey))
            {
                return;
            }

            string content = JsonDataManager.FindStringClone(StringKey, languageName);
            SetText(content);
        }

        private void RefreshFont(LanguageNames languageName)
        {
            if (FontType == GameFontTypes.None) { return; }
            if (UseCustomLanguage)
            {
                languageName = CustomLanguage;
            }

            FontAsset fontData = ScriptableDataManager.Instance.FindFont(languageName);
            if (fontData == null)
            {
                return;
            }

            TMP_FontAsset fontAsset = fontData.FindFont(FontType);
            if (fontAsset == null)
            {
                return;
            }

            if (TextPro != null)
            {
                TextPro.font = fontAsset;
                if (fontData.FindItalic(FontType))
                {
                    TextPro.fontStyle = FontStyles.Italic;
                }
                RefreshFontSize(languageName, fontData);
            }
        }

        #endregion 언어변환 및 폰트 설정

        #region 텍스트 색상

        public void ResetTextColor()
        {
            if (TextPro != null)
            {
                TextPro.color = DefaultTextColor;
            }
        }

        #endregion 텍스트 색상

        #region 크기 조정

        private void RefreshTextRectSize()
        {
            if (SizeToTextLenthX)
            {
                RefreshTextRectSizeByTextWidth();
            }
            if (SizeToTextLenthY)
            {
                RefreshTextRectSizeByTextHeight();
            }
        }

        private void RefreshTextRectSizeByTextHeight()
        {
            if (TextPro == null || TextPro.font == null)
            {
                Log.Warning(LogTags.Font, $"[UILocalizedText] TextPro 또는 font가 null입니다. ({this.GetHierarchyName()})");
                return;
            }
            TextPro.rectTransform.sizeDelta = new Vector2(TextPro.rectTransform.sizeDelta.x, TextPro.preferredHeight);
        }

        private void RefreshTextRectSizeByTextWidth()
        {
            if (TextPro == null || TextPro.font == null)
            {
                Log.Warning(LogTags.Font, $"[UILocalizedText] TextPro 또는 font가 null입니다. ({this.GetHierarchyName()})");
                return;
            }
            TextPro.rectTransform.sizeDelta = new Vector2(TextPro.preferredWidth, TextPro.rectTransform.sizeDelta.y);
        }

        #endregion 크기 조정

        #region 밑줄처리

        public void SetUnderline(bool isActive)
        {
            if (TextPro == null)
            {
                return;
            }

            if (isActive)
            {
                TextPro.fontStyle |= TMPro.FontStyles.Underline;
            }
            else
            {
                TextPro.fontStyle &= ~TMPro.FontStyles.Underline;
            }
        }

        #endregion 밑줄처리
    }
}