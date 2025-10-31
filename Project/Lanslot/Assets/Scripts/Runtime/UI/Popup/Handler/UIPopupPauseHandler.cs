using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// UIPopup의 게임 일시정지 처리를 담당하는 핸들러입니다.
    /// </summary>
    public class UIPopupPauseHandler : MonoBehaviour
    {
        [FoldoutGroup("#Pause Settings")]
        [SerializeField] private bool _isPauseIngame;

        public bool IsPauseIngame => _isPauseIngame;

        public void Initialize()
        {
            // 초기화 작업
        }

        public void Cleanup()
        {
            // 정리 작업
        }

        public void Pause()
        {
            if (_isPauseIngame)
            {
                GameTimeManager.Instance.Pause();
                Log.Info(LogTags.UI_Popup, "게임을 일시정지했습니다.");
            }
        }

        public void Resume()
        {
            if (_isPauseIngame)
            {
                GameTimeManager.Instance.Resume();
                Log.Info(LogTags.UI_Popup, "게임을 재개했습니다.");
            }
        }

        public void SetPauseIngame(bool isPauseIngame)
        {
            _isPauseIngame = isPauseIngame;
            Log.Info(LogTags.UI_Popup, $"게임 일시정지 설정을 변경했습니다: {_isPauseIngame}");
        }
    }
}