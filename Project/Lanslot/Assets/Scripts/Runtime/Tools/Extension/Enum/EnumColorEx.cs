using UnityEngine;

namespace TeamSuneat
{
    public static class EnumColorEx
    {
        public static Color GetGradeColor(this GradeNames gradeName, float alpha)
        {
            Color result = GetGradeColor(gradeName);
            return new Color(result.r, result.g, result.b, alpha);
        }

        public static Color GetGradeColor(this GradeNames gradeName)
        {
            switch (gradeName)
            {
                case GradeNames.Common: return GameColors.Normal;
                case GradeNames.Uncommon: return GameColors.Uncommon;
                case GradeNames.Rare: return GameColors.Rare;
                case GradeNames.Legendary: return GameColors.Legendary;
            }

            return Color.white;
        }

        public static string GetGradeColorHex(GradeNames gradeName)
        {
            switch (gradeName)
            {
                case GradeNames.Common: return GameColors.NormalHex;
                case GradeNames.Uncommon: return GameColors.MagicHex;
                case GradeNames.Rare: return GameColors.RareHex;
                case GradeNames.Legendary: return GameColors.LegendaryHex;
            }

            return "#FFFFFF";
        }

        //

        public static Color GetColor(DamageTypes damageType, float alpha = 1f)
        {
            switch (damageType)
            {
                case DamageTypes.Physical:
                    return GameColors.Physical;

                case DamageTypes.Thorns:
                    return GameColors.Thorns;

                case DamageTypes.DamageOverTime:
                    return GameColors.Bleed;

                default:
                    return GameColors.CreamIvory;
            }
        }

        public static Color GetColor(GameElements element, float alpha = 1f)
        {
            switch (element)
            {
                case GameElements.Fire:
                    return GameColors.Fire;

                case GameElements.Cold:
                    return GameColors.Cold;

                case GameElements.Lightning:
                    return GameColors.Lightning;

                case GameElements.Poison:
                    return GameColors.Poison;

                case GameElements.Holy:
                    return GameColors.Holy;

                case GameElements.Darkness:
                    return GameColors.Darkness;

                case GameElements.Blood:
                    return GameColors.Bleed;

                default:
                    return GameColors.CreamIvory;
            }
        }

        public static Color GetColor(this StateEffects stateEffect)
        {
            switch (stateEffect)
            {
                case StateEffects.Burning:
                    return GameColors.Fire;

                case StateEffects.Chilled:
                case StateEffects.Freeze:
                    return GameColors.Cold;

                case StateEffects.Jolted:
                case StateEffects.ElectricShock:
                    return GameColors.Lightning;

                case StateEffects.Poisoning:
                    return GameColors.Poison;

                case StateEffects.Bleeding:
                    return GameColors.Bleed;

                case StateEffects.Vulnerable:
                case StateEffects.EnhancedVulnerable:
                    return GameColors.Vulnerable;

                case StateEffects.Dazed:
                    return GameColors.Dazed;

                case StateEffects.Paralysis:
                    return GameColors.Paralysis;

                default:
                    return GameColors.CreamIvory;
            }
        }
    }
}