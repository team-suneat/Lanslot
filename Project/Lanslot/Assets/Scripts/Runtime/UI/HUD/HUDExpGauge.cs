using TeamSuneat.UserInterface;
using UnityEngine;

namespace TeamSuneat
{
    public class HUDExpGauge : MonoBehaviour
    {
        [SerializeField] private UIGauge _expGauge;

        private void Start()
        {
            RegisterGlobalEvent();
        }

        private void OnDestroy()
        {
            UnregisterGlobalEvent();
        }

        private void RegisterGlobalEvent()
        {
            GlobalEvent<int, int>.Register(GlobalEventType.GAME_DATA_CHARACTER_ADD_EXPERIENCE, UpdateGauge);
        }

        private void UnregisterGlobalEvent()
        {
            GlobalEvent<int, int>.Unregister(GlobalEventType.GAME_DATA_CHARACTER_ADD_EXPERIENCE, UpdateGauge);
        }

        public void UpdateGauge(int current, int max)
        {
            if (_expGauge != null)
            {
                _expGauge.SetValueText(current, max);
                _expGauge.SetFrontValue(current.SafeDivide(max));
            }
        }
    }
}