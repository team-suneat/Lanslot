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
        [SerializeField] private Image _iconImage;                      // 무기 아이콘
        [SerializeField] private UILocalizedText _nameText;          // 캐릭터 이름
        [SerializeField] private UIPointerEventButton _clickButton;  // 클릭 버튼

        private readonly UnityEvent<int> _onCellClick = new();

        public int Index { get; set; }
        public CharacterNames CharacterName { get; private set; }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _frameImage = this.FindComponent<Image>("Frame Image");
            _iconImage = this.FindComponent<Image>("Character Icon Image");
            _nameText = this.FindComponent<UILocalizedText>("Character Name Text");
            _clickButton = this.FindComponent<UIPointerEventButton>("Click Button");
        }

        /// <summary>
        /// 셀 정보를 바인딩(초기화)합니다.
        /// </summary>
        public void Setup(CharacterNames characterName, int index, bool isSelected)
        {
            Index = index;
            CharacterName = characterName;

            string spriteName = SpriteEx.GetSpriteName(characterName);
            _ = (_iconImage?.TrySetSprite(spriteName, false));
            _nameText?.SetText(characterName.GetLocalizedString());
            _frameImage?.SetActive(isSelected);
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

        public void RegisterClickEvent(UnityAction<int> action)
        {
            _onCellClick.AddListener(action);
        }

        public void Select()
        {
            _frameImage?.SetActive(true);

            VProfile profieInfo = GameApp.GetSelectedProfile();
            profieInfo.Character.Select(CharacterName);
        }

        public void Deselect()
        {
            _frameImage?.SetActive(false);
        }
    }
}