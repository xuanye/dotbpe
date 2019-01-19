using Jil;
using PiggyMetrics.StatisticService.Domain;
using System.Net.Http;

namespace PiggyMetrics.StatisticService.Lib
{
    public class RateApiClient
    {
        public static RateContainer GetRates()
        {
            //http://api.fixer.io/latest?base=CNY


            using (var client = new HttpClient())
            {
                var result = client.GetAsync("http://api.fixer.io/latest?base=RUB").Result;
                if(result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string response = result.Content.ReadAsStringAsync().Result;

                    var data = JSON.Deserialize<RateContainer>(response);
                    return data;
                }
            }
            return null;

        }
    }
}
