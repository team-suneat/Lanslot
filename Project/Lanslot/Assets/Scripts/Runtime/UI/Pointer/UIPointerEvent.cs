using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TeamSuneat.UserInterface
{
    public class UIPointerEvent : XBehaviour
    {
        public enum PadClickTypes
        {
            Right,
            Left,
        }

        [FoldoutGroup("#UIPointerEvent")]
        public PadClickTypes PadClickType;

        [FoldoutGroup("#UIPointerEvent")]
        public UIClickable Clickable;

        [FoldoutGroup("#UIPointerEvent")]
        public UISelectable Selectable;

        [FoldoutGroup("#UIPointerEvent/Select")]
        public bool LockTrigger;

        [FoldoutGroup("#UIPointerEvent/Select")]
        [DisableIf("LockTrigger", true)]
        public int SelectIndex;

        [FoldoutGroup("#UIPointerEvent/Select")]
        [DisableIf("LockTrigger", true)]
        public int SelectOrderIndex;

        [FoldoutGroup("#UIPointerEvent/Select")]
        [DisableIf("LockTrigger", true)]
        public int SelectLeftIndex;

        [FoldoutGroup("#UIPointerEvent/Select")]
        [DisableIf("LockTrigger", true)]
        public int SelectRightIndex;

        [FoldoutGroup("#UIPointerEvent/Select")]
        [DisableIf("LockTrigger", true)]
        public int SelectUpIndex;

        [FoldoutGroup("#UIPointerEvent/Select")]
        [DisableIf("LockTrigger", true)]
        public int SelectDownIndex;

        [FoldoutGroup("#UIPointerEvent/Select")]
        public Vector2 SelectFrameSizeDelta;

        [FoldoutGroup("#UIPointerEvent/Select")]
        public Vector3 SelectFrameOffset;

        private UnityAction _enterEventAction;
        private UnityAction _exitEventAction;
        private UnityAction _overEventAction;

        private const int DEFAULT_SELECT_INDEX = 0;
        protected bool IsEnterPointer { get; private set; }

        /// 컴포넌트를 자동으로 가져옵니다.
        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Clickable = GetComponentInChildren<UIClickable>();
            Selectable = GetComponentInChildren<UISelectable>();
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (Selectable == null && Clickable == null)
            {
                ClearSelectIndex();
            }
        }

        /// 오브젝트 초기화 시 호출됩니다.
        protected virtual void Awake()
        {
            RegisterClickableEvents();
            RegisterSelectableEvents();
        }

        /// 클릭 관련 이벤트를 등록합니다.
        private void RegisterClickableEvents()
        {
            if (Clickable != null)
            {
                Clickable.RegisterPointerClickLeftEvent((PointerEventData pointerEventData) => { OnPointerClickLeft(); });
                Clickable.RegisterPointerClickRightEvent((PointerEventData pointerEventData) => { OnPointerClickRight(); });
                Clickable.RegisterPointerPressLeftEvent((PointerEventData pointerEventData) => { OnPointerPressLeft(); });
                Clickable.RegisterPointerUpLeftEvent((PointerEventData pointerEventData) => { OnPointerUpLeft(); });
            }
        }

        /// 선택 관련 이벤트를 등록합니다.
        private void RegisterSelectableEvents()
        {
            if (Selectable != null)
            {
                Selectable.RegisterPointerEnterEvent(OnPointerEnterEvent);
                Selectable.RegisterPointerExitEvent(OnPointerExitEvent);
            }
        }

        /// 포인터 들어옴 이벤트 처리입니다.
        protected virtual void OnPointerEnterEvent(PointerEventData eventData)
        {
            OnPointerEnter();
        }

        /// 포인터 나감 이벤트 처리입니다.
        protected virtual void OnPointerExitEvent(PointerEventData eventData)
        {
            OnPointerExit();
        }

        /// 좌/우 클릭 타입에 따라 클릭 이벤트를 처리합니다.
        protected void OnPointerClick()
        {
            if (PadClickType == PadClickTypes.Left)
            {
                OnPointerClickLeft();
            }
            else if (PadClickType == PadClickTypes.Right)
            {
                OnPointerClickRight();
            }
        }

        protected virtual void OnPointerPressLeft()
        {
        }

        /// 왼쪽 클릭 이벤트를 처리합니다.
        protected virtual void OnPointerClickLeft()
        {
        }

        /// 오른쪽 클릭 이벤트를 처리합니다.
        protected virtual void OnPointerClickRight()
        {
        }

        /// 왼쪽 버튼 업 이벤트를 처리합니다.
        protected virtual void OnPointerUpLeft()
        {
        }

        /// 포인터 진입 시 처리합니다.
        protected virtual void OnPointerEnter()
        {
            if (_enterEventAction != null)
            {
                _enterEventAction.Invoke();
            }
            IsEnterPointer = true;
        }

        /// 포인터 이탈 시 처리합니다.
        protected virtual void OnPointerExit()
        {
            if (_exitEventAction != null)
            {
                _exitEventAction.Invoke();
            }
            IsEnterPointer = false;
        }

        /// 포인터 진입 이벤트를 등록합니다.
        public virtual void RegisterOnPointEnter(UnityAction unityAction)
        {
            Log.Info(LogTags.UI_SelectEvent, "{0}, 포인터 진입 시 호출될 이벤트를 등록합니다: {1}", this.GetHierarchyName(), unityAction.Method.Name);
            _enterEventAction += unityAction;
        }

        public virtual void RegisterOnPointExit(UnityAction unityAction)
        {
            Log.Info(LogTags.UI_SelectEvent, "{0}, 포인터 이탈 시 호출될 이벤트를 등록합니다: {1}", this.GetHierarchyName(), unityAction.Method.Name);
            _exitEventAction += unityAction;
        }

        /// 포인터가 눌렸을 때 처리합니다.
        public virtual void OnPointerPressed()
        {
        }

        /// 선택 인덱스를 초기화합니다.
        public void ClearSelectIndex()
        {
            SelectIndex = DEFAULT_SELECT_INDEX;
            SelectLeftIndex = DEFAULT_SELECT_INDEX;
            SelectRightIndex = DEFAULT_SELECT_INDEX;
            SelectUpIndex = DEFAULT_SELECT_INDEX;
            SelectDownIndex = DEFAULT_SELECT_INDEX;
        }

        public void SetSelectIndex(int selectIndex)
        {
            SelectIndex = selectIndex;
        }

        //

        private void OnDrawGizmos()
        {
            float offset = 24;
            int fontSize = 10;

            if (SelectLeftIndex > 0)
            {
                GizmoEx.DrawText(SelectLeftIndex.ToColorString(GameColors.WhiteSmoke), position + Vector3.left * offset, fontSize);
            }
            if (SelectRightIndex > 0)
            {
                GizmoEx.DrawText(SelectRightIndex.ToColorString(GameColors.AntiqueWhite), position + Vector3.right * offset, fontSize);
            }
            if (SelectUpIndex > 0)
            {
                GizmoEx.DrawText(SelectUpIndex.ToColorString(GameColors.FloralWhite), position + Vector3.up * offset, fontSize);
            }
            if (SelectDownIndex > 0)
            {
                GizmoEx.DrawText(SelectDownIndex.ToColorString(GameColors.GhostWhite), position + Vector3.down * offset, fontSize);
            }
            if (SelectIndex > 0)
            {
                GizmoEx.DrawText(SelectIndex.ToColorString(GameColors.LimeGreen), position);
            }
        }
    }
}