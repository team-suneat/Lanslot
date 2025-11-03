using Sirenix.OdinInspector;
using TeamSuneat.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 캐릭터 상세 정보(이름, 아이콘, 패시브/무기 등)를 표시하는 UI 패널
    /// </summary>
    public class UICharacterInfoPanel : MonoBehaviour
    {
        [Title("Character")]
        [SerializeField] private Image iconImage;                     // 캐릭터 아이콘/초상화
        [SerializeField] private UILocalizedText nameText;            // 캐릭터 이름

        [Title("Weapon")]
        [SerializeField] private Image weaponIconImage;               // 무기 아이콘
        [SerializeField] private UILocalizedText weaponNameText;      // 무기 이름
        [SerializeField] private UILocalizedText weaponDescText;      // 무기 설명

        [Title("Passive")]
        [SerializeField] private Image passiveIconImage;               // 무기 아이콘
        [SerializeField] private UILocalizedText passiveNameText;     // 패시브 이름
        [SerializeField] private UILocalizedText passiveDescText;     // 패시브 설명

        public void Bind(PlayerCharacterData data)
        {
            string spriteName = SpriteEx.GetSpriteName(data.Name);
            _ = (iconImage?.TrySetSprite(spriteName));
            nameText?.SetText(data.DisplayName);

            spriteName = SpriteEx.GetSpriteName(data.Weapon);

            _ = (weaponIconImage?.TrySetSprite(spriteName, false));
            weaponNameText?.SetText(data.Weapon.GetLocalizedString());
            weaponDescText?.SetText(data.Weapon.GetDescString());

            _ = SpriteEx.GetSpriteName(data.Passive);

            passiveNameText?.SetText(data.Passive.GetNameString());
            passiveDescText?.SetText(data.Passive.GetDescString());
        }
    }
}