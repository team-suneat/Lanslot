using Lean.Pool;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class UIGauge : XBehaviour, IPoolable
    {
        [Title("#UI Gauge", "Component")]
        public Slider FrontSlider;

        public Slider ResourceSlider;
        public TextMeshProUGUI ValueText;

        public UIFollowObject FollowObject;
        public UIBackGauge BackGauge;

        [Title("#UI Gauge", "Toggle")]
        public bool UseFrontValueText;

        public bool UseDespawnOnMissingVital;
        public bool IgnoreDespawn;

        public delegate void OnDespawnedDelegate();

        [Title("#UI Gauge", "Event")]
        public OnDespawnedDelegate OnDespawned;

        //

        [ReadOnly] public float FrontValue;

        /// <summary> 삭제 표시가 되지 않은 게이지는 현재 생명력 값이 0이 되어도 삭제하지 않습니다. </summary>
        [ReadOnly] public bool DespawnMark;

        /// <summary> 연결된 바이탈 </summary>
        [ReadOnly] public Vital LinkedVital;

        /// <summary> 연결된 바이탈을 잃어버릴 경우 한 번에 한하여 디스폰합니다. </summary>
        [ReadOnly] public bool UseDespawnOnceOnMissingVital;

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

            FollowObject = GetComponent<UIFollowObject>();
        }

        protected override void OnStart()
        {
            base.OnStart();

            BackGauge?.ResetBackValue();
        }

        private void LateUpdate()
        {
            if (DespawnMark)
            {
                Despawn();
            }
            else if (LinkedVital != null)
            {
                BackGauge?.Decrease(FrontValue);
            }
            else if (LinkedVital == null)
            {
                if (UseDespawnOnMissingVital)
                {
                    Despawn();
                }
                else if (UseDespawnOnceOnMissingVital)
                {
                    UseDespawnOnceOnMissingVital = false;

                    Despawn();
                }
            }
        }

        // Poolable

        public virtual void OnSpawn()
        {
            LogProgress("게이지의 스폰을 완료합니다. (OnSpawn)");
            ResetDespawnMark();
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
                // Object Pool을 이용해 인게임에서 스폰한 게이지를 디스폰합니다.
                if (!IsDespawned)
                {
                    IsDespawned = true;
                    LogInfo("게이지를 디스폰합니다.");
                    OnDespawned?.Invoke();
                    FollowObject?.Setup(null);

                    UnlinkVital();

                    if (!IsDestroyed)
                    {
                        ResourcesManager.Despawn(gameObject, Time.deltaTime);
                    }
                }
            }
        }

        // 따라다니는 타겟 설정

        public void SetFollowingTarget(Transform transform)
        {
            LogProgress("게이지의 따라가는 목표를 설정합니다.");

            if (FollowObject != null)
            {
                FollowObject.IsWorldSpaceCanvas = true;
                FollowObject.Setup(transform);
            }
        }

        public void SetFollwingWorldOffset(Vector3 offset)
        {
            if (FollowObject != null)
            {
                FollowObject.SetWorldOffset(offset);
            }
        }

        // 바이탈 연결 (Link Vital)

        public void LinkVital(Vital vital, VitalResourceTypes resourceType)
        {
            if (vital != null)
            {
                if (LinkedVital != vital)
                {
                    LogProgress("게이지의 바이탈을 연결합니다.");
                    LinkedVital = vital;
                    BackGauge?.SetLinkedVital(vital, resourceType);
                    SetValueByType(vital, resourceType);
                }
                else
                {
                    LogWarning("게이지의 바이탈이 이미 연결되어있습니다.");
                }
            }
            else
            {
                LogWarning("게이지의 바이탈을 설정할 수 없습니다.");
            }
        }

        private void SetValueByType(Vital vital, VitalResourceTypes resourceType)
        {
            switch (resourceType)
            {
                case VitalResourceTypes.Life:
                    {
                        SetValueText(vital.CurrentLife, vital.MaxLife);
                        SetFrontValue(vital.LifeRate);
                    }
                    break;

                case VitalResourceTypes.Shield:
                    {
                        SetValueText(vital.CurrentShield, vital.MaxShield);
                        SetFrontValue(vital.ShieldRate);
                    }
                    break;
            }
        }

        private void UnlinkVital()
        {
            if (LinkedVital != null)
            {
                LogProgress("게이지의 바이탈과의 연결을 해제합니다.");

                _ = UIManager.Instance.GaugeManager.Unregister(LinkedVital);
                LinkedVital = null;
                BackGauge?.ResetLinkedVital();
            }
            else
            {
                LogError("게이지를 할당 해제할 수 없습니다. 등록된 바이탈이 없습니다.");
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

        // 삭제 표시 (Despawn Mark)

        public void SetDespawnMark()
        {
            if (IgnoreDespawn) { return; }

            if (!DespawnMark)
            {
                DespawnMark = true;
                LogProgress("게이지의 삭제 표시를 활성화합니다.");
            }
            else
            {
                LogWarning("이미 게이지의 삭제 표시가 활성화되어있습니다.");
            }
        }

        private void ResetDespawnMark()
        {
            if (IgnoreDespawn) { return; }

            if (DespawnMark)
            {
                DespawnMark = false;
                LogProgress("게이지의 삭제 표시를 비활성화합니다.");
            }
            else
            {
                LogWarning("이미 게이지의 삭제 표시가 비활성화되어있습니다.");
            }
        }

        public void SetBackValue(float value)
        {
            BackGauge?.SetBackValue(value);
        }

        #region Log

        private string FormatEntityLog(string content)
        {
            if (LinkedVital != null)
            {
                if (LinkedVital.Owner != null)
                {
                    return string.Format("{0}, {1}, {2}", this.GetHierarchyName(), LinkedVital.Owner.GetHierarchyName(), content);
                }
                else
                {
                    return string.Format("{0}, {1}, {2}", this.GetHierarchyName(), LinkedVital.GetHierarchyName(), content);
                }
            }

            return string.Format("{0}, {1}", this.GetHierarchyName(), content);
        }

        protected void LogProgress(string content)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.UI_Gauge, FormatEntityLog(content));
            }
        }

        protected void LogProgress(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Progress(LogTags.UI_Gauge, formattedContent);
            }
        }

        protected void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.UI_Gauge, FormatEntityLog(content));
            }
        }

        protected void LogInfo(string format, params object[] args)
        {
            if (Log.LevelInfo)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Info(LogTags.UI_Gauge, formattedContent);
            }
        }

        protected void LogWarning(string content)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.UI_Gauge, FormatEntityLog(content));
            }
        }

        protected void LogWarning(string format, params object[] args)
        {
            if (Log.LevelWarning)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Warning(LogTags.UI_Gauge, formattedContent);
            }
        }

        protected void LogError(string content)
        {
            if (Log.LevelError)
            {
                Log.Error(FormatEntityLog(content));
            }
        }

        protected void LogError(string format, params object[] args)
        {
            if (Log.LevelError)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Error(formattedContent);
            }
        }

        #endregion Log
    }
}