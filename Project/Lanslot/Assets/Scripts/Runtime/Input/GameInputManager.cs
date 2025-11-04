using Rewired;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public partial class GameInputManager : Singleton<GameInputManager>
    {
        public static Vector2 ThresholdUI = new(0.5f, 0.5f);

        private static readonly Action _rewiredInitCallback = OnRewiredInitialized;

        private ControllerType _fixedControllerType = ControllerType.Custom;
        private ControllerType _currentControllerType = ControllerType.Keyboard;
        private JoystickTypes _currentJoystickType = JoystickTypes.None;

        #region Parameters

        public Player InputPlayer { get; private set; }

        public ControllerType FixedControllerType
        {
            get
            {
                if (GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD)
                {
                    return ControllerType.Custom;
                }

                return _fixedControllerType;
            }
            set
            {
                if (GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD)
                {
                    _fixedControllerType = value;
                }
            }
        }

        public JoystickTypes FixedJoystickType
        {
            get
            {
                if (GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD)
                {
                    return JoystickTypes.None;
                }

                return JoystickTypes.None;
            }
        }

        public ControllerType CurrentControllerType
        {
            get
            {
                if (FixedControllerType != ControllerType.Custom)
                {
                    return FixedControllerType;
                }
                return _currentControllerType;
            }
        }

        public JoystickTypes CurrentJoystickType
        {
            get
            {
                if (FixedJoystickType != JoystickTypes.None)
                {
                    return FixedJoystickType;
                }

                return _currentJoystickType;
            }
        }

        public Controller CurrentJoystick { get; private set; }

        public bool IsInitialized { get; set; }

        #endregion Parameters

        public void Initialize()
        {
            if (!ReInput.isReady)
            {
                ReInput.InitializedEvent -= _rewiredInitCallback;
                ReInput.InitializedEvent += _rewiredInitCallback;
                return;
            }

            if (IsInitialized)
            {
                return;
            }

            IsInitialized = true;

            InputPlayer = ReInput.players.GetPlayer(0);

            LoadDefaultMappings();
            LoadMappings();
            SetupController();
            SetupButtonEvents();

            SubscribeEvents();
        }

        private static void OnRewiredInitialized()
        {
            ReInput.InitializedEvent -= _rewiredInitCallback;
            Instance.Initialize();
        }

        public List<string> GetElementMapsWithAction(ControllerType controllerType, string actionName)
        {
            try
            {
                if (!ValidatePlayer())
                {
                    return null;
                }

                // 1. 액션 이름·방향 보정
                (string targetAction, Pole axis) = ResolveAction(controllerType, actionName);

                // 2. 컨트롤러 ID & 조이스틱 맵 확보
                int controllerId = CurrentJoystick != null ? CurrentJoystick.id : 0;
                Dictionary<string, ActionElementMap> joystickMap =
                    EnsureJoystickMap(controllerId);

                // 3. 매핑 수집
                List<ActionElementMap> elementMaps = new();
                _ = InputPlayer.controllers.maps.GetElementMapsWithAction(
                    controllerType, controllerId, targetAction, true, elementMaps);

                // 4. 반복 처리
                List<string> result = new();
                for (int i = 0; i < elementMaps.Count; i++)
                {
                    ActionElementMap aem = elementMaps[i];
                    ProcessElementMap(aem, controllerType, actionName, axis, joystickMap, result);
                }

                return result;
            }
            catch (Exception e)
            {
                Log.Error("GetElementMapsWithAction 실패: {0}\n{1}", actionName, e);
                return null;
            }
        }

        private bool ValidatePlayer()
        {
            if (InputPlayer != null)
            {
                return true;
            }

            Log.Warning(LogTags.Input, "InputPlayer is null");
            return false;
        }

        private (string, Pole) ResolveAction(ControllerType type, string act)
        {
            if (type != ControllerType.Joystick)
            {
                return (act, Pole.Positive);
            }

            return act switch
            {
                // 턴제 게임에서는 캐릭터 이동이 없으므로 기본값 반환
                _ => (act, Pole.Positive)
            };
        }

        private Dictionary<string, ActionElementMap> EnsureJoystickMap(int id)
        {
            if (!_defaultJoystickElementMapByController.TryGetValue(id, out Dictionary<string, ActionElementMap> map))
            {
                map = new Dictionary<string, ActionElementMap>();
                _defaultJoystickElementMapByController[id] = map;
            }
            return map;
        }

        private void ProcessElementMap(ActionElementMap aem, ControllerType ctrlType, string actionName, Pole targetAxis, Dictionary<string, ActionElementMap> joyMap, List<string> result)
        {
            // 키보드·마우스
            if (ctrlType != ControllerType.Joystick && aem.keyCode != KeyCode.None)
            {
                AddResult(aem.keyCode.ToString(), ctrlType, actionName, result);
                return;
            }

            // 축 방향 체크
            if (aem.axisContribution != targetAxis)
            {
                return;
            }

            // Identifier 이름 확보
            string keyName = aem.elementIdentifierName;
            if (string.IsNullOrEmpty(keyName) && aem.elementIdentifierId != -1)
            {
                if (joyMap.TryGetValue(aem.elementIdentifierId.ToString(), out ActionElementMap m) && m != null)
                {
                    keyName = m.elementIdentifierName;
                }
            }

            if (string.IsNullOrEmpty(keyName) || keyName.Contains("None"))
            {
                return;
            }

            // 스틱 문자열 변환
            keyName = keyName switch
            {
                "Left Stick Y" => targetAxis == Pole.Positive ? "Left Stick Up" : "Left Stick Down",
                "Left Stick X" => targetAxis == Pole.Positive ? "Left Stick Right" : "Left Stick Left",
                "Right Stick Y" => targetAxis == Pole.Positive ? "Right Stick Up" : "Right Stick Down",
                "Right Stick X" => targetAxis == Pole.Positive ? "Right Stick Right" : "Right Stick Left",
                _ => keyName
            };

            AddResult(keyName, ctrlType, actionName, result);
        }

        private void AddResult(string keyName, ControllerType ctrlType, string actionName, List<string> result)
        {
            if (result.Contains(keyName))
            {
                return;
            }

            result.Add(keyName);
            Log.Progress(LogTags.Input, "{0}:{1} → {2}", ctrlType, actionName, keyName.ToSelectString("None"));
        }

        public void GetInputState()
        {
            if (InputPlayer == null)
            {
                Log.Warning(LogTags.Input, "InputPlayer is null");
                return;
            }

            if (_buttonList == null)
            {
                return;
            }

            for (int i = 0; i < _buttonList.Count; i++)
            {
                GameInputButton button = _buttonList[i];
                if (button == null) { continue; }
                if (CurrentControllerType == ControllerType.Mouse)
                {
                    if (!button.IsValidKey(ControllerType.Keyboard))
                    {
                        continue;
                    }
                }
                else if (!button.IsValidKey(CurrentControllerType))
                {
                    continue;
                }

                ProcessButtonInput(button);
            }

            RefreshControllType();
        }

        public GameInputButton GetJoystickInputButton()
        {
            if (InputPlayer == null)
            {
                Log.Warning(LogTags.Input, "InputPlayer is null");
                return null;
            }

            if (_buttonList == null)
            {
                return null;
            }

            if (_buttonList != null)
            {
                for (int i = 0; i < _buttonList.Count; i++)
                {
                    if (_buttonList[i] == null)
                    {
                        continue;
                    }
                    if (_buttonList[i].CheckState(ButtonStates.ButtonDown))
                    {
                        return _buttonList[i];
                    }
                }
            }

            return null;
        }

        private void ProcessButtonInput(GameInputButton button)
        {
            switch (CurrentControllerType)
            {
                case ControllerType.Keyboard:
                case ControllerType.Mouse:
                    ProcessKeyboardOrMouseInput(button);
                    break;

                case ControllerType.Joystick:
                    ProcessJoystickInput(button);
                    break;

                default:
                    Log.Warning(LogTags.Input, "Unhandled controller type: " + CurrentControllerType);
                    break;
            }
        }

        private void ProcessKeyboardOrMouseInput(GameInputButton button)
        {
            List<KeyCode> keyCodes = button.GetKeyCodes(CurrentControllerType);
            bool anyKeyActive = false;
            for (int i = 0; i < keyCodes.Count; i++)
            {
                KeyCode keyCode = keyCodes[i];
                if (Input.GetKeyDown(keyCode))
                {
                    ProcessMovementInput(button);
                    button.TriggerButtonDown();
                    anyKeyActive = true;
                }
                else if (Input.GetKeyUp(keyCode))
                {
                    button.TriggerButtonUp();
                    anyKeyActive = true;
                }
                else if (Input.GetKey(keyCode))
                {
                    ProcessMovementInput(button);
                    button.TriggerButtonPressed();
                    anyKeyActive = true;
                }
            }

            if (!anyKeyActive)
            {
                if (button.CheckState(ButtonStates.ButtonDown) || button.CheckState(ButtonStates.ButtonPressed))
                {
                    Log.Progress(LogTags.Input, "키보드 & 마우스 입력이 없는 경우 버튼 상태를 Off로 전환하여 버튼 상태를 초기화합니다: {0}, {1}", button.ActionName, button.State);
                    button.State.ChangeState(ButtonStates.Off);
                }
            }
        }

        private void ProcessMovementInput(GameInputButton button)
        {
            // 턴제 게임에서는 캐릭터 이동이 없으므로 빈 구현
        }

        private void ProcessJoystickInput(GameInputButton button)
        {
            if (InputPlayer.GetButtonDown(button.ButtonID))
            {
                ProcessMovementInput(button);
                button.TriggerButtonDown();
            }
            else if (InputPlayer.GetButtonUp(button.ButtonID))
            {
                button.TriggerButtonUp();
            }
            else if (InputPlayer.GetButton(button.ButtonID))
            {
                ProcessMovementInput(button);
                button.TriggerButtonPressed();
            }
            else if (button.CheckState(ButtonStates.ButtonDown) || button.CheckState(ButtonStates.ButtonPressed))
            {
                Log.Progress(LogTags.Input, "패드 버튼 입력이 없는 경우 버튼 상태를 Off로 전환하여 버튼 상태를 초기화합니다: {0}, {1}", button.ActionName, button.State);
                button.State.ChangeState(ButtonStates.Off);
            }
        }

        public void ProcessButtonStates()
        {
            if (_buttonList != null)
            {
                for (int i = 0; i < _buttonList.Count; i++)
                {
                    if (_buttonList[i] == null)
                    {
                        continue;
                    }
                    if (_buttonList[i].CheckState(ButtonStates.ButtonDown))
                    {
                        _buttonList[i].State.ChangeState(ButtonStates.ButtonPressed);
                    }
                    if (_buttonList[i].CheckState(ButtonStates.ButtonUp))
                    {
                        _buttonList[i].State.ChangeState(ButtonStates.Off);
                    }
                }
            }
        }

        public void ResetButtonStates()
        {
            if (_buttonList != null)
            {
                for (int i = 0; i < _buttonList.Count; i++)
                {
                    if (_buttonList[i] == null)
                    {
                        continue;
                    }

                    if (_buttonList[i].CheckState(ButtonStates.ButtonDown))
                    {
                        _buttonList[i].State.ChangeState(ButtonStates.Off);
                    }
                    if (_buttonList[i].CheckState(ButtonStates.ButtonPressed))
                    {
                        _buttonList[i].State.ChangeState(ButtonStates.Off);
                    }
                }
            }
        }

        //

        //

        public bool CheckUIMoveLeft()
        {
            if (CheckButtonState(ActionNames.UIMoveLeft, ButtonStates.ButtonDown))
            {
                return true;
            }

            return false;
        }

        public bool CheckUIMoveRight()
        {
            if (CheckButtonState(ActionNames.UIMoveRight, ButtonStates.ButtonDown))
            {
                return true;
            }

            return false;
        }

        public bool CheckUIMoveUp()
        {
            if (CheckButtonState(ActionNames.UIMoveUp, ButtonStates.ButtonDown))
            {
                return true;
            }

            return false;
        }

        public bool CheckUIMoveDown()
        {
            if (CheckButtonState(ActionNames.UIMoveDown, ButtonStates.ButtonDown))
            {
                return true;
            }

            return false;
        }

        //
    }
}