using Rewired;
using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public class GameInputButton
    {
        public ButtonStateMachine State { get; private set; }
        public ActionNames ActionName { get; private set; }
        public string ButtonID { get; private set; }
        public List<string> KeyboardKeys { get; private set; }
        public List<string> JoystickKeys { get; private set; }

        public delegate void ButtonEventHandler(ActionNames actionName);

        public event ButtonEventHandler ButtonDown;

        public event ButtonEventHandler ButtonPressed;

        public event ButtonEventHandler ButtonUp;

        private float _lastButtonDownAt;
        private float _lastButtonUpAt;

        public float TimeSinceLastButtonDown => Time.unscaledTime - _lastButtonDownAt;
        public float TimeSinceLastButtonUp => Time.unscaledTime - _lastButtonUpAt;

        public GameInputButton(ActionNames actionName, ButtonEventHandler buttonDownEvent, ButtonEventHandler buttonPressEvent, ButtonEventHandler buttonUpEvent)
        {
            ActionName = actionName;
            ButtonID = actionName.ToString();
            ButtonDown = buttonDownEvent;
            ButtonPressed = buttonPressEvent;
            ButtonUp = buttonUpEvent;

            SetupKeys();
            InitializeState();
        }

        public void SetupKeys()
        {
            Log.Info(LogTags.Input_ButtonState, "[SetupKeys] 시작 - ButtonID: {0}", ButtonID);

            GameInputManager inputManager = GameInputManager.Instance;
            KeyboardKeys = inputManager.GetElementMapsWithAction(ControllerType.Keyboard, ButtonID);
            JoystickKeys = inputManager.GetElementMapsWithAction(ControllerType.Joystick, ButtonID);

            Log.Info(LogTags.Input_ButtonState, "[SetupKeys] 키 설정 완료 - ButtonID: {0}", ButtonID);
            Log.Info(LogTags.Input_ButtonState, "[SetupKeys] 키보드 키: {0}", string.Join(", ", KeyboardKeys ?? new List<string>()));
            Log.Info(LogTags.Input_ButtonState, "[SetupKeys] 조이스틱 키: {0}", string.Join(", ", JoystickKeys ?? new List<string>()));

            LogSetup();
        }

        public void InitializeState()
        {
            State = new ButtonStateMachine();
            State.ChangeState(ButtonStates.Off);
        }

        public bool IsValidKey(ControllerType controllerType)
        {
            List<string> keys = GetKeys(controllerType);

            if (keys == null)
            {
                Log.Warning(LogTags.Input_ButtonState, "컨트롤러 타입 {0}에 대한 키 목록이 null입니다.", controllerType);
                return false;
            }

            if (keys.Count == 0)
            {
                Log.Warning(LogTags.Input_ButtonState, "컨트롤러 타입 {0}에 대한 키 목록이 비어 있습니다.", controllerType);
                return false;
            }

            if (string.IsNullOrEmpty(keys[0]))
            {
                Log.Warning(LogTags.Input_ButtonState, "컨트롤러 타입 {0}에 대한 첫 번째 키가 null이거나 비어 있습니다.", controllerType);
                return false;
            }

            if (keys[0] == "None")
            {
                Log.Warning(LogTags.Input_ButtonState, "컨트롤러 타입 {0}에 대한 첫 번째 키가 'None'입니다.", controllerType);
                return false;
            }

            return true;
        }

        public bool CheckState(ButtonStates state)
        {
            if (State.CurrentState == state)
            {
                return true;
            }

            Log.Warning(LogTags.Input_ButtonState, "버튼 상태가 일치하지 않습니다. 현재 상태: {0}, 기대한 상태: {1}", State.CurrentState, state);
            return false;
        }

        public string GetKey(ControllerType controllerType)
        {
            switch (controllerType)
            {
                case ControllerType.Mouse:
                case ControllerType.Keyboard:
                    if (KeyboardKeys.IsValid())
                    {
                        return KeyboardKeys[0];
                    }
                    break;

                case ControllerType.Joystick:
                    if (JoystickKeys.IsValid())
                    {
                        return JoystickKeys[0];
                    }
                    break;
            }

            return null;
        }

        public List<string> GetKeys(ControllerType controllerType)
        {
            switch (controllerType)
            {
                case ControllerType.Mouse:
                case ControllerType.Keyboard:
                    return KeyboardKeys;

                case ControllerType.Joystick:
                    return JoystickKeys;

                default:
                    return null;
            }
        }

        public List<KeyCode> GetKeyCodes(ControllerType controllerType)
        {
            List<string> keyIDs = GetKeys(controllerType);
            if (keyIDs == null || keyIDs.Count == 0)
            {
                return null;
            }

            List<KeyCode> keyCodes = new();
            for (int i = 0; i < keyIDs.Count; i++)
            {
                string keyID = keyIDs[i];
                KeyCode keyCode = EnumEx.ConvertTo<KeyCode>(keyID);
                if (keyCode != KeyCode.None)
                {
                    keyCodes.Add(keyCode);
                }
            }

            return keyCodes;
        }

        public void TriggerButtonDown()
        {
            _lastButtonDownAt = Time.unscaledTime;
            ButtonDown?.Invoke(ActionName);
            State.ChangeState(ButtonStates.ButtonDown);
            LogButtonAction("Down");
        }

        public void TriggerButtonPressed()
        {
            ButtonPressed?.Invoke(ActionName);
            State.ChangeState(ButtonStates.ButtonPressed);
            LogButtonAction("Pressed");
        }

        public void TriggerButtonUp()
        {
            _lastButtonUpAt = Time.unscaledTime;
            ButtonUp?.Invoke(ActionName);
            State.ChangeState(ButtonStates.ButtonUp);
            LogButtonAction("Up");
        }

        private void LogSetup()
        {
            if (Log.LevelInfo)
            {
                if (KeyboardKeys != null && JoystickKeys != null)
                {
                    Log.Info(LogTags.Input_ButtonState, "Button setup: ID: {0}, Keyboard: {1}, Joystick: {2}",
                             ButtonID, string.Join(", ", KeyboardKeys), string.Join(", ", JoystickKeys));
                }
                else if (KeyboardKeys != null)
                {
                    Log.Info(LogTags.Input_ButtonState, "Button setup: ID: {0}, Keyboard: {1}, Joystick: NULL",
                             ButtonID, string.Join(", ", KeyboardKeys));
                }
                else if (JoystickKeys != null)
                {
                    Log.Info(LogTags.Input_ButtonState, "Button setup: ID: {0}, Keyboard: NULL, Joystick: {1}",
                             ButtonID, string.Join(", ", JoystickKeys));
                }
                else
                {
                    Log.Info(LogTags.Input_ButtonState, "Button setup: ID: {0}, Keyboard: NULL, Joystick: NULL", ButtonID);
                }
            }
        }

        private void LogButtonAction(string action)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.Input_ButtonState, "{0} Button {1}. Keyboard: {2}, Joystick: {3}",
                         ButtonID, action, string.Join(", ", KeyboardKeys), string.Join(", ", JoystickKeys));
            }
        }
    }

    public class ButtonStateMachine
    {
        public ButtonStates CurrentState { get; private set; }

        public void ChangeState(ButtonStates newState)
        {
            CurrentState = newState;
        }
    }
}