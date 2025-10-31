using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public enum TimeFormat
    {
        MinutesSeconds,              // mm:ss
        MinuteSeconds,               // m:ss
        SecondsOnly,                 // ss
        MinutesSecondsCentiseconds   // mm:ss.ff
    }

    public class HUDStageTimer : MonoBehaviour
    {
        [SerializeField] private UILocalizedText _timerText;
        [SerializeField] private TimeFormat _timeFormat = TimeFormat.MinutesSeconds;

        public void UpdateTimeText(float elapsedTime)
        {
            if (_timerText != null)
            {
                string content = FormatTime(elapsedTime);
                _timerText.SetText(content);
            }
        }

        private string FormatTime(float timeInSeconds)
        {
            int totalSeconds = Mathf.FloorToInt(timeInSeconds);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            switch (_timeFormat)
            {
                case TimeFormat.MinutesSeconds:
                    return $"{minutes:D2}:{seconds:D2}";

                case TimeFormat.MinuteSeconds:
                    return $"{minutes}:{seconds:D2}";

                case TimeFormat.SecondsOnly:
                    return $"{totalSeconds}";

                case TimeFormat.MinutesSecondsCentiseconds:
                    int centiseconds = Mathf.FloorToInt((timeInSeconds - totalSeconds) * 100);
                    return $"{minutes:D2}:{seconds:D2}.{centiseconds:D2}";

                default:
                    return $"{minutes:D2}:{seconds:D2}";
            }
        }
    }
}