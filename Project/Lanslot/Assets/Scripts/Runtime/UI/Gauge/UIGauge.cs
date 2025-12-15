using Lean.Pool;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public partial class UIGauge : XBehaviour, IPoolable
    {
        [FoldoutGroup("#UI Gauge-Component")]
        public Slider FrontSlider;

        [FoldoutGroup("#UI Gauge-Component")]
        public Slider ResourceSlider;

        [FoldoutGroup("#UI Gauge-Component")]
        public TextMeshProUGUI ValueText;

        [FoldoutGroup("#UI Gauge-Component")]
        public UIBackGauge BackGauge;

        [FoldoutGroup("#UI Gauge-Toggle")]
        public bool UseFrontValueText;

        public delegate void OnDespawnedDelegate();

        [FoldoutGroup("#UI Gauge-Event")]
        public OnDespawnedDelegate OnDespawned;

        //

        [ReadOnly] public float FrontValue;

        public bool IsSpawned { get; set; }
        public bool IsDespawned { get; set; }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Transform rect = this.FindTransform("Rect");
            if (rect == null)
            {
                rect = transform;
            }

            FrontSlider = rect.FindComponent<Slider>("Slider (Front)");
            ResourceSlider = rect.FindComponent<Slider>("Slider (Resource)");

            if (ValueText == null)
            {
                ValueText = rect.FindComponent<TextMeshProUGUI>("Value Text");
            }

        }

        protected override void OnStart()
        {
            base.OnStart();

            BackGauge?.ResetBackValue();
        }

        private void LateUpdate()
        {
            BackGauge?.Decrease(FrontValue);
        }

        // Poolable

        public virtual void OnSpawn()
        {
            LogProgress("게이지의 스폰을 완료합니다. (OnSpawn)");
            IsSpawned = true;
            IsDespawned = false;
        }

        public virtual void OnDespawn()
        {
            LogProgress("게이지의 디스폰을 완료합니다. (OnDespawn)");
        }

        public void Despawn()
        {
            if (IsSpawned)
            {
                if (!IsDespawned)
                {
                    IsDespawned = true;
                    LogInfo("게이지를 디스폰합니다.");
                    OnDespawned?.Invoke();

                    if (!IsDestroyed)
                    {
                        ResourcesManager.Despawn(gameObject, Time.deltaTime);
                    }
                }
            }
        }

        // 생명력 텍스트 (Life Text)
        public void ResetValueText()
        {
            if (ValueText != null)
            {
                ValueText.SetText(string.Empty);
            }
        }

        public virtual void SetValueText(string content)
        {
            if (ValueText != null)
            {
                if (UseFrontValueText)
                {
                    ValueText.SetText(content);
                }
                else
                {
                    ValueText.SetText(string.Empty);
                }
            }
        }

        public virtual void SetValueText(int current, int max)
        {
            if (ValueText != null)
            {
                if (UseFrontValueText)
                {
                    if (max > 0)
                    {
                        string content = ValueStringEx.GetNoDigitString(current, max);
                        ValueText.SetText(content);
                        LogProgress("게이지의 값 텍스트를 설정합니다. {0}", content);
                        return;
                    }
                }

                ValueText.SetText(string.Empty);
            }
        }

        // 생명력의 앞 레이어 게이지 설정 (Life Front Value)

        public void SetFrontValue(float currentValue)
        {
            FrontValue = Mathf.Clamp01(currentValue);

            if (FrontSlider != null)
            {
                FrontSlider.value = FrontValue;
            }

            SetFrontLineAnimation(FrontValue);
        }

        public void ResetFrontValue()
        {
            SetFrontValue(0f);
        }

        protected virtual void SetFrontLineAnimation(float fillAmount)
        {
        }

        public void SetFrontColor(Color color)
        {
            FrontSlider.targetGraphic.color = color;
        }

        // 자원 값 (Resource Value)

        public void SetResourceValue(float currentValue)
        {
            if (ResourceSlider != null)
            {
                ResourceSlider.value = currentValue;
            }
        }

        public void SetBackValue(float value)
        {
            BackGauge?.SetBackValue(value);
        }
    }
}