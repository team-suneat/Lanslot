using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TeamSuneat
{
    public partial class UISelectable : XBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // �׷�: ������ ���� �̺�Ʈ ���� �޼���

        #region Pointer Enter Event Management

        public void RegisterPointerEnterEvent(UnityAction<PointerEventData> action)
        {
            AddListenerToEvent(OnPointerEnterEvent, action);
        }

        public void UnregisterPointerEnterEvent(UnityAction<PointerEventData> action)
        {
            RemoveListenerFromEvent(OnPointerEnterEvent, action);
        }

        public void ClearPointerEnterEvent()
        {
            ClearEventListeners(OnPointerEnterEvent);
        }

        public void CallPointerEnterEvent(PointerEventData eventData)
        {
            InvokeEvent(OnPointerEnterEvent, eventData);
            InvokeEvent(OnPointerEnterCallback);
        }

        #endregion Pointer Enter Event Management
    }
}