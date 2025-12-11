using TeamSuneat.Data;

namespace TeamSuneat
{
    public partial class DamageCalculator
    {
        // 피해 계수 사용을 중단했으므로 고정 배율(1)만 반환합니다.
        private float CalculatePhysicalDamageRatio(HitmarkAssetData damageAssetData, int hitmarkLevel) => 1f;
    }
}