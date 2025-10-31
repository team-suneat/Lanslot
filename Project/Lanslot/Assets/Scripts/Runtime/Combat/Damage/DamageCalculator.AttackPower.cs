namespace TeamSuneat
{
    public partial class DamageCalculator
    {
        private float CalculateDamageRatio(DamageAssetData damageAssetData, int hitmarkLevel, bool isPhysical)
        {
            float damageRatio = damageAssetData.DamageRatio;

            float result = StatEx.GetValueByLevel(damageRatio, damageAssetData.DamageRatioByLevel, hitmarkLevel);
            LogDamageRatioCalculation(damageRatio, damageAssetData.DamageRatioByLevel, hitmarkLevel, result, isPhysical);

            return result;
        }

        private void LogDamageRatioCalculation(float damageRatio, float damageRatioByLevel, int hitmarkLevel, float result, bool isPhysical)
        {
            string format = isPhysical
                    ? "공격자의 물리 공격력 계수를 계산합니다. (공격력 계수({0}) + [공격력 성장 계수({1}) * 히트마크 레벨({2} - 1)]) = {3}"
                    : "공격자의 마법 공격력 계수를 계산합니다. (공격력 계수({0}) + [공격력 성장 계수({1}) * 히트마크 레벨({2} - 1)]) = {3}";

            LogProgress(format,
                ValueStringEx.GetPercentString(damageRatio, 1),
                ValueStringEx.GetPercentString(damageRatioByLevel, 1),
                ValueStringEx.GetValueString(hitmarkLevel, hitmarkLevel > 1),
                ValueStringEx.GetPercentString(result, 1));
        }

        private float CalculatePhysicalDamageRatio(DamageAssetData damageAssetData, int hitmarkLevel)
        {
            return CalculateDamageRatio(damageAssetData, hitmarkLevel, true);
        }

        private float CalculateMagicalDamageRatio(DamageAssetData damageAssetData, int hitmarkLevel)
        {
            return CalculateDamageRatio(damageAssetData, hitmarkLevel, false);
        }
    }
}