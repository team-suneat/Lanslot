using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;
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
        [SerializeField] private UILocalizedText rankText;            // 랭크 텍스트
        [SerializeField] private Slider experienceSlider;              // 경험치 슬라이더

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
            VProfile profileInfo = GameApp.GetSelectedProfile();
            bool isLocked = !profileInfo.Character.Contains(data.Name);

            VCharacterInfo characterInfo = profileInfo.Character.GetCharacterInfo(data.Name);

            BindCharacterInfo(data, isLocked, characterInfo);
            BindWeaponInfo(data, isLocked);
            BindPassiveInfo(data, isLocked);
        }

        private void BindCharacterInfo(PlayerCharacterData data, bool isLocked, VCharacterInfo characterInfo)
        {
            BindCharacterBasicInfo(data, isLocked);
            BindCharacterRankInfo(isLocked, characterInfo);
        }

        private void BindCharacterBasicInfo(PlayerCharacterData data, bool isLocked)
        {
            Sprite sprite = SpriteEx.LoadSprite(data.Name);
            iconImage?.SetSprite(sprite);
            nameText?.SetText(data.DisplayName);
        }

        private void BindCharacterRankInfo(bool isLocked, VCharacterInfo characterInfo)
        {
            // 랭크 및 경험치 정보 표시
            if (isLocked || characterInfo == null)
            {
                rankText?.SetText("???");
                if (experienceSlider != null)
                {
                    experienceSlider.value = 0f;
                }
            }
            else
            {
                // 랭크 텍스트 표시
                rankText?.SetText($"Rank {characterInfo.Rank}");

                // 경험치 슬라이더 비율 계산
                BindRankExperience(characterInfo);
            }
        }

        private void BindRankExperience(VCharacterInfo characterInfo)
        {
            if (experienceSlider == null)
            {
                return;
            }

            CharacterRankExpData rankExpData = JsonDataManager.FindCharacterRankExpDataClone(characterInfo.Rank);

            // 다음 랭크 데이터가 없거나 필요 경험치가 0이면 최대 랭크 (100%)
            if (rankExpData == null || rankExpData.RequiredExperience <= 0)
            {
                experienceSlider.value = 1f;
                return;
            }

            // 현재 경험치 / 필요 경험치 비율 계산
            float experienceRatio = characterInfo.RankExperience.SafeDivide(rankExpData.RequiredExperience);
            experienceSlider.value = Mathf.Clamp01(experienceRatio);
        }

        private void BindWeaponInfo(PlayerCharacterData data, bool isLocked)
        {
            if (isLocked)
            {
                // 잠금된 경우 "???" 표시
                weaponNameText?.SetText("???");
                weaponDescText?.SetText("???");
            }
            else
            {
                Sprite sprite = SpriteEx.LoadSprite(data.Weapon);
                weaponIconImage?.SetSprite(sprite, false);
                weaponNameText?.SetText(data.Weapon.GetLocalizedString());
                weaponDescText?.SetText(data.Weapon.GetDescString());
            }
        }

        private void BindPassiveInfo(PlayerCharacterData data, bool isLocked)
        {
            if (isLocked)
            {
                // 잠금된 경우 "???" 표시
                passiveNameText?.SetText("???");
                passiveDescText?.SetText("???");
            }
            else
            {
                _ = SpriteEx.GetSpriteName(data.Passive);
                passiveNameText?.SetText(data.Passive.GetNameString());
                passiveDescText?.SetText(data.Passive.GetDescString());
            }
        }
    }
}