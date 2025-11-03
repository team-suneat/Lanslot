using Sirenix.OdinInspector;
using System.Collections;
using TeamSuneat.Audio;
using TeamSuneat.Data.Game;
using TeamSuneat.Setting;
using TeamSuneat.UserInterface;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat
{
    /// <summary>
    /// 게임 타이틀 씬을 관리하는 클래스
    /// </summary>
    public class GameTitleScene : XScene
    {
        [Title("#Settings")]
        public SoundNames BGMName;
        public string BGMNameString;
        public float DelayTimeForChangeScene;

        [Title("#Component")]
        public Button GameStartButton;
        public Button GameExitButton;

        private bool _isStarted;

        [Button]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void AutoSetting()
        {
            // BGM 이름이 설정되어 있으면 문자열로 변환
            if (BGMName != SoundNames.None)
            {
                BGMNameString = BGMName.ToString();
            }
        }

        /// <summary>
        /// Inspector에서 값이 변경될 때 호출되는 검증 메서드
        /// </summary>
        private void OnValidate()
        {
            _ = EnumEx.ConvertTo(ref BGMName, BGMNameString);
        }

        /// <summary>
        /// 씬 생성 시 호출
        /// </summary>
        protected override void OnCreateScene()
        {
            RegisterButtonEvent();
            SetInteractableButtons(false);
        }

        /// <summary>
        /// 씬 진입 시 호출
        /// </summary>
        protected override void OnEnterScene()
        {
            StopAmbience();
            TryStartChangeBGM();

            _ = StartCoroutine(WaitForInitailze());
        }

        /// <summary>
        /// 씬 종료 시 호출
        /// </summary>
        protected override void OnExitScene()
        {
        }

        /// <summary>
        /// 씬 파괴 시 호출
        /// </summary>
        protected override void OnDestroyScene()
        {
        }

        //───────────────────────────────────────────────────────────────────────────

        private IEnumerator WaitForInitailze()
        {
            yield return new WaitUntil(() => { return GameApp.Instance.IsInitialized; });
            SetInteractableButtons(true);
        }

        private void RegisterButtonEvent()
        {
            GameStartButton.onClick.AddListener(OnGameStart);
            GameExitButton.onClick.AddListener(OnGameExit);
        }

        private void OnGameStart()
        {
            SetInteractableButtons(false);
            SpawnPopup();
        }

        private void SpawnPopup()
        {
            UIPopup popup = UIManager.Instance.PopupManager.SpawnCenterPopup(UIPopupNames.GameStartSelection, OnDespawnPopup);
            if (popup != null)
            {
                UIGameStartSelectionPopup characterPopup = popup as UIGameStartSelectionPopup;
                characterPopup.Setup();
            }
        }

        private void OnDespawnPopup(bool popupResult)
        {
            if (popupResult)
            {
                ChangeMainScene();
            }
            else
            {
                SetInteractableButtons(true);
            }
        }

        private void OnGameExit()
        {
            SetInteractableButtons(false);

#if UNITY_EDITOR
            // 에디터 게임 재생 종료
#else
            Application.Quit();
#endif
        }

        private void SetPlayerCharacter(CharacterNames characterName)
        {
            VProfile profieInfo = GameApp.GetSelectedProfile();
            profieInfo?.Character.Select(characterName);
        }

        private void SetInteractableButtons(bool value)
        {
            GameStartButton.interactable = value;
            GameExitButton.interactable = value;
        }

        //───────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// BGM 변경을 시도합니다
        /// </summary>
        private void TryStartChangeBGM()
        {
            _ = AudioManager.Instance.TryStartChangeBGM(BGMName);
        }

        /// <summary>
        /// 앰비언스 사운드를 정지합니다
        /// </summary>
        private void StopAmbience()
        {
            AudioManager.Instance.StopAmbience();
        }

        /// <summary>
        /// 메인 씬으로 전환을 시작합니다
        /// </summary>
        public void StartChangeMainScene()
        {
            StartChangeScene(ChangeMainScene);
        }

        // 씬 전환 메서드들

        /// <summary>
        /// 씬 전환을 시작하는 공통 메서드
        /// </summary>
        /// <param name="changeSceneAction">실행할 씬 전환 액션</param>
        private void StartChangeScene(UnityAction changeSceneAction)
        {
            if (_isStarted) { return; }

            _isStarted = true;
            GameSetting.Instance.Input.BlockUIInput();
            if (DelayTimeForChangeScene > 0)
            {
                _ = StartCoroutine(ProcessChangeScene(changeSceneAction));
            }
            else
            {
                changeSceneAction.Invoke();
            }
        }

        private IEnumerator ProcessChangeScene(UnityAction changeSceneAction)
        {
            yield return new WaitForSeconds(DelayTimeForChangeScene);
            changeSceneAction.Invoke();
        }

        /// <summary>
        /// 메인 씬으로 전환합니다
        /// </summary>
        private void ChangeMainScene()
        {
            ChangeToScene("GameMain");
        }

        /// <summary>
        /// 지정된 씬으로 전환하는 공통 메서드
        /// </summary>
        /// <param name="sceneName">전환할 씬 이름</param>
        private void ChangeToScene(string sceneName)
        {
            GameSetting.Instance.Input.UnblockUIInput();

            if (DetermineChangeScene(sceneName))
            {
                ChangeScene(sceneName);
            }
        }
    }
}