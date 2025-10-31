using UnityEngine;

namespace TeamSuneat
{
    public partial class Life : VitalResource
    {
        private const float HIT_STOP_DURATION = 0.05f;
        private const float HIT_STOP_FACTOR = 0.01f;

        private Coroutine _hitStopCoroutine;

        public void ApplyHitStop()
        {
            if (!GameDefine.USE_PLAYER_DAMAGE_HIT_STOP)
            {
                return;
            }

            if (Vital.Owner == null)
            {
                return;
            }

            if (!Vital.Owner.IsPlayer)
            {
                return;
            }

            _hitStopCoroutine ??= StartXCoroutine(GameTimeManager.Instance.ActivateSlowMotion(HIT_STOP_DURATION, HIT_STOP_FACTOR, OnCompletedSlowMotion));
        }

        private void OnCompletedSlowMotion()
        {
            _hitStopCoroutine = null;
        }
    }
}