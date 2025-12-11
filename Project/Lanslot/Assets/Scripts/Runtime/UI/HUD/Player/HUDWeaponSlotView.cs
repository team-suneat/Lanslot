using Sirenix.OdinInspector;
using System;
using TeamSuneat.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class HUDWeaponSlotView : UIPointerEvent
    {
        [FoldoutGroup("#HUDWeaponSlotView"), SerializeField] private Image _iconImage;
        [FoldoutGroup("#HUDWeaponSlotView"), SerializeField] private TextMeshProUGUI _levelText;
        [FoldoutGroup("#HUDWeaponSlotView"), SerializeField] private GameObject _lockedOverlay;
        [FoldoutGroup("#HUDWeaponSlotView"), SerializeField] private GameObject _contentRoot;

        public event Action<HUDWeaponSlotView> OnHoverEnter;

        public event Action<HUDWeaponSlotView> OnHoverExit;

        protected override void OnPointerEnter()
        {
            base.OnPointerEnter();
            OnHoverEnter?.Invoke(this);
        }

        protected override void OnPointerExit()
        {
            base.OnPointerExit();
            OnHoverExit?.Invoke(this);
        }

        public void SetSlot(WeaponSlotData data)
        {
            switch (data.State)
            {
                case ItemSlotViewState.Locked:
                    ShowLocked();
                    return;
                case ItemSlotViewState.Empty:
                    ShowEmpty();
                    return;
                case ItemSlotViewState.Equipped:
                    ShowEquipped(data);
                    return;
                default:
                    ShowEmpty();
                    return;
            }
        }

        public void Clear()
        {
            SetActiveSafe(_lockedOverlay, false);
            SetActiveSafe(_contentRoot, false);
            ResetContent();
        }

        private void ShowLocked()
        {
            SetActiveSafe(_lockedOverlay, true);
            SetActiveSafe(_contentRoot, false);
        }

        private void ShowEmpty()
        {
            SetActiveSafe(_lockedOverlay, false);
            SetActiveSafe(_contentRoot, true);
            ResetContent();
        }

        private void ShowEquipped(WeaponSlotData data)
        {
            SetActiveSafe(_lockedOverlay, false);
            SetActiveSafe(_contentRoot, true);
            ApplyContent(data.Icon, data.Level);
        }

        private static void SetActiveSafe(GameObject target, bool isActive)
        {
            if (target != null)
            {
                target.SetActive(isActive);
            }
        }

        private void ApplyContent(Sprite icon, int level)
        {
            if (_iconImage != null)
            {
                _iconImage.enabled = icon != null;
                _iconImage.SetSprite(icon, true);
            }

            if (_levelText != null)
            {
                string format = JsonDataManager.FindStringClone("Format_Level");
                if (level > 0)
                {
                    _levelText.SetText($"{string.Format(format, level)}");
                }
                else
                {
                    _levelText.ResetText();
                }
            }
        }

        private void ResetContent()
        {
            if (_iconImage != null)
            {
                _iconImage.enabled = false;
                _iconImage.ResetSprite();
            }

            if (_levelText != null)
            {
                _levelText.ResetText();
            }
        }
    }
}