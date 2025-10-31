using TeamSuneat.Setting;
using TeamSuneat.UserInterface;

namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        public virtual void SetHUD()
        {
            RefreshLifeGauge();
            RefreshShieldGauge();
        }

        /// <summary> 생명력 게이지가 할당되어있다면 갱신합니다. </summary>
        public virtual void RefreshLifeGauge()
        {
            if (Life != null)
            {
                if (Gauge != null)
                {
                    Gauge.SetValueText(Life.Current, Life.Max);
                    Gauge.SetFrontValue(Life.Rate);
                }
            }
        }

        public virtual void RefreshShieldGauge()
        {
        }

        //

        public void SpawnGauge()
        {
            if (!GameSetting.Instance.Play.UseMonsterGauge) { return; }
            if (!UseGauge) { return; }
            if (!IsAlive) { return; }
            if (Gauge != null) { return; }
            if (Gauge == null)
            {
                UIGauge gauge = UIManager.Instance.GaugeManager.SpawnLifeGauge(Owner);
                if (gauge != null)
                {
                    gauge.SetFollowingTarget(GaugePoint);
                    gauge.LinkVital(this, VitalResourceTypes.Life);
                    gauge.UseDespawnOnceOnMissingVital = true;

                    UIManager.Instance.GaugeManager.Register(this, gauge);

                    Log.Info(LogTags.UI_Gauge, "게이지를 생성하여 바이탈의 게이지로 할당합니다. {0}, {1}",
                        gauge.GetHierarchyName(), this.GetHierarchyPath());
                }
            }
        }

        //

        public virtual void ComsumeLifePotion(int healValue, int healValueOverTime, float duration)
        { }
    }
}