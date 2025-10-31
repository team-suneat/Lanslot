using Rewired;
using System.Collections.Generic;

namespace TeamSuneat
{
    public partial class GameInputManager
    {
        private Dictionary<ActionNames, GameInputButton> _buttonDictionary;
        private List<GameInputButton> _buttonList;

        private void SetupButtonEvents()
        {
            _buttonDictionary = new();
            _buttonList = new();

            ActionNames[] actionNames = EnumEx.GetValues<ActionNames>(true);
            for (int i = 0; i < actionNames.Length; i++)
            {
                ActionNames actionName = actionNames[i];
                GameInputButton button = new(actionName, OnButtonDown, OnButtonPressed, OnButtonUp);
                _buttonDictionary.Add(actionName, button);
                _buttonList.Add(button);

                Log.Info(LogTags.Input, "{0} 버튼이 생성되고, 초기화되었습니다.", actionName);
            }
        }

        private void OnButtonDown(ActionNames actionName)
        {
            if (_buttonDictionary.IsValid())
            {
                if (_buttonDictionary.TryGetValue(actionName, out GameInputButton button))
                {
                    button.State.ChangeState(ButtonStates.ButtonDown);
                }
            }
        }

        private void OnButtonPressed(ActionNames actionName)
        {
            if (_buttonDictionary.IsValid())
            {
                if (_buttonDictionary.TryGetValue(actionName, out GameInputButton button))
                {
                    button.State.ChangeState(ButtonStates.ButtonPressed);
                }
            }
        }

        private void OnButtonUp(ActionNames actionName)
        {
            if (_buttonDictionary.IsValid())
            {
                if (_buttonDictionary.TryGetValue(actionName, out GameInputButton button))
                {
                    button.State.ChangeState(ButtonStates.ButtonUp);
                }
            }
        }

        public bool CheckButtonState(ActionNames button1ID, ActionNames button2ID, ButtonStates buttonState)
        {
            if (CheckButtonState(button1ID, buttonState) || CheckButtonState(button2ID, buttonState))
            {
                return true;
            }

            return false;
        }

        public bool CheckButtonState(ActionNames actionName, ButtonStates buttonState)
        {
            if (_buttonDictionary.IsValid())
            {
                if (_buttonDictionary.ContainsKey(actionName))
                {
                    if (_buttonDictionary[actionName].CheckState(buttonState))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool CheckButtonTimeSinceLastButtonDown(ActionNames actionName, float bufferDuration)
        {
            if (_buttonDictionary.IsValid())
            {
                if (_buttonDictionary.ContainsKey(actionName))
                {
                    if (_buttonDictionary[actionName].TimeSinceLastButtonDown < bufferDuration)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void TriggerButtonUp(ActionNames actionName)
        {
            if (_buttonDictionary.IsValid())
            {
                if (_buttonDictionary.TryGetValue(actionName, out GameInputButton button))
                {
                    button.TriggerButtonUp();
                }
            }
        }

        public GameInputButton GetButton(ActionNames actionName)
        {
            if (_buttonDictionary.IsValid())
            {
                if (_buttonDictionary.ContainsKey(actionName))
                {
                    return _buttonDictionary[actionName];
                }
            }

            return null;
        }

        public string GetKey(ActionNames actionName)
        {
            return GetKey(CurrentControllerType, actionName);
        }

        public string GetKey(ControllerType controllerType, ActionNames actionName)
        {
            if (_buttonDictionary.IsValid())
            {
                if (_buttonDictionary.ContainsKey(actionName))
                {
                    return _buttonDictionary[actionName].GetKey(controllerType);
                }
            }

            return null;
        }

        /// <summary>
        /// 특정 액션의 키 정보를 업데이트합니다.
        /// </summary>
        /// <param name="actionName">업데이트할 액션 이름</param>
        public void UpdateButtonKeys(ActionNames actionName)
        {
            if (_buttonDictionary.IsValid() && _buttonDictionary.ContainsKey(actionName))
            {
                GameInputButton button = _buttonDictionary[actionName];
                button.SetupKeys();
                button.InitializeState();

                Log.Info(LogTags.Input, "{0} 액션의 키 정보가 업데이트되었습니다.", actionName);
            }
        }

        /// <summary>
        /// 모든 버튼의 키 정보를 업데이트합니다.
        /// </summary>
        public void UpdateAllButtonKeys()
        {
            if (_buttonList.IsValid())
            {
                for (int i = 0; i < _buttonList.Count; i++)
                {
                    GameInputButton button = _buttonList[i];
                    if (button != null)
                    {
                        button.SetupKeys();
                        button.InitializeState();
                    }
                }

                Log.Info(LogTags.Input, "모든 버튼의 키 정보가 업데이트되었습니다.");
            }
        }
    }
}