using System.Text;

namespace TeamSuneat
{
    public static partial class SpriteEx
    {
        // 기본 상수들
        private const string CHARACTER_ICON_FORMAT = "ui_character_icon_";
        private const string PASSIVE_ICON_FORMAT = "ui_passive_icon_";
        

        private const string LEGENDARY_ITEM_IDLE_FORMAT = "ui_item_runewords_{0}_idle";        
        private const string UNLOCK_ITEM_IDLE_FORMAT = "ui_item_unlock_{0}_idle";
        private const string RELIC_SYNERGY_FORMAT = "ui_synergy_icon_";
        private const string INVENTORY_ITEM_FORMAT = "ui_item_inven_image_";

        // 공통 StringBuilder 인스턴스
        private static readonly StringBuilder _stringBuilder = new();
    }
}