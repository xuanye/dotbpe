using System.Collections.Generic;
using PiggyMetrics.Common;

namespace PiggyMetrics.StatisticService.Interface
{
    public interface IExchangeRateService
    {
        double Convert(Currency from,Currency to,double value);


        Dictionary<Currency, double> GetRates();

    }
}
