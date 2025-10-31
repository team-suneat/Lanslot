namespace TeamSuneat
{
    /// <summary>
    /// Damage 관련 능력치 업데이트 전략
    /// Damage를 처리합니다.
    /// </summary>
    public class DamageUpdateStrategy : BaseStatUpdateStrategy
    {
        /// <summary>
        /// Damage 관련 능력치가 추가될 때 호출됩니다.
        /// </summary>

        /// <param name="statName">능력치 이름</param>
        /// <param name="value">추가될 값</param>
        public override void OnAdd(StatNames statName, float value)
        {
            RefreshDamage(System);
        }

        /// <summary>
        /// Damage 관련 능력치가 제거될 때 호출됩니다.
        /// </summary>

        /// <param name="statName">능력치 이름</param>
        /// <param name="value">제거될 값</param>
        public override void OnRemove(StatNames statName, float value)
        {
            RefreshDamage(System);
        }

        private void RefreshDamage(StatSystem StatSystem)
        {
            float damage = StatSystem.FindValueOrDefault(StatNames.Damage);
            LogStatUpdate(StatNames.Damage, damage);

            // 데미지 관련 시스템 새로고침
            // 실제 구현은 데미지 계산 시스템에 따라 달라질 수 있습니다
        }
    }
}
