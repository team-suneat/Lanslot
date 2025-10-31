using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class HUDSpeedButton : MonoBehaviour
    {
        [SerializeField] private Button _speedButton;
        [SerializeField] private UILocalizedText _speedButtonText;

        [Range(0f, 5f)]
        [SerializeField] private float _timeFactor;

        [Button]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void AutoGetComponents()
        {
            _speedButton = GetComponent<Button>();
            _speedButtonText = GetComponentInChildren<UILocalizedText>();
        }

        private void Start()
        {
            _speedButton.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            _speedButton.onClick.RemoveAllListeners();
        }

        private void OnClick()
        {
            GameTimeManager.Instance.SetFactor(_timeFactor);
            _speedButtonText.SetText($"{_timeFactor}x SPEED");
        }
    }
}