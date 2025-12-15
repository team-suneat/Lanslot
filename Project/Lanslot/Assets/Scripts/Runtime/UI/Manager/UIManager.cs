using Sirenix.OdinInspector;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public partial class UIManager : XStaticBehaviour<UIManager>
    {
        [Title("Manager")]
        public UICanvasManager CanvasManager;

        public UIPopupManager PopupManager;

        public UIGaugeManager GaugeManager;

        public UIDetailsManager DetailsManager;

        public HUDManager HUDManager;

        public Vector3 WorldPositionMin { get; set; }
        public Vector3 WorldPositionMax { get; set; }

        //──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        public const float WAIT_INPUT_TIME = 0.2f;

        //──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            CanvasManager = GetComponent<UICanvasManager>();
            PopupManager = GetComponentInChildren<UIPopupManager>();
            HUDManager = GetComponentInChildren<HUDManager>();
            GaugeManager = GetComponentInChildren<UIGaugeManager>();
            DetailsManager = GetComponentInChildren<UIDetailsManager>();
        }

        public void Clear()
        {
            PopupManager?.ResetValues();
            GaugeManager?.Clear();
        }

        public CanvasOrder GetCanvas(CanvasOrderNames canvasOrderName)
        {
            return CanvasManager.Get(canvasOrderName);
        }

        public void LogicUpdate()
        {
            if (GameSetting.Instance.Input.IsBlockUIInput)
            {
                return;
            }

            PopupManager?.LogicUpdate();
            HUDManager?.LogicUpdate();
        }

        internal void SpawnSoliloquyNotice(SoliloquyTypes content)
        {
        }

        internal void SpawnSoliloquyNotice(string content)
        {
        }

        internal void SpawnSoliloquyIngame(SoliloquyTypes canNotUsedYet)
        {
        }

        internal void SpawnSoliloquyIngame(SoliloquyTypes unstackEffect, string content)
        {
        }

        internal void SpawnNoticeMessage(string nameContent, string descContent)
        {
        }
    }
}