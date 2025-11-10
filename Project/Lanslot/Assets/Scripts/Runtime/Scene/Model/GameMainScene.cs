using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class GameMainScene : XScene
    {
        #region Private Fields

        private StageSystem _currentStageSystem;

        #endregion Private Fields

        #region XScene

        protected override void OnCreateScene()
        {
        }

        protected override void OnEnterScene()
        {
            RegisterGlobalEvent();
            TurnManager.Instance.Initialize();
            LoadStage();
        }

        protected override void OnExitScene()
        {
            UnregisterGlobalEvent();
        }

        protected override void OnDestroyScene()
        {
        }

        #endregion XScene

        #region Global Event

        protected void RegisterGlobalEvent()
        {
            GlobalEvent.Register(GlobalEventType.MOVE_TO_STAGE, OnMoveToStage);
            GlobalEvent.Register(GlobalEventType.MOVE_TO_TITLE, OnMoveToTitle);
        }

        protected void UnregisterGlobalEvent()
        {
            GlobalEvent.Unregister(GlobalEventType.MOVE_TO_STAGE, OnMoveToStage);
            GlobalEvent.Unregister(GlobalEventType.MOVE_TO_TITLE, OnMoveToTitle);
        }

        #endregion Global Event

        #region Change Scene

        protected override void CleanupCurrentScene()
        {
            // 스테이지 시스템 정리
            if (_currentStageSystem != null)
            {
                _currentStageSystem.CleanupStage();
                Destroy(_currentStageSystem.gameObject);
                _currentStageSystem = null;
            }

            Audio.AudioManager.Instance.Clear();
            UserInterface.UIManager.Instance?.Clear();

            CharacterManager.Instance.ClearMonsterAndAlliance();
            VitalManager.Instance.Clear();

            // DropObjectManager.Instance.Clear();
            VFXManager.ClearNull();

            TurnManager.Instance.Reset();

            GameApp.Instance.SaveGameData();
            base.CleanupCurrentScene();
        }

        private void OnMoveToTitle()
        {
            if (DetermineChangeScene("GameTitle"))
            {
                CharacterManager.Instance.UnregisterPlayer();
                GameApp.Instance.gameManager.ResetStage();

                ChangeScene("GameTitle");
            }
        }

        private void OnMoveToStage()
        {
            if (DetermineChangeScene("GameMain"))
            {
                GameApp.Instance.gameManager.ResetStage();

                // if (_stageSpawnHandler == null)
                // {
                ChangeScene("GameMain");
                //    return;
                // }

                // CleanupCurrentScene();
                // StartXCoroutine(ProcessChangingStage(profileInfo.Stage.CurrentStageName));

                /*
                 * 동일 Area인 경우 ChangeStage, 다른 Area인 경우 ChangeScene을 호출합니다.
                 * 핸들러가 생성된 스테이지 중에 현재 스테이지가 있는지 확인합니다.
                 */
            }
        }

        #endregion Change Scene

        #region Stage Loading

        private async void LoadStage()
        {
            try
            {
                // GameApp 초기화 완료 대기
                await WaitForGameAppInitialized();

                // 현재 스테이지 정보 가져오기
                Data.Game.VProfile profileInfo = GameApp.GetSelectedProfile();
                if (profileInfo == null)
                {
                    Log.Error(LogTags.Stage, "프로필 정보가 없습니다.");
                    return;
                }
                if (profileInfo.Stage == null)
                {
                    Log.Error(LogTags.Stage, "스테이지 정보가 없습니다.");
                    return;
                }

                StageNames currentStageName = profileInfo.Stage.CurrentStage;
                Log.Info(LogTags.Stage, "스테이지 로드 시작: {0}", currentStageName);

                // StageAssetData 로드
                StageData stageData = JsonDataManager.FindStageDataClone(currentStageName);
                if (stageData == null)
                {
                    Log.Error(LogTags.Stage, "스테이지 데이터를 찾을 수 없습니다: {0}", currentStageName);
                    return;
                }

                // 어드레서블 리소스 로드 (스테이지별)
                _ = await ResourcesManager.LoadResourcesByLabelAsync<GameObject>(currentStageName.ToString());

                // StageSystem 프리팹 인스턴스화
                CreateStageSystem(currentStageName);
            }
            catch (System.Exception ex)
            {
                Log.Error(LogTags.Stage, "스테이지 로드 중 오류 발생: {0}", ex.Message);
            }
        }

        private async System.Threading.Tasks.Task WaitForGameAppInitialized()
        {
            // GameApp이 존재하고 초기화될 때까지 대기
            while (GameApp.Instance == null || !GameApp.Instance.IsInitialized)
            {
                await System.Threading.Tasks.Task.Delay(10); // 10ms마다 체크
            }

            Log.Info(LogTags.Stage, "GameApp 초기화 완료 확인");
        }

        private void CreateStageSystem(StageNames stageName)
        {
            _currentStageSystem = ResourcesManager.SpawnStage(stageName, transform);
            if (_currentStageSystem != null)
            {
                _currentStageSystem.Initialize();
                _currentStageSystem.StartStage();

                Log.Info(LogTags.Stage, "스테이지 시스템 생성 완료: {0}", stageName.ToLogString());

                TurnManager.Instance.StartPlayerTurn();
            }
            else
            {
                Log.Error($"{stageName.ToLogString()} 스테이지를 생성할 수 없습니다. 플레이어 턴이 시작되지 않습니다.");
            }
        }

        #endregion Stage Loading
    }
}