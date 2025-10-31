using System;
using System.Collections;
using TeamSuneat.Data.Game;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat
{
    public class GameManager : MonoBehaviour
    {
        public bool IsBattleActive { get; internal set; }

        private void Awake()
        {
            CharacterManager.Instance.Reset();
            GameInputManager.Instance.Initialize();
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => { return GameApp.Instance != null; });
            InitializeGameplayTimeTracking();
        }

        private void InitializeGameplayTimeTracking()
        {
            VProfile profileInfo = GameApp.GetSelectedProfile();

            // 게임 재시작 시 게임플레이 시간 추적 상태를 복원
            if (GameApp.Instance.data == null || profileInfo == null || profileInfo.Statistics == null)
            {
                GameTimeManager.Instance.InitializeGameplayTracking(false);
            }
            else
            {
                bool isChallengeStarted = profileInfo.Statistics.IsChallengeStarted();
                GameTimeManager.Instance.InitializeGameplayTracking(isChallengeStarted);
            }
        }

        private void OnEnable()
        {
            if (GameInputManager.Instance.IsInitialized)
            {
                GameInputManager.Instance.SubscribeEvents();
            }
        }

        private void OnDisable()
        {
            GameInputManager.Instance.UnsubscribeEvents();
        }

        private void OnDestroy()
        {
            // 안전장치
            GameInputManager.Instance.UnsubscribeEvents();
            GameInputManager.Instance.IsInitialized = false;
        }

        private void Update()
        {
            GameTimeManager.Instance.UpdateTimeTracking();
            GameInputManager.Instance.GetInputState();
            CharacterManager.Instance.LogicUpdate();

            // UIManager.Instance?.LogicUpdate();
        }

        private void LateUpdate()
        {
            GameInputManager.Instance.ProcessButtonStates();
            GameSetting.Instance.Video.RefreshResolutionRate();
            // UIManager.Instance?.LateLogicUpdate();
            // PassiveManager.Instance.LateLogicUpdate();
        }

        private void FixedUpdate()
        {
            CharacterManager.Instance.PhysicsUpdate();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                GameInputManager.Instance.ResetButtonStates();
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause) { GameTimeManager.Instance.Pause(); }
            else { GameTimeManager.Instance.Resume(); }
        }

        internal void ResetStage()
        {
        }
    }
}