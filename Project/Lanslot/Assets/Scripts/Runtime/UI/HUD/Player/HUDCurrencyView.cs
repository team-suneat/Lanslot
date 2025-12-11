using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class HUDCurrencyView : XBehaviour
    {
        [SerializeField] private HUDCurrencyItemView _goldView;
        [SerializeField] private HUDCurrencyItemView _gemView;

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();

            GlobalEvent<CurrencyNames, int>.Register(GlobalEventType.CURRENCY_EARNED, OnCurrencyUpdate);
            GlobalEvent<CurrencyNames, int>.Register(GlobalEventType.CURRENCY_PAYED, OnCurrencyUpdate);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();

            GlobalEvent<CurrencyNames, int>.Unregister(GlobalEventType.CURRENCY_EARNED, OnCurrencyUpdate);
            GlobalEvent<CurrencyNames, int>.Unregister(GlobalEventType.CURRENCY_PAYED, OnCurrencyUpdate);
        }

        private void OnCurrencyUpdate(CurrencyNames currencyName, int currentAmount)
        {
            switch (currencyName)
            {
                case CurrencyNames.Gold:
                    _goldView?.Set(currentAmount);
                    break;

                case CurrencyNames.Gem:
                    _gemView?.Set(currentAmount);
                    break;
            }
        }

        public void SetGold(int amount, Sprite icon = null)
        {
            _goldView?.Set(amount, icon);
        }

        public void SetGem(int amount, Sprite icon = null)
        {
            _gemView?.Set(amount, icon);
        }

        public void SetCurrencies(int goldAmount, int gemAmount, Sprite goldIcon = null, Sprite gemIcon = null)
        {
            SetGold(goldAmount, goldIcon);
            SetGem(gemAmount, gemIcon);
        }

        public void Clear()
        {
            _goldView?.Clear();
            _gemView?.Clear();
        }
    }
}