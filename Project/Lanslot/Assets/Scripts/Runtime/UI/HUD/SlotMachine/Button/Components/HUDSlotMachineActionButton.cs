using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 슬롯 머신의 액션 버튼을 관리하는 컴포넌트
    /// 상태에 따라 Spin/Stop 동작을 수행하고 UI를 업데이트합니다.
    /// </summary>
    public class HUDSlotMachineActionButton : XBehaviour
    {
        [FoldoutGroup("#Components")]
        [SerializeField] private UIPointerEventButton _actionButton;
        
        [FoldoutGroup("#Components")]
        [SerializeField] private UILocalizedText _buttonText;

        [FoldoutGroup("#Settings")]
        [SerializeField] private string _spinText = "스핀";
        
        [FoldoutGroup("#Settings")]
        [SerializeField] private string _stopText = "스톱";

        [FoldoutGroup("#Events")]
        public UnityEvent OnSpinRequested;
        
        [FoldoutGroup("#Events")]
        public UnityEvent OnStopRequested;

        private SlotMachineState _currentState = SlotMachineState.None;
        private int _lastStopIndex = -1;
        private int _lastTotalSlots = 0;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _actionButton ??= GetComponent<UIPointerEventButton>();
            _buttonText ??= GetComponentInChildren<UILocalizedText>();
        }

        protected override void OnStart()
        {
            base.OnStart();
            SetupEvents();
        }

        /// <summary>
        /// 이벤트 설정
        /// </summary>
        private void SetupEvents()
        {
            if (_actionButton != null)
            {
                _actionButton.RegisterClickEvent(OnButtonClick);
            }
        }

        /// <summary>
        /// 상태 업데이트 (HUDSlotMachine에서 호출)
        /// </summary>
        public void UpdateState(SlotMachineState newState, int currentStopIndex = 0, int totalSlots = 0)
        {
            bool stateChanged = _currentState != newState;
            bool indexChanged = _lastStopIndex != currentStopIndex || _lastTotalSlots != totalSlots;

            if (!stateChanged && !indexChanged)
            {
                return;
            }

            _currentState = newState;
            _lastStopIndex = currentStopIndex;
            _lastTotalSlots = totalSlots;
            UpdateButtonUI(currentStopIndex, totalSlots);
        }

        /// <summary>
        /// 버튼 UI 업데이트
        /// </summary>
        private void UpdateButtonUI(int currentStopIndex = 0, int totalSlots = 0)
        {
            switch (_currentState)
            {
                case SlotMachineState.Idle:
                    SetButtonText(_spinText);
                    SetButtonEnabled(true);
                    break;

                case SlotMachineState.Spinning:
                    string stopTextWithCount = totalSlots > 0 
                        ? $"{_stopText} ({currentStopIndex}/{totalSlots})" 
                        : _stopText;
                    SetButtonText(stopTextWithCount);
                    SetButtonEnabled(true);
                    break;

                case SlotMachineState.Result:
                    SetButtonText(_spinText);
                    SetButtonEnabled(true);
                    break;

                default:
                    SetButtonEnabled(false);
                    break;
            }
        }

        /// <summary>
        /// 버튼 텍스트 설정
        /// </summary>
        private void SetButtonText(string text)
        {
            if (_buttonText != null)
            {
                _buttonText.SetText(text);
            }
        }

        /// <summary>
        /// 버튼 활성화/비활성화
        /// </summary>
        private void SetButtonEnabled(bool enabled)
        {
            if (_actionButton == null)
            {
                return;
            }

            if (enabled)
            {
                _actionButton.Unlock();
            }
            else
            {
                _actionButton.Lock();
            }
        }

        /// <summary>
        /// 버튼 클릭 이벤트
        /// </summary>
        private void OnButtonClick()
        {
            switch (_currentState)
            {
                case SlotMachineState.Idle:
                    OnSpinRequested?.Invoke();
                    break;

                case SlotMachineState.Spinning:
                    OnStopRequested?.Invoke();
                    break;

                default:
                    // Idle과 Spinning 상태가 아니면 동작하지 않음
                    break;
            }
        }

        /// <summary>
        /// 초기화
        /// </summary>
        public void Initialize(SlotMachineState initialState)
        {
            _currentState = initialState;
            UpdateButtonUI();
        }
    }
}

