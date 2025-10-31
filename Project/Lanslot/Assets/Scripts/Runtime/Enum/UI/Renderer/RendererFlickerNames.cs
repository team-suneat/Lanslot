namespace TeamSuneat
{
    public enum RendererFlickerNames
    {
        None,

        Damage,
        Bleeding,
        Burning,
        Poisoning,
        Chilled,
        Fire,
        Cold,
        Lightning,
        Invulnerable,
        Dash,

        White,
        Yellow,
    }

    public static class FlickerConverter
    {
        public static RendererFlickerNames ConvertToFlicker(this DamageTypes damageType)
        {
            if (damageType == DamageTypes.BleedOverTime)
            {
                return RendererFlickerNames.Bleeding;
            }
            else if (damageType == DamageTypes.FireOverTime)
            {
                return RendererFlickerNames.Burning;
            }
            else if (damageType == DamageTypes.PoisonOverTime)
            {
                return RendererFlickerNames.Poisoning;
            }
            else
            {
                return RendererFlickerNames.Damage;
            }
        }
    }
}