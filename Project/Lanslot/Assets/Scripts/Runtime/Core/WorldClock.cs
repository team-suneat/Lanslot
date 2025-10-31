using System;
using UnityEngine;

// SUMMARY
// 공개 API
// - static GameInstance : WorldClock (씬 내 1개 싱글톤)
// - TicksPerSecond : float (기본 20)
// - TimeScale : int {1,2,4}
// - IsPaused : bool
// - CurrentTickIndex : long
// - event OnTick(int tickIndex)
// 틱 진입점: MonoBehaviour.Update에서 누적 후 고정 주기로 OnTick 발생

namespace TeamSuneat
{
    public sealed class WorldClock : MonoBehaviour
    {
        public static WorldClock GameInstance { get; private set; }

        [Header("Tick Settings")]
        [SerializeField] private float _ticksPerSecond = 20f;
        [SerializeField] private int _timeScale = 1; // 1,2,4
        [SerializeField] private bool _isPaused = false;

        private float _tickInterval;
        private float _accumulator;
        private long _currentTickIndex;

        public event Action<int> OnTick;

        public float TicksPerSecond
        {
            get => _ticksPerSecond;
            set
            {
                _ticksPerSecond = Mathf.Max(1f, value);
                _tickInterval = 1f / _ticksPerSecond;
            }
        }

        public int TimeScale
        {
            get => _timeScale;
            set => _timeScale = Mathf.Clamp(value, 1, 4);
        }

        public bool IsPaused
        {
            get => _isPaused;
            set => _isPaused = value;
        }

        public long CurrentTickIndex => _currentTickIndex;

        private void Awake()
        {
            if (GameInstance != null && GameInstance != this)
            {
                Destroy(gameObject);
                return;
            }

            GameInstance = this;
            DontDestroyOnLoad(gameObject);
            _tickInterval = 1f / _ticksPerSecond;
            _accumulator = 0f;
            _currentTickIndex = 0;
        }

        private void Update()
        {
            if (_isPaused) return;

            _accumulator += Time.unscaledDeltaTime * _timeScale;

            while (_accumulator >= _tickInterval)
            {
                _accumulator -= _tickInterval;
                _currentTickIndex++;
                try
                {
                    OnTick?.Invoke((int)_currentTickIndex);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        [ContextMenu("Log Tick Settings")]
        private void LogTickSettings()
        {
            Debug.Log($"[WorldClock] TPS={_ticksPerSecond}, TimeScale={_timeScale}, Paused={_isPaused}");
        }
    }
}


