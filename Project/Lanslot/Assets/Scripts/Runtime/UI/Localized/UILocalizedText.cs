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
        public int CustomAddtionalFontSize;

        [Title("#UI Localized Text", "Language")]
        public bool UseCustomLanguage;

        public LanguageNames CustomLanguage;

        public float CustomFontSize { get; set; }

        [SerializeField]
        private UILocalizedTextFontController _fontController;
        private string _content;
        private string _spriteContent;

        [SerializeField]
        private UILocalizedTextSizeController _sizeController;

        public Color DefaultTextColor { get; private set; }

        public int FontSize => TextPro != null ? (int)TextPro.fontSize : 0;

        #region 컴포넌트 할당

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            AssignTextComponents();
            AssignFontController();
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

        protected void Awake()
        {
            AssignTextComponents();
            AssignFontController();

            if (TextPro != null)
            {
                DefaultTextColor = TextPro.color;
                _content = TextPro.text;
                FontController?.InitializeDefaultFontSize(TextPro.fontSize);
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
            FontController?.SetDefaultFontSize(fontSize);
        }

        private void RefreshFontSize()
        {
            FontController?.RefreshFontSize();
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
            FontController?.RefreshFont(languageName);
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
            UILocalizedTextSizeController controller = SizeController;
            if (controller == null)
            {
                return;
            }

            controller.Refresh(TextPro);
        }

        private UILocalizedTextSizeController SizeController => _sizeController ??= GetComponent<UILocalizedTextSizeController>();

        private UILocalizedTextFontController FontController => _fontController ??= GetComponent<UILocalizedTextFontController>();

        private void AssignFontController()
        {
            if (_fontController == null)
            {
                _fontController = GetComponent<UILocalizedTextFontController>();
            }
        }

        #endregion 크기 조정

        #region 밑줄 처리

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