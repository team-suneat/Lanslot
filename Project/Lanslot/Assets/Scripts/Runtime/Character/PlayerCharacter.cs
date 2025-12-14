using System.Collections;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat
{
    public class PlayerCharacter : Character
    {
        public override LogTags LogTag => LogTags.Player;

        public override void Initialize()
        {
            VProfile profileInfo = GameApp.GetSelectedProfile();
            if (profileInfo != null)
            {
                Name = profileInfo.Character.SelectedCharacterName;

                SetupLevel();

                ProfileInfo.Character.Unlock(Name);

                base.Initialize();
            }
        }

        public override void BattleReady()
        {
            base.BattleReady();

            Buff?.OnBattleReady();
            CharacterManager.Instance.RegisterPlayer(this);
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

                PlayerCharacterData characterData = JsonDataManager.FindPlayerCharacterDataClone(Name);
                if (characterData != null)
                {
                    ApplyGrowthStats(characterData);
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

        public override void AddCharacterStats()
        {
            PlayerCharacterData data = JsonDataManager.FindPlayerCharacterDataClone(Name);
            if (data != null)
            {
                // 기본 스탯 적용
                ApplyBaseStats(data);

                // 레벨별 성장 스탯 적용
                ApplyGrowthStats(data);

                LogInfo("캐릭터 스탯이 스크립터블 데이터에서 적용되었습니다. 캐릭터: {0}, 레벨: {1}", Name, Level);
            }
        }

        private void ApplyBaseStats(PlayerCharacterData data)
        {
            if (!data.IsValid()) return;
            for (int i = 0; i < data.BaseStats.Length; i++)
            {
                StatNames baseStatName = data.BaseStats[i];
                float baseStatValue = data.BaseStatValues[i];

                if (baseStatName == StatNames.None || baseStatValue.IsZero()) continue;

                Stat.AddWithSourceInfo(baseStatName, baseStatValue, this, NameString, "CharacterBase");
            }
        }

        private void ApplyGrowthStats(PlayerCharacterData data)
        {
            if (!data.IsValid()) return;
            for (int i = 0; i < data.GrowStats.Length; i++)
            {
                StatNames baseStatName = data.GrowStats[i];
                float baseStatValue = data.GrowStatValues[i];

                if (baseStatName == StatNames.None || baseStatValue.IsZero()) continue;

                Stat.AddWithSourceInfo(baseStatName, baseStatValue, this, NameString, "CharacterGrowth");
            }
        }

        //

        protected override void OnDeath(DamageResult damageResult)
        {
            base.OnDeath(damageResult);

            CharacterManager.Instance.UnregisterPlayer(this);
            GameApp.Instance.data.ClearIngameData();

            CoroutineNextTimer(1f, SendGlobalEventForMoveToTitle);
        }

        private void SendGlobalEventForMoveToTitle()
        {
            GlobalEvent.Send(GlobalEventType.MOVE_TO_TITLE);
        }
    }
}