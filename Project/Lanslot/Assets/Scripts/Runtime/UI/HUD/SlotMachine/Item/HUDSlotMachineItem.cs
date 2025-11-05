using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 개별 슬롯 UI 컴포넌트
    /// </summary>
    public class HUDSlotMachineItem : XBehaviour
    {
        [FoldoutGroup("#Component")][SerializeField] private HUDSlotMachineItemScroller _scroller;
        [FoldoutGroup("#Component")][SerializeField] private HUDSlotMachineItemLock _lock;
        [FoldoutGroup("#Component")][SerializeField] private HUDSlotMachineItemAnimator _animator;
        [FoldoutGroup("#Component")][SerializeField] private UILocalizedText _itemNameText;

        private SlotState _currentState = SlotState.None;
        private Sprite _currentSprite;
        private ItemNames _currentItemName = ItemNames.None;

        public SlotState CurrentState => _currentState;
        public Sprite CurrentSprite => _currentSprite;
        public bool IsLocked => _lock != null && _lock.IsLocked;

        public System.Action<HUDSlotMachineItem> OnSlotStopped;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _scroller ??= GetComponentInChildren<HUDSlotMachineItemScroller>();
            _lock ??= GetComponentInChildren<HUDSlotMachineItemLock>();
            _animator ??= GetComponent<HUDSlotMachineItemAnimator>();
            _itemNameText ??= this.FindComponent<UILocalizedText>("ItemName Text");
        }

        protected override void OnStart()
        {
            base.OnStart();

            // Animator에 Scroller 참조 설정
            if (_animator != null && _scroller != null)
            {
                _animator.SetScroller(_scroller);
            }

            // 아이템 이름 텍스트 초기화
            ResetItemNameText();

            SetState(SlotState.Idle);
        }

        private void Update()
        {
            LogicUpdate();
        }

        /// <summary>
        /// 슬롯 스핀 시작
        /// </summary>
        public void StartSpin(Sprite[] sprites)
        {
            if (_currentState != SlotState.Idle)
            {
                Log.Warning(LogTags.UI_SlotMachine, "슬롯 스핀을 시작할 수 없습니다. 현재 상태: {0}", _currentState);
                return;
            }

            // 잠금 해제
            if (_lock != null && _lock.IsLocked)
            {
                _lock.Unlock();
            }

            // 스크롤 시작
            if (_scroller != null)
            {
                _scroller.StartScrolling(sprites);
            }

            SetState(SlotState.Spinning);
        }

        /// <summary>
        /// 슬롯 스핀 중지
        /// </summary>
        public void StopSpin(Sprite targetSprite, ItemNames itemName)
        {
            if (_currentState != SlotState.Spinning)
            {
                Log.Warning(LogTags.UI_SlotMachine, "슬롯 스핀을 중지할 수 없습니다. 현재 상태: {0}", _currentState);
                return;
            }

            if (targetSprite == null)
            {
                Log.Warning(LogTags.UI_SlotMachine, "목표 스프라이트가 null입니다.");
                return;
            }

            _currentSprite = targetSprite;
            _currentItemName = itemName;
            SetState(SlotState.Stopping);

            // 애니메이션 시작
            if (_animator != null)
            {
                _animator.StopAnimation(targetSprite, OnAnimationComplete);
            }
            else
            {
                OnAnimationComplete();
            }
        }

        /// <summary>
        /// 애니메이션 완료 콜백
        /// </summary>
        private void OnAnimationComplete()
        {
            SetState(SlotState.Stopped);

            // 아이템 이름 설정
            SetItemNameText();

            OnSlotStopped?.Invoke(this);
        }

        /// <summary>
        /// 로직 업데이트
        /// </summary>
        public void LogicUpdate()
        {
            if (_currentState == SlotState.Spinning && _scroller != null)
            {
                _scroller.UpdateScrolling();
            }
        }

        /// <summary>
        /// 상태 설정
        /// </summary>
        private void SetState(SlotState newState)
        {
            _currentState = newState;

            // 스크롤 상태에 따라 스크롤러 제어
            if (_scroller != null)
            {
                if (newState == SlotState.Spinning)
                {
                    // UpdateScrolling은 LogicUpdate에서 호출됨
                }
                else
                {
                    _scroller.StopScrolling();
                }
            }
        }

        /// <summary>
        /// 슬롯 잠금
        /// </summary>
        public void LockSlot()
        {
            if (_lock != null)
            {
                _lock.Lock();
            }
        }

        /// <summary>
        /// 슬롯 해금
        /// </summary>
        public void UnlockSlot()
        {
            if (_lock != null)
            {
                _lock.Unlock();
            }
        }

        /// <summary>
        /// 잠금 상태 토글
        /// </summary>
        public void ToggleLock()
        {
            if (_lock != null)
            {
                _lock.Toggle();
            }
        }

        /// <summary>
        /// 슬롯 리셋
        /// </summary>
        public void ResetSlot()
        {
            // 애니메이션 중지
            if (_animator != null)
            {
                _animator.Stop();
            }

            SetState(SlotState.Idle);
            _currentSprite = null;
            _currentItemName = ItemNames.None;

            // 아이템 이름 텍스트 초기화
            ResetItemNameText();

            // 각 컴포넌트 리셋
            if (_scroller != null)
            {
                _scroller.Reset();
            }

            if (_lock != null)
            {
                _lock.Reset();
            }
        }

        /// <summary>
        /// 아이템 이름 텍스트 설정
        /// </summary>
        private void SetItemNameText()
        {
            if (_itemNameText == null)
            {
                return;
            }

            if (_currentItemName == ItemNames.None)
            {
                ResetItemNameText();
                return;
            }

            string itemNameString = _currentItemName.GetLocalizedString();
            _itemNameText.SetText(itemNameString);
        }

        /// <summary>
        /// 아이템 이름 텍스트 초기화
        /// </summary>
        private void ResetItemNameText()
        {
            if (_itemNameText != null)
            {
                _itemNameText.ResetText();
            }
        }
    }
}