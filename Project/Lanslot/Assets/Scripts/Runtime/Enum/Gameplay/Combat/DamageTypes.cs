namespace TeamSuneat
{
    public enum DamageTypes
    {
        None,

        Physical,  // 물리
        Grab,      // 잡기
        Thorns,    // 가시
        Overwhelm, // 압도

        #region 마법 즉시 피해 (Magical Instant Damage)

        Magical, // 마법
        Fire, // 화염
        Cold, // 서리
        Lightning, // 번개
        Poison, // 독
        Darkness, // 암흑
        Holy, // 신성

        #endregion 마법 즉시 피해 (Magical Instant Damage)

        FireOverTime, // 화염 지속 피해
        ColdOverTime, // 서리 지속 피해
        LightningOverTime, // 번개 지속 피해
        PoisonOverTime, // 독 지속 피해
        DarknessOverTime, // 암흑 지속 피해
        HolyOverTime, // 신성 지속 피해
        BleedOverTime, // 피 지속 피해
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
                case DamageTypes.Magical:
                case DamageTypes.Fire:
                case DamageTypes.Cold:
                case DamageTypes.Lightning:
                case DamageTypes.Poison:
                case DamageTypes.Darkness:
                case DamageTypes.Holy:

                    return true;
            }

            return false;
        }

        public static bool IsMagicalDamage(this DamageTypes damageType)
        {
            switch (damageType)
            {
                case DamageTypes.Magical:
                case DamageTypes.Fire:
                case DamageTypes.Cold:
                case DamageTypes.Lightning:
                    return true;
            }

            return false;
        }

        public static bool IsDamageOverTime(this DamageTypes damageType)
        {
            switch (damageType)
            {
                case DamageTypes.BleedOverTime:
                case DamageTypes.FireOverTime:
                case DamageTypes.ColdOverTime:
                case DamageTypes.LightningOverTime:
                case DamageTypes.PoisonOverTime:
                case DamageTypes.DarknessOverTime:
                case DamageTypes.HolyOverTime:
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
                case DamageTypes.Magical:
                case DamageTypes.Fire:
                case DamageTypes.Cold:
                case DamageTypes.Lightning:
                case DamageTypes.Poison:
                case DamageTypes.Darkness:
                case DamageTypes.Holy:
                case DamageTypes.FireOverTime:
                case DamageTypes.ColdOverTime:
                case DamageTypes.LightningOverTime:
                case DamageTypes.PoisonOverTime:
                case DamageTypes.DarknessOverTime:
                case DamageTypes.HolyOverTime:
                    {
                        return true;
                    }
            }
            return false;
        }

        public static bool IsInstantElemental(this DamageTypes key)
        {
            switch (key)
            {
                case DamageTypes.Fire:
                case DamageTypes.Cold:
                case DamageTypes.Lightning:
                case DamageTypes.Poison:
                case DamageTypes.Darkness:
                case DamageTypes.Holy:
                    {
                        return true;
                    }
            }
            return false;
        }

        public static bool IsElemental(this DamageTypes key)
        {
            switch (key)
            {
                case DamageTypes.Fire:
                case DamageTypes.Cold:
                case DamageTypes.Lightning:
                case DamageTypes.Poison:
                case DamageTypes.Darkness:
                case DamageTypes.Holy:
                case DamageTypes.FireOverTime:
                case DamageTypes.ColdOverTime:
                case DamageTypes.LightningOverTime:
                case DamageTypes.PoisonOverTime:
                case DamageTypes.DarknessOverTime:
                case DamageTypes.HolyOverTime:
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