using UnityEngine;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 개별 슬롯 UI 컴포넌트
    /// </summary>
    public class HUDSlotMachineItem : XBehaviour
    {
        [Header("컴포넌트")]
        [SerializeField] private HUDSlotMachineItemScroller _scroller;
        [SerializeField] private HUDSlotMachineItemLock _lock;
        [SerializeField] private HUDSlotMachineItemAnimator _animator;

        private SlotState _currentState = SlotState.None;
        private Sprite _currentSprite;

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
        }

        protected override void OnStart()
        {
            base.OnStart();

            // Animator에 Scroller 참조 설정
            if (_animator != null && _scroller != null)
            {
                _animator.SetScroller(_scroller);
            }

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
        public void StopSpin(Sprite targetSprite)
        {
            if (_currentState != SlotState.Spinning)
            {
                return;
            }

            _currentSprite = targetSprite;
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
    }
}