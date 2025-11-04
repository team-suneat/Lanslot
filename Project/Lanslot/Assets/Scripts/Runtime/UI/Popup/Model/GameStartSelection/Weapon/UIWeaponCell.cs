using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 무기 선택 슬롯용 셀. (아이콘/프레임/이름/선택 상태 등)
    /// </summary>
    public class UIWeaponCell : XBehaviour
    {
        [Title("무기 슬롯 UI 컴포넌트")]
        [SerializeField] private Image _frameImage;                    // 선택 등 프레임 강조 표시
        [SerializeField] private Image _iconImage;                     // 무기 아이콘
        [SerializeField] private UILocalizedText _nameText;            // 무기 이름
        [SerializeField] private UIPointerEventButton _clickButton;    // 클릭 버튼

        private readonly UnityEvent<int> _onCellClick = new();

        public int Index { get; set; }
        public ItemNames WeaponName { get; private set; }
        public bool IsDecided { get; private set; }
        public bool IsLocked { get; private set; }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _frameImage = this.FindComponent<Image>("Frame Image");
            _iconImage = this.FindComponent<Image>("Weapon Icon Image");
            _nameText = this.FindComponent<UILocalizedText>("Weapon Name Text");
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

        /// <summary>
        /// 셀 정보를 바인딩(초기화)합니다.
        /// </summary>
        public void Setup(WeaponData weaponData, int index, bool isSelected, bool isDecided, bool isLocked)
        {
            Index = index;
            WeaponName = weaponData.Name;
            IsDecided = isDecided;
            IsLocked = isLocked;

            SetIconImage(weaponData.Name, isDecided, isLocked);
            SetNameText(weaponData.Name, isLocked);
            RefreshSelected(isSelected);
            RefreshFrameColor(isDecided);
        }

        private void SetIconImage(ItemNames weaponName, bool isDecided, bool isLocked)
        {
            if (_iconImage != null)
            {
                // 잠금된 무기는 WeaponNames.None으로 스프라이트를 가져옴
                ItemNames spriteWeaponName = isLocked ? ItemNames.None : weaponName;
                string spriteName = SpriteEx.GetSpriteName(spriteWeaponName);
                _ = _iconImage.TrySetSprite(spriteName, false);

                // 잠금된 무기는 시각적으로 구분 (아이콘 색상 변경)
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

        private void SetNameText(ItemNames weaponName, bool isLocked)
        {
            if (_nameText != null)
            {
                string content = weaponName.GetLocalizedString();
                _nameText.SetText(content);
                _nameText.SetTextColor(isLocked ? GameColors.Disable : GameColors.CreamIvory);
            }
        }

        private void RefreshSelected(bool isSelected)
        {
            if (_frameImage != null)
            {
                _frameImage.SetActive(isSelected);
            }
        }

        private void RefreshFrameColor(bool isDecided)
        {
            if (_frameImage != null)
            {
                // 결정된 무기는 프레임 색상을 노란색으로 표시
                _frameImage.SetColor(isDecided ? GameColors.ActivateYellow : GameColors.Ivory, 0.75f);
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
            profieInfo?.Weapon.AddWeapon(WeaponName);
        }

        public void Deselect()
        {
            // 미리 결정된 셀은 선택 해제할 수 없음
            if (IsDecided)
            {
                return;
            }

            _frameImage?.SetActive(false);

            VProfile profieInfo = GameApp.GetSelectedProfile();
            profieInfo?.Weapon.RemoveWeapon(WeaponName);
        }
    }
}