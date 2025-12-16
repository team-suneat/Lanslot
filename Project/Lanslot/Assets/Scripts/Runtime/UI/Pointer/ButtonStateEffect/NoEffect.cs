using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class NoEffect : IButtonStateEffect
    {
        public void Apply(Image target, ToggleButtonStates state, UIPointerEventButton button)
        {
            // 아무 동작도 수행하지 않음
        }
    }
}


