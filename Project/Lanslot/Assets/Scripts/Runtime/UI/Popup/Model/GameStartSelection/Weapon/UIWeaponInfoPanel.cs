using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TeamSuneat.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 무기 상세 정보(이름, 아이콘, 공격 범위, 패시브, 레벨별 스탯 등)를 표시하는 UI 패널
    /// </summary>
    public class UIWeaponInfoPanel : XBehaviour
    {
        [Title("Weapon Basic Info")]
        [SerializeField] private Image _weaponIconImage;              // 무기 아이콘
        [SerializeField] private UILocalizedText _weaponNameText;     // 무기 이름
        [SerializeField] private UILocalizedText _weaponDescText;     // 무기 설명

        [Title("Weapon Stats")]
        [SerializeField] private UILocalizedText _damageText;         // 피해량
        [SerializeField] private UILocalizedText _attackCountText;    // 공격 횟수
        [SerializeField] private UILocalizedText _multiHitCountText;  // 공격 타수
        [SerializeField] private UILocalizedText _attackRangeText;    // 공격 사거리
        [SerializeField] private UILocalizedText _attackAreaText;     // 공격 범위

        private WeaponData _currentWeaponData;
        private List<WeaponLevelData> _currentWeaponLevelDataList;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _weaponIconImage = this.FindComponent<Image>("Weapon Icon Image");
            _weaponNameText = this.FindComponent<UILocalizedText>("Weapon Name Text");
            _weaponDescText = this.FindComponent<UILocalizedText>("Weapon Desc Text");

            _damageText = this.FindComponent<UILocalizedText>("#Status/Damage Text");
            _attackCountText = this.FindComponent<UILocalizedText>("#Status/AttackCount Text");
            _multiHitCountText = this.FindComponent<UILocalizedText>("#Status/MultiHitCount Text");
            _attackRangeText = this.FindComponent<UILocalizedText>("#Status/AttackRange Text");
            _attackAreaText = this.FindComponent<UILocalizedText>("#Status/AttackArea Text");
        }

        /// <summary>
        /// 무기 데이터를 바인딩하여 표시합니다.
        /// </summary>
        public void Bind(WeaponData weaponData)
        {
            _currentWeaponData = weaponData;

            if (weaponData == null || weaponData.Name == WeaponNames.None)
            {
                Clear();
                return;
            }

            // 레벨별 스탯 정보 조회
            _currentWeaponLevelDataList = JsonDataManager.GetWeaponLevelDataClone(weaponData.Name);

            // 무기 정보 표시
            DisplayWeaponInfo(weaponData);
        }

        private void DisplayWeaponInfo(WeaponData weaponData)
        {
            DisplayWeaponBasicInfo(weaponData);
            DisplayWeaponStats(weaponData);
        }

        private void DisplayWeaponBasicInfo(WeaponData weaponData)
        {
            // 무기 아이콘
            string spriteName = SpriteEx.GetSpriteName(weaponData.Name);
            _weaponIconImage?.TrySetSprite(spriteName, false);

            // 무기 이름
            _weaponNameText?.SetText(weaponData.Name.GetLocalizedString());

            // 무기 설명
            _weaponDescText?.SetText(weaponData.Name.GetDescString());
        }

        private void DisplayWeaponStats(WeaponData weaponData)
        {
            // WeaponLevelData에서 조회하는 스탯
            SetStatTextFromWeaponLevelData(_damageText, StatNames.Damage);
            SetStatTextFromWeaponLevelData(_attackCountText, StatNames.AttackCount);

            // WeaponData에서 직접 조회하는 스탯
            SetStatTextFromWeaponData(_multiHitCountText, StatNames.MultiHitCount, weaponData.MultiHitCount);
            SetStatTextFromWeaponData(_attackRangeText, StatNames.AttackRange, weaponData.AttackRange);
            SetStatTextFromWeaponArea(_attackAreaText, weaponData);
        }

        /// <summary>
        /// WeaponLevelData 리스트에서 해당 StatName을 찾아 텍스트를 설정합니다.
        /// </summary>
        private void SetStatTextFromWeaponLevelData(UILocalizedText textComponent, StatNames statName)
        {
            if (textComponent == null)
            {
                return;
            }

            WeaponLevelData levelData = FindWeaponLevelDataByStatName(statName);
            if (levelData != null && levelData.BaseStatValue > 0)
            {
                string statNameString = statName.GetLocalizedString();
                textComponent.SetText($"{statNameString}: {levelData.BaseStatValue}");
            }
            else
            {
                textComponent.ResetText();
            }
        }

        /// <summary>
        /// WeaponData에서 직접 값을 가져와 텍스트를 설정합니다.
        /// </summary>
        private void SetStatTextFromWeaponData(UILocalizedText textComponent, StatNames statName, int value)
        {
            if (textComponent == null)
            {
                return;
            }

            if (value > 0)
            {
                string statNameString = statName.GetLocalizedString();
                textComponent.SetText($"{statNameString}: {value}");
            }
            else
            {
                textComponent.ResetText();
            }
        }

        /// <summary>
        /// 공격 범위(행x열) 텍스트를 설정합니다.
        /// </summary>
        private void SetStatTextFromWeaponArea(UILocalizedText textComponent, WeaponData weaponData)
        {
            if (textComponent == null)
            {
                return;
            }

            if (weaponData.AttackRow > 0 && weaponData.AttackColumn > 0)
            {
                string statNameString = StatNames.AttackArea.GetLocalizedString();
                textComponent.SetText($"{statNameString}: {weaponData.AttackRow}x{weaponData.AttackColumn}");
            }
            else
            {
                textComponent.ResetText();
            }
        }

        /// <summary>
        /// WeaponLevelData 리스트에서 지정된 StatName에 해당하는 데이터를 찾습니다.
        /// </summary>
        private WeaponLevelData FindWeaponLevelDataByStatName(StatNames statName)
        {
            if (_currentWeaponLevelDataList == null || _currentWeaponLevelDataList.Count == 0)
            {
                return null;
            }

            return _currentWeaponLevelDataList.FirstOrDefault(data => data != null && data.StatName == statName);
        }

        private void Clear()
        {
            _weaponIconImage?.gameObject.SetActive(false);
            _weaponNameText?.ResetText();
            _weaponDescText?.ResetText();

            _damageText?.ResetText();
            _attackCountText?.ResetText();
            _multiHitCountText?.ResetText();
            _attackRangeText?.ResetText();
            _attackAreaText?.ResetText();
        }
    }
}