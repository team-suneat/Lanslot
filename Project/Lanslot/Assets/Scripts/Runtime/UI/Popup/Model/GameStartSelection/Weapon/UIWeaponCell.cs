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
        public WeaponNames WeaponName { get; private set; }
        public bool IsLocked { get; private set; }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _frameImage = this.FindComponent<Image>("Frame Image");
            _iconImage = this.FindComponent<Image>("Weapon Icon Image");
            _nameText = this.FindComponent<UILocalizedText>("Weapon Name Text");
            _clickButton = this.FindComponent<UIPointerEventButton>("Click Button");
        }

        /// <summary>
        /// 셀 정보를 바인딩(초기화)합니다.
        /// </summary>
        public void Setup(WeaponData weaponData, int index, bool isSelected, bool isLocked = false)
        {
            Index = index;
            WeaponName = weaponData.Name;
            IsLocked = isLocked;

            string spriteName = SpriteEx.GetSpriteName(weaponData.Name);
            _ = (_iconImage?.TrySetSprite(spriteName, false));

            _nameText?.SetText(weaponData.Name.GetLocalizedString());

            if (_frameImage != null)
            {
                _frameImage.SetActive(isSelected);
                _frameImage.SetColor(isLocked ? GameColors.ActivateYellow : GameColors.Ivory, 0.75f);
            }
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
            // 잠금된 셀은 클릭 이벤트를 발생시키지 않음
            if (IsLocked)
            {
                return;
            }

            _onCellClick?.Invoke(Index);
        }

        public void RegisterClickEvent(UnityAction<int> action)
        {
            _onCellClick.AddListener(action);
        }

        public void Select()
        {
            _frameImage?.SetActive(true);

            VProfile profieInfo = GameApp.GetSelectedProfile();
            profieInfo?.Weapon.SelectWeapon(WeaponName);
        }

        public void Deselect()
        {
            // 잠금된 셀은 선택 해제할 수 없음
            if (IsLocked)
            {
                return;
            }

            _frameImage?.SetActive(false);

            VProfile profieInfo = GameApp.GetSelectedProfile();
            profieInfo?.Weapon.DeselectWeapon(WeaponName);
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