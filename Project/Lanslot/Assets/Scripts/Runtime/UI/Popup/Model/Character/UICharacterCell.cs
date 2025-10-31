using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 캐릭터 선택 슬롯용 셀. (아이콘/프레임/이름/선택 상태 등)
    /// </summary>
    public class UICharacterCell : MonoBehaviour
    {
        [Title("캐릭터 슬롯 UI 컴포넌트")]
        [SerializeField] private Image _frameImage;                  // 선택 등 프레임 강조 표시
        [SerializeField] private UILocalizedText _nameText;          // 캐릭터 이름
        [SerializeField] private UIPointerEventButton _clickButton;  // 클릭 버튼

        private UnityEvent<int> _onCellClick = new UnityEvent<int>();
        public int Index { get; set; }

        /// <summary>
        /// 셀 정보를 바인딩(초기화)합니다.
        /// </summary>
        public void Setup(string characterName, int index, bool isSelected)
        {
            Index = index;

            _nameText?.SetText(characterName);
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
            if (_onCellClick != null)
            {
                _onCellClick.Invoke(Index);
            }
        }

        public void RegisterClickEvent(UnityAction<int> action)
        {
            _onCellClick.AddListener(action);
        }

        public void Select()
        {
            _frameImage?.SetActive(true);
        }

        public void Deselect()
        {
            _frameImage?.SetActive(false);
        }
    }
}