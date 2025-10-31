using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace TeamSuneat
{
    // 간단한 데모용: 빈 씬에서도 월드 틱 로그를 확인하기 위한 컴포넌트
    public sealed class ClockDebugLogger : MonoBehaviour
    {
        private void Update()
        {
            // 키보드 토글: P=정지/해제, 1/2/4=속도
            var wc = WorldClock.GameInstance;
            if (wc != null)
            {
                #if ENABLE_INPUT_SYSTEM
                if (Keyboard.current != null)
                {
                    if (Keyboard.current.pKey.wasPressedThisFrame) wc.IsPaused = !wc.IsPaused;
                    if (Keyboard.current.digit1Key.wasPressedThisFrame) wc.TimeScale = 1;
                    if (Keyboard.current.digit2Key.wasPressedThisFrame) wc.TimeScale = 2;
                    if (Keyboard.current.digit4Key.wasPressedThisFrame) wc.TimeScale = 4;
                }
                #else
                if (Input.GetKeyDown(KeyCode.P)) wc.IsPaused = !wc.IsPaused;
                if (Input.GetKeyDown(KeyCode.Alpha1)) wc.TimeScale = 1;
                if (Input.GetKeyDown(KeyCode.Alpha2)) wc.TimeScale = 2;
                if (Input.GetKeyDown(KeyCode.Alpha4)) wc.TimeScale = 4;
                #endif
            }
        }

        private void OnEnable()
        {
            if (WorldClock.GameInstance != null)
            {
                WorldClock.GameInstance.OnTick += HandleTick;
            }
        }

        private void OnDisable()
        {
            if (WorldClock.GameInstance != null)
            {
                WorldClock.GameInstance.OnTick -= HandleTick;
            }
        }

        private void HandleTick(int tick)
        {
            if ((tick % 20) == 0)
            {
                Debug.Log($"[Tick] {tick} | paused={WorldClock.GameInstance?.IsPaused} x{WorldClock.GameInstance?.TimeScale}");
            }
        }
    }
}


