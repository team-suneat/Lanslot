using System;
using System.Collections.Generic;

namespace TeamSuneat.Data.Game
{
    [Serializable]
    public class VCurrency
    {
        private Dictionary<string, int> _amounts = new(StringComparer.Ordinal);

        public VCurrency()
        {
        }

        public void OnLoadGameData()
        {
        }

        public int GetAmount(CurrencyNames currencyName)
        {
            return GetAmount(currencyName.ToString());
        }

        public void Add(CurrencyNames currencyName, int amount)
        {
            Add(currencyName.ToString(), amount);
            GlobalEvent<CurrencyNames, int>.Send(GlobalEventType.CURRENCY_EARNED, currencyName, GetAmount(currencyName));
        }

        public bool CanUseOrNotify(CurrencyNames currencyName, int amount)
        {
            return CanUse(currencyName.ToString(), amount);
        }

        public void Use(CurrencyNames currencyName, int amount)
        {
            int before = GetAmount(currencyName);
            Use(currencyName.ToString(), amount);

            int after = GetAmount(currencyName);
            if (after < before)
            {
                GlobalEvent<CurrencyNames, int>.Send(GlobalEventType.CURRENCY_PAYED, currencyName, after);
            }
        }

        public void UseAll(CurrencyNames currencyName)
        {
            UseAll(currencyName.ToString());
            GlobalEvent<CurrencyNames, int>.Send(GlobalEventType.CURRENCY_PAYED, currencyName, 0);
        }

        //

        private int GetAmount(string currencyId)
        {
            if (string.IsNullOrEmpty(currencyId))
            {
                return 0;
            }

            return _amounts != null && _amounts.TryGetValue(currencyId, out var value) ? value : 0;
        }

        private void Add(string currencyId, int amount)
        {
            if (string.IsNullOrEmpty(currencyId))
            {
                return;
            }

            if (amount == 0)
            {
                return;
            }

            if (_amounts.TryGetValue(currencyId, out var current))
            {
                _amounts[currencyId] = current + amount;
            }
            else
            {
                _amounts[currencyId] = amount;
            }

            Log.Info(LogTags.Currency, "[GameData] {0} {1}를 획득합니다.", amount, currencyId);
        }

        private bool CanUse(string currencyId, int amount)
        {
            int currentAmount = GetAmount(currencyId);
            return currentAmount >= amount;
        }

        private void Use(string currencyId, int amount)
        {
            if (string.IsNullOrEmpty(currencyId))
            {
                return;
            }

            if (amount <= 0)
            {
                return;
            }

            if (_amounts.TryGetValue(currencyId, out var current) && current >= amount)
            {
                _amounts[currencyId] = current - amount;
                Log.Info(LogTags.Currency, "[GameData] {0} {1}를 사용합니다.", amount, currencyId);
            }
        }

        private void UseAll(string currencyId)
        {
            if (string.IsNullOrEmpty(currencyId))
            {
                return;
            }

            if (_amounts.ContainsKey(currencyId))
            {
                _amounts[currencyId] = 0;

                Log.Info(LogTags.Currency, $"[GameData] {currencyId}를 모두 사용합니다.");
            }
        }

        //

        public void ClearIngameCurrencies()
        {
            UseAll(CurrencyNames.Gold);
        }

        public static VCurrency CreateDefault()
        {
            return new VCurrency();
        }
    }
}