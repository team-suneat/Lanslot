using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class ColorChangeEffect : IButtonStateEffect
    {
        public void Apply(Image target, ToggleButtonStates state, UIPointerEventButton button)
        {
            if (target == null)
            {
                return;
            }

            Color color = GetColorForState(state, button);
            target.SetColor(color);
        }

        private Color GetColorForState(ToggleButtonStates state, UIPointerEventButton button)
        {
            switch (state)
            {
                case ToggleButtonStates.Lock:
                    return GameColors.Disable;

                case ToggleButtonStates.Select:
                    return GameColors.White;

                case ToggleButtonStates.MouseOver:
                    return GameColors.Gray;

                case ToggleButtonStates.Click:
                    return GameColors.White;

                case ToggleButtonStates.Unselect:
                case ToggleButtonStates.Unlock:
                case ToggleButtonStates.Enter:
                case ToggleButtonStates.Exit:
                case ToggleButtonStates.None:
                default:
                    return GameColors.WhiteSmoke;
            }
        }
    }
}