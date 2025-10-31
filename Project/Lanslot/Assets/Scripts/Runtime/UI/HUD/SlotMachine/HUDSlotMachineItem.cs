using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 개별 슬롯 UI 컴포넌트
    /// </summary>
    public class HUDSlotMachineItem : MonoBehaviour
    {
        [Header("UI 컴포넌트")]
        [SerializeField] private Image _slotImage;
        [SerializeField] private UILocalizedText _slotText;

        [Header("애니메이션 설정")]
        [SerializeField] private float _spinSpeed = 10f;
        [SerializeField] private AnimationCurve _stopCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        private float _spinTimer;
        private int _currentItemIndex;
        private SlotItemData[] _availableItems;

        public SlotState CurrentState { get; private set; } = SlotState.None;
        public SlotItemData CurrentItem { get; private set; }
        public bool IsLocked { get; private set; } = false;

        public System.Action<HUDSlotMachineItem> OnSlotStopped;

        private void Awake()
        {
            AutoGetComponents();
        }

        private void AutoGetComponents()
        {
            _slotImage = GetComponent<Image>();
            _slotText = GetComponentInChildren<UILocalizedText>();
        }

        private void Start()
        {
            SetState(SlotState.Idle);
        }

        private void Update()
        {
            LogicUpdate();
        }

        /// <summary>
        /// 슬롯 스핀 시작
        /// </summary>
        public void StartSpin(SlotItemData[] items)
        {
            if (CurrentState != SlotState.Idle || IsLocked)
            {
                return;
            }

            _availableItems = items;
            SetState(SlotState.Spinning);
            _currentItemIndex = 0;
            _spinTimer = 0f;
        }

        /// <summary>
        /// 슬롯 스핀 중지
        /// </summary>
        public void StopSpin(SlotItemData targetItem)
        {
            if (CurrentState != SlotState.Spinning || IsLocked)
            {
                return;
            }

            SetState(SlotState.Stopping);
            CurrentItem = targetItem;
        }

        /// <summary>
        /// 로직 업데이트
        /// </summary>
        public void LogicUpdate()
        {
            switch (CurrentState)
            {
                case SlotState.Spinning:
                    UpdateSpinning();
                    break;

                case SlotState.Stopping:
                    UpdateStopping();
                    break;
            }
        }

        /// <summary>
        /// 스핀 상태 업데이트
        /// </summary>
        private void UpdateSpinning()
        {
            _spinTimer += Time.deltaTime;

            // 아이템을 빠르게 순환하며 표시
            if (_spinTimer >= 0.1f)
            {
                if (_availableItems != null && _availableItems.Length > 0)
                {
                    _currentItemIndex = (_currentItemIndex + 1) % _availableItems.Length;
                    UpdateSlotDisplay(_availableItems[_currentItemIndex]);
                }
                _spinTimer = 0f;
            }
        }

        /// <summary>
        /// 스톱 상태 업데이트
        /// </summary>
        private void UpdateStopping()
        {
            // 멈추는 애니메이션 처리
            SetState(SlotState.Stopped);
            OnSlotStopped?.Invoke(this);
        }

        /// <summary>
        /// 상태 설정
        /// </summary>
        private void SetState(SlotState newState)
        {
            CurrentState = newState;
        }

        /// <summary>
        /// 슬롯 표시 업데이트
        /// </summary>
        private void UpdateSlotDisplay(SlotItemData item)
        {
            if (item != null)
            {
                if (_slotImage != null)
                {
                    _slotImage.SetSprite(item.ItemIcon);
                }

                if (_slotText != null)
                {
                    _slotText.SetText(item.ItemName);
                }
            }
        }

        /// <summary>
        /// 슬롯 잠금
        /// </summary>
        public void LockSlot()
        {
            IsLocked = true;
            UpdateLockVisual();
        }

        /// <summary>
        /// 슬롯 해금
        /// </summary>
        public void UnlockSlot()
        {
            IsLocked = false;
            UpdateLockVisual();
        }

        /// <summary>
        /// 잠금 상태 토글
        /// </summary>
        public void ToggleLock()
        {
            if (IsLocked)
            {
                UnlockSlot();
            }
            else
            {
                LockSlot();
            }
        }

        /// <summary>
        /// 잠금 시각적 효과 업데이트
        /// </summary>
        private void UpdateLockVisual()
        {
            // 잠금 상태에 따른 시각적 효과 처리
            if (_slotImage != null)
            {
                // 잠금 상태일 때 투명도 조정 또는 색상 변경
                Color imageColor = _slotImage.color;
                imageColor.a = IsLocked ? 0.5f : 1f;
                _slotImage.color = imageColor;
            }

            if (_slotText != null)
            {
                // 잠금 상태일 때 텍스트 색상 변경
                _slotText.SetTextColor(IsLocked ? Color.gray : Color.white);
            }
        }

        /// <summary>
        /// 슬롯 리셋
        /// </summary>
        public void ResetSlot()
        {
            SetState(SlotState.Idle);
            CurrentItem = null;
            _currentItemIndex = 0;
            _spinTimer = 0f;
            IsLocked = false;

            if (_slotImage != null)
            {
                _slotImage.sprite = null;
                Color imageColor = _slotImage.color;
                imageColor.a = 1f;
                _slotImage.color = imageColor;
            }

            if (_slotText != null)
            {
                _slotText.ResetText();
                _slotText.ResetTextColor();
            }
        }
    }
}