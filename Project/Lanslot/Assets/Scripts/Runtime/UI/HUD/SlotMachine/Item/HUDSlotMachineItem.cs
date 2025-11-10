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
        private ItemNames _currentItemName = ItemNames.None;
        private Sprite _currentSprite;

        private Sprite[] _shuffledSprites;
        private ItemNames[] _shuffledItemNames;

        public SlotState CurrentState => _currentState;
        public Sprite CurrentSprite => _currentSprite;
        public ItemNames CurrentItemName => _currentItemName;
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
        public void StartSpin(Sprite[] sprites, ItemNames[] itemNames)
        {
            if (_currentState != SlotState.Idle)
            {
                Log.Warning(LogTags.UI_SlotMachine, "슬롯 스핀을 시작할 수 없습니다. 현재 상태: {0}", _currentState);
                return;
            }

            // 입력 검증
            if (sprites == null || itemNames == null || sprites.Length == 0 || itemNames.Length == 0)
            {
                Log.Warning(LogTags.UI_SlotMachine, "스프라이트 또는 아이템 이름 배열이 null이거나 비어있습니다.");
                return;
            }

            if (sprites.Length != itemNames.Length)
            {
                Log.Warning(LogTags.UI_SlotMachine, "스프라이트와 아이템 이름 배열의 길이가 일치하지 않습니다.");
                return;
            }

            // Deck을 사용한 배열 섞기
            Deck<Sprite> spriteDeck = new Deck<Sprite>();
            Deck<ItemNames> itemNameDeck = new Deck<ItemNames>();

            // 중복 허용 설정 (같은 아이템이 여러 번 나올 수 있음)
            spriteDeck.AllowDuplicateValues = true;
            itemNameDeck.AllowDuplicateValues = true;

            // 전달받은 배열의 요소들을 Deck에 추가
            for (int i = 0; i < sprites.Length; i++)
            {
                spriteDeck.Add(sprites[i]);
                itemNameDeck.Add(itemNames[i]);
            }

            // Deck 섞기
            spriteDeck.Shuffle();
            itemNameDeck.Shuffle();

            // 섞인 배열로 변환
            _shuffledSprites = spriteDeck.ToArray();
            _shuffledItemNames = itemNameDeck.ToArray();

            // 잠금 해제
            if (_lock != null && _lock.IsLocked)
            {
                _lock.Unlock();
            }

            // 스크롤 시작
            if (_scroller != null)
            {
                _scroller.StartScrolling(_shuffledSprites);
            }

            SetState(SlotState.Spinning);
        }

        /// <summary>
        /// 슬롯 스핀 중지
        /// </summary>
        public void StopSpin()
        {
            if (_currentState != SlotState.Spinning)
            {
                Log.Warning(LogTags.UI_SlotMachine, "슬롯 스핀을 중지할 수 없습니다. 현재 상태: {0}", _currentState);
                return;
            }

            // 섞인 배열 검증
            if (_shuffledSprites == null || _shuffledSprites.Length == 0 ||
                _shuffledItemNames == null || _shuffledItemNames.Length == 0)
            {
                Log.Warning(LogTags.UI_SlotMachine, "섞인 배열이 없습니다.");
                return;
            }

            if (_shuffledSprites.Length != _shuffledItemNames.Length)
            {
                Log.Warning(LogTags.UI_SlotMachine, "섞인 배열의 길이가 일치하지 않습니다.");
                return;
            }

            // 랜덤 인덱스 선택
            int randomIndex = Random.Range(0, _shuffledSprites.Length);
            Sprite targetSprite = _shuffledSprites[randomIndex];
            ItemNames itemName = _shuffledItemNames[randomIndex];

            if (targetSprite == null)
            {
                Log.Warning(LogTags.UI_SlotMachine, "선택된 스프라이트가 null입니다.");
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
            SetItemNameText();
            OnSlotStopped?.Invoke(this);

        }

        /// <summary>
        /// 로직 업데이트
        /// </summary>
        public void LogicUpdate()
        {
            // Spinning 또는 Stopping 상태일 때 스크롤링 계속 유지
            // Stopping 상태에서도 아이템 순환이 계속되어 자연스러운 정지 애니메이션 보장
            if ((_currentState == SlotState.Spinning || _currentState == SlotState.Stopping) && _scroller != null)
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
                if (newState == SlotState.Spinning || newState == SlotState.Stopping)
                {
                    // UpdateScrolling은 LogicUpdate에서 호출됨
                    // Stopping 상태에서도 스크롤링을 계속 유지하여 아이템 순환 보장
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

            // 섞인 배열 초기화
            _shuffledSprites = null;
            _shuffledItemNames = null;

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