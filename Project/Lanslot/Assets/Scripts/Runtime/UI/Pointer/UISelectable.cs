using Sirenix.OdinInspector;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TeamSuneat
{
    [System.Serializable]
    public class SelectableOverEvent : UnityEvent
    { }

    [System.Serializable]
    public class SelectableEvent : UnityEvent<PointerEventData>
    { }

    public partial class UISelectable : XBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public int Index;

        [FoldoutGroup("#Event")]
        public SelectableEvent OnPointerEnterEvent;

        [FoldoutGroup("#Event")]
        public SelectableOverEvent OnPointerOverEvent;

        [FoldoutGroup("#Event")]
        public SelectableEvent OnPointerExitEvent;

        internal UnityEvent OnPointerEnterCallback = new();

        private bool _isEntered;

        // 그룹: 초기화 관련 메서드

        #region Initialization

        public override void AutoSetting()
        {
            base.AutoSetting();
            EnableRaycastOnImage();
        }

        private void EnableRaycastOnImage()
        {
            Image image = GetComponent<Image>();
            if (image != null)
            {
                image.raycastTarget = true;
            }
        }

        #endregion Initialization

        // 그룹: 업데이트 관련 메서드

        #region Update Methods

        private void Update()
        {
            if (_isEntered)
            {
                OnPointerOver();
            }
        }

        private void OnPointerOver()
        {
            InvokeEvent(OnPointerOverEvent);
        }

        #endregion Update Methods

        // 그룹: 포인터 이벤트 처리 메서드

        #region Pointer Event Handlers

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (IsMouseController())
            {
                HandlePointerEnter(eventData);
            }
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (IsMouseController())
            {
                HandlePointerExit(eventData);
            }
        }

        private bool IsMouseController()
        {
            return GameInputManager.Instance.CurrentControllerType == Rewired.ControllerType.Mouse;
        }

        private void HandlePointerEnter(PointerEventData eventData)
        {
            InvokeEvent(OnPointerEnterEvent, eventData);
            _isEntered = true;
        }

        private void HandlePointerExit(PointerEventData eventData)
        {
            InvokeEvent(OnPointerExitEvent, eventData);
            _isEntered = false;
        }

        #endregion Pointer Event Handlers
    }
}