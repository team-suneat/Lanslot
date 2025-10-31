using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TeamSuneat
{
    public partial class UISelectable : XBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // �׷�: �̺�Ʈ ��ƿ��Ƽ �޼���

        #region Event Utilities

        private void AddListenerToEvent<T>(UnityEvent<T> unityEvent, UnityAction<T> action)
        {
            unityEvent?.AddListener(action);
        }

        private void RemoveListenerFromEvent<T>(UnityEvent<T> unityEvent, UnityAction<T> action)
        {
            unityEvent?.RemoveListener(action);
        }

        private void ClearEventListeners<T>(UnityEvent<T> unityEvent)
        {
            unityEvent?.RemoveAllListeners();
        }

        private void AddListenerToEvent(UnityEvent unityEvent, UnityAction action)
        {
            unityEvent?.AddListener(action);
        }

        private void RemoveListenerFromEvent(UnityEvent unityEvent, UnityAction action)
        {
            unityEvent?.RemoveListener(action);
        }

        private void ClearEventListeners(UnityEvent unityEvent)
        {
            unityEvent?.RemoveAllListeners();
        }

        private void InvokeEvent<T>(UnityEvent<T> unityEvent, T param)
        {
            unityEvent?.Invoke(param);
        }

        private void InvokeEvent(UnityEvent unityEvent)
        {
            unityEvent?.Invoke();
        }

        #endregion Event Utilities
    }
}