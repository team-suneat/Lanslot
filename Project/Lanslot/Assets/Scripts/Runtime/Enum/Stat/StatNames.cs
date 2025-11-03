namespace TeamSuneat
{
    public enum StatNames
    {
        None, // 없음

        Health,                  // 최대 체력
        HealthMulti,             // 최대 체력 배율(%)
        HealthRegen,             // 체력 재생량 (1분당 재생되는 체력)
        Shield,                  // 최대 쉴드(보호막)
        ShieldMulti,             // 최대 쉴드(보호막) 배율(%)

        AttackCount,             // 공격 횟수 (일정 시간 내 공격 가능한 횟수/스택)
        MultiHitCount,           // 공격 1회당 타격 수(멀티 히트)

        AttackRange,             // 공격 사거리
        AttackArea,              // 공격 영역

        Damage,                  // 피해량
        DamageMulti,             // 피해량 배율(%)
        DamageMultiToElite,      // 엘리트 몬스터에게 주는 피해량 증가 배율(%)
        DamageMultiToBoss,       // 보스 몬스터에게 주는 피해량 증가 배율(%)
        DamageRateByShield,      // 쉴드량의 n% 만큼 추가 피해 비율(%)

        FireDamage,              // 화염 속성 피해
        BleedDamage,             // 출혈(지속 피해, 도트 데미지)
        ThornsDamage,            // 적이 공격 시 반사되는 "가시" 피해
        DamageOverTime,          // 지속 피해

        CriticalChance,          // 치명타 확률(%)
        CriticalDamageMulti,     // 치명타 시 추가 피해 배율(%)
        CriticalDamageMultiToBoss, // 보스 몬스터에게 치명타 시 추가 피해 배율(%)
        ProjectileRicochet,      // 발사체 또는 투사체 튕김 횟수

        Luck,                    // 행운(드랍률, 확률 등 긍정적 효과에 영향)

        ExecutionChance,         // 처형(즉사) 확률
        StunChance,              // 기절 확률
        EvasionChance,           // 회피율(적 공격을 회피할 확률)
        KnockbackChance,         // 적을 밀어내는 넉백 발생 확률

        LifeSteal,               // 체력 흡수(%)

        DamageReduction,         // 받는 피해량 감소(%)

        DeathDefianceCount,      // 부활 가능 횟수(죽음 저항 스택)
        DeathDefianceHealRate,   // 부활 시 회복되는 체력 비율(%)

        InstantHeal,             // 즉시 체력 회복량
        InstantShield,           // 즉시 쉴드(보호막) 회복량
        InstantGold,             // 즉시 획득하는 골드(금화) 양
        InstantGem,              // 즉시 획득하는 보석량

        XPGain,                  // 경험치 획득량, 경험치 획득률 증가(%)
        GoldGain,                // 골드(금화, 돈) 획득량, 획득률 증가(%)
        GemGain,                 // 다이아몬드 획득량, 획득률 증가(%)

        Difficulty,              // 난이도 (적 능력치 증가 / 등장 수 증가)
        EliteSpawnChance,        // 엘리트 몬스터 추가 등장 확률(%)
    }
}