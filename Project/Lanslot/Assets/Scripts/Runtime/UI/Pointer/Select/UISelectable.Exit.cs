using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TeamSuneat
{
    public partial class UISelectable : XBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // �׷�: ������ ����Ʈ �̺�Ʈ ���� �޼���

        #region Pointer Exit Event Management

        public void RegisterPointerExitEvent(UnityAction<PointerEventData> action)
        {
            AddListenerToEvent(OnPointerExitEvent, action);
        }

        public void UnregisterPointerExitEvent(UnityAction<PointerEventData> action)
        {
            RemoveListenerFromEvent(OnPointerExitEvent, action);
        }

        public void ClearPointerExitEvent()
        {
            ClearEventListeners(OnPointerExitEvent);
        }

        public void CallPointerExitEvent(PointerEventData eventData)
        {
            InvokeEvent(OnPointerExitEvent, eventData);
        }

        #endregion Pointer Exit Event Management
    }
}