using System.Collections.Generic;

namespace PiggyMetrics.StatisticService.Domain
{
    public class RateContainer
    {
        public string date { get; set; }
        public Dictionary<string, double> rates {get;set;}
    }
}
