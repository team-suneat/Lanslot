using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace TeamSuneat
{
    [Serializable]
    public class WaveData
    {
        [Title("웨이브 기본 정보")]
        [SerializeField] private string _waveName = "Wave";
        [SerializeField] private int _waveIndex = 0;

        [Title("스폰 설정")]
        [SerializeField] private float _spawnInterval = 3f;
        [SerializeField] private int _maxMonsters = 30;
        [SerializeField] private float _spawnRadius = 15f;
        [SerializeField] private float _minDistanceFromPlayer = 5f;

        [Title("몬스터 타입")]
        [SerializeField] private CharacterNames[] _monsterNames;
        [SerializeField] private string[] _monsterNameStrings;

        [Title("웨이브 지속 시간")]
        [SerializeField] private float _waveDuration = 60f; // 웨이브 지속 시간 (초)

        public string WaveName => _waveName;
        public int WaveIndex => _waveIndex;
        public float SpawnInterval => _spawnInterval;
        public int MaxMonsters => _maxMonsters;
        public float SpawnRadius => _spawnRadius;
        public float MinDistanceFromPlayer => _minDistanceFromPlayer;
        public float WaveDuration => _waveDuration;
        public string[] MonsterNameStrings => _monsterNameStrings;

        public void Validate()
        {
            EnumEx.ConvertTo(ref _monsterNames, _monsterNameStrings);
        }

        public void Refresh()
        {
            _monsterNameStrings = _monsterNames.ToStringArray();
        }

        public CharacterNames GetRandomMonsterType()
        {
            if (!_monsterNames.IsValid())
            {
                return CharacterNames.None;
            }

            return _monsterNames[RandomEx.Range(0, _monsterNames.Length)];
        }
    }
}