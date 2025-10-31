using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    /// <summary>
    /// GameTimeManager의 타임스케일 관리 기능을 담당하는 파셜 클래스
    /// </summary>
    public partial class GameTimeManager
    {
        #region Private Fields

        private float _factor = 1.0f;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// 시간 스케일 팩터를 설정합니다.
        /// </summary>
        /// <param name="factor">시간 스케일 팩터</param>
        /// <param name="useSetScale">Time.timeScale을 설정할지 여부</param>
        public void SetFactor(float factor, bool useSetScale = true)
        {
            _factor = factor;

            if (useSetScale)
            {
                Time.timeScale = _factor;
                Log.Info(LogTags.Time, "시간 스케일을 {0}로 설정합니다.", _factor);
            }
        }

        /// <summary>
        /// 게임을 일시정지합니다.
        /// </summary>
        public void Pause()
        {
            Time.timeScale = 0f;
            Log.Info(LogTags.Time, "게임 시간을 일시정지합니다.");
        }

        /// <summary>
        /// 게임을 재개합니다.
        /// </summary>
        public void Resume()
        {
            Time.timeScale = _factor;
            Log.Info(LogTags.Time, "게임 시간을 재개합니다. 스케일: {0}", _factor);
        }

        /// <summary>
        /// 슬로우 모션을 활성화합니다.
        /// </summary>
        /// <param name="duration">지속 시간</param>
        /// <param name="factor">슬로우 모션 팩터</param>
        /// <param name="onCompleted">완료 시 호출될 콜백</param>
        /// <returns>슬로우 모션 코루틴</returns>
        public IEnumerator ActivateSlowMotion(float duration, float factor, UnityAction onCompleted)
        {
            Time.timeScale = factor;

            yield return new WaitForSecondsRealtime(duration);

            Time.timeScale = _factor;

            onCompleted?.Invoke();
        }

        /// <summary>
        /// 타임스케일을 기반으로 일시정지 상태를 체크하고 업데이트합니다.
        /// </summary>
        public void CheckPauseState()
        {
            bool shouldBePaused = Time.timeScale <= 0f;

            // 타임스케일과 일시정지 상태가 불일치하는 경우 디버그 로그
            if ((Time.timeScale > 0f && IsPaused) || (Time.timeScale <= 0f && !IsPaused))
            {
                Log.Info(LogTags.Time, "(Manager) 타임스케일 불일치 감지 - TimeScale: {0:F2}, IsPaused: {1}", Time.timeScale, IsPaused);
            }

            SetPaused(shouldBePaused);
        }

        /// <summary>
        /// 일시정지 상태를 설정합니다.
        /// </summary>
        /// <param name="paused">일시정지 여부</param>
        public void SetPaused(bool paused)
        {
            if (IsPaused == paused)
            {
                return;
            }

            IsPaused = paused;

            if (paused)
            {
                SaveGameplayTimeToStatistics();
                Log.Info(LogTags.Time, "(Manager) 게임이 일시정지되었습니다.");
            }
            else
            {
                Log.Info(LogTags.Time, "(Manager) 게임이 재개되었습니다.");
            }
        }

        #endregion Public Methods
    }
}