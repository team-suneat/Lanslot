using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 플레이어 캐릭터의 능력치를 표시하는 HUD 클래스
    /// </summary>
    public class HUDPlayerStatsDisplay : XBehaviour
    {
        [Title("#HUD Player Stats Display")]
        [SerializeField] private UILocalizedText _levelText;
        [SerializeField] private UILocalizedText[] _statTexts;

        [Title("#HUD Player Stats Display", "Settings")]
        [SerializeField] private bool _showOnlyActiveStats = true;
        [SerializeField] private bool _autoRefreshOnStatChange = true;

        private PlayerCharacter _playerCharacter;
        private StatSystem _playerStatSystem;

        #region Unity Lifecycle

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _levelText = this.FindComponent<UILocalizedText>("Player Level Text");
            _statTexts = this.FindComponentsInChildren<UILocalizedText>("#Stats");
        }

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();

            GlobalEvent.Register(GlobalEventType.PLAYER_CHARACTER_BATTLE_READY, OnPlayerCharacterSpawned);
            GlobalEvent<int>.Register(GlobalEventType.GAME_DATA_CHARACTER_LEVEL_CHANGED, OnPlayerCharacterLevelChanged);
            GlobalEvent.Register(GlobalEventType.PLAYER_CHARACTER_DESPAWNED, OnPlayerCharacterDespawned);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();

            GlobalEvent.Unregister(GlobalEventType.PLAYER_CHARACTER_BATTLE_READY, OnPlayerCharacterSpawned);
            GlobalEvent<int>.Unregister(GlobalEventType.GAME_DATA_CHARACTER_LEVEL_CHANGED, OnPlayerCharacterLevelChanged);
            GlobalEvent.Unregister(GlobalEventType.PLAYER_CHARACTER_DESPAWNED, OnPlayerCharacterDespawned);
        }

        #endregion Unity Lifecycle

        #region 이벤트 핸들러

        private void OnPlayerCharacterSpawned()
        {
            _playerCharacter = CharacterManager.Instance.Player;
            if (_playerCharacter != null)
            {
                _playerStatSystem = _playerCharacter.Stat;

                if (_autoRefreshOnStatChange)
                {
                    _playerStatSystem.RegisterOnRefresh(OnStatChanged);
                }

                RefreshStatsDisplay();
            }
        }

        private void OnPlayerCharacterLevelChanged(int playerLevel)
        {
            _levelText.SetText($"Lv.{playerLevel}");
            RefreshStatsDisplay();
        }

        private void OnPlayerCharacterDespawned()
        {
            if (_playerStatSystem != null && _autoRefreshOnStatChange)
            {
                _playerStatSystem.UnregisterOnRefresh(OnStatChanged);
            }

            _playerCharacter = null;
            _playerStatSystem = null;

            ClearStatsDisplay();
        }

        private void OnStatChanged(StatNames statName, float statValue)
        {
            RefreshStatsDisplay();
        }

        #endregion 이벤트 핸들러

        #region 스탯 표시 로직

        /// <summary>
        /// 스탯 표시를 새로고침합니다.
        /// </summary>
        public void RefreshStatsDisplay()
        {
            if (_playerCharacter == null || _playerStatSystem == null)
            {
                ClearStatsDisplay();
                return;
            }

            int textIndex = 0;
            int maxDisplayCount = _statTexts.Length;

            // 플레이어의 모든 스탯을 순회하며 0이 아닌 값만 표시
            for (int i = 0; i < _playerStatSystem.AllStats.Length && textIndex < maxDisplayCount; i++)
            {
                CharacterStat stat = _playerStatSystem.AllStats[i];
                if (stat.Name == StatNames.None)
                {
                    continue;
                }

                float statValue = stat.Value;
                if (statValue != 0f)
                {
                    string displayText = FormatStatDisplay(stat.Name, statValue);
                    _statTexts[textIndex].SetText(displayText);
                    _statTexts[textIndex].Activate();
                    textIndex++;
                }
            }

            // 남은 텍스트 컴포넌트들 비활성화
            for (int i = textIndex; i < _statTexts.Length; i++)
            {
                _statTexts[i].Deactivate();
            }
        }

        /// <summary>
        /// 스탯 표시 형식을 포맷합니다.
        /// </summary>
        private string FormatStatDisplay(StatNames statName, float statValue)
        {
            string statDisplayName = GetStatDisplayName(statName);
            string formattedValue = statName.GetStatValueString(statValue, false);
            return $"{statDisplayName} : {formattedValue}";
        }

        /// <summary>
        /// 스탯 이름의 표시명을 가져옵니다.
        /// </summary>
        private string GetStatDisplayName(StatNames statName)
        {
            // StatNames enum의 주석에서 표시명 추출하거나, 별도 매핑 테이블 사용
            return statName switch
            {
                StatNames.Health => "Health",
                StatNames.Damage => "Attack",
                StatNames.CriticalChance => "Crit Chance",
                StatNames.CriticalDamageMulti => "Crit Damage",
                StatNames.Shield => "Shield",
                _ => statName.ToString(),
            };
        }

        private void ClearStatsDisplay()
        {
            if (_statTexts == null)
            {
                return;
            }

            foreach (UILocalizedText statText in _statTexts)
            {
                statText?.Deactivate();
            }
        }

        #endregion 스탯 표시 로직

        #region Public Methods

        /// <summary>
        /// 활성화된 스탯만 표시할지 설정합니다.
        /// </summary>
        public void SetShowOnlyActiveStats(bool showOnlyActive)
        {
            _showOnlyActiveStats = showOnlyActive;
            RefreshStatsDisplay();
        }

        /// <summary>
        /// 스탯 변경 시 자동 새로고침 여부를 설정합니다.
        /// </summary>
        public void SetAutoRefreshOnStatChange(bool autoRefresh)
        {
            _autoRefreshOnStatChange = autoRefresh;

            if (_playerStatSystem != null)
            {
                if (autoRefresh)
                {
                    _playerStatSystem.RegisterOnRefresh(OnStatChanged);
                }
                else
                {
                    _playerStatSystem.UnregisterOnRefresh(OnStatChanged);
                }
            }
        }

        #endregion Public Methods
    }
}