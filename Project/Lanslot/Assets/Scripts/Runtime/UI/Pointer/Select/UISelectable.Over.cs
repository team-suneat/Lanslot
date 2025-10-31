using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TeamSuneat
{
    public partial class UISelectable : XBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // 그룹: 포인터 오버 이벤트 관리 메서드

        #region Pointer Over Event Management

        public void RegisterPointerOverEvent(UnityAction action)
        {
            AddListenerToEvent(OnPointerOverEvent, action);
        }

        public void UnregisterPointerOverEvent(UnityAction action)
        {
            RemoveListenerFromEvent(OnPointerOverEvent, action);
        }

        public void ClearPointerOverEvent()
        {
            ClearEventListeners(OnPointerOverEvent);
        }

        public void CallPointerOverEvent()
        {
            InvokeEvent(OnPointerOverEvent);
        }

        #endregion Pointer Over Event Management
    }
}