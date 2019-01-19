using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DotBPE.Gateway
{
    public class NullHttpMetric:IHttpMetric
    {
        public void Dispose()
        {

        }

        public Task AddToMetricsAsync(HttpContext context)
        {
            return Task.CompletedTask;
        }


    }
}
