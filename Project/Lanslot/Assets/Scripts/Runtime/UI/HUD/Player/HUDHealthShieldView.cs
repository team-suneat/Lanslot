using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class HUDHealthShieldView : MonoBehaviour
    {
        [SerializeField] private UIGauge _healthGauge;
        [SerializeField] private UIGauge _shieldGauge;

        public void SetHealth(int current, int max)
        {
            if (_healthGauge == null)
            {
                return;
            }

            float rate = max > 0 ? (float)current / max : 0f;
            _healthGauge.SetValueText(current, max);
            _healthGauge.SetFrontValue(rate);
        }

        public void SetShield(int current, int max)
        {
            if (_shieldGauge == null)
            {
                return;
            }

            float rate = max > 0 ? (float)current / max : 0f;
            _shieldGauge.SetValueText(current, max);
            _shieldGauge.SetFrontValue(rate);
        }

        public void Clear()
        {
            _healthGauge?.ResetValueText();
            _healthGauge?.ResetFrontValue();

            _shieldGauge?.ResetValueText();
            _shieldGauge?.ResetFrontValue();
        }
    }
}

