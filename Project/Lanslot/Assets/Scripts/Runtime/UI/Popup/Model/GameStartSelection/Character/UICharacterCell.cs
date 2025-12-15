using Sirenix.OdinInspector;
using TeamSuneat.Data.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // 캐릭터 선택 슬롯용 셀 (아이콘/프레임/이름/선택/보너스/잠금 표시)
    public class UICharacterCell : XBehaviour
    {
        [Title("캐릭터 슬롯 UI 컴포넌트")]
        [SerializeField] private Image _lockedFrameImage;               // 잠금 상태 프레임
        [SerializeField] private Image _unlockedFrameImage;             // 해금 상태 프레임
        [SerializeField] private Image _lockImage;                      // 잠금 아이콘
        [SerializeField] private Image _selectedImage;                  // 선택 표시 이미지
        [SerializeField] private Image _bonusImage;                     // 보너스 표시 이미지
        [SerializeField] private Image _iconImage;                      // 무기 아이콘
        [SerializeField] private UILocalizedText _nameText;             // 캐릭터 이름
        [SerializeField] private UIPointerEventButton _clickButton;     // 클릭 버튼

        private readonly UnityEvent<int> _onCellClick = new();
        private bool _isSelected;

        public int Index { get; set; }
        public CharacterNames CharacterName { get; private set; }
        public bool IsLocked { get; private set; }
        public bool HasBonus { get; private set; }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _lockedFrameImage = this.FindComponent<Image>("LockedFrame Image");
            _unlockedFrameImage = this.FindComponent<Image>("UnlockedFrame Image");
            _lockImage = this.FindComponent<Image>("Lock Image");
            _selectedImage = this.FindComponent<Image>("Selected Image");
            _bonusImage = this.FindComponent<Image>("Bonus Image");
            _iconImage = this.FindComponent<Image>("Background Image/Character Icon Image");
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

        public void Setup(CharacterNames characterName, int index, bool isSelected, bool isLocked, bool hasBonus = false)
        {
            Index = index;
            CharacterName = characterName;
            IsLocked = isLocked;
            HasBonus = hasBonus;
            _isSelected = isSelected;

            SetIconImage(characterName, isLocked);
            SetNameText(characterName);
            ApplyVisualStates();
            RefreshButtonLock(isLocked);
        }

        private void SetIconImage(CharacterNames characterName, bool isLocked)
        {
            if (_iconImage != null)
            {
                Sprite sprite = characterName.LoadSprite();
                _iconImage.SetSprite(sprite, true);

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

        private void ApplyVisualStates()
        {
            SetFrameImageByLock(IsLocked);
            SetLockImage(IsLocked);
            SetSelectedImage(_isSelected);
            SetBonusImage(HasBonus);
        }

        private void SetFrameImageByLock(bool isLocked)
        {
            _lockedFrameImage?.SetActive(isLocked);
            _unlockedFrameImage?.SetActive(!isLocked);
        }

        private void SetLockImage(bool isLocked)
        {
            _lockImage?.SetActive(isLocked);
        }

        private void SetSelectedImage(bool isSelected)
        {
            _selectedImage?.SetActive(isSelected);
        }

        private void SetBonusImage(bool hasBonus)
        {
            _bonusImage?.SetActive(hasBonus);
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
            _isSelected = isSelected;
            ApplyVisualStates();
        }

        //

        public void RegisterClickEvent(UnityAction<int> action)
        {
            _onCellClick.AddListener(action);
        }

        public void Select()
        {
            RefreshSelected(true);
        }

        public void Deselect()
        {
            if (IsLocked)
            {
                return;
            }

            RefreshSelected(false);
        }

        public void Lock()
        {
            IsLocked = true;
            _isSelected = true;
            ApplyVisualStates();
        }

        public void Unlock()
        {
            IsLocked = false;
            ApplyVisualStates();
        }
    }
}