using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class SpriteChangeEffect : IButtonStateEffect
    {
        public void Apply(Image target, ToggleButtonStates state, UIPointerEventButton button)
        {
            if (target == null)
            {
                return;
            }

            Sprite sprite = GetSpriteForState(state, button);
            if (sprite != null)
            {
                target.SetSprite(sprite, button.UseNativeSpriteSize);
            }
        }

        private Sprite GetSpriteForState(ToggleButtonStates state, UIPointerEventButton button)
        {
            switch (state)
            {
                case ToggleButtonStates.Lock:
                    return button.LockSprite;

                case ToggleButtonStates.Select:
                    return button.SelectSprite;

                case ToggleButtonStates.MouseOver:
                    return button.MouseOverSprite;

                case ToggleButtonStates.Click:
                    return button.ClickSprite;

                case ToggleButtonStates.Unselect:
                case ToggleButtonStates.Unlock:
                case ToggleButtonStates.Enter:
                case ToggleButtonStates.Exit:
                case ToggleButtonStates.None:
                default:
                    return button.NormalSprite;
            }
        }
    }
}