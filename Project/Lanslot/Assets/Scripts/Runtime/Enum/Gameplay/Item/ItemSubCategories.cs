using System.Collections.Generic;

namespace TeamSuneat
{
    public enum ItemSubCategories
    {
        None,

        //────────────────────────────────────────────────────────────────────────

        /// <summary> 한손 검 (마검사, 광전사) </summary>
        OneHandedSword,

        /// <summary> 양손 검 (광전사) </summary>
        TwoHandedSword,

        /// <summary> 단검 (마검사) </summary>
        Knife,

        /// <summary> 투척 단검 (사냥꾼) </summary>
        ThrowingKnife,

        /// <summary> 한손 도끼 (마검사, 광전사) </summary>
        OneHandedAxe,

        /// <summary> 양손 도끼 (광전사) </summary>
        TwoHandedAxe,

        /// <summary> 둔기 (마검사) </summary>
        Blunt,

        /// <summary> 투창 (사냥꾼) </summary>
        Javelin,

        /// <summary> 탈리스만 (마검사) </summary>
        Talisman,

        /// <summary> 우상 (사냥꾼) </summary>
        Focus,

        //────────────────────────────────────────────────────────────────────────

        /// <summary> 투구 </summary>
        Helmet,

        /// <summary> 갑옷 </summary>
        Armor,

        /// <summary> 허리띠 </summary>
        Belt,

        /// <summary> 장갑 </summary>
        Gloves,

        /// <summary> 장화 </summary>
        Boots,

        //────────────────────────────────────────────────────────────────────────

        /// <summary> 반지 </summary>
        Ring,

        /// <summary> 목걸이 </summary>
        Amulet,

        //────────────────────────────────────────────────────────────────────────

        /// <summary> 충전형 </summary>
        Refill,

        /// <summary> 정수 </summary>
        Essence,

        /// <summary> 균열 보석 </summary>
        RiftGem,
    }

    public static class ItemSubCategoryHandler
    {
        /// <summary> 카테고리에 따른 기본 능력치를 반환합니다. </summary>
        public static StatNames[] GetBaseStats(this ItemSubCategories subCategory, CharacterNames ownerName)
        {
            List<StatNames> statNames = new();
            switch (subCategory)
            {
                case ItemSubCategories.OneHandedSword:
                case ItemSubCategories.TwoHandedSword:
                case ItemSubCategories.OneHandedAxe:
                case ItemSubCategories.TwoHandedAxe:
                case ItemSubCategories.Knife:
                case ItemSubCategories.Blunt:
                case ItemSubCategories.Javelin:
                case ItemSubCategories.ThrowingKnife:
                case ItemSubCategories.Talisman:
                case ItemSubCategories.Focus:
                    {
                        statNames.Add(StatNames.Damage);
                    }
                    break;

                case ItemSubCategories.Ring:
                case ItemSubCategories.Amulet:
                    {
                        statNames.Add(StatNames.Damage);
                    }
                    break;
            }

            return statNames.ToArray();
        }
    }
}
