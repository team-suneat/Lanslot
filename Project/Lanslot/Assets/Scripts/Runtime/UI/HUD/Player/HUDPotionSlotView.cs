using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class HUDPotionSlotView : UIPointerEvent
    {
        [FoldoutGroup("#HUDPotionSlotView"), SerializeField] private Image _iconImage;
        [FoldoutGroup("#HUDPotionSlotView"), SerializeField] private GameObject _lockedOverlay;
        [FoldoutGroup("#HUDPotionSlotView"), SerializeField] private GameObject _contentRoot;

        public event Action<HUDPotionSlotView> OnHoverEnter;

        public event Action<HUDPotionSlotView> OnHoverExit;

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

        public void SetSlot(PotionSlotData data)
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
            ResetIcon();
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
            ResetIcon();
        }

        private void ShowEquipped(PotionSlotData data)
        {
            SetActiveSafe(_lockedOverlay, false);
            SetActiveSafe(_contentRoot, true);
            SetIcon(data.Icon);
        }

        private static void SetActiveSafe(GameObject target, bool isActive)
        {
            if (target != null)
            {
                target.SetActive(isActive);
            }
        }

        private void SetIcon(Sprite icon)
        {
            if (_iconImage != null)
            {
                _iconImage.enabled = icon != null;
                _iconImage.SetSprite(icon, true);
            }
        }

        private void ResetIcon()
        {
            if (_iconImage != null)
            {
                _iconImage.enabled = false;
                _iconImage.ResetSprite();
            }
        }
    }
}