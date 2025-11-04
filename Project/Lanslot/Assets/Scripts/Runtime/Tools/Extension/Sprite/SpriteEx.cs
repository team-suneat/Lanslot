using System.Text;

namespace TeamSuneat
{
    public static partial class SpriteEx
    {
        // 기본 상수들
        private const string CHARACTER_ICON_FORMAT = "ui_character_icon_";
        private const string PASSIVE_ICON_FORMAT = "ui_passive_icon_";
        private const string ITEM_ICON_FORMAT = "ui_item_icon_";
        private const string CURRENCY_ICON_FORMAT = "ui_currency_icon_";

        // 공통 StringBuilder 인스턴스
        private static readonly StringBuilder _stringBuilder = new();

        public static string GetSpriteName(this CharacterNames key)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append(CHARACTER_ICON_FORMAT);
            _stringBuilder.Append(key.ToLowerString());

            return _stringBuilder.ToString();
        }

        public static string GetSpriteName(this PassiveNames key)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append(PASSIVE_ICON_FORMAT);
            _stringBuilder.Append(key.ToLowerString());

            return _stringBuilder.ToString();
        }

        public static string GetSpriteName(this ItemNames key)
        {
            _ = _stringBuilder.Clear();
            _ = _stringBuilder.Append(ITEM_ICON_FORMAT);
            _ = _stringBuilder.Append(key.ToLowerString());

            return _stringBuilder.ToString();
        }

        public static string GetSpriteName(this CurrencyNames key)
        {
            _ = _stringBuilder.Clear();
            _ = _stringBuilder.Append(CURRENCY_ICON_FORMAT);
            _ = _stringBuilder.Append(key.ToLowerString());

            return _stringBuilder.ToString();
        }
    }
}