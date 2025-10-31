using TeamSuneat.Data;
using TeamSuneat.Feedbacks;
using TeamSuneat.UserInterface;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary> 캐릭터의 보호막을 관리하는 클래스입니다. </summary>
    public partial class Shield : VitalResource
    {
        [FoldoutGroup("#Feedback")] public GameFeedbacks ActivateFeedbacks;
        [FoldoutGroup("#Feedback")] public GameFeedbacks DamageFeedbacks;
        [FoldoutGroup("#Feedback")] public GameFeedbacks DestroyFeedbacks;

        public override VitalResourceTypes Type => VitalResourceTypes.Shield;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Transform feedbackParent = this.FindTransform("#Feedbacks-Shield");
            if (feedbackParent != null)
            {
                DamageFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("Damage");
                DestroyFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("Destroy");
                ActivateFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("Activate");
            }
        }

        public override void LoadCurrentValue()
        {
            LoadCurrentValueFromSavedData();
        }

        /// <summary> 저장된 데이터에서 보호막 현재값을 불러옵니다. (초기화 시에만 사용) </summary>
        public void LoadCurrentValueFromSavedData()
        {
            if (Vital.Owner != null && Vital.Owner.IsPlayer)
            {
                Data.Game.VProfile profileInfo  = GameApp.GetSelectedProfile();
                if (profileInfo != null)
                {
                    int current = profileInfo.Stat.CurrentShield;
                    if (current != 0)
                    {
                        Current = Mathf.Min(current, Max);
                        Vital.RefreshShieldGauge();
                        LogShieldLoaded(Current, Max);
                        return;
                    }
                }
            }

            Current = Max;
            Vital.RefreshShieldGauge();
            LogShieldInitialized(Current, Max);
        }

        /// <summary> 능력치 변경 시 보호막 현재값을 최대값으로 설정합니다. </summary>
        public void LoadCurrentValueToMax()
        {
            Current = Max;
            Vital.RefreshShieldGauge();
            LogShieldInitialized(Current, Max);
        }

        public override bool AddCurrentValue(int value)
        {
            if (base.AddCurrentValue(value))
            {
                Vital.RefreshShieldGauge();

                if (Vital.Owner != null && Vital.Owner.IsPlayer)
                {
                    GlobalEvent<int, int>.Send(GlobalEventType.PLAYER_CHARACTER_SHIELD_CHARGE, Current, Max);
                }
                return true;
            }

            return false;
        }

        public override bool UseCurrentValue(int value, DamageResult damageResult)
        {
            if (base.UseCurrentValue(value, damageResult))
            {
                if (Current > 0)
                {
                    OnDamageShield(damageResult);
                }
                else
                {
                    OnDestroyShield();
                }

                Vital.RefreshShieldGauge();
                return true;
            }

            LogShieldUsageFailure(value);
            return false;
        }

        public void SpawnShieldFloatyText(int damageValue)
        {
            string content = damageValue.ToString();

            UIFloatyText floatyText = null;
            Collider2D vitalCollider = Vital.GetNotGuardCollider();
            if (vitalCollider != null)
            {
                floatyText = SpawnFloatyText(content, vitalCollider.transform, UIFloatyMoveNames.Shield);
            }
            else if (Vital.Owner != null)
            {
                floatyText = SpawnFloatyText(content, DamageTextPoint, UIFloatyMoveNames.Shield);
            }
        }
    }
}