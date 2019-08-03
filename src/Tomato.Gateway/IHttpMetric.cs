using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Tomato.Gateway
{
    public interface IHttpMetric:IDisposable
    {
        Task AddToMetricsAsync(HttpContext context);
    }

    public interface IHttpMetricFactory
    {
        IHttpMetric Create();
    }
}
