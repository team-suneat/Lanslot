using TeamSuneat.Setting;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// UIPopup의 입력 차단 처리를 담당하는 핸들러입니다.
    /// </summary>
    public class UIPopupInputBlockHandler : MonoBehaviour, IUIPopupInputBlockHandler
    {
        [FoldoutGroup("#Input Block Settings")]
        [SerializeField] private bool _blockCharacterInput;

        [FoldoutGroup("#Input Block Settings")]
        [SerializeField] private bool _blockSpawnPopup;

        public bool IsBlockInput { get; private set; }

        public void Initialize()
        {
            IsBlockInput = false;
        }

        public void Cleanup()
        {
            ResetBlockInput();
        }

        public void StartBlockInput()
        {
            SetBlockInput();
            Log.Info(LogTags.UI_Popup, "입력 차단을 시작합니다.");
        }

        public void SetBlockInput()
        {
            IsBlockInput = true;
        }

        public void ResetBlockInput()
        {
            IsBlockInput = false;
            Log.Info(LogTags.UI_Popup, "입력 차단을 해제했습니다.");
        }

        public void SetBlockCharacterInput(bool blockCharacterInput)
        {
            _blockCharacterInput = blockCharacterInput;
            Log.Info(LogTags.UI_Popup, $"캐릭터 입력 차단 설정을 변경했습니다: {_blockCharacterInput}");
        }

        public void SetBlockSpawnPopup(bool blockSpawnPopup)
        {
            _blockSpawnPopup = blockSpawnPopup;
            Log.Info(LogTags.UI_Popup, $"팝업 스폰 차단 설정을 변경했습니다: {_blockSpawnPopup}");
        }

        public void ConfigurePopupSettings(bool isOpening)
        {
            if (isOpening)
            {
                if (_blockCharacterInput)
                {
                    GameSetting.Instance.Input.BlockCharacterInput();
                }
                if (_blockSpawnPopup)
                {
                    UIManager.Instance.PopupManager.BlockSpawnPopup = true;
                }
            }
            else
            {
                if (_blockCharacterInput)
                {
                    UIManager.Instance.PopupManager.WaitUnlockCharacterInput();
                }
                if (_blockSpawnPopup)
                {
                    UIManager.Instance.PopupManager.BlockSpawnPopup = false;
                }
            }
        }
    }
}