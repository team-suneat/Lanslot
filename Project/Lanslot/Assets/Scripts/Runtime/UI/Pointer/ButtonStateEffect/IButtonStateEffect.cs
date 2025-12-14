using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public interface IButtonStateEffect
    {
        void Apply(Image target, ToggleButtonStates state, UIPointerEventButton button);
    }
}

