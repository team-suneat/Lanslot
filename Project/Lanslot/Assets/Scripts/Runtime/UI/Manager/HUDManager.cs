using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat.UserInterface
{
    public class HUDManager : XBehaviour
    {     
        public UICanvasGroupFader HUDCanvasGroupFader;

        public HUDSlotMachine SlotMachine;
        
        public HUDStageTimer Timer;

        public bool IsLockHUD{get;set;} = false;
        
        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            HUDCanvasGroupFader = GetComponentInChildren<UICanvasGroupFader>();
            SlotMachine = GetComponentInChildren<HUDSlotMachine>();
            Timer = GetComponentInChildren<HUDStageTimer>();

        }
        public void LogicUpdate()
        {
            SlotMachine.LogicUpdate();
        }
    }
}