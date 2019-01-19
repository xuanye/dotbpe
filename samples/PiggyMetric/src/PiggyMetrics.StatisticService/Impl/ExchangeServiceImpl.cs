using System;
using System.Collections.Generic;
using PiggyMetrics.Common;
using PiggyMetrics.StatisticService.Interface;
using PiggyMetrics.StatisticService.Lib;

namespace PiggyMetrics.StatisticService.Impl
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly Dictionary<Currency, double> defaultRate = new Dictionary<Currency, double>
        {
            { Currency.Cynone,1 },
            { Currency.Rub,1 },
            { Currency.Eur,10 },
            { Currency.Usd,6.7 }
        };
        private Dictionary<Currency, double> currentRate;
        private string rateDate;

        public double Convert(Currency from, Currency to, double value)
        {
            var rates = GetRates();
            var rateFrom = rates.ContainsKey(from)? rates[from]: defaultRate[from];
            var rateTo = rates.ContainsKey(to) ? rates[to] : defaultRate[to];
            return value / rateFrom * rateTo;

        }

        public Dictionary<Currency, double> GetRates()
        {
            if(currentRate == null || rateDate != DateTime.Today.ToString("yyyy-MM-dd"))
            {
                var container = RateApiClient.GetRates();
                if(container != null)
                {
                    currentRate = new Dictionary<Currency, double>();
                    currentRate.Add(Currency.Rub, 1);
                    currentRate.Add(Currency.Cynone, 1);
                    rateDate = DateTime.Today.ToString("yyyy-MM-dd");
                    foreach(var kv in container.rates)
                    {
                        switch (kv.Key.ToUpper())
                        {
                            case "USD":
                                currentRate.Add(Currency.Usd, kv.Value);
                                break;
                            case "EUR":
                                currentRate.Add(Currency.Eur, kv.Value);
                                break;
                        }
                    }
                }
            }
            if(currentRate == null)
            {
                return defaultRate;
            }
            return currentRate;
        }
    }
}
