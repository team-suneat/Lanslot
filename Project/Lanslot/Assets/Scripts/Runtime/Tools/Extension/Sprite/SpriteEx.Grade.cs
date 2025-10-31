namespace TeamSuneat
{
    public static partial class SpriteEx
    {
        public static string GetGradeBoxName(this GradeNames key)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append("ui_inventory_item_text_box_");
            _stringBuilder.Append(key.ToLowerString());
            //_stringBuilder.Append("_0");

            return _stringBuilder.ToString();
        }

        public static string GetItemSlotGradeName(this GradeNames key)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append("ui_item_equipment_grade_");

            _stringBuilder.Append(key.ToLowerString());

            return _stringBuilder.ToString();
        }

        public static string GetRelicItemSlotGradeName(this GradeNames key)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append("ui_relic_info_popup_panel_");

            _stringBuilder.Append(key.ToLowerString());

            return _stringBuilder.ToString();
        }

        public static string GetRelicItemSlotGradeName(this string key)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append("ui_relic_info_popup_panel_");
            _stringBuilder.Append(key.ToLowerString());

            return _stringBuilder.ToString();
        }

        public static string GetGradeDesignFrameName(this GradeNames key)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append("ui_item_design_frame_");
            _stringBuilder.Append(key.ToLowerString());

            return _stringBuilder.ToString();
        }
    }
}