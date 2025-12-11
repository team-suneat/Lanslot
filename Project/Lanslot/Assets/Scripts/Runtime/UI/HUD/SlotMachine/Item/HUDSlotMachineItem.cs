using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class HUDSlotMachineItem : XBehaviour
    {
        [FoldoutGroup("#Component")][SerializeField] private HUDSlotMachineItemScroller _scroller;
        [FoldoutGroup("#Component")][SerializeField] private HUDSlotMachineItemLock _lock;
        [FoldoutGroup("#Component")][SerializeField] private HUDSlotMachineItemAnimator _animator;
        [FoldoutGroup("#Component")][SerializeField] private UILocalizedText _itemNameText;

        private SlotMachineEffectHandler _slotMachineHandler;
        private Sprite[] _shuffledSprites;
        private ItemNames[] _shuffledItemNames;

        public SlotState CurrentState { get; private set; } = SlotState.None;
        public Sprite CurrentSprite { get; private set; }
        public ItemNames CurrentItemName { get; private set; } = ItemNames.None;
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

        public void StartSpin(Sprite[] sprites, ItemNames[] itemNames)
        {
            if (CurrentState != SlotState.Idle)
            {
                Log.Warning(LogTags.UI_SlotMachine, "슬롯 스핀을 시작할 수 없습니다. 현재 상태: {0}", CurrentState);
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
            Deck<Sprite> spriteDeck = new();
            Deck<ItemNames> itemNameDeck = new();

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

        public void StopSpin()
        {
            if (CurrentState != SlotState.Spinning)
            {
                Log.Warning(LogTags.UI_SlotMachine, "슬롯 스핀을 중지할 수 없습니다. 현재 상태: {0}", CurrentState);
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

            CurrentSprite = targetSprite;
            CurrentItemName = itemName;
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

        private void OnAnimationComplete()
        {
            SetState(SlotState.Stopped);

            SetItemNameText();

            ApplyItemEffect();

            OnSlotStopped?.Invoke(this);
        }

        public void LogicUpdate()
        {
            // Spinning 또는 Stopping 상태일 때 스크롤링 계속 유지
            // Stopping 상태에서도 아이템 순환이 계속되어 자연스러운 정지 애니메이션 보장
            if ((CurrentState == SlotState.Spinning || CurrentState == SlotState.Stopping) && _scroller != null)
            {
                _scroller.UpdateScrolling();
            }
        }

        private void SetState(SlotState newState)
        {
            CurrentState = newState;

            // 스크롤 상태에 따라 스크롤러 제어
            if (_scroller != null)
            {
                if (newState is SlotState.Spinning or SlotState.Stopping)
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

        public void LockSlot()
        {
            if (_lock != null)
            {
                _lock.Lock();
            }
        }

        public void UnlockSlot()
        {
            if (_lock != null)
            {
                _lock.Unlock();
            }
        }

        public void ToggleLock()
        {
            if (_lock != null)
            {
                _lock.Toggle();
            }
        }

        public void ResetSlot()
        {
            // 애니메이션 중지
            if (_animator != null)
            {
                _animator.Stop();
            }

            SetState(SlotState.Idle);
            CurrentSprite = null;
            CurrentItemName = ItemNames.None;

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

        private void SetItemNameText()
        {
            if (_itemNameText == null)
            {
                return;
            }

            if (CurrentItemName == ItemNames.None)
            {
                ResetItemNameText();
                return;
            }

            string itemNameString = CurrentItemName.GetLocalizedString();
            _itemNameText.SetText(itemNameString);

            Log.Info(LogTags.UI_SlotMachine, "아이템 이름 텍스트 설정: {0}", itemNameString);
        }

        private void ResetItemNameText()
        {
            if (_itemNameText != null)
            {
                _itemNameText.ResetText();

                Log.Info(LogTags.UI_SlotMachine, "아이템 이름 텍스트 초기화");
            }
        }

        private void ApplyItemEffect()
        {
            if (CurrentItemName == ItemNames.None)
            {
                return;
            }

            if (TryLoadSlotMachineHandler())
            {
                _slotMachineHandler.ApplySlotResult(CurrentItemName);
            }
        }

        private bool TryLoadSlotMachineHandler()
        {
            if (_slotMachineHandler == null)
            {
                if (CharacterManager.Instance != null)
                {
                    _slotMachineHandler = CharacterManager.Instance.Player?.GetComponent<SlotMachineEffectHandler>();
                }
            }

            if (_slotMachineHandler == null)
            {
                Log.Warning(LogTags.UI_SlotMachine, "슬롯머신 효과 핸들러를 찾을 수 없습니다.");
                return false;
            }

            return true;
        }
    }
}