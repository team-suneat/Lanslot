using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// UIPopup의 입력 처리를 담당하는 핸들러입니다.
    /// </summary>
    public class UIPopupInputHandler : MonoBehaviour, IUIPopupInputHandler
    {
        [FoldoutGroup("#Input Settings")]
        [SerializeField] private int _currentTargetIndex;

        [FoldoutGroup("#Input Settings")]
        [SerializeField] private int _maxTargetIndex;

        [FoldoutGroup("#Input Settings")]
        [SerializeField] private UIPointerEvent[] _pointerEvents;

        public int CurrentTargetIndex => _currentTargetIndex;
        public int MaxTargetIndex => _maxTargetIndex;

        public void Initialize()
        {
            _currentTargetIndex = 0;
            _maxTargetIndex = 0;

            if (_pointerEvents == null)
            {
                _pointerEvents = GetComponentsInChildren<UIPointerEvent>();
            }
        }

        public void Cleanup()
        {
            _currentTargetIndex = 0;
            _maxTargetIndex = 0;
        }

        public void SetupInput(int maxIndex)
        {
            _maxTargetIndex = maxIndex;
            _currentTargetIndex = 0;

            Log.Info(LogTags.UI_Popup, $"입력 시스템을 설정했습니다. 최대 인덱스: {_maxTargetIndex}");
        }

        public void ActivateTarget(int index)
        {
            if (IsValidIndex(index))
            {
                _currentTargetIndex = index;
                Log.Info(LogTags.UI_Popup, $"타겟 {index}를 활성화했습니다.");
            }
            else
            {
                Log.Warning(LogTags.UI_Popup, $"유효하지 않은 타겟 인덱스: {index}");
            }
        }

        public void NextTarget()
        {
            if (_maxTargetIndex > 0)
            {
                _currentTargetIndex = (_currentTargetIndex + 1) % _maxTargetIndex;
                Log.Info(LogTags.UI_Popup, $"다음 타겟으로 이동: {_currentTargetIndex}");
            }
        }

        public void PrevTarget()
        {
            if (_maxTargetIndex > 0)
            {
                _currentTargetIndex = (_currentTargetIndex - 1 + _maxTargetIndex) % _maxTargetIndex;
                Log.Info(LogTags.UI_Popup, $"이전 타겟으로 이동: {_currentTargetIndex}");
            }
        }

        public void SelectTarget(int index)
        {
            if (IsValidIndex(index))
            {
                _currentTargetIndex = index;
                Log.Info(LogTags.UI_Popup, $"타겟 {index}를 선택했습니다.");
            }
            else
            {
                Log.Warning(LogTags.UI_Popup, $"유효하지 않은 타겟 인덱스: {index}");
            }
        }

        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < _maxTargetIndex;
        }

        private void OnValidate()
        {
            if (_pointerEvents == null)
            {
                Log.Error($"[{nameof(UIPointerEvent)}] PointerEvents 배열이 null입니다: {name}");
                return;
            }

            for (int i = 0; i < _pointerEvents.Length; i++)
            {
                if (_pointerEvents[i] == null)
                {
                    Log.Error($"[{nameof(UIPointerEvent)}] PointerEvents 배열의 인덱스 {i}가 null이거나 Missing Reference입니다: {name}");
                }
            }
        }
    }
}