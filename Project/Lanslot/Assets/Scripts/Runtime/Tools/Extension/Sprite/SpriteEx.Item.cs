namespace TeamSuneat
{
    public static partial class SpriteEx
    {
        public static string GetSpriteName(this ItemNames key)
        {
            _ = _stringBuilder.Clear();
            _ = _stringBuilder.Append(INVENTORY_ITEM_FORMAT);
            _ = _stringBuilder.Append(key.ToLowerString());

            return _stringBuilder.ToString();
        }

        public static string GetLegendarySpriteName(this ItemNames itemName, int index)
        {
            return string.Format(LEGENDARY_ITEM_IDLE_FORMAT, itemName.ToLowerString()) + "_" + index.ToString();
        }

        public static string GetDetailsStartName(this ItemNames key)
        {
            _ = _stringBuilder.Clear();
            _ = _stringBuilder.Append("ui_item_runewords_");
            _ = _stringBuilder.Append(key.ToLowerString());
            _ = _stringBuilder.Append("_info_start");

            return _stringBuilder.ToString();
        }

        public static string GetDetailsLoopName(this ItemNames key)
        {
            _ = _stringBuilder.Clear();
            _ = _stringBuilder.Append("ui_item_runewords_");
            _ = _stringBuilder.Append(key.ToLowerString());
            _ = _stringBuilder.Append("_info_idle");

            return _stringBuilder.ToString();
        }

        public static string GetSpriteDesignName(this ItemNames key)
        {
            _ = _stringBuilder.Clear();
            _ = _stringBuilder.Append("ui_runeword_design_image_");
            _ = _stringBuilder.Append(key.ToLowerString());

            return _stringBuilder.ToString();
        }

        #region 물약 (Potion)

        public static string GetSpriteName(this PotionNames key)
        {
            _ = _stringBuilder.Clear();
            _ = _stringBuilder.Append(INVENTORY_ITEM_FORMAT);
            _ = _stringBuilder.Append(key.ToLowerString());

            return _stringBuilder.ToString();
        }

        #endregion 물약 (Potion)
    }
}