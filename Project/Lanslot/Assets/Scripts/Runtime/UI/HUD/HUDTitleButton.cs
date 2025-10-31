using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class HUDTitleButton : MonoBehaviour
    {
        [SerializeField] private Button _titleButton;

        [Button]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void AutoGetComponents()
        {
            _titleButton = GetComponent<Button>();
        }

        private void Start()
        {
            _titleButton.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            _titleButton.onClick.RemoveAllListeners();
        }

        private void OnClick()
        {
            GlobalEvent.Send(GlobalEventType.MOVE_TO_TITLE);
        }
    }
}