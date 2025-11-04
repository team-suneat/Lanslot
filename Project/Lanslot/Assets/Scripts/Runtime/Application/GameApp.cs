using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat
{
    public sealed class GameApp : XGameApp
    {
        public static GameApp Instance;
        public float prevChangeSceneTime;

        public GameData data
        {
            get
            {
                if (dataManager != null)
                {
                    return dataManager.Data;
                }

                return null;
            }
            set => dataManager.Data = value;
        }

        public GameDataManager dataManager;
        public GameManager gameManager;

        public bool IsInitialized { get; private set; }

        //

        public void ForceApplicationStart()
        {
            OnApplicationStart();
        }

        //

        protected override void OnApplicationStart()
        {
            Instance = this;

            // LOG
            Log.LoadLevel();
            Log.Initialize();

            // SETTING
            GameSetting.Instance.Initialize();
            _ = StartCoroutine(GameSetting.Instance.Video.ProcessMoveDisplay());

            // PREF DATA
            GamePrefs.ClearOnEntryPoint();

            // FIXED DATA
            LoadAsync();
        }

        private async void LoadAsync()
        {
            await PathManager.LoadAllAsync();

            // 구글 시트에서 동기화
            bool googleSheetSynced = await GoogleSheetRuntimeSync.FetchConvertAndApplyAllAsync();
            if (!googleSheetSynced)
            {
                //  생성된 JSON 파일 로드
                await JsonDataManager.LoadJsonSheetsAsync();
            }

            await ScriptableDataManager.Instance.LoadScriptableAssetsAsync();
            await ResourcesManager.LoadResourcesByLabelAsync<GameObject>("Ingame");

            // LOAD SAVED DATA
            LoadGameData();

            IsInitialized = true;
        }

        protected override void OnApplicationStarted()
        {
            gameManager = gameObject.AddComponent<GameManager>();
        }

        private void Update()
        {
            // 대기 중인 저장 처리
            dataManager?.ProcessPendingSave();
        }

        public static GameApp Create()
        {
            GameObject gameApp = new("@GameApp");

            return gameApp.AddComponent<GameApp>();
        }

        public void LoadGameData()
        {
            dataManager ??= new GameDataManager();
            dataManager.LoadGameDataWithRecovery();
        }

        public void SaveGameData()
        {
            // data.Statistics.RegisterLastSaveTime();
            dataManager?.Save();
        }

        public static VProfile GetSelectedProfile()
        {
            return Instance?.data?.GetSelectedProfile();
        }
    }
}