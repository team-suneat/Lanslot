using TeamSuneat.Data;

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

            GameApp.Instance.SaveGameData();
            base.CleanupCurrentScene();
        }

        private void OnMoveToTitle()
        {
            if (DetermineChangeScene("GameTitle"))
            {
                CharacterManager.Instance.DoDestroyPlayerOnLoad();
                CharacterManager.Instance.UnregisterPlayer();
                GameApp.Instance.gameManager.ResetStage();

                ChangeScene("GameTitle");
            }
        }

        private void OnMoveToStage()
        {
            if (DetermineChangeScene("GameMain"))
            {
                CharacterManager.Instance.DontDestroyPlayerOnLoad();
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

        private void LoadStage()
        {
            try
            {
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

                StageNames currentStageName = profileInfo.Stage.CurrentStageName;
                Log.Info(LogTags.Stage, "스테이지 로드 시작: {0}", currentStageName);

                // StageAssetData 로드
                StageAssetData stageData = ScriptableDataManager.Instance.FindStage(currentStageName)?.Data;
                if (stageData == null)
                {
                    Log.Error(LogTags.Stage, "스테이지 데이터를 찾을 수 없습니다: {0}", currentStageName);
                    return;
                }

                if (!stageData.HasValidWaves)
                {
                    Log.Error(LogTags.Stage, "유효한 웨이브가 없는 스테이지입니다: {0}", currentStageName);
                    return;
                }

                // StageSystem 프리팹 인스턴스화
                // CreateStageSystem(currentStageName);
            }
            catch (System.Exception ex)
            {
                Log.Error(LogTags.Stage, "스테이지 로드 중 오류 발생: {0}", ex.Message);
            }
        }

        private async void CreateStageSystem(StageNames stageName)
        {
            _ = await ResourcesManager.LoadResourcesByLabelAsync<UnityEngine.GameObject>(stageName.ToString());

            _currentStageSystem = ResourcesManager.SpawnStage(stageName, transform);
            _currentStageSystem.Initialize();
            _currentStageSystem.StartStage();

            Log.Info(LogTags.Stage, "스테이지 시스템 생성 완료: {0}", stageName.ToLogString());
        }

        #endregion Stage Loading
    }
}