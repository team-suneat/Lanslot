using Newtonsoft.Json;
using System.Collections.Generic;

namespace TeamSuneat.Data
{
    public partial class JsonDataManager
    {
        #region Field

        /// <summary>
        /// 비동기 모드 사용 여부를 나타내는 상수
        /// </summary>
        public const bool IS_ASYNC_MODE = true;

        private static readonly JsonSerializerSettings _deserializeSettings;

        private static readonly Dictionary<int, PlayerCharacterData> _playerCharacterSheetData = new();
        private static readonly Dictionary<int, MonsterCharacterData> _monsterCharacterSheetData = new();
        private static readonly Dictionary<int, PassiveData> _passiveSheetData = new();
        private static readonly Dictionary<int, StatData> _statSheetData = new();
        private static readonly Dictionary<int, WeaponData> _weaponSheetData = new();
        private static readonly ListMultiMap<int, WeaponLevelData> _weaponLevelSheetData = new();
        private static readonly Dictionary<int, PotionData> _potionSheetData = new();
        private static readonly Dictionary<int, StageData> _stageSheetData = new();
        private static readonly ListMultiMap<int, WaveData> _waveSheetData = new();
        private static readonly Dictionary<string, StringData> _stringSheetData = new();
        private static readonly Dictionary<int, CharacterLevelExpData> _characterLevelExpSheetData = new();
        private static readonly Dictionary<int, CharacterRankExpData> _characterRankExpSheetData = new();

        #endregion Field

        public static void ClearAll()
        {
            _playerCharacterSheetData.Clear();
            _monsterCharacterSheetData.Clear();
            _passiveSheetData.Clear();
            _statSheetData.Clear();
            _weaponSheetData.Clear();
            _weaponLevelSheetData.Clear();
            _potionSheetData.Clear();
            _stageSheetData.Clear();
            _waveSheetData.Clear();
            _stringSheetData.Clear();
            _characterLevelExpSheetData.Clear();
            _characterRankExpSheetData.Clear();
        }

        public static bool CheckLoaded()
        {
            return _playerCharacterSheetData.Count > 0;
        }

        public static void SetPlayerCharacterData(IEnumerable<PlayerCharacterData> list)
        {
            _playerCharacterSheetData.Clear();
            if (list == null)
            {
                return;
            }

            foreach (PlayerCharacterData item in list)
            {
                if (item == null)
                {
                    continue;
                }

                int key = item.GetKey();
                if (_playerCharacterSheetData.ContainsKey(key))
                {
                    LogWarning("PlayerCharacterData 키 중복: {0}", key);
                    continue;
                }
                _playerCharacterSheetData.Add(key, item);
            }
        }

        public static void SetMonsterCharacterData(IEnumerable<MonsterCharacterData> list)
        {
            _monsterCharacterSheetData.Clear();
            if (list == null)
            {
                return;
            }

            foreach (MonsterCharacterData item in list)
            {
                if (item == null)
                {
                    continue;
                }

                int key = item.GetKey();
                if (_monsterCharacterSheetData.ContainsKey(key))
                {
                    LogWarning("MonsterCharacterData 키 중복: {0}", key);
                    continue;
                }
                _monsterCharacterSheetData.Add(key, item);
            }
        }

        public static void SetPassiveData(IEnumerable<PassiveData> list)
        {
            _passiveSheetData.Clear();
            if (list == null)
            {
                return;
            }

            foreach (PassiveData item in list)
            {
                if (item == null)
                {
                    continue;
                }

                int key = item.GetKey();
                if (_passiveSheetData.ContainsKey(key))
                {
                    LogWarning("PassiveData 키 중복: {0}", key);
                    continue;
                }
                _passiveSheetData.Add(key, item);
            }
        }

        public static void SetStageData(IEnumerable<StageData> list)
        {
            _stageSheetData.Clear();
            if (list == null)
            {
                return;
            }

            foreach (StageData item in list)
            {
                if (item == null)
                {
                    continue;
                }

                int key = item.GetKey();
                if (_stageSheetData.ContainsKey(key))
                {
                    LogWarning("StageData 키 중복: {0}", key);
                    continue;
                }
                _stageSheetData.Add(key, item);
            }
        }

        public static void SetStatData(IEnumerable<StatData> list)
        {
            _statSheetData.Clear();
            if (list == null)
            {
                return;
            }

            foreach (StatData item in list)
            {
                if (item == null)
                {
                    continue;
                }

                int key = item.GetKey();
                if (_statSheetData.ContainsKey(key))
                {
                    LogWarning("StatData 키 중복: {0}", key);
                    continue;
                }
                _statSheetData.Add(key, item);
            }
        }

        public static void SetWeaponData(IEnumerable<WeaponData> list)
        {
            _weaponSheetData.Clear();
            if (list == null)
            {
                return;
            }

            foreach (WeaponData item in list)
            {
                if (item == null)
                {
                    continue;
                }

                int key = item.GetKey();
                if (_weaponSheetData.ContainsKey(key))
                {
                    LogWarning("WeaponData 키 중복: {0}", key);
                    continue;
                }
                _weaponSheetData.Add(key, item);
            }
        }

        public static void SetWeaponLevelData(IEnumerable<WeaponLevelData> list)
        {
            _weaponLevelSheetData.Clear();
            if (list == null)
            {
                return;
            }

            foreach (WeaponLevelData item in list)
            {
                if (item == null)
                {
                    continue;
                }

                int key = item.GetKey();
                _weaponLevelSheetData.Add(key, item);
            }
        }

        public static void SetPotionData(IEnumerable<PotionData> list)
        {
            _potionSheetData.Clear();
            if (list == null)
            {
                return;
            }

            foreach (PotionData item in list)
            {
                if (item == null)
                {
                    continue;
                }

                int key = item.GetKey();
                if (_potionSheetData.ContainsKey(key))
                {
                    LogWarning("PotionData 키 중복: {0}", key);
                    continue;
                }
                _potionSheetData.Add(key, item);
            }
        }

        public static void SetStringData(IEnumerable<StringData> list)
        {
            _stringSheetData.Clear();
            if (list == null)
            {
                return;
            }

            foreach (StringData item in list)
            {
                if (item == null)
                {
                    continue;
                }

                string key = item.GetKey();
                if (_stringSheetData.ContainsKey(key))
                {
                    LogWarning("StringData 키 중복: {0}", key);
                    continue;
                }
                _stringSheetData.Add(key, item);
            }
        }

        public static void SetCharacterLevelExpData(IEnumerable<CharacterLevelExpData> list)
        {
            _characterLevelExpSheetData.Clear();
            if (list == null)
            {
                return;
            }

            foreach (CharacterLevelExpData item in list)
            {
                if (item == null)
                {
                    continue;
                }

                int key = item.GetKey();
                if (_characterLevelExpSheetData.ContainsKey(key))
                {
                    LogWarning("CharacterLevelExpData 키 중복: {0}", key);
                    continue;
                }
                _characterLevelExpSheetData.Add(key, item);
            }
        }

        public static void SetCharacterRankExpData(IEnumerable<CharacterRankExpData> list)
        {
            _characterRankExpSheetData.Clear();
            if (list == null)
            {
                return;
            }

            foreach (CharacterRankExpData item in list)
            {
                if (item == null)
                {
                    continue;
                }

                int key = item.GetKey();
                if (_characterRankExpSheetData.ContainsKey(key))
                {
                    LogWarning("CharacterRankExpData 키 중복: {0}", key);
                    continue;
                }
                _characterRankExpSheetData.Add(key, item);
            }
        }

        public static void SetWaveData(IEnumerable<WaveData> list)
        {
            _waveSheetData.Clear();
            if (list == null)
            {
                return;
            }

            foreach (WaveData item in list)
            {
                if (item == null)
                {
                    continue;
                }

                int key = item.GetKey();
                _waveSheetData.Add(key, item);
            }
        }
    }
}