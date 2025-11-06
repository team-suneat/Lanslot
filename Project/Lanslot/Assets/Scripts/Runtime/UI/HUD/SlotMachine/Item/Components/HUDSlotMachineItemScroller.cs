using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 슬롯 머신 아이템의 스크롤링 기능을 담당하는 컴포넌트
    /// </summary>
    public class HUDSlotMachineItemScroller : XBehaviour
    {
        [FoldoutGroup("#Setting")]
        [SerializeField]
        private float _scrollSpeed = 500f;

        [FoldoutGroup("#Setting")]
        [SerializeField]
        [Tooltip("화면에 보이는 아이템 수 대비 생성할 아이템의 배수. 예: 1.5 = 보이는 아이템 수의 1.5배")]
        private float _itemMultiplier = 2f;

        [field: FoldoutGroup("#Component")]
        [field: SerializeField]
        public RectTransform MaskContainer { get; private set; }

        [field: FoldoutGroup("#Component")]
        [field: SerializeField]
        public RectTransform ScrollContent { get; private set; }

        [field: FoldoutGroup("#Setting")]
        [field: SerializeField]
        public float ItemHeight { get; } = 50f;

        // 상태 관리
        private Sprite[] _availableSprites;
        private bool _isScrolling = false;
        private float _scrollOffset = 0f;
        private int _itemCountPerSlot = 0; // 후보 수
        private int _totalItemCount = 0; // 실제 생성된 아이템 수
        private float _centerOffset = 0f; // 중앙 정렬을 위한 오프셋

        // 프로퍼티
        public int ItemCountPerSlot => _itemCountPerSlot;
        public float CenterOffset => _centerOffset;

        // 아이템 관리
        public Image[] SlotItemImages { get; private set; }

        public RectTransform[] SlotItemTransforms { get; private set; }
        private int[] _itemSpriteIndices; // 각 아이템의 스프라이트 인덱스

        public bool IsInitialized { get; private set; } = false;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            MaskContainer ??= GetComponent<RectTransform>();
            ScrollContent ??= this.FindComponent<RectTransform>("Scroll Content");
        }

        // ============================================
        // 초기화
        // ============================================

        private void Initialize(int itemCount)
        {
            if (ScrollContent == null)
            {
                Log.Error(LogTags.UI_SlotMachine, "ScrollContent가 null입니다.");
                return;
            }

            if (MaskContainer == null)
            {
                Log.Warning(LogTags.UI_SlotMachine, "MaskContainer가 null입니다.");
            }

            if (IsInitialized)
            {
                DespawnSlotItems();
            }

            _itemCountPerSlot = itemCount;
            _totalItemCount = CalculateTotalItemCount();
            SetupScrollContentSize();
            CreateSlotItems();

            if (!IsInitialized)
            {
                IsInitialized = true;
            }
        }

        /// <summary>
        /// 생성할 총 아이템 수 계산
        /// </summary>
        private int CalculateTotalItemCount()
        {
            if (MaskContainer == null)
            {
                // MaskContainer가 없으면 후보 수의 기본 배수 사용
                return Mathf.Max(_itemCountPerSlot * 2, _itemCountPerSlot + 3);
            }

            // 화면에 보이는 아이템 수 계산
            float visibleHeight = MaskContainer.rect.height;
            int visibleItemCount = Mathf.CeilToInt(visibleHeight / ItemHeight);

            // 최소 개수 보장 (보이는 개수 + 여유분)
            int minItemCount = visibleItemCount + 3;

            // 후보 수의 배수로 계산
            int multipliedCount = Mathf.CeilToInt(_itemCountPerSlot * _itemMultiplier);

            // 두 값 중 더 큰 값 사용
            return Mathf.Max(minItemCount, multipliedCount);
        }

        private void SetupScrollContentSize()
        {
            float totalHeight = ItemHeight * _totalItemCount;
            ScrollContent.sizeDelta = new Vector2(ScrollContent.sizeDelta.x, totalHeight);
        }

        private void CreateSlotItems()
        {
            SlotItemImages = new Image[_totalItemCount];
            SlotItemTransforms = new RectTransform[_totalItemCount];
            _itemSpriteIndices = new int[_totalItemCount];

            for (int i = 0; i < _totalItemCount; i++)
            {
                CreateSlotItem(i);
            }
        }

        private void CreateSlotItem(int index)
        {
            GameObject itemObj = ResourcesManager.SpawnPrefab("UISlotItem", ScrollContent);
            if (itemObj == null)
            {
                Log.Error(LogTags.UI_SlotMachine, "슬롯 아이템 프리팹을 생성할 수 없습니다. 인덱스: {0}", index);
                return;
            }

            RectTransform rectTransform = itemObj.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                Log.Error(LogTags.UI_SlotMachine, "RectTransform 컴포넌트를 찾을 수 없습니다. 인덱스: {0}", index);
                return;
            }

            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(0.5f, 1);
            rectTransform.sizeDelta = new Vector2(ItemHeight, ItemHeight);
            rectTransform.anchoredPosition = new Vector2(0, -index * ItemHeight);

            Image image = itemObj.GetComponent<Image>();
            if (image == null)
            {
                Log.Error(LogTags.UI_SlotMachine, "Image 컴포넌트를 찾을 수 없습니다. 인덱스: {0}", index);
                return;
            }

            image.preserveAspect = true;

            SlotItemImages[index] = image;
            SlotItemTransforms[index] = rectTransform;
        }

        private void DespawnSlotItems()
        {
            if (SlotItemTransforms != null)
            {
                for (int i = 0; i < SlotItemTransforms.Length; i++)
                {
                    if (SlotItemTransforms[i] != null)
                    {
                        ResourcesManager.Despawn(SlotItemTransforms[i].gameObject);
                    }
                }

                SlotItemTransforms = null;
            }

            SlotItemImages = null;
            _itemSpriteIndices = null;
        }

        public void StartScrolling(Sprite[] sprites)
        {
            if (sprites == null || sprites.Length == 0)
            {
                Log.Warning(LogTags.UI_SlotMachine, "스크롤을 시작할 수 없습니다. 사용 가능한 스프라이트가 없습니다.");
                return;
            }

            // 초기화
            if (!IsInitialized || _itemCountPerSlot != sprites.Length)
            {
                Initialize(sprites.Length);
            }

            _availableSprites = sprites;
            SetupScrollItems(sprites);
            ResetScrollPosition();
            _isScrolling = true;
        }

        public void StopScrolling()
        {
            _isScrolling = false;
        }

        public void UpdateScrolling()
        {
            if (!_isScrolling || ScrollContent == null || SlotItemTransforms == null)
            {
                return;
            }

            UpdateScrollPosition();
        }

        private void SetupScrollItems(Sprite[] sprites)
        {
            if (!sprites.IsValid() || !SlotItemImages.IsValid())
            {
                return;
            }

            // 각 아이템에 스프라이트를 순환 배치하고 인덱스 저장
            for (int i = 0; i < SlotItemImages.Length; i++)
            {
                Image image = SlotItemImages[i];
                if (image == null)
                {
                    continue;
                }

                int spriteIndex = i % sprites.Length;
                _itemSpriteIndices[i] = spriteIndex;
                image.SetSprite(sprites[spriteIndex]);
            }
        }

        private void ResetScrollPosition()
        {
            if (ScrollContent != null && MaskContainer != null)
            {
                // ScrollContent는 고정
                ScrollContent.anchoredPosition = Vector2.zero;
                _scrollOffset = 0f;

                // 중앙 정렬을 위한 오프셋 계산
                float centerItemIndex = _itemCountPerSlot - 0.5f;
                float centerItemPosition = centerItemIndex * ItemHeight;
                float maskCenterY = MaskContainer.rect.height * 0.5f;
                _centerOffset = centerItemPosition - maskCenterY;

                // 각 아이템의 초기 위치를 중앙 정렬 오프셋을 포함하여 설정
                UpdateItemPositions();
            }
        }

        private void UpdateScrollPosition()
        {
            // 스크롤 오프셋 증가
            float deltaY = _scrollSpeed * Time.deltaTime;
            _scrollOffset += deltaY;

            // 하나의 아이템 높이만큼 지나갔는지 확인
            if (_scrollOffset >= ItemHeight)
            {
                _scrollOffset -= ItemHeight;
                ShiftItemsUp();
            }

            // 각 아이템의 위치를 개별적으로 업데이트
            UpdateItemPositions();
        }

        private void UpdateItemPositions()
        {
            if (SlotItemTransforms == null)
            {
                return;
            }

            // 각 아이템의 기본 위치에서 스크롤 오프셋과 중앙 정렬 오프셋을 적용
            for (int i = 0; i < SlotItemTransforms.Length; i++)
            {
                if (SlotItemTransforms[i] == null)
                {
                    continue;
                }

                // 기본 위치: -i * ItemHeight (위에서 아래로)
                // 스크롤 오프셋: 아래로 이동 (양수면 아래로)
                // 중앙 정렬 오프셋: 초기 위치 조정
                float baseY = -i * ItemHeight;
                float finalY = baseY + _scrollOffset + _centerOffset;

                SlotItemTransforms[i].anchoredPosition = new Vector2(0, finalY);
            }
        }

        private void ShiftItemsUp()
        {
            if (SlotItemImages == null || SlotItemTransforms == null || _itemSpriteIndices == null)
            {
                return;
            }

            if (SlotItemImages.Length == 0 || _availableSprites == null || _availableSprites.Length == 0)
            {
                return;
            }

            int totalItemCount = SlotItemImages.Length;

            // 가장 위 아이템 정보 저장
            Image topImage = SlotItemImages[0];
            RectTransform topTransform = SlotItemTransforms[0];

            // 모든 아이템을 한 칸씩 위로 이동
            for (int i = 0; i < totalItemCount - 1; i++)
            {
                SlotItemImages[i] = SlotItemImages[i + 1];
                SlotItemTransforms[i] = SlotItemTransforms[i + 1];
                _itemSpriteIndices[i] = _itemSpriteIndices[i + 1];
            }

            // 가장 위 아이템을 가장 아래로 이동
            SlotItemImages[totalItemCount - 1] = topImage;
            SlotItemTransforms[totalItemCount - 1] = topTransform;

            // 가장 아래로 이동한 아이템의 스프라이트를 업데이트
            // 이동 전 가장 아래 아이템의 다음 스프라이트가 되어야 함
            int bottomSpriteIndexBeforeShift = _itemSpriteIndices[totalItemCount - 1];
            int nextSpriteIndex = (bottomSpriteIndexBeforeShift + 1) % _availableSprites.Length;
            _itemSpriteIndices[totalItemCount - 1] = nextSpriteIndex;
            topImage.SetSprite(_availableSprites[nextSpriteIndex]);

            // 모든 아이템의 위치를 재계산 (UpdateItemPositions에서 처리)
        }

        public void Reset()
        {
            _isScrolling = false;
            ResetScrollPosition();
            DespawnSlotItems();

            SlotItemImages = null;
            SlotItemTransforms = null;
            _itemSpriteIndices = null;
            IsInitialized = false;
        }
    }
}