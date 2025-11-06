using Sirenix.OdinInspector;
using System.Collections.Generic;
using TeamSuneat.Data.Game;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 슬롯 머신 전체를 관리하는 HUD 컴포넌트
    /// </summary>
    public class HUDSlotMachine : XBehaviour
    {
        [FoldoutGroup("#Components")][SerializeField] private HUDSlotMachineActionButton _actionButton;
        [FoldoutGroup("#Components")][SerializeField] private HUDSlotMachineItem[] _items;
        [FoldoutGroup("#Components")][SerializeField] private UILocalizedText _statusText;

        [FoldoutGroup("#Settings")][SerializeField] private int _slotCount = 6;
        [FoldoutGroup("#Settings")][SerializeField] private float _spinDuration = 2f;
        [FoldoutGroup("#Settings")][SerializeField] private float _stopDelay = 0.5f;
        private Sprite[] _availableSprites;
        private ItemNames[] _availableItemNames;

        [FoldoutGroup("#Event")][SerializeField] private UnityEvent OnAllSlotsStopped;
        [FoldoutGroup("#Event")][SerializeField] private UnityEvent<Sprite[]> OnSlotResult;

        private int _stoppedSlotCount = 0;
        private int _currentStopIndex = 0;
        private Sprite[] _currentResults;

        public SlotMachineState CurrentState { get; private set; } = SlotMachineState.None;
        public bool CanSpin => CurrentState == SlotMachineState.None;

        public System.Action<Sprite[]> OnSlotMachineCompleted;

        private void Awake()
        {
            AutoGetComponents();
        }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _actionButton ??= GetComponentInChildren<HUDSlotMachineActionButton>();
            _items = GetComponentsInChildren<HUDSlotMachineItem>();
            _statusText = this.FindComponent<UILocalizedText>("Status Text");
        }

        protected override void OnStart()
        {
            base.OnStart();

            Initialize();
            SetupEvents();
        }

        private void Initialize()
        {
            LoadAvailables();
            SetupSlots();

            if (_actionButton != null)
            {
                _actionButton.Initialize(CurrentState);
            }

            UpdateUI();
            StartSpin();
        }

        private void SetupSlots()
        {
            for (int i = 0; i < _items.Length; i++)
            {
                _items[i].OnSlotStopped += OnSlotStopped;
            }
        }

        private void SetupEvents()
        {
            if (_actionButton != null)
            {
                _actionButton.OnStopRequested.AddListener(StopNextSlot);
            }
        }

        public void StartSpin()
        {
            if (!CanSpin)
            {
                Log.Warning(LogTags.UI_SlotMachine, "슬롯머신을 시작할 수 없습니다. 현재 상태: {0}", CurrentState);
                return;
            }

            SetState(SlotMachineState.Spinning);
            _stoppedSlotCount = 0;
            _currentStopIndex = 0;
            _currentResults = new Sprite[_items.Length];

            Log.Info(LogTags.UI_SlotMachine, "슬롯머신 스핀 시작. 슬롯 개수: {0}", _items.Length);

            // 모든 슬롯 시작
            for (int i = 0; i < _items.Length; i++)
            {
                _items[i].StartSpin(_availableSprites, _availableItemNames);
            }

            UpdateUI();
        }

        public void StopNextSlot()
        {
            if (CurrentState != SlotMachineState.Spinning)
            {
                Log.Warning(LogTags.UI_SlotMachine, "슬롯을 멈출 수 없습니다. 현재 상태: {0}", CurrentState);
                return;
            }

            // 다음 멈출 슬롯 찾기
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i].CurrentState == SlotState.Spinning)
                {
                    _items[i].StopSpin();
                    _currentStopIndex++;
                    Log.Info(LogTags.UI_SlotMachine, "슬롯 멈춤. 인덱스: {0}/{1}", _currentStopIndex, _items.Length);
                    break;
                }
            }

            UpdateUI();
        }

        private void OnSlotStopped(HUDSlotMachineItem slot)
        {
            // 슬롯 인덱스 찾기
            int slotIndex = -1;
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i] == slot)
                {
                    slotIndex = i;
                    break;
                }
            }

            if (slotIndex < 0)
            {
                Log.Warning(LogTags.UI_SlotMachine, "슬롯 인덱스를 찾을 수 없습니다.");
                return;
            }

            // 결과 저장
            _currentResults[slotIndex] = slot.CurrentSprite;
            _stoppedSlotCount++;

            if (_stoppedSlotCount >= _items.Length)
            {
                Log.Info(LogTags.UI_SlotMachine, "모든 슬롯이 멈췄습니다. 결과 처리 중.");
                OnAllSlotsStopped?.Invoke();
                OnSlotResult?.Invoke(_currentResults);
                OnSlotMachineCompleted?.Invoke(_currentResults);
                SetState(SlotMachineState.Result);
                UpdateUI();
            }
        }

        private void SetState(SlotMachineState newState)
        {
            if (CurrentState != newState)
            {
                Log.Info(LogTags.UI_SlotMachine, "슬롯머신 상태 변경: {0} -> {1}", CurrentState, newState);
            }
            CurrentState = newState;

            // 버튼 컴포넌트에 상태 업데이트
            if (_actionButton != null)
            {
                _actionButton.UpdateState(newState, _currentStopIndex, _items.Length);
            }
        }

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

        private void UpdateUI()
        {
            switch (CurrentState)
            {
                case SlotMachineState.Spinning:
                    if (_statusText != null)
                    {
                        _statusText.SetText($"정지 버튼을 눌러 슬롯을 멈추세요 ({_currentStopIndex}/{_items.Length})");
                    }

                    // 스핀 중에는 버튼 컴포넌트에 현재 인덱스 업데이트
                    if (_actionButton != null)
                    {
                        _actionButton.UpdateState(CurrentState, _currentStopIndex, _items.Length);
                    }
                    break;

                case SlotMachineState.Result:
                    if (_statusText != null)
                    {
                        _statusText.SetText("결과를 확인하세요");
                    }
                    break;
            }
        }

        public void LoadAvailables()
        {
            List<ItemNames> itemNames = CollectAvailableNames();
            (List<Sprite> loadedSprites, List<ItemNames> validItemNames) = LoadSpritesFromItemNames(itemNames);

            _availableSprites = loadedSprites.ToArray();
            _availableItemNames = validItemNames.ToArray();

            if (_availableSprites.Length == 0)
            {
                Log.Warning(LogTags.UI_SlotMachine, "로드된 스프라이트가 없습니다. 슬롯머신을 사용할 수 없습니다.");
            }
            else
            {
                Log.Info(LogTags.UI_SlotMachine, "사용 가능한 스프라이트 로드 완료: {0}개", _availableSprites.Length);
            }
        }

        private List<ItemNames> CollectAvailableNames()
        {
            Deck<ItemNames> itemDeck = new();
            VProfile profile = GameApp.GetSelectedProfile();

            if (profile == null)
            {
                Log.Warning(LogTags.UI_SlotMachine, "프로필을 찾을 수 없습니다. 스프라이트를 로드할 수 없습니다.");
                return new List<ItemNames>();
            }

            int weaponCount = CollectAvailableWeaponNames(profile, itemDeck);
            int potionCount = CollectAvailablePotionNames(profile, itemDeck);
            int itemCount = CollectAvailableItemNames(profile, itemDeck);

            List<ItemNames> result = itemDeck.ToList();
            Log.Info(LogTags.UI_SlotMachine, "수집된 아이템: {0}개 (무기: {1}개, 물약: {2}개, 아이템: {3}개), {4}",
                result.Count, weaponCount, potionCount, itemCount, result.JoinToString());
            return result;
        }

        private int CollectAvailableWeaponNames(VProfile profile, Deck<ItemNames> itemDeck)
        {
            int weaponCount = 0;

            if (profile.Weapon != null)
            {
                List<ItemNames> weaponName = profile.Weapon.GetWeaponNames();
                if (weaponName.IsValid())
                {
                    for (int i = 0; i < weaponName.Count; i++)
                    {
                        itemDeck.Add(weaponName[i]);
                        weaponCount++;
                    }
                }
            }

            return weaponCount;
        }

        private int CollectAvailablePotionNames(VProfile profile, Deck<ItemNames> itemDeck)
        {
            int potionCount = 0;

            if (profile.Potion != null)
            {
                List<ItemNames> potionNames = profile.Potion.GetPotionNames();
                if (potionNames.IsValid())
                {
                    for (int i = 0; i < potionNames.Count; i++)
                    {
                        itemDeck.Add(potionNames[i]);
                        potionCount++;
                    }
                }
            }

            return potionCount;
        }

        private int CollectAvailableItemNames(VProfile profile, Deck<ItemNames> itemDeck)
        {
            int itemCount = 0;

            if (profile.Item != null)
            {
                List<ItemNames> itemNames = profile.Item.GetItemNames();
                if (itemNames.IsValid())
                {
                    for (int i = 0; i < itemNames.Count; i++)
                    {
                        itemDeck.Add(itemNames[i]);
                        itemCount++;
                    }
                }
            }

            return itemCount;
        }

        private (List<Sprite> sprites, List<ItemNames> itemNames) LoadSpritesFromItemNames(List<ItemNames> itemNames)
        {
            List<Sprite> loadedSprites = new();
            List<ItemNames> validItemNames = new();

            if (itemNames == null || itemNames.Count == 0)
            {
                Log.Warning(LogTags.UI_SlotMachine, "로드할 아이템이 없습니다.");
                return (loadedSprites, validItemNames);
            }

            int successCount = 0;
            int failCount = 0;

            for (int i = 0; i < itemNames.Count; i++)
            {
                ItemNames itemName = itemNames[i];
                if (itemName == ItemNames.None)
                {
                    continue;
                }

                Sprite sprite = itemName.LoadSprite();
                if (sprite != null)
                {
                    loadedSprites.Add(sprite);
                    validItemNames.Add(itemName);
                    successCount++;
                }
                else
                {
                    failCount++;
                    Log.Warning(LogTags.UI_SlotMachine, "스프라이트를 로드할 수 없습니다: {0}", itemName);
                }
            }

            Log.Info(LogTags.UI_SlotMachine, "스프라이트 로드 완료: 성공 {0}개, 실패 {1}개", successCount, failCount);
            return (loadedSprites, validItemNames);
        }
    }
}