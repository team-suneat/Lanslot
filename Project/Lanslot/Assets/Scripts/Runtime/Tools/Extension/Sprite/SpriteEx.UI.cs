namespace TeamSuneat
{
    public static partial class SpriteEx
    {
        public static string GetSpriteName(this WeaponNames key)
        {
            _ = _stringBuilder.Clear();
            _ = _stringBuilder.Append("ui_weapon_");
            _ = _stringBuilder.Append(key.ToLowerString());

            return _stringBuilder.ToString();
        }

        public static string GetSpriteName(this CurrencyNames key)
        {
            _ = _stringBuilder.Clear();
            _ = _stringBuilder.Append("ui_currency_");
            _ = _stringBuilder.Append(key.ToLowerString());

            return _stringBuilder.ToString();
        }

        public static string GetSpriteName(this RewardTypes key)
        {
            switch (key)
            {
                case RewardTypes.MagicStone:
                case RewardTypes.AdvancedMagicStone:
                    _ = _stringBuilder.Clear();
                    _ = _stringBuilder.Append("ui_currency_");
                    _ = _stringBuilder.Append(key.ToLowerString());

                    return _stringBuilder.ToString();
            }

            return string.Empty;
        }

        public static string GetStatMiniIconName(this StatNames key)
        {
            _ = _stringBuilder.Clear();
            _ = _stringBuilder.Append("ui_inventory_item_text_box_mini_icon_");
            _ = _stringBuilder.Append(key.ToLowerString());
            return _stringBuilder.ToString();
        }
    }
}