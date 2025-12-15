using TeamSuneat;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class HUDHealthShieldView : MonoBehaviour
    {
        [SerializeField] private UIGauge _healthGauge;
        [SerializeField] private UIGauge _shieldGauge;

        private PlayerCharacter _player;
        private Vital _vital;

        private void OnDisable()
        {
            Unbind();
        }

        public void Bind(PlayerCharacter player)
        {
            Unbind();

            if (player == null)
            {
                return;
            }

            _player = player;
            _vital = player.MyVital;

            if (_vital != null)
            {
                if (_vital.Life != null)
                {
                    _vital.Life.OnValueChanged += OnLifeChanged;
                    SetHealth(_vital.Life.Current, _vital.Life.Max);
                }

                if (_vital.Shield != null)
                {
                    _vital.Shield.OnValueChanged += OnShieldChanged;
                    SetShield(_vital.Shield.Current, _vital.Shield.Max);
                }
                else
                {
                    SetShield(0, 0);
                }
            }
        }

        public void Unbind()
        {
            if (_vital != null)
            {
                if (_vital.Life != null)
                {
                    _vital.Life.OnValueChanged -= OnLifeChanged;
                }

                if (_vital.Shield != null)
                {
                    _vital.Shield.OnValueChanged -= OnShieldChanged;
                }
            }

            _vital = null;
            _player = null;
        }

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

            Unbind();
        }

        private void OnLifeChanged(int current, int max)
        {
            SetHealth(current, max);
        }

        private void OnShieldChanged(int current, int max)
        {
            SetShield(current, max);
        }
    }
}

