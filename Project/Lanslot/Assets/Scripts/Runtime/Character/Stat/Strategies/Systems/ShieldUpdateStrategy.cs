namespace TeamSuneat
{
    /// <summary>
    /// Shield 관련 능력치 업데이트 전략
    /// Shield를 처리합니다.
    /// </summary>
    public class ShieldUpdateStrategy : BaseStatUpdateStrategy
    {
        /// <summary>
        /// Shield 관련 능력치가 추가될 때 호출됩니다.
        /// </summary>

        /// <param name="statName">능력치 이름</param>
        /// <param name="value">추가될 값</param>
        public override void OnAdd(StatNames statName, float value)
        {
            RefreshShield(true);
        }

        /// <summary>
        /// Shield 관련 능력치가 제거될 때 호출됩니다.
        /// </summary>

        /// <param name="statName">능력치 이름</param>
        /// <param name="value">제거될 값</param>
        public override void OnRemove(StatNames statName, float value)
        {
            RefreshShield(true);
        }

        /// <summary>
        /// Shield 시스템을 새로고침합니다.
        /// </summary>

        /// <param name="shouldAddExcessToCurrent">초과분을 현재값에 추가할지 여부</param>
        private void RefreshShield(bool shouldAddExcessToCurrent)
        {
            if (System.Owner.MyVital.Shield != null)
            {
                LogRefresh("Shield");

                // shouldAddExcessToCurrent가 false일 때는 현재값을 최대값으로 설정
                bool shouldLoadCurrentValueToMax = !shouldAddExcessToCurrent;
                System.Owner.MyVital.Shield.RefreshMaxValue(shouldAddExcessToCurrent, shouldLoadCurrentValueToMax);
                System.Owner.MyVital.RefreshShieldGauge();
            }
        }
    }
}
