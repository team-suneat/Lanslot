using Sirenix.OdinInspector;
using TeamSuneat.Data.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 캐릭터 선택 슬롯용 셀. (아이콘/프레임/이름/선택 상태 등)
    /// </summary>
    public class UICharacterCell : XBehaviour
    {
        [Title("캐릭터 슬롯 UI 컴포넌트")]
        [SerializeField] private Image _frameImage;                  // 선택 등 프레임 강조 표시
        [SerializeField] private Image _iconImage;                   // 무기 아이콘
        [SerializeField] private UILocalizedText _nameText;          // 캐릭터 이름
        [SerializeField] private UIPointerEventButton _clickButton;  // 클릭 버튼

        private readonly UnityEvent<int> _onCellClick = new();

        public int Index { get; set; }
        public CharacterNames CharacterName { get; private set; }
        public bool IsLocked { get; private set; }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _frameImage = this.FindComponent<Image>("Frame Image");
            _iconImage = this.FindComponent<Image>("Character Icon Image");
            _nameText = this.FindComponent<UILocalizedText>("Character Name Text");
            _clickButton = this.FindComponent<UIPointerEventButton>("Click Button");
        }

        private void Awake()
        {
            // 버튼 클릭시 외부 콜백 실행되게 연결
            if (_clickButton != null)
            {
                _clickButton.RegisterClickEvent(OnClickButton);
            }
        }

        private void OnClickButton()
        {
            _onCellClick?.Invoke(Index);
        }

        //

        public void Setup(CharacterNames characterName, int index, bool isSelected, bool isLocked)
        {
            Index = index;
            CharacterName = characterName;
            IsLocked = isLocked;

            SetIconImage(characterName, isLocked);
            SetNameText(characterName);
            RefreshSelected(isSelected);
            RefreshButtonLock(isLocked);
        }

        private void SetIconImage(CharacterNames characterName, bool isLocked)
        {
            if (_iconImage != null)
            {
                Sprite sprite = characterName.LoadSprite();
                _iconImage.SetSprite(sprite, false);

                if (isLocked)
                {
                    _iconImage.SetColor(GameColors.DeepCharcoalBlack);
                }
                else
                {
                    _iconImage.SetColor(GameColors.White);
                }
            }
        }

        private void SetNameText(CharacterNames characterName)
        {
            if (_nameText != null)
            {
                string content = characterName.GetLocalizedString();
                _nameText.SetText(content);
            }
        }

        private void RefreshButtonLock(bool isLocked)
        {
            // 잠금 상태와 관계없이 버튼은 항상 활성화 (선택 가능하도록)
            if (_clickButton != null)
            {
                _clickButton.Unlock();
            }
        }

        private void RefreshSelected(bool isSelected)
        {
            if (_frameImage != null)
            {
                _frameImage.SetActive(isSelected);
            }
        }

        //

        public void RegisterClickEvent(UnityAction<int> action)
        {
            _onCellClick.AddListener(action);
        }

        public void Select()
        {
            _frameImage?.SetActive(true);

            VProfile profieInfo = GameApp.GetSelectedProfile();
            // profieInfo.Character.Select(CharacterName);
        }

        public void Deselect()
        {
            _frameImage?.SetActive(false);
        }

        /// <summary>
        /// 셀을 잠금 처리합니다. 잠금된 셀은 선택 해제할 수 없습니다.
        /// </summary>
        public void Lock()
        {
            IsLocked = true;
            _frameImage?.SetColor(GameColors.ActivateYellow, 0.75f);
            Select(); // 잠금 시 선택 상태로 만듦
        }

        /// <summary>
        /// 셀의 잠금을 해제합니다.
        /// </summary>
        public void Unlock()
        {
            IsLocked = false;
            _frameImage?.SetColor(GameColors.Ivory, 0.75f);
        }
    }
}