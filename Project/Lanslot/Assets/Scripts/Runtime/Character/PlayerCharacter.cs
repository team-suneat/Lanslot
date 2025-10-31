using System.Collections;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class PlayerCharacter : Character
    {
        protected override void OnStart()
        {
            base.OnStart();

            StartXCoroutine(InitializeForDev());
        }

        private IEnumerator InitializeForDev()
        {
            yield return new WaitUntil(() =>
            {
                if (GameApp.Instance == null) return false;
                if (GameApp.Instance.data == null) return false;
                return true;
            });

            Initialize();
        }

        public override void Initialize()
        {
            SetupLevel();

            CharacterManager.Instance.Register(this);

            ProfileInfo.Character.Register(Name);

            base.Initialize();
        }

        public override void BattleReady()
        {
            base.BattleReady();

            Buff?.OnBattleReady();

            SetupAnimatorLayerWeight();

            IsBattleReady = true;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
        }

        public override void PhysicsUpdate()
        {
            if (!ActiveSelf)
            {
                return;
            }

            base.PhysicsUpdate();
        }

        //

        public override void SetupLevel()
        {
            base.SetupLevel();

            Level = ProfileInfo.Level.Level;
        }

        public void OnLevelup(int addedLevel)
        {
            SetupLevel();
            if (Stat != null)
            {
                int previousLife = MyVital.CurrentLife;

                CharacterStatAsset statAsset = ScriptableDataManager.Instance.GetCharacterStatAsset(Name);
                if (statAsset != null)
                {
                    ApplyGrowthStats(statAsset);
                    LogInfo("캐릭터 스탯이 스크립터블 데이터에서 적용되었습니다. 캐릭터: {0}, 레벨: {1}", Name, Level);
                }

                Stat.OnLevelUp();
                MyVital?.OnLevelUp(Stat, previousLife);
            }
            if (OnLevelUpFeedbacks != null)
            {
                OnLevelUpFeedbacks.PlayFeedbacks(position, 0);
            }

            SpawnLevelUpText(addedLevel);
        }

        public void OnLevelDown()
        {
            SetupLevel();
            MyVital?.OnLevelDown();
        }

        private void SpawnLevelUpText(int addedLevel)
        {
            if (addedLevel == 0)
            {
                return;
            }

            string format = JsonDataManager.FindStringClone("LevelUpFormat");
            string content = string.Format(format, addedLevel);

            _ = ResourcesManager.SpawnFloatyText(content, true, transform);
        }

        //

        protected override void OnDeath(DamageResult damageResult)
        {
            base.OnDeath(damageResult);

            CharacterManager.Instance.Unregister(this);
            GameApp.Instance.data.ClearIngameData();

            CoroutineNextTimer(1f, SendGlobalEventForMoveToTitle);
        }

        private void SendGlobalEventForMoveToTitle()
        {
            GlobalEvent.Send(GlobalEventType.MOVE_TO_TITLE);
        }
    }
}