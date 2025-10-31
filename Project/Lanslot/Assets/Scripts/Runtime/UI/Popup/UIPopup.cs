using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 자동배틀에서 팝업 UI 요소를 관리하기 위한 UIPopup 클래스.
    /// </summary>
    public class UIPopup : XBehaviour, IPoolable
    {
        // 핵심 UI 컴포넌트

        [FoldoutGroup("#UIPopup")]
        public UILocalizedText TitleText;

        [FoldoutGroup("#UIPopup")]
        public Image DeactiveImage;

        [FoldoutGroup("#UIPopup")]
        public UIPointerEventButton CancelButton;

        // 핸들러들

        private IUIPopupInputHandler _inputHandler;
        private IUIPopupFeedbackHandler _feedbackHandler;
        private IUIPopupCallbackHandler _callbackHandler;
        private IUIPopupInputBlockHandler _inputBlockHandler;
        private UIPopupPauseHandler _pauseHandler;
        private UIPopupFloatyHandler _floatyHandler;

        // 핵심 설정

        [FoldoutGroup("#UIPopup/Settings")]
        public bool NotCheckIfAllPopupsClosed;

        [FoldoutGroup("#UIPopup/Settings")]
        public bool UseFullScreenSize;

        // 속성

        public bool DeactivateMode { get; set; }

        public virtual UIPopupNames Name => UIPopupNames.None;
        protected virtual float InputWaitTimeOnOpenPopup => GameDefine.INPUT_WAIT_TIME;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            // 핵심 UI 컴포넌트 자동 찾기
            TitleText ??= this.FindComponent<UILocalizedText>("Rect/Title Text");
            CancelButton ??= this.FindComponent<UIPointerEventButton>("Rect/Cancel Button");
            CancelButton ??= this.FindComponent<UIPointerEventButton>("Rect/#Content/Cancel Button");
            DeactiveImage ??= this.FindComponent<Image>("Rect/Deactive Image");
        }

        public override void AutoNaming()
        {
            SetGameObjectName(string.Format("UI{0}Popup", Name.ToString()));
        }

        protected virtual void Awake()
        {
            if (CancelButton != null)
            {
                CancelButton.RegisterClickEvent(CloseWithFailure);
            }

            InitializeHandlers();
        }

        /// <summary>
        /// 핸들러들을 초기화합니다.
        /// </summary>
        private void InitializeHandlers()
        {
            // 핸들러들 찾기 및 초기화
            _inputHandler = GetComponent<IUIPopupInputHandler>();
            _feedbackHandler = GetComponent<IUIPopupFeedbackHandler>();
            _callbackHandler = GetComponent<IUIPopupCallbackHandler>();
            _inputBlockHandler = GetComponent<IUIPopupInputBlockHandler>();
            _pauseHandler = GetComponent<UIPopupPauseHandler>();
            _floatyHandler = GetComponent<UIPopupFloatyHandler>();

            // 각 핸들러 초기화
            _inputHandler?.Initialize();
            _feedbackHandler?.Initialize();
            _callbackHandler?.Initialize();
            _inputBlockHandler?.Initialize();
            _pauseHandler?.Initialize();
            _floatyHandler?.Initialize();
        }

        /// <summary>
        /// 팝업의 제목 텍스트를 새로고침합니다.
        /// </summary>
        protected virtual void RefreshTitleText()
        {
            if (TitleText != null)
            {
                TitleText.SetText(Name.GetLocalizedString());
            }
        }

        /// <summary>
        /// 팝업이 스폰될 때 초기화 처리를 합니다.
        /// </summary>
        public void OnSpawn()
        {
            if (UseFullScreenSize)
            {
                rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
                rectTransform.anchoredPosition = Vector2.zero;

                CanvasOrder hudCanvas = UIManager.Instance.GetCanvas(CanvasOrderNames.HUD);
                if (hudCanvas != null)
                {
                    hudCanvas.SetEnabledRaycaster(false);
                }
            }
        }

        /// <summary>
        /// 팝업이 디스폰될 때 초기화 처리를 합니다.
        /// </summary>
        public virtual void OnDespawn()
        {
            // 핸들러들 정리
            _inputHandler?.Cleanup();
            _feedbackHandler?.Cleanup();
            _callbackHandler?.Cleanup();
            _inputBlockHandler?.Cleanup();
            _pauseHandler?.Cleanup();
            _floatyHandler?.Cleanup();

            if (UseFullScreenSize)
            {
                CanvasOrder hudCanvas = UIManager.Instance.GetCanvas(CanvasOrderNames.HUD);
                if (hudCanvas != null)
                {
                    hudCanvas.SetEnabledRaycaster(true);
                }
            }
        }

        /// <summary>
        /// 팝업의 로직을 업데이트합니다.
        /// </summary>
        public virtual void LogicUpdate()
        {
            // 기본 로직.
        }

        /// <summary>
        /// 팝업을 엽니다.
        /// </summary>
        public virtual void Open()
        {
            LogInfo("팝업을 엽니다.");
            UIManager.Instance.PopupManager.StartLockPopupWhileOpening(InputWaitTimeOnOpenPopup);

            ConfigurePopupSettings(true);
            Activate();
            StartBlockInput();
            TriggerOpenFeedback();
            Pause();

            _ = GlobalEvent<UIPopupNames>.Send(GlobalEventType.GAME_POPUP_OPEN, Name);
        }

        /// <summary>
        /// 팝업 설정을 구성합니다.
        /// </summary>
        /// <param name="isOpening">팝업이 열리는 중인지 여부</param>
        private void ConfigurePopupSettings(bool isOpening)
        {
            _inputBlockHandler?.ConfigurePopupSettings(isOpening);
        }

        /// <summary>
        /// 모든 팝업을 닫습니다.
        /// </summary>
        protected virtual void CloseAll()
        {
            UIManager.Instance.PopupManager.CloseAllPopups();
        }

        /// <summary>
        /// 성공 결과로 팝업을 닫습니다.
        /// </summary>
        public void CloseWithSuccess()
        {
            LogClosePopup(true);
            OnClose(true);
            Despawn();

            _ = GlobalEvent<UIPopupNames>.Send(GlobalEventType.GAME_POPUP_CLOSE, Name);
        }

        /// <summary>
        /// 실패 결과로 팝업을 닫습니다.
        /// </summary>
        public void CloseWithFailure()
        {
            LogClosePopup(false);
            OnClose(false);
            Despawn();

            _ = GlobalEvent<UIPopupNames>.Send(GlobalEventType.GAME_POPUP_CLOSE, Name);
        }

        /// <summary>
        /// 팝업이 닫힐 때 후처리를 합니다.
        /// </summary>
        /// <param name="result">닫기 결과의 여부</param>
        public virtual void OnClose(bool result)
        {
            UIManager.Instance.DetailsManager.Clear();
            Resume();
            ConfigurePopupSettings(false);
            InvokeCloseCallback(result);
            TriggerCloseFeedback();
        }

        /// <summary>
        /// 팝업을 디스폰합니다.
        /// </summary>
        public virtual void Despawn()
        {
            if (!IsDestroyed)
            {
                ResourcesManager.Despawn(gameObject);
            }
        }

        /// <summary>
        /// 팝업을 활성화합니다.
        /// </summary>
        public virtual void Activate()
        {
            if (DeactiveImage != null)
            {
                DeactiveImage.raycastTarget = false;
                DeactiveImage.SetAlpha(0f);
            }

            DeactivateMode = false;
            SelectFirstSlotEvent();

            GlobalEvent.Send(GlobalEventType.GAME_POPUP_REFRESH_ACTIVATE);
        }

        /// <summary>
        /// 팝업을 비활성화합니다.
        /// </summary>
        public virtual void Deactivate()
        {
            if (GameInputManager.Instance.CurrentControllerType == Rewired.ControllerType.Joystick)
            {
                if (DeactiveImage != null)
                {
                    DeactiveImage.raycastTarget = true;
                    DeactiveImage.SetAlpha(0.6f);
                }

                DeactivateMode = true;

                GlobalEvent.Send(GlobalEventType.GAME_POPUP_REFRESH_ACTIVATE);
            }
        }

        #region 핸들러 위임 메서드들

        /// <summary>
        /// 입력 시스템을 설정합니다.
        /// </summary>
        /// <param name="maxIndex">최대 타겟 인덱스</param>
        public virtual void InputSetup(int maxIndex)
        {
            _inputHandler?.SetupInput(maxIndex);
        }

        /// <summary>
        /// 지정된 인덱스의 타겟을 활성화합니다.
        /// </summary>
        /// <param name="index">활성화할 타겟의 인덱스</param>
        public virtual void InputActivateTarget(int index)
        {
            _inputHandler?.ActivateTarget(index);
        }

        /// <summary>
        /// 다음 타겟으로 이동합니다.
        /// </summary>
        public virtual void InputNextTarget()
        {
            _inputHandler?.NextTarget();
        }

        /// <summary>
        /// 이전 타겟으로 이동합니다.
        /// </summary>
        public virtual void InputPrevTarget()
        {
            _inputHandler?.PrevTarget();
        }

        /// <summary>
        /// 지정된 인덱스의 타겟을 선택합니다.
        /// </summary>
        /// <param name="index">선택할 타겟의 인덱스</param>
        public virtual void InputSelectTarget(int index)
        {
            _inputHandler?.SelectTarget(index);
        }

        /// <summary>
        /// 팝업 닫기 콜백을 등록합니다.
        /// </summary>
        /// <param name="action">등록할 콜백 액션</param>
        public void RegisterCloseCallback(UnityAction<bool> action)
        {
            _callbackHandler?.RegisterCloseCallback(action);
        }

        /// <summary>
        /// 팝업 닫기 콜백을 해제합니다.
        /// </summary>
        /// <param name="action">해제할 콜백 액션</param>
        public void UnregisterCloseCallback(UnityAction<bool> action)
        {
            _callbackHandler?.UnregisterCloseCallback(action);
        }

        /// <summary>
        /// Floaty 텍스트를 생성합니다.
        /// </summary>
        /// <param name="content">표시할 내용</param>
        /// <param name="moveType">이동 타입</param>
        public void SpawnFloatyText(string content, UIFloatyMoveNames moveType)
        {
            _floatyHandler?.SpawnFloatyText(content, moveType);
        }

        /// <summary>
        /// Floaty 텍스트를 생성합니다. (직접 문자열 전달)
        /// </summary>
        /// <param name="content">표시할 내용</param>
        /// <param name="moveType">이동 타입</param>
        public void SpawnFloatyGetStringText(string content, UIFloatyMoveNames moveType)
        {
            _floatyHandler?.SpawnFloatyGetStringText(content, moveType);
        }

        #endregion 핸들러 위임 메서드들

        #region 피드백

        /// <summary>
        /// 열기 피드백을 트리거합니다.
        /// </summary>
        private void TriggerOpenFeedback()
        {
            _feedbackHandler?.TriggerOpenFeedback();
        }

        /// <summary>
        /// 닫기 피드백을 트리거합니다.
        /// </summary>
        private void TriggerCloseFeedback()
        {
            _feedbackHandler?.TriggerCloseFeedback();
        }

        #endregion 피드백

        #region 콜백

        /// <summary>
        /// 닫기 콜백을 호출합니다.
        /// </summary>
        /// <param name="result">닫기 결과</param>
        private void InvokeCloseCallback(bool result)
        {
            _callbackHandler?.InvokeCloseCallback(result);
        }

        #endregion 콜백

        #region 입력 차단

        /// <summary>
        /// 입력 차단을 시작합니다.
        /// </summary>
        protected void StartBlockInput()
        {
            _inputBlockHandler?.StartBlockInput();
        }

        #endregion 입력 차단

        #region 일시정지

        /// <summary>
        /// 게임을 일시정지합니다.
        /// </summary>
        private void Pause()
        {
            _pauseHandler?.Pause();
        }

        /// <summary>
        /// 게임을 재개합니다.
        /// </summary>
        private void Resume()
        {
            _pauseHandler?.Resume();
        }

        #endregion 일시정지

        /// <summary>
        /// 첫 번째 슬롯 이벤트를 선택합니다.
        /// </summary>
        public virtual void SelectFirstSlotEvent()
        {
            LogInfo("팝업에서 첫 번째 슬롯 이벤트를 선택합니다.");
        }

        // Log

        /// <summary>
        /// 정보 로그를 출력합니다.
        /// </summary>
        /// <param name="content">로그 내용</param>
        protected virtual void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.UI_Popup, $"{Name.ToLogString()} {content}");
            }
        }

        /// <summary>
        /// 팝업 닫기 로그를 출력합니다.
        /// </summary>
        /// <param name="result">닫기 결과</param>
        protected void LogClosePopup(bool result)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.UI_Popup, $"{Name.ToLogString()} 팝업을 닫습니다. 결과: {result.ToBoolString()}");
            }
        }
    }
}