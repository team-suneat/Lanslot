using DG.Tweening;
using TeamSuneat.Audio;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace TeamSuneat.UserInterface
{
    [RequireComponent(typeof(UIClickable), typeof(UISelectable))]
    public class UIPointerEventButton : UIPointerEvent
    {
        [FoldoutGroup("#이벤트 버튼")]
        [SuffixLabel("선택형 버튼일 경우 활성화합니다.")]
        public bool SelectMode;

        [FoldoutGroup("#이벤트 버튼/컴포넌트")] public Image ButtonIcon;
        [FoldoutGroup("#이벤트 버튼/컴포넌트")] public Image ValueImage;
        [FoldoutGroup("#이벤트 버튼/컴포넌트")] public UILocalizedText NameText;

        [FoldoutGroup("#이벤트 버튼/Sprite")] public Sprite NormalSprite;
        [FoldoutGroup("#이벤트 버튼/Sprite")] public Sprite LockSprite;
        [FoldoutGroup("#이벤트 버튼/Sprite")] public Sprite MouseOverSprite;
        [FoldoutGroup("#이벤트 버튼/Sprite")] public Sprite ClickSprite;
        [FoldoutGroup("#이벤트 버튼/Sprite")] public Sprite SelectSprite;
        [FoldoutGroup("#이벤트 버튼/Sprite")] public bool UseNativeSpriteSize = true;

        [FoldoutGroup("#이벤트 버튼/MouseOver")] public bool IsMouseOverTextColor;
        [FoldoutGroup("#이벤트 버튼/MouseOver")] public Color MouseOverTextColor = GameColors.Red;
        [FoldoutGroup("#이벤트 버튼/MouseOver")] public Image MouseOverImage;

        [SuffixLabel("마우스 Enter/Exit에 텍스트 색상을 자동으로 변경하지 않습니다.")]
        [FoldoutGroup("#이벤트 버튼/Toggle")] public bool BlockColorChangeOnMouseOver;

        [SuffixLabel("마우스 Enter/Exit에 텍스트 밑줄을 표시합니다.")]
        [FoldoutGroup("#이벤트 버튼/Toggle")] public bool UseUnderlineOnMouseOver;

        [FoldoutGroup("#이벤트 버튼/Toggle")] public bool AutoSelectFrameNormal;

        private const SoundNames SOUND_POINTER_ENTER = SoundNames.UI_Sound_Move_Button;
        private const SoundNames SOUND_POIUNTER_CLICK = SoundNames.UI_Sound_Click_Button;

        public bool IsLocked { get; private set; }
        public bool IsSelected { get; private set; }

        private Tweener LockTweener { get; set; }
        private UnityAction _clickLeftEvent;
        private Coroutine _clickCoroutine;

        public override void AutoSetting()
        {
            base.AutoSetting();
        }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
            ButtonIcon = this.FindComponent<Image>("Button Icon");
            ValueImage = this.FindComponent<Image>("Value Image");
            NameText = this.FindComponent<UILocalizedText>("Name Text");
        }

        private void OnValidate()
        {
            if (ButtonIcon != null && NormalSprite == null)
            {
                Log.Error("버튼의 노멀 스프라이트가 설정되어 있지 않습니다. {0}", this.GetHierarchyPath());
            }
        }

        protected override void Awake()
        {
            base.Awake();
            InitializeComponents();
        }

        protected override void OnStart()
        {
            base.OnStart();
            Prepare();
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();

            if (IsStarted)
            {
                Prepare();
            }
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            StopLockTweener();
        }

        private void Prepare()
        {
            ResetNameTextColor();

            if (!IsEnterPointer)
                DeactivateMouseOverImage();
        }

        #region Pointer Event

        protected override void OnPointerEnter()
        {
            base.OnPointerEnter();

            HandleNameTextColorChange();
            HandleUnderlineOnMouseOver();
            HandleButtonIconSpriteOnEnter();
            ActivateMouseOverImage();

            PlayPointerEnterSound();
            SelectByMouse();
        }

        protected override void OnPointerExit()
        {
            base.OnPointerExit();

            ResetNameTextColor();
            HandleUnderlineOnMouseExit();
            HandleButtonIconSpriteOnExit();
            DeactivateMouseOverImage();
        }

        protected override void OnPointerPressLeft()
        {
            base.OnPointerPressLeft();
        }

        protected override void OnPointerClickLeft()
        {
            base.OnPointerClickLeft();
            HandlePointerDownClickLeft();

            AudioManager.Instance.PlaySFXOneShotUnscaled(SOUND_POIUNTER_CLICK);
        }

        protected override void OnPointerClickRight()
        {
            base.OnPointerClickRight();
            HandlePointerClickRight();

            AudioManager.Instance.PlaySFXOneShotUnscaled(SOUND_POIUNTER_CLICK);
        }

        #endregion Pointer Event

        #region Pointer Enter Event

        private void HandleNameTextColorChange()
        {
            if (NameText != null && !BlockColorChangeOnMouseOver)
            {
                if (IsLocked)
                {
                    NameText.SetTextColor(GameColors.Disable);
                }
                else if (IsMouseOverTextColor)
                {
                    NameText.SetTextColor(MouseOverTextColor);
                }
                else
                {
                    NameText.ResetTextColor();
                }
            }
        }

        private void HandleUnderlineOnMouseOver()
        {
            if (NameText != null && UseUnderlineOnMouseOver)
            {
                NameText.SetUnderline(true);
            }
        }

        private void HandleButtonIconSpriteOnEnter()
        {
            if (!IsLocked && !IsSelected)
            {
                SetButtonIconSpriteByState(ToggleButtonStates.MouseOver);
            }
            else if (IsLocked)
            {
                SetButtonIconSpriteByState(ToggleButtonStates.Lock);
            }
        }

        private void ActivateMouseOverImage()
        {
            MouseOverImage?.SetActive(true);
        }

        private void PlayPointerEnterSound()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFXOneShotUnscaled(SOUND_POINTER_ENTER);
            }
        }

        private void SelectByMouse()
        {
            if (GameInputManager.Instance != null)
            {
                if (GameInputManager.Instance.CurrentControllerType == Rewired.ControllerType.Mouse)
                {
                    // UIManager.Instance.PopupManager.SelectController.SelectByMouse(SelectIndex);
                }
            }
        }

        #endregion Pointer Enter Event

        #region Pointer Exit Event

        private void ResetNameTextColor()
        {
            if (NameText != null && !BlockColorChangeOnMouseOver)
            {
                if (IsLocked)
                {
                    NameText.SetTextColor(GameColors.Disable);
                }
                else
                {
                    NameText.ResetTextColor();
                }
            }
        }

        private void HandleUnderlineOnMouseExit()
        {
            if (NameText != null && UseUnderlineOnMouseOver)
            {
                NameText.SetUnderline(false);
            }
        }

        private void HandleButtonIconSpriteOnExit()
        {
            if (IsLocked)
            {
                SetButtonIconSpriteByState(ToggleButtonStates.Lock);
            }
            else if (IsSelected)
            {
                SetButtonIconSpriteByState(ToggleButtonStates.Select);
            }
            else
            {
                if (SelectMode)
                {
                    SetButtonIconSpriteByState(ToggleButtonStates.Unselect);
                }
                else
                {
                    SetButtonIconSpriteByState(ToggleButtonStates.Unlock);
                }
            }
        }

        private void DeactivateMouseOverImage()
        {
            MouseOverImage?.SetActive(false);
        }

        #endregion Pointer Exit Event

        #region Select

        public virtual void Select()
        {
            IsSelected = true;
            SetButtonIconSpriteByState(ToggleButtonStates.Select);
            AutoSelectFrameIfNeeded();

            Log.Info(LogTags.UI_Button, "{0} 버튼을 선택합니다.", name);
        }

        public virtual void Deselect()
        {
            IsSelected = false;
            RefreshButtonIconSpriteByState();
            DespawnSelectSlotFrameIfNeeded();

            Log.Info(LogTags.UI_Button, "{0} 버튼의 선택을 해제합니다.", name);
        }

        #endregion Select

        #region Lock

        public virtual void Lock()
        {
            IsLocked = true;
            SetButtonIconSpriteByState(ToggleButtonStates.Lock);
            UpdateNameTextForLock();
            DeactivateMouseOverImage();

            Log.Info(LogTags.UI_Button, "{0} 버튼을 잠급니다.", name);
        }

        public virtual void Unlock()
        {
            IsLocked = false;
            SetButtonIconSpriteByState(ToggleButtonStates.Unlock);
            ResetNameTextColor();

            Log.Info(LogTags.UI_Button, "{0} 버튼을 해제합니다.", name);
        }

        #endregion Lock

        #region Underline

        public void ResetUnderline()
        {
            NameText?.SetUnderline(false);
        }

        #endregion Underline

        #region Click Event

        public virtual void CallClickLeftEvent()
        { }

        public virtual void CallClickRightEvent()
        { }

        public void RegisterClickEvent(UnityAction action)
        {
            _clickLeftEvent ??= action;
        }

        public void CallClickLeftEventByRegister()
        {
            Log.Info(LogTags.UI_SelectEvent, "등록된 클릭 이벤트를 호출합니다. {0}", this.GetHierarchyName());
            _clickLeftEvent?.Invoke();
        }

        #endregion Click Event

        public virtual void Clear()
        {
            Deselect();
        }

        private void ClickLockAnimation()
        {
            if (NameText == null || LockTweener != null)
            {
                return;
            }

            NameText.SetTextColor(GameColors.Red);

            LockTweener = NameText.transform.DOPunchScale(Vector3.right * 0.2f, 0.2f);
            LockTweener.OnComplete(() =>
            {
                NameText.transform.localScale = Vector3.one;
                NameText.SetTextColor(GameColors.Disable);
                LockTweener = null;
            });
        }

        private void StopLockTweener()
        {
            if (LockTweener != null)
            {
                LockTweener.Kill();
                LockTweener = null;
            }
        }

        /// 선택 슬롯 프레임을 생성합니다.
        public void SpawnSelectSlotFrame(UISelectFrameTypes type, UnityEngine.Transform target, UnityEngine.Transform parent = null)
        {
            Log.Info(LogTags.UI_SelectEvent, "SpawnSelectSlotFrame 호출.");
            // UIManager.Instance.PopupManager.SelectController.SpawnSelectSlotFrame(type, SelectFrameSizeDelta, SelectFrameOffset, target, parent);
        }

        /// 선택 슬롯 프레임을 제거합니다.
        public void DespawnSelectSlotFrame()
        {
            Log.Info(LogTags.UI_SelectEvent, "DeSpawnSelectSlotFrame 호출.");
            // UIManager.Instance.PopupManager.SelectController.DespawnSelectSlotFrame();
        }

        #region Helper Methods

        private void InitializeComponents()
        {
            if (NameText == null)
            {
                NameText = this.FindComponent<UILocalizedText>("Name Text");
            }

            ButtonIcon ??= this.FindComponent<Image>("Button Icon");
        }

        private void AutoSelectFrameIfNeeded()
        {
            if (AutoSelectFrameNormal)
            {
                SpawnSelectSlotFrame(UISelectFrameTypes.Normal, transform);
            }
        }

        private void DespawnSelectSlotFrameIfNeeded()
        {
            if (AutoSelectFrameNormal)
            {
                DespawnSelectSlotFrame();
            }
        }

        private void UpdateNameTextForLock()
        {
            if (NameText != null)
            {
                if (IsMouseOverTextColor)
                {
                    NameText.SetTextColor(GameColors.Disable);
                }

                if (UseUnderlineOnMouseOver)
                {
                    NameText.SetUnderline(false);
                }
            }
        }

        private void HandlePointerDownClickLeft()
        {
            if (!IsLocked)
            {
                SetButtonIconSpriteByState(ToggleButtonStates.Click);
                CoroutineNextRealTimer(0.1f, UpdateButtonIconOnClick);

                CallClickLeftEvent();
                CallClickLeftEventByRegister();
            }
            else
            {
                ClickLockAnimation();
            }
        }

        private void HandlePointerClickRight()
        {
            if (!IsLocked)
            {
                CallClickRightEvent();
            }
        }

        private void UpdateButtonIconOnClick()
        {
            if (!IsLocked && !IsSelected)
            {
                if (SelectMode)
                {
                    SetButtonIconSpriteByState(ToggleButtonStates.Unselect);
                }
                else
                {
                    SetButtonIconSpriteByState(ToggleButtonStates.Click);
                    _clickCoroutine ??= StartXCoroutine(ProcessClick());
                }
            }
            else if (IsLocked)
            {
                SetButtonIconSpriteByState(ToggleButtonStates.Lock);
            }
            else if (IsSelected)
            {
                SetButtonIconSpriteByState(ToggleButtonStates.Select);
            }
        }

        private void UpdateButtonIcon()
        {
            if (IsLocked)
            {
                SetButtonIconSpriteByState(ToggleButtonStates.Lock);
            }
            else if (IsSelected)
            {
                SetButtonIconSpriteByState(ToggleButtonStates.Select);
            }
            else
            {
                SetButtonIconSpriteByState(ToggleButtonStates.Unselect);
            }
        }

        private IEnumerator ProcessClick()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            UpdateButtonIcon();
            _clickCoroutine = null;
        }

        private void SetButtonIconSpriteByState(ToggleButtonStates state)
        {
            if (ButtonIcon != null)
            {
                switch (state)
                {
                    case ToggleButtonStates.Lock:
                        if (LockSprite != null)
                        {
                            ButtonIcon.SetSprite(LockSprite, UseNativeSpriteSize);
                        }
                        break;

                    case ToggleButtonStates.Select:
                        if (SelectSprite != null)
                        {
                            ButtonIcon.SetSprite(SelectSprite, UseNativeSpriteSize);
                        }
                        break;

                    case ToggleButtonStates.MouseOver:
                        if (MouseOverSprite != null)
                        {
                            ButtonIcon.SetSprite(MouseOverSprite, UseNativeSpriteSize);
                        }
                        break;

                    case ToggleButtonStates.Click:
                        if (ClickSprite != null)
                        {
                            ButtonIcon.SetSprite(ClickSprite, UseNativeSpriteSize);
                        }
                        break;

                    case ToggleButtonStates.Unselect:
                    case ToggleButtonStates.Unlock:
                    case ToggleButtonStates.Enter:
                    case ToggleButtonStates.Exit:
                    default:
                        if (NormalSprite != null)
                        {
                            ButtonIcon.SetSprite(NormalSprite, UseNativeSpriteSize);
                        }
                        break;
                }
            }
        }

        private void RefreshButtonIconSpriteByState()
        {
            if (!IsLocked)
            {
                if (SelectMode)
                {
                    SetButtonIconSpriteByState(ToggleButtonStates.Unselect);
                }
                else
                {
                    SetButtonIconSpriteByState(ToggleButtonStates.Unlock);
                }
            }
            else
            {
                SetButtonIconSpriteByState(ToggleButtonStates.Lock);
            }
        }

        #endregion Helper Methods

        public void ActivateRaycast()
        {
            if (ButtonIcon != null)
            {
                ButtonIcon.raycastTarget = true;
            }

            if (ValueImage != null)
            {
                ValueImage.raycastTarget = true;
            }

            if (NameText != null)
            {
                NameText.TextPro.raycastTarget = true;
            }
        }

        public void DeactivateRaycast()
        {
            if (ButtonIcon != null)
            {
                ButtonIcon.raycastTarget = false;
            }

            if (ValueImage != null)
            {
                ValueImage.raycastTarget = false;
            }

            if (NameText != null)
            {
                NameText.TextPro.raycastTarget = false;
            }
        }
    }
}
