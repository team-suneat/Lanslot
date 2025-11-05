using UnityEngine;
using UnityEngine.UI;
using TeamSuneat;
using Sirenix.OdinInspector;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 슬롯 머신 아이템의 스크롤링 기능을 담당하는 컴포넌트
    /// </summary>
    public class HUDSlotMachineItemScroller : XBehaviour
    {
        [FoldoutGroup("#Component")][SerializeField] private RectTransform _maskContainer;
        [FoldoutGroup("#Component")][SerializeField] private RectTransform _scrollContent;

        [FoldoutGroup("#Setting")][SerializeField] private float _scrollSpeed = 500f;
        [FoldoutGroup("#Setting")][SerializeField] private float _itemHeight = 50f;

        private Image[] _slotItemImages;
        private RectTransform[] _slotItemTransforms;
        private Sprite[] _availableSprites;
        private bool _isInitialized = false;
        private bool _isScrolling = false;
        private float _scrollOffset = 0f;
        private int _itemCountPerSlot = 0;

        public RectTransform MaskContainer => _maskContainer;
        public RectTransform ScrollContent => _scrollContent;
        public Image[] SlotItemImages => _slotItemImages;
        public RectTransform[] SlotItemTransforms => _slotItemTransforms;
        public float ItemHeight => _itemHeight;
        public bool IsInitialized => _isInitialized;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _maskContainer ??= GetComponent<RectTransform>();
            _scrollContent ??= this.FindComponent<RectTransform>("Scroll Content");
        }

        /// <summary>
        /// 스크롤 시스템 초기화
        /// </summary>
        private void Initialize(int itemCount)
        {
            if (_scrollContent == null)
            {
                Log.Error(LogTags.UI_SlotMachine, "ScrollContent가 null입니다.");
                return;
            }

            if (_maskContainer == null)
            {
                Log.Warning(LogTags.UI_SlotMachine, "MaskContainer가 null입니다.");
            }

            // 기존 아이템이 있다면 제거
            if (_isInitialized && _slotItemTransforms != null)
            {
                for (int i = 0; i < _slotItemTransforms.Length; i++)
                {
                    if (_slotItemTransforms[i] != null)
                    {
                        GameObject.Destroy(_slotItemTransforms[i].gameObject);
                    }
                }
            }

            _itemCountPerSlot = itemCount;
            SetupScrollContentSize();
            CreateSlotItems();

            _isInitialized = true;
        }

        /// <summary>
        /// 스크롤 컨텐츠 크기 설정
        /// </summary>
        private void SetupScrollContentSize()
        {
            float totalHeight = _itemHeight * _itemCountPerSlot;
            _scrollContent.sizeDelta = new Vector2(_scrollContent.sizeDelta.x, totalHeight);
        }

        /// <summary>
        /// 슬롯 아이템들 생성
        /// </summary>
        private void CreateSlotItems()
        {
            _slotItemImages = new Image[_itemCountPerSlot];
            _slotItemTransforms = new RectTransform[_itemCountPerSlot];

            for (int i = 0; i < _itemCountPerSlot; i++)
            {
                CreateSlotItem(i);
            }
        }

        /// <summary>
        /// 개별 슬롯 아이템 생성
        /// </summary>
        private void CreateSlotItem(int index)
        {
            GameObject itemObj = ResourcesManager.SpawnPrefab("UISlotItem", _scrollContent);
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
            rectTransform.sizeDelta = new Vector2(_itemHeight, _itemHeight);
            rectTransform.anchoredPosition = new Vector2(0, -index * _itemHeight);

            Image image = itemObj.GetComponent<Image>();
            if (image == null)
            {
                Log.Error(LogTags.UI_SlotMachine, "Image 컴포넌트를 찾을 수 없습니다. 인덱스: {0}", index);
                return;
            }

            image.preserveAspect = true;

            _slotItemImages[index] = image;
            _slotItemTransforms[index] = rectTransform;
        }

        /// <summary>
        /// 스크롤 시작
        /// </summary>
        public void StartScrolling(Sprite[] sprites)
        {
            if (sprites == null || sprites.Length == 0)
            {
                Log.Warning(LogTags.UI_SlotMachine, "스크롤을 시작할 수 없습니다. 사용 가능한 스프라이트가 없습니다.");
                return;
            }

            // 스프라이트 배열 길이만큼 아이템 개수 설정
            if (!_isInitialized || _itemCountPerSlot != sprites.Length)
            {
                Initialize(sprites.Length);
            }

            _availableSprites = sprites;
            SetupScrollItems(sprites);
            ResetScrollPosition();
            _isScrolling = true;
        }

        /// <summary>
        /// 스크롤 중지
        /// </summary>
        public void StopScrolling()
        {
            _isScrolling = false;
        }

        /// <summary>
        /// 스크롤 아이템들 설정
        /// </summary>
        private void SetupScrollItems(Sprite[] sprites)
        {
            if (sprites == null || sprites.Length == 0 || _slotItemImages == null)
            {
                return;
            }

            for (int i = 0; i < _slotItemImages.Length; i++)
            {
                if (_slotItemImages[i] == null)
                {
                    continue;
                }

                _slotItemImages[i].SetSprite(sprites[i]);
            }
        }

        /// <summary>
        /// 스크롤 위치 초기화
        /// </summary>
        public void ResetScrollPosition()
        {
            if (_scrollContent == null)
            {
                return;
            }

            _scrollOffset = 0f;
            _scrollContent.anchoredPosition = Vector2.zero;
        }

        /// <summary>
        /// 스크롤 업데이트
        /// </summary>
        public void UpdateScrolling()
        {
            if (!_isScrolling || _scrollContent == null || _slotItemTransforms == null)
            {
                return;
            }

            UpdateScrollPosition();
        }

        /// <summary>
        /// 스크롤 위치 업데이트 (모듈러 방식)
        /// </summary>
        private void UpdateScrollPosition()
        {
            float deltaY = _scrollSpeed * Time.deltaTime;
            _scrollOffset += deltaY;

            float totalHeight = _itemHeight * _itemCountPerSlot;

            // 모듈러 연산으로 순환 (배경 스크롤링처럼)
            if (_scrollOffset >= totalHeight)
            {
                _scrollOffset -= totalHeight;
                // 모든 아이템을 한 칸씩 위로 이동하고 스프라이트 재할당
                ShiftItemsUp();
            }

            _scrollContent.anchoredPosition = new Vector2(0, -_scrollOffset);
        }

        /// <summary>
        /// 아이템들을 한 칸씩 위로 이동 (순환)
        /// </summary>
        private void ShiftItemsUp()
        {
            if (_slotItemImages == null || _slotItemTransforms == null || _slotItemImages.Length == 0)
            {
                return;
            }

            // 가장 위 아이템을 임시로 저장
            Image topImage = _slotItemImages[0];
            RectTransform topTransform = _slotItemTransforms[0];

            // 모든 아이템을 한 칸씩 위로 이동
            for (int i = 0; i < _itemCountPerSlot - 1; i++)
            {
                _slotItemImages[i] = _slotItemImages[i + 1];
                _slotItemTransforms[i] = _slotItemTransforms[i + 1];
                _slotItemTransforms[i].anchoredPosition = new Vector2(0, -i * _itemHeight);
            }

            // 가장 위 아이템을 가장 아래로 이동
            _slotItemImages[_itemCountPerSlot - 1] = topImage;
            _slotItemTransforms[_itemCountPerSlot - 1] = topTransform;
            _slotItemTransforms[_itemCountPerSlot - 1].anchoredPosition = new Vector2(0, -(_itemCountPerSlot - 1) * _itemHeight);

            // 새로운 랜덤 스프라이트 할당
            // AssignRandomSprite(_itemCountPerSlot - 1);
        }

        /// <summary>
        /// 아이템에 랜덤 스프라이트 할당
        /// </summary>
        private void AssignRandomSprite(int itemIndex)
        {
            if (_availableSprites == null || _availableSprites.Length == 0 || _slotItemImages[itemIndex] == null)
            {
                return;
            }

            Sprite randomSprite = _availableSprites[Random.Range(0, _availableSprites.Length)];
            _slotItemImages[itemIndex].SetSprite(randomSprite);
        }

        /// <summary>
        /// 리셋
        /// </summary>
        public void Reset()
        {
            _isScrolling = false;
            ResetScrollPosition();

            // 생성된 아이템들 제거
            if (_slotItemTransforms != null)
            {
                for (int i = 0; i < _slotItemTransforms.Length; i++)
                {
                    if (_slotItemTransforms[i] != null)
                    {
                        GameObject.Destroy(_slotItemTransforms[i].gameObject);
                    }
                }
            }

            _slotItemImages = null;
            _slotItemTransforms = null;
            _isInitialized = false;
        }
    }
}