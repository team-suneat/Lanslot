using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TeamSuneat;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 슬롯 머신 아이템의 애니메이션 기능을 담당하는 컴포넌트
    /// </summary>
    public class HUDSlotMachineItemAnimator : XBehaviour
    {
        [Header("애니메이션 설정")]
        [SerializeField] private float _stopDuration = 1f;
        [SerializeField] private Ease _stopEase = Ease.OutQuad;

        private HUDSlotMachineItemScroller _scroller;
        private Tween _stopTween;

        public bool IsAnimating => _stopTween != null && _stopTween.IsActive();

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            // Scroller는 자식 GameObject에 있으므로 부모에서 자식으로 찾기
            HUDSlotMachineItem parentItem = GetComponentInParent<HUDSlotMachineItem>();
            if (parentItem != null)
            {
                _scroller ??= parentItem.GetComponentInChildren<HUDSlotMachineItemScroller>();
            }
        }

        /// <summary>
        /// Scroller 참조 설정 (외부에서 설정 가능)
        /// </summary>
        public void SetScroller(HUDSlotMachineItemScroller scroller)
        {
            _scroller = scroller;
        }

        /// <summary>
        /// 멈춤 애니메이션 시작
        /// </summary>
        public void StopAnimation(Sprite targetSprite, System.Action onComplete)
        {
            if (_scroller == null || targetSprite == null)
            {
                Log.Warning(LogTags.UI_SlotMachine, "멈춤 애니메이션을 시작할 수 없습니다. Scroller: {0}, TargetSprite: {1}", 
                    _scroller != null ? "있음" : "null", targetSprite != null ? "있음" : "null");
                onComplete?.Invoke();
                return;
            }

            // 기존 트윈 종료
            if (_stopTween != null && _stopTween.IsActive())
            {
                _stopTween.Kill();
            }

            // 목표 스프라이트 찾기 또는 설정
            RectTransform targetItemTransform = FindOrCreateTargetSprite(targetSprite);
            if (targetItemTransform == null)
            {
                onComplete?.Invoke();
                return;
            }

            // 가운데 위치 계산 (마스크의 중앙)
            float maskHeight = _scroller.MaskContainer != null ? _scroller.MaskContainer.rect.height : _scroller.ItemHeight;
            float itemHeight = _scroller.ItemHeight;

            // 아이템의 중심이 마스크 중심에 오도록 계산
            // 아이템의 anchoredPosition은 위쪽 피벗 기준이므로, 중심은 Y - itemHeight/2
            // 스크롤 컨텐츠의 Y 위치를 조정하여 아이템 중심이 마스크 중심(0)에 오도록 함
            float itemCenterY = targetItemTransform.anchoredPosition.y - (itemHeight * 0.5f);
            float targetScrollY = -itemCenterY;

            // DOTween으로 부드럽게 이동
            Vector2 targetPosition = new(_scroller.ScrollContent.anchoredPosition.x, targetScrollY);

            _stopTween = _scroller.ScrollContent.DOAnchorPos(targetPosition, _stopDuration)
                .SetEase(_stopEase)
                .OnComplete(() =>
                {
                    _stopTween = null;
                    onComplete?.Invoke();
                });
        }

        /// <summary>
        /// 목표 스프라이트 찾기 또는 생성
        /// </summary>
        private RectTransform FindOrCreateTargetSprite(Sprite targetSprite)
        {
            if (!ValidateScrollerData(targetSprite))
            {
                return null;
            }

            Image[] slotItemImages = _scroller.SlotItemImages;
            RectTransform[] slotItemTransforms = _scroller.SlotItemTransforms;

            // 현재 스크롤 아이템들 중에서 목표 스프라이트와 일치하는 것 찾기
            RectTransform foundTransform = FindTargetSprite(targetSprite, slotItemImages, slotItemTransforms);
            if (foundTransform != null)
            {
                return foundTransform;
            }

            // 없으면 가장 가운데에 가까운 아이템을 목표 스프라이트로 설정
            return CreateTargetSpriteAtClosestItem(targetSprite, slotItemImages, slotItemTransforms);
        }

        /// <summary>
        /// Scroller 데이터 유효성 검증
        /// </summary>
        private bool ValidateScrollerData(Sprite targetSprite)
        {
            if (_scroller == null || targetSprite == null)
            {
                return false;
            }

            Image[] slotItemImages = _scroller.SlotItemImages;
            RectTransform[] slotItemTransforms = _scroller.SlotItemTransforms;

            if (slotItemImages == null || slotItemTransforms == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 목표 스프라이트와 일치하는 아이템 찾기
        /// </summary>
        private RectTransform FindTargetSprite(Sprite targetSprite, Image[] slotItemImages, RectTransform[] slotItemTransforms)
        {
            if (slotItemImages == null || slotItemTransforms == null)
            {
                return null;
            }

            for (int i = 0; i < slotItemImages.Length; i++)
            {
                if (slotItemImages[i] != null && slotItemImages[i].sprite == targetSprite)
                {
                    return slotItemTransforms[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 마스크 중심에 가장 가까운 아이템을 목표 스프라이트로 설정
        /// </summary>
        private RectTransform CreateTargetSpriteAtClosestItem(Sprite targetSprite, Image[] slotItemImages, RectTransform[] slotItemTransforms)
        {
            if (slotItemImages == null || slotItemTransforms == null || 
                slotItemImages.Length == 0 || slotItemTransforms.Length == 0)
            {
                return null;
            }

            int closestIndex = FindClosestItemIndexToCenter(slotItemTransforms);
            if (closestIndex < 0 || closestIndex >= slotItemImages.Length)
            {
                return null;
            }

            // 가장 가까운 아이템을 목표 스프라이트로 설정
            slotItemImages[closestIndex].SetSprite(targetSprite);
            return slotItemTransforms[closestIndex];
        }

        /// <summary>
        /// 마스크 중심에 가장 가까운 아이템의 인덱스 찾기
        /// </summary>
        private int FindClosestItemIndexToCenter(RectTransform[] slotItemTransforms)
        {
            if (slotItemTransforms == null || slotItemTransforms.Length == 0)
            {
                return -1;
            }

            float currentScrollY = _scroller.ScrollContent.anchoredPosition.y;
            int closestIndex = 0;
            float closestDistance = float.MaxValue;
            float itemHeight = _scroller.ItemHeight;

            for (int i = 0; i < slotItemTransforms.Length; i++)
            {
                if (slotItemTransforms[i] == null)
                {
                    continue;
                }

                float itemCenterY = currentScrollY + slotItemTransforms[i].anchoredPosition.y - (itemHeight * 0.5f);
                float distance = Mathf.Abs(itemCenterY);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }

        /// <summary>
        /// 애니메이션 중지
        /// </summary>
        public void Stop()
        {
            if (_stopTween != null && _stopTween.IsActive())
            {
                _stopTween.Kill();
                _stopTween = null;
            }
        }

        private void OnDestroy()
        {
            // 트윈 정리
            if (_stopTween != null && _stopTween.IsActive())
            {
                _stopTween.Kill();
            }
        }
    }
}