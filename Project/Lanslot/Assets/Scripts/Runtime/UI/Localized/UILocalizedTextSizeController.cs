using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    [DisallowMultipleComponent]
    public class UILocalizedTextSizeController : XBehaviour
    {
        [Title("#UI Localized Text Size")]
        [SerializeField]
        private bool _sizeToTextLengthX;

        [SerializeField]
        private bool _sizeToTextLengthY;

        [SerializeField]
        private LayoutElement _layoutElement;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            AssignComponents();
        }

        private void OnValidate()
        {
            ValidateLayoutComponents();
        }

        private void AssignComponents()
        {
            if (_layoutElement == null)
            {
                _layoutElement = GetComponent<LayoutElement>();
            }
        }

        private void ValidateLayoutComponents()
        {
            if (!_sizeToTextLengthX && !_sizeToTextLengthY)
            {
                return;
            }

            bool hasLayoutElement = _layoutElement != null || TryGetComponent(out LayoutElement assignedLayoutElement);
            if (!hasLayoutElement)
            {
                Log.Warning($"[UILocalizedText] LayoutElement가 없습니다. ({this.GetHierarchyName()})");
            }
        }

        public void Refresh(TextMeshProUGUI textPro)
        {
            if (!_sizeToTextLengthX && !_sizeToTextLengthY)
            {
                return;
            }

            if (_layoutElement == null)
            {
                Log.Warning(LogTags.Font, $"[UILocalizedTextSizeController] LayoutElement가 없습니다. ({this.GetHierarchyName()})");
                return;
            }

            if (textPro == null || textPro.font == null)
            {
                Log.Warning(LogTags.Font, $"[UILocalizedTextSizeController] TextPro 또는 font가 null입니다. ({this.GetHierarchyName()})");
                return;
            }

            if (_sizeToTextLengthX)
            {
                _layoutElement.preferredWidth = textPro.preferredWidth;
            }

            if (_sizeToTextLengthY)
            {
                _layoutElement.preferredHeight = textPro.preferredHeight;
            }
        }
    }
}