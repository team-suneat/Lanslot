namespace TeamSuneat
{
    public enum DamageTypes
    {
        None,

        Physical,  // 물리
        Grab,      // 잡기
        Thorns,    // 가시
        Overwhelm, // 압도

        DamageOverTime, // 일반 지속 피해
        HealOverTime, // 생명력 지속 회복

        #region 처형 (Execution)

        Execution, // 처형

        #endregion 처형 (Execution)

        #region 회복

        Heal, // 생명력 회복
        Charge, // 보호막 충전

        #endregion 회복

        Spawn, // 캐릭터 또는 피해 없는 발사체 생성
        Sacrifice, // 희생 (스스로 피해)
    }

    public static class DamageTypeChecker
    {
        public static bool IsInstantDamage(this DamageTypes damageType)
        {
            switch (damageType)
            {
                case DamageTypes.Physical:
                case DamageTypes.Grab:
                case DamageTypes.Thorns:
                case DamageTypes.Overwhelm:
                case DamageTypes.Execution:

                    return true;
            }

            return false;
        }

        public static bool IsDamageOverTime(this DamageTypes damageType)
        {
            switch (damageType)
            {
                case DamageTypes.DamageOverTime:
                    {
                        return true;
                    }
            }

            return false;
        }

        public static bool IsDamage(this DamageTypes key)
        {
            switch (key)
            {
                case DamageTypes.Physical:
                case DamageTypes.Grab:
                case DamageTypes.Thorns:
                case DamageTypes.Overwhelm:
                case DamageTypes.DamageOverTime:
                    {
                        return true;
                    }
            }
            return false;
        }

        public static bool IsHeal(this DamageTypes key)
        {
            switch (key)
            {
                case DamageTypes.Heal:
                case DamageTypes.HealOverTime:
                    {
                        return true;
                    }
            }
            return false;
        }
    }
}