using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 슬롯 머신 아이템의 스크롤링 기능을 담당하는 컴포넌트
    /// </summary>
    public class HUDSlotMachineItemScroller : XBehaviour
    {
        [Header("UI 컴포넌트")]
        [SerializeField] private RectTransform _maskContainer;
        [SerializeField] private RectTransform _scrollContent;

        [Header("스크롤링 설정")]
        [SerializeField] private float _scrollSpeed = 500f;
        [SerializeField] private int _itemCountPerSlot = 5;
        [SerializeField] private float _itemHeight = 100f;

        private Image[] _slotItemImages;
        private RectTransform[] _slotItemTransforms;
        private Sprite[] _availableSprites;
        private bool _isInitialized = false;
        private bool _isScrolling = false;

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

        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// 스크롤 시스템 초기화
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized || _scrollContent == null)
            {
                return;
            }

            // 스크롤 컨텐츠 크기 설정
            float totalHeight = _itemHeight * _itemCountPerSlot;
            _scrollContent.sizeDelta = new Vector2(_scrollContent.sizeDelta.x, totalHeight);

            // 아이템 이미지들 생성
            _slotItemImages = new Image[_itemCountPerSlot];
            _slotItemTransforms = new RectTransform[_itemCountPerSlot];

            for (int i = 0; i < _itemCountPerSlot; i++)
            {
                GameObject itemObj = new($"SlotItem_{i}");
                itemObj.transform.SetParent(_scrollContent, false);

                RectTransform rectTransform = itemObj.AddComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.pivot = new Vector2(0.5f, 1);
                rectTransform.sizeDelta = new Vector2(0, _itemHeight);
                rectTransform.anchoredPosition = new Vector2(0, -i * _itemHeight);

                Image image = itemObj.AddComponent<Image>();
                image.preserveAspect = true;

                _slotItemImages[i] = image;
                _slotItemTransforms[i] = rectTransform;
            }

            _isInitialized = true;
        }

        /// <summary>
        /// 스크롤 시작
        /// </summary>
        public void StartScrolling(Sprite[] sprites)
        {
            if (!_isInitialized)
            {
                Initialize();
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

            // 각 스크롤 아이템에 랜덤 스프라이트 할당
            for (int i = 0; i < _slotItemImages.Length; i++)
            {
                Sprite randomSprite = sprites[Random.Range(0, sprites.Length)];
                _slotItemImages[i].SetSprite(randomSprite);
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

            _scrollContent.anchoredPosition = Vector2.zero;
        }

        /// <summary>
        /// 스크롤 업데이트
        /// </summary>
        public void UpdateScrolling()
        {
            if (!_isScrolling || _scrollContent == null || _maskContainer == null || _slotItemTransforms == null)
            {
                return;
            }

            // 위로 스크롤
            float deltaY = _scrollSpeed * Time.deltaTime;
            Vector2 currentPos = _scrollContent.anchoredPosition;
            currentPos.y -= deltaY;
            _scrollContent.anchoredPosition = currentPos;

            // 마스크 높이 계산
            float maskHeight = _maskContainer.rect.height;

            // 가장 위에 있는 아이템 찾기 (가장 큰 Y 값)
            int topItemIndex = 0;
            float topY = float.MinValue;

            for (int i = 0; i < _slotItemTransforms.Length; i++)
            {
                if (_slotItemTransforms[i] == null)
                {
                    continue;
                }

                float itemTopY = _scrollContent.anchoredPosition.y + _slotItemTransforms[i].anchoredPosition.y;
                if (itemTopY > topY)
                {
                    topY = itemTopY;
                    topItemIndex = i;
                }
            }

            // 가장 위 아이템의 맨 아래가 마스크 위로 완전히 벗어났는지 확인
            float itemBottomY = topY - _itemHeight;
            if (itemBottomY < -maskHeight)
            {
                // 가장 아래 아이템 찾기 (가장 작은 Y 값)
                int bottomItemIndex = 0;
                float bottomY = float.MaxValue;

                for (int i = 0; i < _slotItemTransforms.Length; i++)
                {
                    if (_slotItemTransforms[i] == null)
                    {
                        continue;
                    }

                    float itemY = _slotItemTransforms[i].anchoredPosition.y;
                    if (itemY < bottomY)
                    {
                        bottomY = itemY;
                        bottomItemIndex = i;
                    }
                }

                // 가장 위 아이템을 가장 아래 아이템 아래로 이동
                float newY = bottomY - _itemHeight;
                _slotItemTransforms[topItemIndex].anchoredPosition = new Vector2(
                    _slotItemTransforms[topItemIndex].anchoredPosition.x,
                    newY
                );

                // 새로운 랜덤 스프라이트 할당
                if (_availableSprites != null && _availableSprites.Length > 0 && _slotItemImages[topItemIndex] != null)
                {
                    Sprite randomSprite = _availableSprites[Random.Range(0, _availableSprites.Length)];
                    _slotItemImages[topItemIndex].SetSprite(randomSprite);
                }
            }
        }

        /// <summary>
        /// 리셋
        /// </summary>
        public void Reset()
        {
            _isScrolling = false;
            ResetScrollPosition();

            // 아이템 이미지들 초기화
            if (_slotItemImages != null)
            {
                for (int i = 0; i < _slotItemImages.Length; i++)
                {
                    if (_slotItemImages[i] != null)
                    {
                        _slotItemImages[i].sprite = null;
                    }
                }
            }
        }
    }
}

