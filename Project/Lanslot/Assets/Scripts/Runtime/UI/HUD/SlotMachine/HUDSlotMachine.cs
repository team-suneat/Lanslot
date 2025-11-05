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
        [FoldoutGroup("#Components")][SerializeField] private HUDSlotMachineActionButton _actionButtonComponent;
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
        public bool CanSpin => CurrentState == SlotMachineState.Idle;

        public System.Action<Sprite[]> OnSlotMachineCompleted;

        private void Awake()
        {
            AutoGetComponents();
        }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _actionButtonComponent ??= GetComponentInChildren<HUDSlotMachineActionButton>();
            _items = GetComponentsInChildren<HUDSlotMachineItem>();
            _statusText = this.FindComponent<UILocalizedText>("Status Text");
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
            LoadAvailableSprites();
            SetState(SlotMachineState.Idle);
            SetupSlots();
            
            if (_actionButtonComponent != null)
            {
                _actionButtonComponent.Initialize(CurrentState);
            }
            
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
            if (_actionButtonComponent != null)
            {
                _actionButtonComponent.OnSpinRequested.AddListener(StartSpin);
                _actionButtonComponent.OnStopRequested.AddListener(StopNextSlot);
            }
        }

        /// <summary>
        /// 스핀 시작
        /// </summary>
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
                Log.Warning(LogTags.UI_SlotMachine, "슬롯을 멈출 수 없습니다. 현재 상태: {0}", CurrentState);
                return;
            }

            // 다음 멈출 슬롯 찾기
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i].CurrentState == SlotState.Spinning)
                {
                    (Sprite sprite, ItemNames itemName) = GetRandomItem();
                    _items[i].StopSpin(sprite, itemName);
                    _currentResults[i] = sprite;
                    _currentStopIndex++;
                    Log.Info(LogTags.UI_SlotMachine, "슬롯 멈춤. 인덱스: {0}/{1}", _currentStopIndex, _items.Length);
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
                Log.Info(LogTags.UI_SlotMachine, "모든 슬롯이 멈췄습니다. 결과 처리 중.");
                OnAllSlotsStopped?.Invoke();
                OnSlotResult?.Invoke(_currentResults);
                OnSlotMachineCompleted?.Invoke(_currentResults);
                SetState(SlotMachineState.Result);
                UpdateUI();
            }
        }


        /// <summary>
        /// 랜덤 아이템 선택 (스프라이트와 ItemNames)
        /// </summary>
        private (Sprite sprite, ItemNames itemName) GetRandomItem()
        {
            if (_availableSprites == null || _availableSprites.Length == 0 || _availableItemNames == null || _availableItemNames.Length == 0)
            {
                Log.Warning(LogTags.UI_SlotMachine, "사용 가능한 아이템이 없습니다.");
                return (null, ItemNames.None);
            }

            if (_availableSprites.Length != _availableItemNames.Length)
            {
                Log.Warning(LogTags.UI_SlotMachine, "스프라이트와 ItemNames 배열의 길이가 일치하지 않습니다.");
                return (null, ItemNames.None);
            }

            int randomIndex = Random.Range(0, _availableSprites.Length);
            return (_availableSprites[randomIndex], _availableItemNames[randomIndex]);
        }

        /// <summary>
        /// 상태 설정
        /// </summary>
        private void SetState(SlotMachineState newState)
        {
            if (CurrentState != newState)
            {
                Log.Info(LogTags.UI_SlotMachine, "슬롯머신 상태 변경: {0} -> {1}", CurrentState, newState);
            }
            CurrentState = newState;
            
            // 버튼 컴포넌트에 상태 업데이트
            if (_actionButtonComponent != null)
            {
                _actionButtonComponent.UpdateState(newState, _currentStopIndex, _items.Length);
            }
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
                    break;

                case SlotMachineState.Spinning:
                    if (_statusText != null)
                    {
                        _statusText.SetText($"스톱 버튼을 눌러 슬롯을 멈추세요 ({_currentStopIndex}/{_items.Length})");
                    }
                    
                    // 스핀 중에는 버튼 컴포넌트에 현재 인덱스 업데이트
                    if (_actionButtonComponent != null)
                    {
                        _actionButtonComponent.UpdateState(CurrentState, _currentStopIndex, _items.Length);
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

        /// <summary>
        /// 슬롯 머신 리셋
        /// </summary>
        public void ResetSlotMachine()
        {
            Log.Info(LogTags.UI_SlotMachine, "슬롯머신 리셋");
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

        /// <summary>
        /// 현재 소지한 아이템(무기, 물약, 아이템)의 이름을 수집합니다.
        /// </summary>
        private List<ItemNames> CollectAvailableItemNames()
        {
            HashSet<ItemNames> itemNameSet = new();
            VProfile profile = GameApp.GetSelectedProfile();

            if (profile == null)
            {
                Log.Warning(LogTags.UI_SlotMachine, "프로필을 찾을 수 없습니다. 스프라이트를 로드할 수 없습니다.");
                return new List<ItemNames>();
            }

            int weaponCount = 0;
            int potionCount = 0;
            int itemCount = 0;

            // 무기 수집 (현재 소지한 무기)
            if (profile.Weapon != null)
            {
                List<ItemNames> weaponNames = profile.Weapon.GetWeaponNames();
                if (weaponNames.IsValid())
                {
                    for (int i = 0; i < weaponNames.Count; i++)
                    {
                        if (itemNameSet.Add(weaponNames[i]))
                        {
                            weaponCount++;
                        }
                    }
                }
            }

            // 물약 수집 (현재 소지한 물약)
            if (profile.Potion != null)
            {
                List<ItemNames> potionNames = profile.Potion.GetPotionNames();
                if (potionNames.IsValid())
                {
                    for (int i = 0; i < potionNames.Count; i++)
                    {
                        if (itemNameSet.Add(potionNames[i]))
                        {
                            potionCount++;
                        }
                    }
                }
            }

            // 일반 아이템 수집 (현재 소지한 아이템)
            if (profile.Item != null)
            {
                List<ItemNames> itemNames = profile.Item.GetItemNames();
                if (itemNames.IsValid())
                {
                    for (int i = 0; i < itemNames.Count; i++)
                    {
                        if (itemNameSet.Add(itemNames[i]))
                        {
                            itemCount++;
                        }
                    }
                }
            }

            List<ItemNames> result = new(itemNameSet);

            Log.Info(LogTags.UI_SlotMachine, "수집된 아이템 개수: {0}개 (무기: {1}개, 물약: {2}개, 아이템: {3}개)",
                result.Count, weaponCount, potionCount, itemCount);

            return result;
        }

        /// <summary>
        /// 아이템 이름 리스트를 스프라이트 배열로 변환합니다.
        /// </summary>
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

        /// <summary>
        /// 스프라이트 이름으로 스프라이트를 로드합니다. (아틀라스 포함)
        /// </summary>
        private Sprite LoadSpriteByName(string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName))
            {
                return null;
            }

            Sprite sprite = ResourcesManager.LoadSprite(spriteName, "atlas_items");
            if (sprite != null)
            {
                return sprite;
            }

            return null;
        }

        /// <summary>
        /// 현재 가진 아이템의 스프라이트를 로드합니다.
        /// </summary>
        public void LoadAvailableSprites()
        {
            List<ItemNames> itemNames = CollectAvailableItemNames();
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
    }
}