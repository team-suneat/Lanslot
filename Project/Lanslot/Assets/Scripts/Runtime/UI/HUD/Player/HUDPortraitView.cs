using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class HUDPortraitView : MonoBehaviour
    {
        [SerializeField] private Image _portraitImage;
        [SerializeField] private TextMeshProUGUI _nameText;

        public void SetPortrait(CharacterNames characterName)
        {
            Sprite portraitSprite = characterName.LoadSprite();
            if (portraitSprite != null)
            {
                _portraitImage.sprite = portraitSprite;
                _portraitImage.enabled = true;
            }
            else
            {
                _portraitImage.enabled = false;
            }
        }

        public void SetName(CharacterNames characterName)
        {
            _nameText.text = characterName.GetLocalizedString();
        }

        public void Clear()
        {
            _portraitImage.enabled = false;
            _nameText.text = string.Empty;
        }
    }
}