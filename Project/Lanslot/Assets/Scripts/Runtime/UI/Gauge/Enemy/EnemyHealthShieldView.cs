using TeamSuneat;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class EnemyHealthShieldView : MonoBehaviour, IEnemyGaugeView
    {
        [SerializeField] private UIGauge _healthGauge;
        [SerializeField] private UIGauge _shieldGauge;
        [SerializeField] private UIFollowObject _followObject;

        [SerializeField] private Vector3 _worldOffset;
        [SerializeField] private Vector3 _screenOffset;

        private MonsterCharacter _monster;
        private Vital _vital;
        private bool _despawnMark;

        private void OnDisable()
        {
            Unbind();
        }

        private void Update()
        {
            if (_despawnMark)
            {
                _despawnMark = false;
                Clear();
            }
        }

        public void Bind(MonsterCharacter monster)
        {
            Unbind();

            if (monster == null)
            {
                return;
            }

            Vital vital = monster.MyVital;
            if (vital == null)
            {
                return;
            }

            _monster = monster;
            _vital = vital;

            UIGaugeManager gaugeManager = UIManager.Instance != null ? UIManager.Instance.GaugeManager : null;
            if (gaugeManager != null)
            {
                if (!gaugeManager.RegisterEnemy(_vital, this))
                {
                    _monster = null;
                    _vital = null;
                    return;
                }
            }

            if (_vital.DieEvent != null)
            {
                _vital.DieEvent.AddListener(OnVitalDied);
            }

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

            Transform anchor = _vital.GaugePoint != null ? _vital.GaugePoint : _monster.transform;
            SetupFollow(anchor);
        }

        public void Unbind()
        {
            UIGaugeManager gaugeManager = UIManager.Instance != null ? UIManager.Instance.GaugeManager : null;
            if (gaugeManager != null && _vital != null)
            {
                _ = gaugeManager.UnregisterEnemy(_vital);
            }

            if (_vital != null && _vital.DieEvent != null)
            {
                _vital.DieEvent.RemoveListener(OnVitalDied);
            }

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

            _monster = null;
            _vital = null;

            _followObject?.StopFollowing();
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

            bool hasShield = max > 0;
            _shieldGauge.gameObject.SetActive(hasShield);

            if (!hasShield)
            {
                _shieldGauge.ResetValueText();
                _shieldGauge.ResetFrontValue();
                return;
            }

            float rate = (float)current / max;
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

        public void SetDespawnMark()
        {
            if (_despawnMark)
            {
                return;
            }

            _despawnMark = true;
        }

        private void SetupFollow(Transform anchor)
        {
            if (_followObject == null)
            {
                return;
            }

            _followObject.IsWorldSpaceCanvas = true;
            _followObject.SetWorldOffset(_worldOffset);
            _followObject.ScreenOffset = _screenOffset;
            _followObject.Setup(anchor);
        }

        private void OnLifeChanged(int current, int max)
        {
            SetHealth(current, max);
        }

        private void OnShieldChanged(int current, int max)
        {
            SetShield(current, max);
        }

        private void OnVitalDied()
        {
            Clear();
        }
    }
}