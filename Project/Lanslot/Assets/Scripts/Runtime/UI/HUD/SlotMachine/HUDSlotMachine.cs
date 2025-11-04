using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 슬롯 머신 전체를 관리하는 HUD 컴포넌트
    /// </summary>
    public class HUDSlotMachine : XBehaviour
    {
        [FoldoutGroup("#Components")][SerializeField] private UIPointerEventButton _spinButton;
        [FoldoutGroup("#Components")][SerializeField] private UIPointerEventButton _stopButton;
        [FoldoutGroup("#Components")][SerializeField] private HUDSlotMachineItem[] _items;
        [FoldoutGroup("#Components")][SerializeField] private UILocalizedText _statusText;

        [FoldoutGroup("#Settings")][SerializeField] private int _slotCount = 6;
        [FoldoutGroup("#Settings")][SerializeField] private float _spinDuration = 2f;
        [FoldoutGroup("#Settings")][SerializeField] private float _stopDelay = 0.5f;
        [FoldoutGroup("#Settings")][SerializeField] private Sprite[] _availableSprites;

        [FoldoutGroup("#Event")][SerializeField] private UnityEvent OnAllSlotsStopped;
        [FoldoutGroup("#Event")][SerializeField] private UnityEvent<Sprite[]> OnSlotResult;

        private int _stoppedSlotCount = 0;
        private int _currentStopIndex = 0;
        private Sprite[] _currentResults;

        public SlotMachineState CurrentState { get; private set; } = SlotMachineState.None;
        public bool CanSpin => CurrentState == SlotMachineState.Idle;

        public System.Action<Sprite[]> OnSlotMachineCompleted;

        private void Awake()
        {
            AutoGetComponents();
        }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _spinButton = this.FindComponent<UIPointerEventButton>("Spin Button");
            _stopButton = this.FindComponent<UIPointerEventButton>("Stop Button");
            _items = GetComponentsInChildren<HUDSlotMachineItem>();
            _statusText = GetComponentInChildren<UILocalizedText>();
        }

        protected override void OnStart()
        {
            base.OnStart();

            Initialize();
            SetupEvents();
        }

        private void Update()
        {
            LogicUpdate();
        }

        /// <summary>
        /// 초기화
        /// </summary>
        private void Initialize()
        {
            SetState(SlotMachineState.Idle);
            SetupSlots();
            UpdateUI();
        }

        /// <summary>
        /// 슬롯 설정
        /// </summary>
        private void SetupSlots()
        {
            for (int i = 0; i < _items.Length; i++)
            {
                _items[i].OnSlotStopped += OnSlotStopped;
            }
        }

        /// <summary>
        /// 이벤트 설정
        /// </summary>
        private void SetupEvents()
        {
            _spinButton.RegisterClickEvent(OnSpinButtonClick);
            _stopButton.RegisterClickEvent(OnStopButtonClick);
        }

        /// <summary>
        /// 스핀 시작
        /// </summary>
        public void StartSpin()
        {
            if (!CanSpin)
            {
                return;
            }

            SetState(SlotMachineState.Spinning);
            _stoppedSlotCount = 0;
            _currentStopIndex = 0;
            _currentResults = new Sprite[_items.Length];

            // 모든 슬롯 시작
            for (int i = 0; i < _items.Length; i++)
            {
                _items[i].StartSpin(_availableSprites);
            }

            UpdateUI();
        }

        /// <summary>
        /// 다음 슬롯 멈추기
        /// </summary>
        public void StopNextSlot()
        {
            if (CurrentState != SlotMachineState.Spinning)
            {
                return;
            }

            // 다음 멈출 슬롯 찾기
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i].CurrentState == SlotState.Spinning)
                {
                    Sprite targetSprite = GetRandomSprite();
                    _items[i].StopSpin(targetSprite);
                    _currentResults[i] = targetSprite;
                    _currentStopIndex++;
                    break;
                }
            }

            UpdateUI();
        }

        /// <summary>
        /// 슬롯이 멈췄을 때 호출
        /// </summary>
        private void OnSlotStopped(HUDSlotMachineItem slot)
        {
            _stoppedSlotCount++;

            if (_stoppedSlotCount >= _items.Length)
            {
                OnAllSlotsStopped?.Invoke();
                OnSlotResult?.Invoke(_currentResults);
                OnSlotMachineCompleted?.Invoke(_currentResults);
                SetState(SlotMachineState.Result);
                UpdateUI();
            }
        }

        /// <summary>
        /// 스핀 버튼 클릭
        /// </summary>
        private void OnSpinButtonClick()
        {
            if (CurrentState == SlotMachineState.Idle)
            {
                StartSpin();
            }
        }

        /// <summary>
        /// 스톱 버튼 클릭
        /// </summary>
        private void OnStopButtonClick()
        {
            if (CurrentState == SlotMachineState.Spinning)
            {
                StopNextSlot();
            }
        }

        /// <summary>
        /// 랜덤 스프라이트 선택
        /// </summary>
        private Sprite GetRandomSprite()
        {
            if (_availableSprites == null || _availableSprites.Length == 0)
            {
                Debug.LogWarning("사용 가능한 스프라이트가 없습니다.");
                return null;
            }

            return _availableSprites[Random.Range(0, _availableSprites.Length)];
        }

        /// <summary>
        /// 상태 설정
        /// </summary>
        private void SetState(SlotMachineState newState)
        {
            CurrentState = newState;
        }

        /// <summary>
        /// 로직 업데이트
        /// </summary>
        public void LogicUpdate()
        {
            if (CurrentState == SlotMachineState.Spinning)
            {
                for (int i = 0; i < _items.Length; i++)
                {
                    _items[i].LogicUpdate();
                }
            }
        }

        /// <summary>
        /// UI 업데이트
        /// </summary>
        private void UpdateUI()
        {
            switch (CurrentState)
            {
                case SlotMachineState.Idle:
                    if (_statusText != null)
                    {
                        _statusText.SetText("스핀 버튼을 눌러주세요");
                    }

                    _spinButton.Unlock();
                    _stopButton.Lock();
                    break;

                case SlotMachineState.Spinning:
                    if (_statusText != null)
                    {
                        _statusText.SetText($"스톱 버튼을 눌러 슬롯을 멈추세요 ({_currentStopIndex}/{_items.Length})");
                    }

                    _spinButton.Lock();
                    _stopButton.Unlock();
                    break;

                case SlotMachineState.Result:
                    if (_statusText != null)
                    {
                        _statusText.SetText("결과를 확인하세요");
                    }

                    _spinButton.Unlock();
                    _stopButton.Lock();
                    break;
            }
        }

        /// <summary>
        /// 슬롯 머신 리셋
        /// </summary>
        public void ResetSlotMachine()
        {
            SetState(SlotMachineState.Idle);
            _stoppedSlotCount = 0;
            _currentStopIndex = 0;
            _currentResults = null;

            for (int i = 0; i < _items.Length; i++)
            {
                _items[i].ResetSlot();
            }

            UpdateUI();
        }

        /// <summary>
        /// 사용 가능한 스프라이트 설정
        /// </summary>
        public void SetAvailableSprites(Sprite[] sprites)
        {
            _availableSprites = sprites;
        }
    }
}