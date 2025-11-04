using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 슬롯 머신 아이템의 잠금 기능을 담당하는 컴포넌트
    /// </summary>
    public class HUDSlotMachineItemLock : XBehaviour
    {
        [Header("UI 컴포넌트")]
        [SerializeField] private Image _lockImage;
        [SerializeField] private UILocalizedText _slotText;

        private bool _isLocked = true;

        public bool IsLocked
        {
            get => _isLocked;
            private set
            {
                if (_isLocked != value)
                {
                    _isLocked = value;
                    UpdateVisual();
                }
            }
        }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _lockImage ??= GetComponent<Image>();
            _slotText ??= this.FindComponent<UILocalizedText>("Slot Text");
            
            // 부모에서 찾기 시도
            if (_slotText == null)
            {
                HUDSlotMachineItem parentItem = GetComponentInParent<HUDSlotMachineItem>();
                if (parentItem != null)
                {
                    _slotText = parentItem.GetComponentInChildren<UILocalizedText>();
                }
            }
        }

        private void Start()
        {
            UpdateVisual();
        }

        /// <summary>
        /// 슬롯 잠금
        /// </summary>
        public void Lock()
        {
            IsLocked = true;
        }

        /// <summary>
        /// 슬롯 해금
        /// </summary>
        public void Unlock()
        {
            IsLocked = false;
        }

        /// <summary>
        /// 잠금 상태 토글
        /// </summary>
        public void Toggle()
        {
            if (IsLocked)
            {
                Unlock();
            }
            else
            {
                Lock();
            }
        }

        /// <summary>
        /// 잠금 시각적 효과 업데이트
        /// </summary>
        private void UpdateVisual()
        {
            // 잠금 이미지 표시/숨김 (아이콘보다 앞에 위치)
            if (_lockImage != null)
            {
                _lockImage.gameObject.SetActive(IsLocked);

                // Canvas 순서를 높여서 앞에 표시 (한 번만 설정)
                if (IsLocked)
                {
                    Canvas canvas = _lockImage.GetComponent<Canvas>();
                    if (canvas == null)
                    {
                        canvas = _lockImage.gameObject.AddComponent<Canvas>();
                    }
                    canvas.overrideSorting = true;
                    canvas.sortingOrder = 10;
                }
            }

            if (_slotText != null)
            {
                // 잠금 상태일 때 텍스트 색상 변경
                _slotText.SetTextColor(IsLocked ? Color.gray : Color.white);
            }
        }

        /// <summary>
        /// 리셋
        /// </summary>
        public void Reset()
        {
            IsLocked = true;

            if (_slotText != null)
            {
                _slotText.ResetText();
                _slotText.ResetTextColor();
            }
        }
    }
}

