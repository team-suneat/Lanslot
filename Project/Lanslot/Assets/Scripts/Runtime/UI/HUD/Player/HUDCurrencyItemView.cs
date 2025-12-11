using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class HUDCurrencyItemView : MonoBehaviour
    {
        [SerializeField] CurrencyNames _currencyName;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amountText;

        public void Set(int amount, Sprite icon = null)
        {
            if (_icon != null)
            {
                bool hasIcon = icon != null;
                _icon.enabled = hasIcon || _icon.sprite != null;
                if (hasIcon)
                {
                    _icon.sprite = icon;
                }
            }

            if (_amountText != null)
            {
                _amountText.text = amount.ToString();
            }
        }

        public void Clear()
        {
            if (_icon != null)
            {
                _icon.enabled = false;
                _icon.sprite = null;
            }

            if (_amountText != null)
            {
                _amountText.text = string.Empty;
            }
        }
    }
}

