using Sirenix.OdinInspector;
using TeamSuneat.Data;
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

        private void OnButtonClick()
        {
            switch (_currentState)
            {
                case SlotMachineState.Spinning:
                    OnStopRequested?.Invoke();
                    break;
            }
        }

        public void Initialize(SlotMachineState initialState)
        {
            _currentState = initialState;
            UpdateButtonUI();
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
                case SlotMachineState.Spinning:
                    {
                        string content = JsonDataManager.FindStringClone("Button_Stop");
                        SetButtonText(content);
                        SetButtonEnabled(true);
                    }
                    break;

                default:
                    SetButtonEnabled(false);
                    return;
            }
        }

        private void SetButtonText(string text)
        {
            if (_buttonText != null)
            {
                _buttonText.SetText(text);
            }
        }

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
    }
}